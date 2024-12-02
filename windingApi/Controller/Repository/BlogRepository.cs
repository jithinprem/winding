using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using windingApi.Constants;
using windingApi.Controller.Repository.RepositoryInterfaces;
using windingApi.Data;
using windingApi.Migrations;
using WindingBlog = windingApi.Models.WindingBlog;

namespace windingApi.Controller.Repository;

public class BlogRepository: Repository<WindingBlog, int>, IBlogRepository
{
    private readonly IdContext _context;
    private readonly DbSet<WindingBlog> _blogsDbSet;

    public BlogRepository(IdContext context) : base(context)
    {
        _context = context;
        _blogsDbSet = context.Blogs;
    }

    public async Task<IEnumerable<WindingBlog>> GetPageBlogs(int page)
    {
        page -= 1;
        return await _context.Blogs
            .Include(blog => blog.User)
            .Skip(page * AccountConstants.BlogPageSize)
            .Take(AccountConstants.BlogPageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<WindingBlog>> GetMyBlogs(string userId)
    {
        return await _blogsDbSet.Where(blog => blog.UserId == userId).ToListAsync();
    }
}