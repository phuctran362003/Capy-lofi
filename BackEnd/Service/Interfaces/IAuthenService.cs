using Repository.Commons;

namespace Service.Interfaces;

public interface IAuthenService
{
    Task<ApiResult<User>> VerifyOtpAsync(string email, string otpCode);
    Task<ApiResult<bool>> UpdateRefreshTokenAsync(User user, string refreshToken);
    Task<ApiResult<User>> LoginAsync(string username, string password);

}
