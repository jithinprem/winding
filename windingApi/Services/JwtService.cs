using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using windingApi.Models;

namespace windingApi.Services;

public class JwtService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _jwt;
    private readonly UserManager<User> _userManager;
    
    
    public JwtService(IConfiguration configuration, UserManager<User> userManager)
    {
        _configuration = configuration;
        // generate the key
        _jwt = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
        _userManager = userManager;

    }

    public async Task<string> CreateJwtToken(User user)
    {
        // make claims
        // put claims, expires, credential etc into a package
        // tokenDescriptor packs it
        // tokenHandler constructs token

        var userClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.UserName),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName)
        };

        var roles = await _userManager.GetRolesAsync(user);
        userClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var credential = new SigningCredentials(_jwt, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(userClaims),
            Issuer = _configuration.GetSection("JWT")["Issuer"],
            SigningCredentials = credential,
            Expires = DateTime.UtcNow.AddDays(Int32.Parse(_configuration.GetSection("JWT")["ExpiresInDays"]))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(jwt);

    }
}