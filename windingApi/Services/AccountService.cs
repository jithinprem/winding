using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using windingApi.Models;
using windingApi.Services.Interfaces;

public class AccountService: IAccountService
{
    private UserManager<User> _userManager;
    private IEmailSender _emailSender;
    private IConfiguration _configuration;
    
    public AccountService(UserManager<User> userManager, IEmailSender emailSender, IConfiguration configuration)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _configuration = configuration;
    }
    public async Task<bool> CheckUserExistAsync(string email)
    {
        var currUser = await _userManager.FindByEmailAsync(email);
        if (currUser == null)
        {
            return false;
        }
        return true;
    }

    public async Task<bool> SendConfirmEmail(User user)
    {
        // generate token from user object
        // encode token
        // set url to be attached with email and token to be confirmed 
        // set body of the email
        // create a emailSendDto and use EmailSendService which makes use of Mailjet to send email
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var url = $"{_configuration["Email:ClientUrl"]}/{_configuration["Email:TokenVerifyPath"]}?token={token}&email={user.Email}";
        
        var body = $"<p>Hello: {user.FirstName} {user.LastName} </p>" +
                   $"<p>Please confirm your email address by clicking on the following link.</p>" +
                   $"<p><a href=\"{url}\">click here</a></p>" +
                   $"<p>Thank you,</p>" +
                   $"<br>{_configuration["Email:ApplicationName"]}";

        try
        {
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email", body);
            return true;
        }
        catch (Exception exception)
        {
            return false;
        }
        
    }
}