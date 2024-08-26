using Repository.Commons;

namespace Service.Interfaces;

public interface IOtpService
{
    Task<ApiResult<string>> GenerateOtpAsync(string email);
    Task<ApiResult<string>> ValidateOtpAsync(string email, string otp);
}
