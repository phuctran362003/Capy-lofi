using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.DTOs.Response;
using Domain.Entities;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Repository.Commons;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;  // Sử dụng ITokenService thay cho TokenGenerators
        private readonly IAuthRepository _authRepository;
        private readonly IOtpService _otpService;
        private readonly IConfiguration _configuration;

        public AuthenticationService(IUserRepository userRepository, ITokenService tokenService, IAuthRepository authRepository, IOtpService otpService, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _authRepository = authRepository;
            _otpService = otpService;
            _configuration = configuration;
        }

        public async Task<ApiResult<Authenticator>> AuthenGoogleUser(string token)
        {
            try
            {
                var clientId = _configuration["Authentication:Google:ClientId"].Trim();

                if (string.IsNullOrEmpty(clientId))
                {
                    return ApiResult<Authenticator>.Error(null, "ClientId is null!");
                }

                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { clientId },
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

                if (payload == null)
                {
                    return ApiResult<Authenticator>.Error(null, "Credential incorrect!");
                }

                var userEmail = payload.Email;
                var userFullName = payload.Name;
                var userPhotoUrl = payload.Picture ?? string.Empty;

                var user = await _userRepository.GetUserByEmailAsync(userEmail);

                if (user != null)
                {
                    user.Name = userFullName;
                    user.DisplayName = userFullName;
                    user.PhotoUrl = userPhotoUrl;

                    await _userRepository.UpdateUserAsync(user);
                }
                else
                {
                    user = new User
                    {
                        Email = userEmail,
                        Name = userFullName,
                        DisplayName = userFullName,
                        PhotoUrl = userPhotoUrl,
                        Coins = 0,
                        ProfileInfo = string.Empty,
                        RefreshToken = string.Empty,
                        LearningSessions = new List<LearningSession>(),
                        Orders = new List<Order>(),
                        UserMusics = new List<UserMusic>(),
                        UserBackgrounds = new List<UserBackground>(),
                        Feedbacks = new List<Feedback>()
                    };

                    await _userRepository.CreateUserAsync(user);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                };

                var (accessToken, refreshToken) = _tokenService.GenerateTokens(claims);

                await _authRepository.UpdateRefreshToken(user.Id, refreshToken);

                var authenticator = new Authenticator()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                };

                return ApiResult<Authenticator>.Succeed(authenticator, "User authenticated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResult<Authenticator>.Fail(ex);
            }
        }

        public async Task<ApiResult<Authenticator>> RefreshTokens(string accessToken, string refreshToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return ApiResult<Authenticator>.Error(null, "Invalid access token");
            }

            if (!int.TryParse(principal.FindFirst(ClaimTypes.Name)?.Value, out var userId))
            {
                return ApiResult<Authenticator>.Error(null, "Invalid user ID in token");
            }

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null || user.RefreshToken != refreshToken)
            {
                return ApiResult<Authenticator>.Error(null, "Invalid refresh token");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var (newAccessToken, newRefreshToken) = _tokenService.GenerateTokens(claims);
            await _authRepository.UpdateRefreshToken(user.Id, newRefreshToken);

            var authenticator = new Authenticator()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            return ApiResult<Authenticator>.Succeed(authenticator, "Tokens refreshed successfully.");
        }

        public async Task<ApiResult<Authenticator>> VerifyOtpAndLoginAsync(string email, string otp)
        {
            // Gọi phương thức ValidateOtpAsync và lấy kết quả
            var otpResult = await _otpService.ValidateOtpAsync(email, otp);
    
            // Kiểm tra kết quả ValidateOtpAsync
            if (!otpResult.Success)
            {
                // Trả về lỗi nếu OTP không hợp lệ
                return ApiResult<Authenticator>.Error(null, otpResult.Message ?? "Invalid OTP.");
            }

            // Kiểm tra xem người dùng đã tồn tại chưa
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                // Nếu người dùng chưa tồn tại, tạo người dùng mới
                user = new User
                {
                    Email = email,
                    Name = email.Split('@')[0],
                };
                await _userRepository.CreateUserAsync(user);
            }

            // Tạo token cho người dùng
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var (accessToken, refreshToken) = _tokenService.GenerateTokens(claims);
            await _authRepository.UpdateRefreshToken(user.Id, refreshToken);

            // Tạo đối tượng Authenticator để trả về
            var authenticator = new Authenticator
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

            // Trả về kết quả thành công
            return ApiResult<Authenticator>.Succeed(authenticator, "User authenticated successfully.");
        }


        public User GetUserById(int id)
        {
            return _userRepository.GetUserByIdAsync(id).Result;
        }
    }
}
