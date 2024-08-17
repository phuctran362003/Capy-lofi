using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Service.Interfaces;
using TokenRequest = Domain.DTOs.Request.TokenRequest;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public AuthController(IUserService userService, ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }
    //
    // [HttpPost("login")]
    // public async Task<IActionResult> Login([FromBody] LoginRequest request)
    // {
    //     var user = await _userService.ValidateUserAsync(request.Username, request.Password);
    //     if (user == null)
    //     {
    //         return Unauthorized(new { message = "Invalid username or password." });
    //     }
    //
    //     var token = _tokenService.GenerateToken(user);
    //     return Ok(new { token });
    // }


        [HttpPost("google-callback")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GoogleLogin([FromBody] string token)
        {
            try
            {
                var checkToken = await _authenticationService.AuthenGoogleUser(token);
                return Ok(checkToken);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        if (!int.TryParse(principal.FindFirst(ClaimTypes.Name).Value, out var userId))
        {
            return BadRequest("Invalid user ID in token");
        }

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null || user.RefreshToken != request.RefreshToken)
        {
            return BadRequest("Invalid refresh token");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var newAccessToken = _tokenService.GenerateToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userService.UpdateUserAsync(user);

        return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            return BadRequest("Invalid user ID in token");
        }

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null) return BadRequest();

        user.RefreshToken = null;
        await _userService.UpdateUserAsync(user);

        Response.Cookies.Delete("jwt");

        return Ok();
    }
}
