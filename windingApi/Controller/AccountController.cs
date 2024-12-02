using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using windingApi.Constants;
using windingApi.DTO;
using windingApi.Models;
using windingApi.Services;
using windingApi.Services.Interfaces;

namespace windingApi.Controller;

[ApiController]
[Route("api/[controller]")]

public class AccountController: Microsoft.AspNetCore.Mvc.Controller
{

    private readonly UserManager<User> _userManager;
    private readonly IAccountService _accountService;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtService _jwtService;

    public AccountController(UserManager<User> userManager, IAccountService accountService, SignInManager<User> signInManager, JwtService jwtService)
    {
        _userManager = userManager;
        _accountService = accountService;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
    {
        // get the userRegisterDto + validation happens
        // check if the user exist, if there are any model errors and respond
        // create user object and create user via userManager
        // if not succeeded return, else adding roles , assign generalUser role
        // add roles
        // send email
        // use exception handling to catch any issues

        if (await _accountService.CheckUserExistAsync(registerUserDto.Email.ToLower()))
        {
            return BadRequest(new JsonResult(new { title = "registration failed", message = "account already exist" }));
        }
        
        var newUser = new User
        {
            UserName = registerUserDto.Email.ToLower(),
            FirstName = registerUserDto.FirstName.ToLower(),
            LastName = registerUserDto.LastName.ToLower(),
            Email = registerUserDto.Email.ToLower(),

        };
        var result = await _userManager.CreateAsync(newUser, password: registerUserDto.Password);
        await _userManager.AddToRoleAsync(newUser, AccountConstants.GenericUserRole);
        if (!result.Succeeded)
        {
            return BadRequest(new JsonResult(new {title="account creation failed", message="please contact admin"}));
        }

        try
        {
            if (!await _accountService.SendConfirmEmail(newUser))
            {
                return BadRequest(new JsonResult(new { title = "failed", message = "account creation failed" }));
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new JsonResult(new { title = "failed", message = "account creation failed" }));
        }

        return Ok(new JsonResult(
            new { title = "account Created", message = "your account is successfully created" }));

    }

    [HttpPost("register-with-third-party")]
    public async Task<ActionResult> LoginWithThirdParty()
    {
        // google
        // the frontend will have the service to open google authentication and provide us with 
        // required model with accessToken, and userId 
        // validate the accessToken and userId with google using googleService
        // create an account with the UserName as the userId and given password, it will have no email, and email confirmed is set to true
        // if already have account direct to login with that account
        return null;
    }

    [Authorize]
    [HttpGet("refresh-token")]
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
        var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
        var user = await _userManager.FindByEmailAsync(emailClaim);
        if (await _userManager.IsLockedOutAsync(user))
        {
            return Unauthorized("you have been locked out");
        }

        return await CreateApplicationUserDto(user);
    }

    [HttpPost("verify-token")]
    public async Task<IActionResult> VerifyEmailToken(EmailConfirmDto emailConfirmDto)
    {
        // Decode the encrypted token
        // check if the email exist and the ConfirmedEmail is false
        // if false make it true and retrun success
        // handle other scenarious like already confirmed, no user found, failed etc
        var user = await _userManager.FindByEmailAsync(emailConfirmDto.Email.ToLower());
        if (user == null) return Unauthorized("this email is not registered yet");
        if (user.EmailConfirmed) return BadRequest("email was confirmed");
        try
        {
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(emailConfirmDto.Token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (result.Succeeded)
            {
                return Ok(new JsonResult( new {message="email confirmed"}));
            }

            return BadRequest("email confirmation failed");
        }
        catch (Exception ex)
        {
            return BadRequest("email confirmation failed");
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        // check if the account exist
        // check email confirmed redirect accordingly in FE
        // check if the account is in lockout state
        // if account exist email confirmed check password if succeeded proceed 
        // if wrong password add to the lockout count
        if (! await _accountService.CheckUserExistAsync(loginDto.Email.ToLower()))
        {
            return Unauthorized("Invalid username or password");
        }

        var user = await _userManager.FindByEmailAsync(loginDto.Email.ToLower());
        if (!user.EmailConfirmed) return Unauthorized("please confirm your mail");
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (result.IsLockedOut)
        {
            return Unauthorized($"Your account has been locked, should wait till {user.LockoutEnd} UTC to enable login");
        }

        if (!result.Succeeded)
        {
            // if not admin add to lockout count
            if (! user.UserName.Equals(AccountConstants.AdminUserName))
            {
                // incrementing AccessFailedCount
                await _userManager.AccessFailedAsync(user);
            }

            if (user.AccessFailedCount > AccountConstants.maxAllowedFailedLoginAttempts)
            {
                // lock the account for a day
                await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(1));
                return Unauthorized($"your account has been locked, please try again after UTC {user.LockoutEnd}");
            }

            return Unauthorized("Invalid username or password");
        }

        await _userManager.ResetAccessFailedCountAsync(user);
        await _userManager.SetLockoutEndDateAsync(user, null);

        return await CreateApplicationUserDto(user);

    }


    #region HelperMethods

    private async Task<UserDto> CreateApplicationUserDto(User user)
    {
        return new UserDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            JWT = await _jwtService.CreateJwtToken(user)
        };
    }

    #endregion

}