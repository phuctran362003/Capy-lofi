using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Repository.Commons;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Services
{
    public class AuthenService : IAuthenService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOtpService _otpService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AuthenService> _logger;

        public AuthenService(IUserRepository userRepository, IOtpService otpService, IPasswordHasher<User> passwordHasher, ILogger<AuthenService> logger)
        {
            _userRepository = userRepository;
            _otpService = otpService;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<ApiResult<User>> LoginAsync(string username, string password)
        {
            try
            {
                _logger.LogInformation("Login attempt for username: {username}", username);

                var user = await _userRepository.GetUseByUserName(username);
                if (user == null)
                {
                    _logger.LogWarning("User not found for username: {username}", username);
                    return ApiResult<User>.Error(null, "User not found");
                }

                // Use PasswordHasher to verify the password
                var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

                if (verificationResult == PasswordVerificationResult.Success)
                {
                    _logger.LogInformation("Login successful for username: {username}", username);
                    return ApiResult<User>.Succeed(user, "Login Successful");
                }
                else
                {
                    _logger.LogWarning("Invalid password for username: {username}", username);
                    return ApiResult<User>.Error(null, "Invalid Password");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for username: {username}", username);
                return ApiResult<User>.Fail(ex);
            }
        }

        public async Task<ApiResult<User>> VerifyOtpAsync(string email, string otpCode)
        {
            try
            {
                _logger.LogInformation("OTP verification attempt for email: {email}", email);

                // Validate email and otpCode
                if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                {
                    _logger.LogWarning("Invalid email provided: {email}", email);
                    return ApiResult<User>.Error(null, "Invalid email.");
                }

                if (string.IsNullOrWhiteSpace(otpCode))
                {
                    _logger.LogWarning("OTP code is empty for email: {email}", email);
                    return ApiResult<User>.Error(null, "OTP code cannot be empty.");
                }

                var user = await _userRepository.GetUserByEmailAsync(email);

                if (user == null)
                {
                    _logger.LogWarning("User not found for email: {email}", email);
                    return ApiResult<User>.Error(null, "User not found.");
                }

                var otpValidationResult = await _otpService.ValidateOtpAsync(user, otpCode);
                if (!otpValidationResult.Success || !otpValidationResult.Data)
                {
                    _logger.LogWarning("Invalid OTP for email: {email}", email);
                    return ApiResult<User>.Error(null, "Invalid OTP.");
                }

                _logger.LogInformation("OTP verified successfully for email: {email}", email);
                return ApiResult<User>.Succeed(user, "OTP verified successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during OTP verification for email: {email}", email);
                return ApiResult<User>.Fail(ex);
            }
        }

        public async Task<ApiResult<bool>> UpdateRefreshTokenAsync(User user, string refreshToken)
        {
            try
            {
                _logger.LogInformation("Refresh token update attempt for userId: {userId}", user?.Id);

                // Validate user and refreshToken
                if (user == null)
                {
                    _logger.LogWarning("User is null during refresh token update.");
                    return ApiResult<bool>.Error(false, "User cannot be null.");
                }

                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    _logger.LogWarning("Refresh token is empty for userId: {userId}", user.Id);
                    return ApiResult<bool>.Error(false, "Refresh token cannot be empty.");
                }

                await _userRepository.UpdateRefreshTokenAsync(user, refreshToken);
                _logger.LogInformation("Refresh token updated successfully for userId: {userId}", user.Id);

                return ApiResult<bool>.Succeed(true, "Refresh token updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during refresh token update for userId: {userId}", user?.Id);
                return ApiResult<bool>.Fail(ex);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                _logger.LogWarning("Invalid email format: {email}", email);
                return false;
            }
        }
    }

}
