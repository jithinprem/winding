using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using windingApi.Models;

namespace windingApi.Data;

public class IdContext: IdentityDbContext<User>
{
    public IdContext(DbContextOptions<IdContext> options) : base(options)
    {
        
    }
}