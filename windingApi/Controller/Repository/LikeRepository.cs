using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using windingApi.Controller.Repository.RepositoryInterfaces;
using windingApi.Data;
using windingApi.Models;

namespace windingApi.Controller.Repository;

public class LikeRepository: Repository<Like, int> ,ILikeRepository
{
    private readonly DbSet<Like> _likeDbSet;
    
    public LikeRepository(IdContext context) : base(context)
    {
        _likeDbSet = context.Likes;
    }
    
    public Task<int> GetLikesForBlogAsync(int blogId)
    {
        return _likeDbSet.CountAsync(like => like.BlogId == blogId);
    }

    public async Task<Like> FindBlogLikedByUserAsync(int blogId, string userId)
    {
        return await _likeDbSet.FirstOrDefaultAsync(like => like.UserId == userId && like.BlogId == blogId);
    }
}