using System.Collections.Generic;
using System.Threading.Tasks;
using windingApi.Models;

namespace windingApi.Controller.Repository.RepositoryInterfaces;

public interface IBlogRepository: IRepository<WindingBlog, int>
{
    public Task<IEnumerable<WindingBlog>> GetPageBlogs(int page);
    public Task<IEnumerable<WindingBlog>> GetMyBlogs(string userId);
}