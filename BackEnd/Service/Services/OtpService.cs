using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Service.Interfaces;

public class OtpService : IOtpService
{
    private readonly UserManager<User> _userManager;
    private readonly EmailService _emailService;  
    private readonly ILogger<OtpService> _logger;
    private readonly IPasswordHasher<User> _passwordHasher;

    public OtpService(UserManager<User> userManager, EmailService emailService, ILogger<OtpService> logger, IPasswordHasher<User> passwordHasher)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public async Task<string> GenerateOtpAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("User with email {Email} not found.", email);
            throw new ArgumentException("User not found");
        }

        // Generate a 6-digit OTP code
        var otpCode = new Random().Next(100000, 999999).ToString();

        // Hash the OTP before storing it
        var hashedOtp = _passwordHasher.HashPassword(user, otpCode);
        user.Otp = hashedOtp;
        await _userManager.UpdateAsync(user);

        // Gửi OTP đến email của người dùng bằng phương thức SendOtpEmailAsync từ EmailService
        await _emailService.SendOtpEmailAsync(email, otpCode);

        return otpCode;
    }

    public async Task<bool> ValidateOtpAsync(string email, string otp)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("User with email {Email} not found.", email);
            return false;
        }

        // Verify the OTP by comparing the hash
        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Otp, otp);
        if (verificationResult == PasswordVerificationResult.Success)
        {
            // OTP is valid, clear it from the database
            user.Otp = null;
            await _userManager.UpdateAsync(user);
            _logger.LogInformation("OTP for user {Email} validated successfully.", email);
            return true;
        }
        else
        {
            _logger.LogWarning("Invalid OTP for user {Email}.", email);
            return false;
        }
    }
}


