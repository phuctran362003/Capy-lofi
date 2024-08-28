
using Repository.Commons;

namespace Service.Interfaces;

public interface IOtpService
{
    string GenerateOtp();
    string HashOtp(string otp);
    Task<ApiResult<bool>> ValidateOtpAsync(User user, string otp);
    Task<ApiResult<bool>> SaveOtpAsync(User user, string otp);
}