using Domain.DTOs.UserDTOs;
using Repository.Commons;
using Repository.Interfaces;
using Service.Interfaces;


public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly EmailService _emailService;
    private readonly IOtpService _otpService;

    public UserService(IUserRepository userRepository, EmailService emailService, IOtpService otpService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _otpService = otpService;
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
            var otpCode = _otpService.GenerateOtp();
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user != null)
            {
                await _otpService.SaveOtpAsync(user, otpCode);
            }
            else
            {
                user = new User
                {
                    Email = email,
                    UserName = email,
                    Name = name ?? string.Empty,
                    DisplayName = name ?? string.Empty,
                    EmailConfirmed = true
                };

                var result = await _userRepository.CreateUserAsync(user);
                if (!result.Succeeded)
                {
                    return ApiResult<UserDto>.Error(null, "Failed to create user.");
                }

                await _otpService.SaveOtpAsync(user, otpCode);
            }

            await _emailService.SendOtpAsync(email, otpCode);

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

    public async Task<ApiResult<User>> VerifyOtpAsync(string email, string otpCode)
    {
        try
        {
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null || !await _otpService.ValidateOtpAsync(user, otpCode))
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
}
