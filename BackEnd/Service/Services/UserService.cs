using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.Identity.Client;
using Repository.Commons;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Services
{
   public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IOtpService _otpService;
    private readonly ITokenService _tokenService;

    public UserService(IUserRepository userRepository, IOtpService otpService, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _otpService = otpService;
        _tokenService = tokenService;
    }

    public async Task<ApiResult<string>> CreateOrUpdateUserAndSendOtpAsync(string email, string name)
    {
        try
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
        
            if (user == null)
            {
                // Nếu người dùng không tồn tại, tạo người dùng mới
                user = new User
                {
                    Email = email,
                    UserName = email, // hoặc bất kỳ logic nào bạn sử dụng cho tên đăng nhập
                    Name = name,
                    DisplayName = name ,// Gán giá trị cho DisplayName
                    Otp = string.Empty 
                };
                await _userRepository.CreateUserAsync(user);
            }
            else
            {
                // Nếu người dùng đã tồn tại, cập nhật thông tin nếu cần
                user.Name = name;
                user.DisplayName = name; // Gán lại giá trị cho DisplayName
                await _userRepository.UpdateUserAsync(user);
            }

            // Gửi OTP tới email của người dùng
            var otpSent = await _otpService.GenerateOtpAsync(email);

            if (string.IsNullOrEmpty(otpSent))
            {
                return ApiResult<string>.Error(null, "Failed to send OTP.");
            }

            // Tạo và trả về token để đăng nhập
            var token = _tokenService.GenerateToken(user);
            return ApiResult<string>.Succeed(token, "OTP sent and token generated successfully.");
        }
        catch (Exception ex)
        {
            return ApiResult<string>.Fail(ex);
        }
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        return await _userRepository.GetUserByIdAsync(userId);
    }

    public async Task UpdateUserAsync(User user)
    {
        await _userRepository.UpdateUserAsync(user);
    }

    public async Task<ApiResult<string>> VerifyOtpAndLoginAsync(string email, string otp)
    {
        try
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return ApiResult<string>.Error(null, "User not found.");
            }

            var otpValid = await _otpService.ValidateOtpAsync(email, otp);
            if (!otpValid)
            {
                return ApiResult<string>.Error(null, "Invalid OTP.");
            }

            var token = _tokenService.GenerateToken(user);
            return ApiResult<string>.Succeed(token, "OTP validated and token generated successfully.");
        }
        catch (Exception ex)
        {
            return ApiResult<string>.Fail(ex);
        }
    }
}




}