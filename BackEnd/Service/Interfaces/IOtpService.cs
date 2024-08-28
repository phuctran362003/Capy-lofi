
namespace Service.Interfaces;

public interface IOtpService
{
    string GenerateOtp();
    string HashOtp(string otp);
    Task<bool> ValidateOtpAsync(User user, string otp);
    Task SaveOtpAsync(User user, string otp);
}