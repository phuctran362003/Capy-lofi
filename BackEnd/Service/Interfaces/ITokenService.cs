using System.Security.Claims;
using Domain.Entities;
using Newtonsoft.Json.Linq;

namespace Service.Interfaces;

public interface ITokenService
{
    Tokens GenerateTokens(User user);
}

public class Tokens
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}