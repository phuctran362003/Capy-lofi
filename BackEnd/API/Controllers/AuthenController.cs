using Domain.DTOs.Request;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.Security.Claims;

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

    [HttpPost("google-callback")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GoogleCallback([FromBody] string token)
    {
        try
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded) return BadRequest();

            var expectedState = HttpContext.Session.GetString("OAuthState");
            var returnedState = authenticateResult.Properties.Items["state"];

            if (expectedState != returnedState)
            {
                return BadRequest("Invalid state parameter");
            }

            var userEmail = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
            var userName = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);

            var user = await _userService.CreateOrUpdateUserAsync(userEmail, userName);

            var jwtToken = _tokenService.GenerateToken(user);

            Response.Cookies.Append("jwt", jwtToken, new CookieOptions { HttpOnly = true, Secure = true });

            return Redirect("http://localhost:3000/");
        }
        catch (ApplicationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("current-user")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            return BadRequest("Invalid user ID in token");
        }

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null) return NotFound();

        return Ok(user);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest request)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            return BadRequest("Invalid access token");
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