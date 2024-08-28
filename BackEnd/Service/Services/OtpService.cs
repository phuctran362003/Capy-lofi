using Repository.Commons;
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

    // Lưu OTP đã băm và thời gian tạo vào cơ sở dữ liệu
    public async Task<ApiResult<bool>> SaveOtpAsync(User user, string otp)
    {
        if (user == null)
        {
            return ApiResult<bool>.Error(false, "User is null");
        }

        try
        {
            var hashedOtp = HashOtp(otp);
            var otpData = $"{hashedOtp}:{DateTime.UtcNow:o}"; // Kết hợp OTP băm và thời gian tạo
            await _userRepository.UpdateOtpAsync(user, otpData);
            return ApiResult<bool>.Succeed(true, "OTP saved successfully");
        }
        catch (Exception ex)
        {
            return ApiResult<bool>.Fail(ex);
        }
    }

    // Xác thực OTP đã băm và kiểm tra thời gian hết hạn
    public async Task<ApiResult<bool>> ValidateOtpAsync(User user, string otp)
    {
        if (user == null)
        {
            return ApiResult<bool>.Error(false, "User is null");
        }

        try
        {
            var otpData = user.Otp; // Lấy dữ liệu OTP từ người dùng

            if (string.IsNullOrEmpty(otpData))
            {
                return ApiResult<bool>.Error(false, "OTP data is missing");
            }

            var parts = otpData.Split(':');
            if (parts.Length != 2)
            {
                return ApiResult<bool>.Error(false, "Invalid OTP data format");
            }

            var storedHashedOtp = parts[0];
            var storedGeneratedTime = DateTime.Parse(parts[1], null, System.Globalization.DateTimeStyles.RoundtripKind);

            // Kiểm tra thời gian OTP có quá 3 phút không
            if (DateTime.UtcNow > storedGeneratedTime.AddMinutes(3))
            {
                return ApiResult<bool>.Error(false, "OTP has expired");
            }

            // Băm mã OTP nhập vào để so sánh
            var hashedOtp = HashOtp(otp);
            if (storedHashedOtp == hashedOtp)
            {
                return ApiResult<bool>.Succeed(true, "OTP is valid");
            }
            else
            {
                return ApiResult<bool>.Error(false, "Invalid OTP");
            }
        }
        catch (Exception ex)
        {
            return ApiResult<bool>.Fail(ex);
        }
    }
}






