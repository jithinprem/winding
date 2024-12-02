using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using windingApi.Controller.Repository.RepositoryInterfaces;
using windingApi.DTO;
using windingApi.Models;

namespace windingApi.Services;

public class BlogService
{

    //private readonly IBlogRepository _blogRepository;
    private readonly IBlogRepository _blogRepository;
    private readonly AzureBlobService _azureBlobService;
    private readonly UserManager<User> _userManager;
    private readonly ILikeRepository _likeRepository;
    private readonly ITagDefinitionRepository _tagDefinitionRepository;
    private readonly IBlogTagRepository _blogTagRepository;

    public BlogService(IBlogRepository blogRepository, UserManager<User> userManager, AzureBlobService azureBlobService
        ,ILikeRepository likeRepository, ITagDefinitionRepository tagDefinitionRepository, IBlogTagRepository blogTagRepository)
    {
        _blogRepository = blogRepository;
        _userManager = userManager;
        _azureBlobService = azureBlobService;
        _likeRepository = likeRepository;
        _tagDefinitionRepository = tagDefinitionRepository;
        _blogTagRepository = blogTagRepository;
    }
    
    public async Task<WindingBlog> AddBlogAsync(BlogDto blogDto, string userId)
    {
        // name the blob
        var blobName = $"{Guid.NewGuid()}.txt";
        // add blog to blob
        using var contentStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(blogDto.Content));
        var blobUrl = await _azureBlobService.UploadBlobAsync(blobName, contentStream);
        var newBlog = new WindingBlog
        {
            Title = blogDto.Title,
            Description = blogDto.Description,
            CreatedAt = DateTime.UtcNow,
            BlobStorageUrl = blobUrl,
            UserId = userId
        };
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            List<int> tagDefinitionIds = new List<int>();
            List<BlogTag> newBlogTags = new List<BlogTag>();
            List<TagDefinition> newTagDefinitions = new List<TagDefinition>();
            
            // add the blog
            var windingBlog = await _blogRepository.AddAsync(newBlog);
            // add tags
            foreach (var tag in blogDto.Tags)
            {
                var tagDefinition = await _tagDefinitionRepository.FindTagDefintionWithTagName(tag);
                if (tagDefinition == null)
                {
                    var newTag = new TagDefinition {
                        TagName = tag
                    };
                    newTagDefinitions.Add(newTag);
                }
                else
                {
                    tagDefinitionIds.Add(tagDefinition.TagDefinitionId);
                }
            }
            var tgList = await _tagDefinitionRepository.AddList(newTagDefinitions);
            foreach (var tg in tgList)
            {
                tagDefinitionIds.Add(tg.TagDefinitionId);
            }

            foreach (var tagId in tagDefinitionIds)
            {
                var tagBlog = new BlogTag
                {
                    BlogId = windingBlog.BlogId,
                    TagDefinitionId = tagId,
                };
                newBlogTags.Add(tagBlog);
            }
            var tgBlog = await _blogTagRepository.AddList(newBlogTags);
            
