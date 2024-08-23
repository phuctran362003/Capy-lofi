using Domain.DTOs;
using Domain.DTOs.Response;
using Domain.Entities;
using Repository.Commons;

namespace Service.Interfaces;

public interface IAuthenticationService
{
    Task<ApiResult<Authenticator>> AuthenGoogleUser(string token);
    User GetUserById(int id);
    Task<ApiResult<Authenticator>> RefreshTokens(string accessToken, string refreshToken);
    Task<ApiResult<Authenticator>> VerifyOtpAndLoginAsync(string email, string otp);
}