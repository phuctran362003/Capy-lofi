using Domain.Entities;
using Microsoft.Identity.Client;
using Repository.Commons;

namespace Service.Interfaces;

public interface IUserService
{
    Task<ApiResult<string>> CreateOrUpdateUserAndSendOtpAsync(string email, string name);
    Task<User> GetUserByIdAsync(int userId);
    Task UpdateUserAsync(User user);
    Task<ApiResult<string>> VerifyOtpAndLoginAsync(string email, string otp);
}