            scope.Complete();
        }
        return newBlog;
    }

    public async Task<bool> UdateBlogAsync(BlogDto blogDto, int blogId)
    {
        var blog = await _blogRepository.GetByIdAsync(blogId);
        if (blog == null) return false;
        
        using var contentStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(blogDto.Content));
        var blobName = Path.GetFileName(new Uri(blog.BlobStorageUrl).AbsolutePath);
        await _azureBlobService.DeleteBlobAsync(blobName); // Delete old blob
        string newBlobUrl = await _azureBlobService.UploadBlobAsync(blobName, contentStream); // Upload new blob
        
        blog.BlobStorageUrl = newBlobUrl;
        blog.Title = blogDto.Title;
        blog.Description = blogDto.Description;
        await _blogRepository.UpdateAsync(blog);
        return true;
    }

    public async Task<BlogDto> DeleteBlogAsync(int blogId)
    {
        var blog = await _blogRepository.GetByIdAsync(blogId);
        if (blog == null) throw new Exception("file not found");

        // Delete associated blob from Azure Blob Storage
        var blobName = Path.GetFileName(new Uri(blog.BlobStorageUrl).AbsolutePath);

        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                await _azureBlobService.DeleteBlobAsync(blobName);
                await _blogRepository.DeleteAsync(blogId);
                scope.Complete();
            }
            catch (Exception ex)
            {
                throw new Exception("delete failed");
            }
        }
        return new BlogDto
        {
            Title = blog.Title,
            Description = blog.Description
        };
    }

    public async Task<IEnumerable<BlogListResponseDto>> FetchPaginatedBlogs(int page)
    {
        // collect blog data
        // collect likes
        // collect tags
        // collect author
        IEnumerable<WindingBlog> windingBlogs = await _blogRepository.GetPageBlogs(page);
        List<BlogListResponseDto> blogListDtos = new List<BlogListResponseDto>();

        foreach (var blog in windingBlogs)
        {
            var fetchBlogListDto = new BlogListResponseDto
            {
                BlogId = blog.BlogId,
                Title = blog.Title,
                Description = blog.Description,
                Tags = _blogTagRepository.GetAllBlogTags(blog.BlogId),
                Author = blog.User.FirstName + " " + blog.User.LastName, 
                Likes = await GetBlogLikesAsync(blog.BlogId),
            };
            blogListDtos.Add(fetchBlogListDto);
        }

        return blogListDtos;
    }

    public async Task<BlogDto> GetBlog(int id)
    {
        var blog = await _blogRepository.GetByIdAsync(id);
        var user = await _userManager.FindByIdAsync(blog.UserId);
        var blobName = Path.GetFileName(new Uri(blog.BlobStorageUrl).AbsolutePath);
        var blogContent = await _azureBlobService.GetBlobContentAsync(blobName);
        return new BlogDto
        {
            Title = blog.Title,
            PublishDate = blog.CreatedAt,
            Description = blog.Description,
            Tags = _blogTagRepository.GetAllBlogTags(blog.BlogId),
            Content = blogContent,
            Author = user.FirstName + " " + user.LastName,
            Likes = await GetBlogLikesAsync(blog.BlogId),
        };
    }

    public async Task<bool> LikePost(int id, string userId)
    {
        var like = await _likeRepository.FindBlogLikedByUserAsync(id, userId);
        if (like == null)
        {
            // add like
            await _likeRepository.AddAsync(new Like
            {
                UserId = userId,
                BlogId = id
            });
            return true;
        }

        await _likeRepository.DeleteAsync(like.LikeId);
        return false;

    }

    public async Task<bool> CheckLikedByUser(int blogId, string userId)
    {
        var liked = await _likeRepository.FindBlogLikedByUserAsync(blogId, userId);
        if (liked == null)
        {
            return false;
        }

        return true;
    }

    public async Task<IEnumerable<BlogListResponseDto>> GetMyBlogs(string userId)
    {
        var windingBlogs = await _blogRepository.GetMyBlogs(userId);
        List<BlogListResponseDto> blogListDtos = new List<BlogListResponseDto>();
        
        foreach (var blog in windingBlogs)
        {
            var fetchBlogListDto = new BlogListResponseDto
            {
                BlogId = blog.BlogId,
                Title = blog.Title,
                Description = blog.Description,
                Tags = _blogTagRepository.GetAllBlogTags(blog.BlogId),
                Author = blog.User.FirstName + " " + blog.User.LastName, 
                Likes = await GetBlogLikesAsync(blog.BlogId),
            };
            blogListDtos.Add(fetchBlogListDto);
        }

        return blogListDtos;
    }
    
    # region privatemethods

    private Task<int> GetBlogLikesAsync(int blogId)
    {
        return _likeRepository.GetLikesForBlogAsync(blogId);
    }

    #endregion
}