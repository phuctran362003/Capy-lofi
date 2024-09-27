namespace Service.Interfaces;

public interface ITokenService
{
    Task<Tokens> GenerateTokensAsync(User user);
}

public class Tokens
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}