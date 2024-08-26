using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Entities;
using Repository.Commons;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IOtpService _otpService;

        public UserService(IUserRepository userRepository, ITokenService tokenService, IOtpService otpService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
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

        public async Task<ApiResult<bool>> UpdateUserAsync(User user)
        {
            try
            {
                await _userRepository.UpdateUserAsync(user);
                return ApiResult<bool>.Succeed(true, "User updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex);
            }
        }

        public async Task<ApiResult<string>> CreateOrUpdateUserAndSendOtpAsync(string email, string name)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return ApiResult<string>.Error(null, "Email cannot be null or empty.");
                }

                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user != null)
                {
                    // Nếu người dùng đã tồn tại, tạo token và trả về
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email)
                    };

                    var (accessToken, refreshToken) = _tokenService.GenerateTokens(claims);

                    // Cập nhật refresh token trong bảng người dùng
                    user.RefreshToken = refreshToken;
                    await _userRepository.UpdateUserAsync(user);

                    return ApiResult<string>.Succeed(accessToken, "User authenticated successfully.");
                }
                else
                {
                    // Nếu người dùng không tồn tại, tạo người dùng mới và gửi OTP
                    user = new User
                    {
                        Email = email,
                        UserName = name ?? email.Split('@')[0] ?? string.Empty, // Xử lý null hoặc empty
                        DisplayName = name ?? email.Split('@')[0] ?? string.Empty, // Xử lý null hoặc empty
                        PhotoUrl = string.Empty, // Giá trị mặc định cho PhotoUrl
                        Coins = 0, // Giá trị mặc định cho Coins
                        ProfileInfo = string.Empty, // Giá trị mặc định cho ProfileInfo
                        RefreshToken = string.Empty, // Giá trị mặc định cho RefreshToken
                        Otp = string.Empty, // Giá trị mặc định cho Otp

                        // Khởi tạo các thuộc tính điều hướng (nếu có)
                        LearningSessions = new List<LearningSession>(),
                        Orders = new List<Order>(),
                        UserMusics = new List<UserMusic>(),
                        UserBackgrounds = new List<UserBackground>(),
                        Feedbacks = new List<Feedback>()
                    };

                    await _userRepository.CreateUserAsync(user);

                    // Gửi OTP sau khi tạo người dùng
                    var otpResult = await _otpService.GenerateOtpAsync(email);
                    if (!otpResult.Success)
                    {
                        return ApiResult<string>.Error(null, otpResult.Message ?? "Failed to send OTP.");
                    }

                    return ApiResult<string>.Succeed(null, "User created and OTP sent to email.");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<string>.Fail(ex);
            }
        }


        public async Task<ApiResult<string>> VerifyOtpAndLoginAsync(string email, string otp)
        {
            var otpResult = await _otpService.ValidateOtpAsync(email, otp);

            if (!otpResult.Success)
            {
                return ApiResult<string>.Error(null, otpResult.Message ?? "Invalid OTP.");
            }

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                // Tạo người dùng mới sau khi OTP được xác minh thành công
                user = new User
                {
                    Email = email,
                    UserName = email.Split('@')[0],
                    DisplayName = email.Split('@')[0]
                };
                await _userRepository.CreateUserAsync(user);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var (accessToken, refreshToken) = _tokenService.GenerateTokens(claims);

            // Cập nhật refresh token trong bảng người dùng
            user.RefreshToken = refreshToken;
            await _userRepository.UpdateUserAsync(user);

            return ApiResult<string>.Succeed(accessToken, "User authenticated successfully.");
        }
    }
}