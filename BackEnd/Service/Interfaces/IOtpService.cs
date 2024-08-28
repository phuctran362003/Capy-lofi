
using Repository.Commons;

namespace Service.Interfaces;

public interface IOtpService
{
    string GenerateOtp();
    Task<ApiResult<bool>> ValidateOtpAsync(User user, string otp);
    Task<ApiResult<bool>> SaveOtpAsync(User user, string otp);
}