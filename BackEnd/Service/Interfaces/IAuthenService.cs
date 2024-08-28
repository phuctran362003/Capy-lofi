using Domain.DTOs;
using Domain.DTOs.Response;
using Domain.Entities;
using Newtonsoft.Json.Linq;
using Repository.Commons;

namespace Service.Interfaces;

public interface IAuthenService
{
    Task<ApiResult<User>> VerifyOtpAsync(string email, string otpCode);
    Task<ApiResult<bool>> UpdateRefreshTokenAsync(User user, string refreshToken);
}
