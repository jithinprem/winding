using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using windingApi.Constants;
using windingApi.Data;
using windingApi.Models;

namespace windingApi.Services;

public class ContextSeedService
{
    private readonly IdContext _idContext;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public ContextSeedService(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IConfiguration configuration, IdContext idContext)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
        _idContext = idContext;
    }
    
    public async Task InitializeContextAsync()
    {
        if (_idContext.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count() > 0)
        {
            await _idContext.Database.MigrateAsync();
        }

        if (!_roleManager.Roles.Any())
        {
            await _roleManager.CreateAsync(new IdentityRole { Name = AccountConstants.AdminRole });
            await _roleManager.CreateAsync(new IdentityRole { Name = AccountConstants.GenericUserRole });
        }

        if (!_userManager.Users.AnyAsync().GetAwaiter().GetResult())
        {
            var admin = new User
            {
                FirstName = "admin",
                LastName = "user",
                Email = AccountConstants.AdminUserName,
                EmailConfirmed = true,
            };
            await _userManager.CreateAsync(admin, _configuration["AdminPassword"]);
            await _userManager.AddToRolesAsync(admin,
                new[] { AccountConstants.AdminRole, AccountConstants.GenericUserRole });
            await _userManager.AddClaimsAsync(admin, new Claim[]
            {
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim(ClaimTypes.GivenName, admin.FirstName),
                new Claim(ClaimTypes.Surname, admin.LastName)
            });
            
            var genericUser = new User
            {
                FirstName = "generic",
                LastName = "user",
                Email = AccountConstants.DefaultGenericUserName,
                EmailConfirmed = true
            };
            
            await _userManager.CreateAsync(genericUser, _configuration["AdminPassword"]);
            await _userManager.AddToRolesAsync(genericUser,
                new[] { AccountConstants.GenericUserRole });
            await _userManager.AddClaimsAsync(genericUser, new Claim[]
            {
                new Claim(ClaimTypes.Email, genericUser.Email),
                new Claim(ClaimTypes.GivenName, genericUser.FirstName),
                new Claim(ClaimTypes.Surname, genericUser.LastName)
            });
        }
    }
    
}