using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Repository.Commons;
using Service.Interfaces;

public class OtpService : IOtpService
{
    private readonly UserManager<User> _userManager;
    private readonly EmailService _emailService;
    private readonly ILogger<OtpService> _logger;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;

    public OtpService(UserManager<User> userManager, EmailService emailService, ILogger<OtpService> logger, 
                      IPasswordHasher<User> passwordHasher, ITokenService tokenService)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    public async Task<ApiResult<string>> GenerateOtpAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return ApiResult<string>.Error(null, "User not found."); // Không tạo người dùng mới ở đây
            }

            var otpCode = new Random().Next(100000, 999999).ToString();
            var hashedOtp = _passwordHasher.HashPassword(user, otpCode);
            user.Otp = hashedOtp;
            await _userManager.UpdateAsync(user);

            await _emailService.SendOtpEmailAsync(email, otpCode);

            return ApiResult<string>.Succeed(otpCode, "OTP generated and sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating OTP for user with email {Email}", email);
            return ApiResult<string>.Fail(ex);
        }
    }

    public async Task<ApiResult<string>> ValidateOtpAsync(string email, string otp)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found.", email);
                return ApiResult<string>.Error(null, "User not found");
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Otp, otp);
            if (verificationResult == PasswordVerificationResult.Success)
            {
                user.Otp = null;
                await _userManager.UpdateAsync(user);

                return ApiResult<string>.Succeed(null, "OTP validated successfully.");
            }
            else
            {
                return ApiResult<string>.Error(null, "Invalid OTP");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating OTP for user with email {Email}", email);
            return ApiResult<string>.Fail(ex);
        }
    }
}
