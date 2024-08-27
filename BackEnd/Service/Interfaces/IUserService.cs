using Domain.DTOs.UserDTOs;
using Domain.Entities;
using Microsoft.Identity.Client;
using Repository.Commons;

namespace Service.Interfaces;

public interface IUserService
{
    Task<ApiResult<User>> GetUserByIdAsync(int userId);
    Task<ApiResult<User>> VerifyOtpAsync(string email, string otpCode);
    Task<ApiResult<UserDto>> CreateOrUpdateUserAndSendOtpAsync(string email, string name);
    Task<ApiResult<bool>> UpdateRefreshTokenAsync(User user, string refreshToken);
    Task<ApiResult<string>> GenerateOtpCodeAsync(string email);
}



