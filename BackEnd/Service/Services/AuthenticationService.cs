using Domain.DTOs.Response;
using Domain.Entities;
using Google.Apis.Auth;
using Repository.Commons;
using Repository.Interfaces;
using Service.Interfaces;
using System.Security.Claims;

namespace Service
{
    public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly TokenGenerators _tokenGenerators;
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;

    public AuthenticationService(IUserRepository userRepository, TokenGenerators tokenGenerators, IAuthRepository authRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _tokenGenerators = tokenGenerators;
        _authRepository = authRepository;
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

            var (accessToken, refreshToken) = _tokenGenerators.GenerateTokens(claims);

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

    public User GetUserById(int id)
    {
        return _userRepository.GetUserByIdAsync(id).Result;
    }

    public async Task<ApiResult<Authenticator>> RefreshTokens(string accessToken, string refreshToken)
    {
        var principal = _tokenGenerators.GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
        {
            return ApiResult<Authenticator>.Error(null, "Invalid access token");
        }

        if (!int.TryParse(principal.FindFirst(ClaimTypes.Name).Value, out var userId))
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
            new Claim(ClaimTypes.Name, user.Id.ToString()), // Convert int to string for claim
            new Claim(ClaimTypes.Email, user.Email)
        };

        var (newAccessToken, newRefreshToken) = _tokenGenerators.GenerateTokens(claims);
        await _authRepository.UpdateRefreshToken(user.Id, newRefreshToken);

        var authenticator = new Authenticator()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };

        return ApiResult<Authenticator>.Succeed(authenticator, "Tokens refreshed successfully.");
    }


}






}
