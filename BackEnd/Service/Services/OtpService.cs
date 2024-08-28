using Repository.Interfaces;
using Service.Interfaces;
using System.Security.Cryptography;
using System.Text;


public class OtpService : IOtpService
{
    private readonly IUserRepository _userRepository;

    public OtpService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // Tạo OTP ngẫu nhiên
    public string GenerateOtp()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString(); // 6-digit OTP
    }

    // Băm OTP sử dụng SHA256
    public string HashOtp(string otp)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(otp));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    // Lưu OTP đã băm vào cơ sở dữ liệu
    public async Task SaveOtpAsync(User user, string otp)
    {
        var hashedOtp = HashOtp(otp);
        await _userRepository.UpdateOtpAsync(user, hashedOtp);
    }

    // Xác thực OTP đã băm
    public async Task<bool> ValidateOtpAsync(User user, string otp)
    {
        var hashedOtp = HashOtp(otp);
        return await _userRepository.VerifyOtpAsync(user, hashedOtp);
    }
}

