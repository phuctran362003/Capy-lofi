using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Repository.Commons;
using Repository.Interfaces;
using Service.Interfaces;

public class OtpService : IOtpService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<OtpService> _logger;

    public OtpService(IUserRepository userRepository, ILogger<OtpService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    // Tạo OTP ngẫu nhiên và thiết lập thời gian hết hạn
    public string GenerateOtp()
    {
        _logger.LogInformation("Generating a new secure OTP.");
        using (var rng = new RNGCryptoServiceProvider())
        {
            var randomBytes = new byte[4]; 
            rng.GetBytes(randomBytes);
            var randomValue = BitConverter.ToUInt32(randomBytes, 0) % 1000000;
            return randomValue.ToString("D6"); 
        }
    }

    // Lưu OTP và thời gian hết hạn vào cơ sở dữ liệu
    public async Task<ApiResult<bool>> SaveOtpAsync(User user, string otp)
    {
        if (user == null)
        {
            _logger.LogWarning("SaveOtpAsync called with null user.");
            return ApiResult<bool>.Error(false, "User is null");
        }

        if (string.IsNullOrEmpty(otp) || otp.Length != 6 || !otp.All(char.IsDigit))
        {
            _logger.LogWarning("Invalid OTP format for user {UserId}. OTP: {Otp}", user.Id, otp);
            return ApiResult<bool>.Error(false, "Invalid OTP format");
        }

        try
        {
            _logger.LogInformation("Saving OTP for user {UserId}.", user.Id);

            // Thiết lập thời gian hết hạn OTP là 3 phút từ bây giờ
            user.OtpExpiryTime = DateTime.UtcNow.AddMinutes(3).ToString("o", CultureInfo.InvariantCulture);
            user.Otp = otp;

            await _userRepository.UpdateOtpAsync(user, otp);
            _logger.LogInformation("OTP saved successfully for user {UserId}.", user.Id);

            return ApiResult<bool>.Succeed(true, "OTP saved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving OTP for user {UserId}.", user.Id);
            return ApiResult<bool>.Fail(ex);
        }
    }

    // Xác thực OTP và kiểm tra thời gian hết hạn
    // ValidateOtpAsync Method
    public async Task<ApiResult<bool>> ValidateOtpAsync(User user, string otp)
    {
        if (user == null)
        {
            _logger.LogWarning("ValidateOtpAsync called with null user.");
            return ApiResult<bool>.Error(false, "User is null");
        }

        try
        {
            var storedOtp = user.Otp;
            var storedExpiryTime = user.OtpExpiryTime;

            _logger.LogDebug("Stored OTP for user {UserId} is: {StoredOtp}", user.Id, storedOtp);
            _logger.LogDebug("Stored OTP expiry time for user {UserId} is: {StoredExpiryTime}", user.Id, storedExpiryTime);

            if (string.IsNullOrEmpty(storedOtp) || string.IsNullOrEmpty(storedExpiryTime))
            {
                _logger.LogWarning("No OTP data found for user {UserId}.", user.Id);
                return ApiResult<bool>.Error(false, "No OTP data found");
            }

            if (DateTime.TryParse(storedExpiryTime, null, DateTimeStyles.RoundtripKind, out DateTime expiryTime))
            {
                if (DateTime.UtcNow > expiryTime)
                {
                    _logger.LogInformation("OTP has expired for user {UserId}.", user.Id);
                    return ApiResult<bool>.Error(false, "OTP has expired");
                }
            }
            else
            {
                _logger.LogWarning("Invalid OTP expiry time format for user {UserId}.", user.Id);
                return ApiResult<bool>.Error(false, "Invalid OTP expiry time format");
            }

            if (storedOtp == otp)
            {
                _logger.LogInformation("OTP is valid for user {UserId}.", user.Id);
                return ApiResult<bool>.Succeed(true, "OTP is valid");
            }
            else
            {
                _logger.LogInformation("Invalid OTP for user {UserId}.", user.Id);
                return ApiResult<bool>.Error(false, "Invalid OTP");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while validating OTP for user {UserId}.", user.Id);
            return ApiResult<bool>.Fail(ex);
        }
    }



}
