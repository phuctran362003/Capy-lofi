using System.Security.Claims;
using Domain.Entities;

namespace Service.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    
    (string accessToken, string refreshToken) GenerateTokens(IEnumerable<Claim> claims);

}