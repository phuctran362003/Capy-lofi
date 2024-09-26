using Domain.DTOs.UserDTOs;
using Repository.Commons;

namespace Service.Interfaces;

public interface IUserService
{
    Task<ApiResult<User>> GetUserByIdAsync(int userId);
    Task<ApiResult<UpdateUserProfileDto>> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateUserProfileDto);
    Task<ApiResult<UserDto>> CreateOrUpdateUserAndSendOtpAsync(string email, string name);
}

