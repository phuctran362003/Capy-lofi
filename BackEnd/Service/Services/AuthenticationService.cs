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

        public AuthenticationService(IUserRepository userRepository, TokenGenerators tokenGenerators, IAuthRepository authRepository)
        {
            _userRepository = userRepository;
            _tokenGenerators = tokenGenerators;
            _authRepository = authRepository;
        }

        public async Task<Authenticator> AuthenGoogleUser(string token)
        {
            string clientId = "1097822686619-u8ekfqo6bb9q4lsqv4kbp99sqaai0ee0.apps.googleusercontent.com";

            if (string.IsNullOrEmpty(clientId))
            {
                throw new Exception("ClientId is null!");
            }

            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string> { clientId },
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

            if (payload == null)
            {
                throw new Exception("Credential incorrect!");
            }

            var userEmail = payload.Email;
            var user = await _userRepository.GetUserByEmail(userEmail);

            if (user == null)
            {
                throw new ApplicationException("User not found.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var (accessToken, refreshToken) = _tokenGenerators.GenerateTokens(claims);

            await _authRepository.UpdateRefreshToken(user.Id, refreshToken);
            return new Authenticator()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }


        public async Task<ApiResult<object>> UserGetInfoSignUpByGoogle(string token)
        {
            try
            {
                string clientId = "797695382663-8o7c81nsa1g1k4re5k7152noj0ida3p7.apps.googleusercontent.com";

                if (string.IsNullOrEmpty(clientId))
                {
                    return ApiResult<object>.Error(null, "ClientId is null!");
                }

                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { clientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

                if (payload == null)
                {
                    return ApiResult<object>.Error(null, "Credential incorrect!");
                }

                var userEmail = payload.Email;
                var userFullName = payload.Name;
                var userPhotoUrl = payload.Picture ?? string.Empty;

                var isCheckUser = await _userRepository.GetUserByEmail(userEmail);

                if (isCheckUser != null)
                {
                    // Update the user information from Google
                    isCheckUser.Name = userFullName;
                    isCheckUser.DisplayName = userFullName;
                    isCheckUser.PhotoUrl = userPhotoUrl;

                    await _userRepository.UpdateUser(isCheckUser);

                    // Create an object to store user details
                    var updatedUserDetails = new { Email = userEmail, Name = userFullName };

                    return ApiResult<object>.Succeed(updatedUserDetails, "User information updated successfully.");
                }
                else
                {
                    // Create a new user entity with all necessary fields populated
                    var newUser = new User
                    {
                        Email = userEmail,
                        Name = userFullName,
                        DisplayName = userFullName, // Assuming display name is the same as full name
                        PhotoUrl = userPhotoUrl,
                        Coins = 0, // Assuming default coins are 0
                        ProfileInfo = string.Empty, // Assuming default profile info is empty
                        RefreshToken = string.Empty, // Assuming default refresh token is empty
                        LearningSessions = new List<LearningSession>(),
                        Orders = new List<Order>(),
                        UserMusics = new List<UserMusic>(),
                        UserBackgrounds = new List<UserBackground>(),
                        Feedbacks = new List<Feedback>()
                    };

                    await _userRepository.RegisterUser(newUser);

                    // Create an object to store user details
                    var userDetails = new { Email = userEmail, Name = userFullName };

                    return ApiResult<object>.Succeed(userDetails, "Successfully registered.");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<object>.Fail(ex);
            }
        }



    }
}
