using System.Threading.Tasks;
using windingApi.Models;

namespace windingApi.Services.Interfaces;

public interface IAccountService
{
    public Task<bool> CheckUserExistAsync(string email);
    public Task<bool> SendConfirmEmail(User user);
}