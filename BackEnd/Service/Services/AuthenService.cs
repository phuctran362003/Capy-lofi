using Microsoft.AspNetCore.Identity;
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
        public AuthenService(IUserRepository userRepository, IOtpService otpService, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _otpService = otpService;
            _passwordHasher = passwordHasher;
        }



        public async Task<ApiResult<User>> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _userRepository.GetUseByUserName(username);
                if (user == null)
                {
                    return ApiResult<User>.Error(null, "User not found");
                }

                // Use PasswordHasher to verify the password
                var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

                if (verificationResult == PasswordVerificationResult.Success)
                {
                    return ApiResult<User>.Succeed(user, "Login Successful");
                }
                else
                {
                    return ApiResult<User>.Error(null, "Invalid Password");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<User>.Fail(ex);
            }
        }



        public async Task<ApiResult<User>> VerifyOtpAsync(string email, string otpCode)
        {
            try
            {
                // Validate email and otpCode
                if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                {
                    return ApiResult<User>.Error(null, "Invalid email.");
                }
                if (string.IsNullOrWhiteSpace(otpCode))
                {
                    return ApiResult<User>.Error(null, "OTP code cannot be empty.");
                }

                var user = await _userRepository.GetUserByEmailAsync(email); // This is probably null

                if (user == null)
                {
                    return ApiResult<User>.Error(null, "User not found.");  // Add this check to handle null user
                }

                var otpValidationResult = await _otpService.ValidateOtpAsync(user, otpCode);
                if (!otpValidationResult.Success || !otpValidationResult.Data)
                {
                    return ApiResult<User>.Error(null, "Invalid OTP.");
                }

                return ApiResult<User>.Succeed(user, "OTP verified successfully.");
            }
            catch (Exception ex)
            {
                return ApiResult<User>.Fail(ex);
            }
        }


        public async Task<ApiResult<bool>> UpdateRefreshTokenAsync(User user, string refreshToken)
        {
            try
            {
                // Validate user and refreshToken
                if (user == null)
                {
                    return ApiResult<bool>.Error(false, "User cannot be null.");
                }
                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return ApiResult<bool>.Error(false, "Refresh token cannot be empty.");
                }

                await _userRepository.UpdateRefreshTokenAsync(user, refreshToken);
                return ApiResult<bool>.Succeed(true, "Refresh token updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex);
            }
        }

        private bool IsValidEmail(string email)
        {
            // Simple email validation logic (can be replaced with more robust logic)
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }




    }
}
