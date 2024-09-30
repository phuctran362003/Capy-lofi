using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;

    // Constructor
    public TokenService(IConfiguration configuration, UserManager<User> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;  // Inject UserManager
    }

    public async Task<Tokens> GenerateTokensAsync(User user)
    {
        // Get JWT settings from configuration
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var accessTokenExpirationMinutes = int.Parse(jwtSettings["AccessTokenExpirationMinutes"]);
        var refreshTokenExpirationMinutes = int.Parse(jwtSettings["RefreshTokenExpirationMinutes"]);

        // Fetch the roles of the user
        var userRoles = await _userManager.GetRolesAsync(user);

        // Create claims for the token
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add role claims
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Generate Access Token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var accessToken = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(accessTokenExpirationMinutes),
            signingCredentials: creds
        );

        var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);

        // Generate Refresh Token
        var refreshToken = GenerateRefreshToken();

        return new Tokens
        {
            AccessToken = accessTokenString,
            RefreshToken = refreshToken
        };
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
        {
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
