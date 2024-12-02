using System.Collections.Generic;
using System.Threading.Tasks;
using windingApi.Models;

namespace windingApi.Controller.Repository.RepositoryInterfaces;

public interface ILikeRepository: IRepository<Like, int>
{
    public Task<int> GetLikesForBlogAsync(int blogId);
    public Task<Like> FindBlogLikedByUserAsync(int blogId, string userId);
}