using System.Collections.Generic;
using windingApi.Models;

namespace windingApi.Controller.Repository.RepositoryInterfaces;

public interface IBlogTagRepository: IRepository<BlogTag, int>
{
    public IEnumerable<string> GetAllBlogTags(int blogId);
}