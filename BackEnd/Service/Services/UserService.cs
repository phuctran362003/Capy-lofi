using Domain.DTOs.UserDTOs;
using Repository.Commons;
using Repository.Interfaces;
using Service.Interfaces;


public class UserService : IUserService
{
    private readonly EmailService _emailService;
    private readonly IOtpService _otpService;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(EmailService emailService, IOtpService otpService, IUnitOfWork unitOfWork)
    {
        _emailService = emailService;
        _otpService = otpService;
        _unitOfWork = unitOfWork;
    }


    public async Task<ApiResult<User>> UpdateUserDisplayNameAsync(int userId, string newDisplayName)
    {
        try
        {
            if (userId <= 0 || string.IsNullOrEmpty(newDisplayName))
            {
                return ApiResult<User>.Error(null, "Invalid userID or display name");

            }


            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return ApiResult<User>.Error(null, "User not found");

            }

            var result = await _unitOfWork.UserRepository.UpdateDisplayNameAsync(user, newDisplayName);
            if (!result.Succeeded)
            {
                return ApiResult<User>.Error(null, "Failed to update display name.");

            }
            await _unitOfWork.SaveChangeAsync();
            return ApiResult<User>.Succeed(null, "Display name updated successfully.");
        }
        catch (Exception ex)
        {

            return ApiResult<User>.Fail(ex);
        }
    }

    public async Task<ApiResult<User>> GetUserByIdAsync(int userId)
    {
        try
        {
            if (userId <= 0)
            {
                return ApiResult<User>.Error(null, "Invalid user ID.");
            }

            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);
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
            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(email);

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

                var result = await _unitOfWork.UserRepository.CreateDefaultUserAsync(user);

                if (!result.Succeeded)
                {
                    return ApiResult<UserDto>.Error(null, "Failed to create user.");
                }

                await _otpService.SaveOtpAsync(user, otpCode);

            }
            await _unitOfWork.SaveChangeAsync();
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


