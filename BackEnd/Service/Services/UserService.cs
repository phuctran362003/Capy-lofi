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
            if (userId <= 0)
            {
                return ApiResult<User>.Error(null, "Invalid user ID.");
            }

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
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                return ApiResult<UserDto>.Error(null, "Invalid email.");
            }

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
    private bool IsValidEmail(string email)
    {
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


