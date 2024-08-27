using System.Security.Claims;
using System.Threading.Tasks;
using Domain.DTOs.UserDTOs;
using Domain.Entities;
using Repository.Commons;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly EmailService _emailService;

        public UserService(IUserRepository userRepository, EmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<ApiResult<User>> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return ApiResult<User>.Error(null, "User not found.");
                }
                return ApiResult<User>.Succeed(user, "User retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResult<User>.Fail(ex);
            }
        }

        public async Task<ApiResult<UserDto>> CreateOrUpdateUserAndSendOtpAsync(string email, string name)
        {
            try
            {
                // Generate a 6-digit OTP
                var otpCode = GenerateRandomOtp();
                var user = await _userRepository.GetUserByEmailAsync(email);

                if (user != null)
                {
                    // User exists, update OTP
                    await _userRepository.UpdateOtpAsync(user, otpCode);
                }
                else
                {
                    // User does not exist, create new user with default values
                    user = new User
                    {
                        Email = email,
                        UserName = email,
                        Name = name ?? string.Empty,
                        Otp = otpCode,
                        DisplayName = name ?? string.Empty,
                        EmailConfirmed = true
                    };

                    var result = await _userRepository.CreateUserAsync(user);
                    if (!result.Succeeded)
                    {
                        return ApiResult<UserDto>.Error(null, "Failed to create user.");
                    }
                }

                // Send OTP via email
                await _emailService.SendOtpAsync(email, otpCode);

                // Create a DTO with only the necessary fields
                var userDto = new UserDto
                {
                    Name = user.Name,
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                };

                return ApiResult<UserDto>.Succeed(userDto, "OTP sent successfully.");
            }
            catch (Exception ex)
            {
                return ApiResult<UserDto>.Fail(ex);
            }
        }



        public async Task<ApiResult<string>> GenerateOtpCodeAsync(string email)
        {
            try
            {
                var otpCode = GenerateRandomOtp();
                var user = await _userRepository.GetUserByEmailAsync(email);

                if (user != null)
                {
                    await _userRepository.UpdateOtpAsync(user, otpCode);
                }
                else
                {
                    user = new User { Email = email, UserName = email, Otp = otpCode };
                    var result = await _userRepository.CreateUserAsync(user);
                    if (!result.Succeeded)
                    {
                        return ApiResult<string>.Error(null, "Failed to create user for OTP generation.");
                    }
                }

                return ApiResult<string>.Succeed(otpCode, "OTP generated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResult<string>.Fail(ex);
            }
        }

        public async Task<ApiResult<User>> VerifyOtpAsync(string email, string otpCode)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(email);

                if (user == null || !await _userRepository.VerifyOtpAsync(user, otpCode))
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
                await _userRepository.UpdateRefreshTokenAsync(user, refreshToken);
                return ApiResult<bool>.Succeed(true, "Refresh token updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex);
            }
        }

        private string GenerateRandomOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6-digit OTP
        }
    }


}