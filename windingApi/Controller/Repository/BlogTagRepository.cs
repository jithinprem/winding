using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using windingApi.Data;
using windingApi.Models;

namespace windingApi.Controller.Repository.RepositoryInterfaces;

public class BlogTagRepository: Repository<BlogTag, int>, IBlogTagRepository
{
    private readonly DbSet<BlogTag> _dbSetBlogTag;
    private readonly DbSet<TagDefinition> _dbSetTagDefinition;

    public BlogTagRepository(IdContext context) : base(context)
    {
        _dbSetBlogTag = context.BlogTags;
        _dbSetTagDefinition = context.TagDefinitions;
    }


    public IEnumerable<string> GetAllBlogTags(int blogId)
    {
        return _dbSetBlogTag.Where(blogTag => blogTag.BlogId == blogId)
            .Join(
                _dbSetTagDefinition,
                blogTag => blogTag.TagDefinitionId,
                tagDefinition => tagDefinition.TagDefinitionId,
                (blogTag, tagDefinition) => tagDefinition.TagName
            );
    }
}