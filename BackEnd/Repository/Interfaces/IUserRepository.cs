using Microsoft.AspNetCore.Identity;

namespace Repository.Interfaces;

public interface IUserRepository
{
    Task<User> GetUserByIdAsync(int userId);
    Task<User> GetUseByUserName(string userName);
    Task<User> GetUserByEmailAsync(string email);
    Task<IdentityResult> CreateUserAsync(User user);
    Task UpdateOtpAsync(User user, string otpCode);
    Task<bool> VerifyOtpAsync(User user, string otpCode);
    Task UpdateRefreshTokenAsync(User user, string refreshToken);
}