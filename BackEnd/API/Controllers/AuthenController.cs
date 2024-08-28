using Domain.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.Security.Claims;
using Repository.Commons;

namespace API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : Controller
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IAuthenService _authenService;

    public AuthController(IUserService userService, ITokenService tokenService, IAuthenService authenService)
    {
        _userService = userService;
        _tokenService = tokenService;
        _authenService = authenService;
    }

    [HttpPost("otp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendOtp([FromBody] OtpRequests.SendOtpRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResult<string>
                {
                    Success = false,
                    Data = null,
                    Message = "Invalid request."
                });
            }

            var result = await _userService.CreateOrUpdateUserAndSendOtpAsync(request.Email, request.Name);
            if (!result.Success)
            {
                return BadRequest(result); // Return the ApiResult object directly
            }

            return Ok(result); // Return the ApiResult object directly
        }
        catch (Exception ex)
        {
            // Log the exception details here if necessary
            var errorResult = ApiResult<string>.Fail(ex);
            return StatusCode(StatusCodes.Status500InternalServerError, errorResult);
        }
    }

    [HttpPost("otp/verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpRequests.VerifyOtpRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResult<string>.Error(null, "Invalid request."));
            }

            var result = await _authenService.VerifyOtpAsync(request.Email, request.OtpCode);
            if (!result.Success)
            {
                return Unauthorized(ApiResult<string>.Error(null, result.Message));
            }

            var user = result.Data;

            // Generate tokens using TokenService
            var tokens = _tokenService.GenerateTokens(user);

            // Update the refresh token in the database
            await _authenService.UpdateRefreshTokenAsync(user, tokens.RefreshToken);

            // Set JWT cookie
            Response.Cookies.Append("jwt", tokens.AccessToken, new CookieOptions { HttpOnly = true, Secure = true });

            var successResult = ApiResult<object>.Succeed(new
            {
                accessToken = tokens.AccessToken,
                refreshToken = tokens.RefreshToken
            }, "OTP verified successfully.");

            return Ok(successResult);
        }
        catch (Exception ex)
        {
            // Log the exception details here if necessary
            var errorResult = ApiResult<string>.Fail(ex);
            return StatusCode(StatusCodes.Status500InternalServerError, errorResult);
        }
    }
}


//[HttpPost("google-callback")]
//[ProducesResponseType(StatusCodes.Status200OK)]
//[ProducesResponseType(StatusCodes.Status400BadRequest)]
//public async Task<IActionResult> GoogleCallback([FromBody] string token)
//{
//    try
//    {
//        try
//        {
//            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
//            if (!authenticateResult.Succeeded) return BadRequest();

//            var expectedState = HttpContext.Session.GetString("OAuthState");
//            var returnedState = authenticateResult.Properties.Items["state"];

//            if (expectedState != returnedState)
//            {
//                return BadRequest("Invalid state parameter");
//            }

//            var userEmail = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
//            var userName = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);

//            var result = await _userService.CreateOrUpdateUserAndSendOtpAsync(userEmail, userName);

//            if (!result.Success)
//            {
//                return BadRequest(result.Message);
//            }

//            var jwtToken = result.Data;

//            Response.Cookies.Append("jwt", jwtToken, new CookieOptions { HttpOnly = true, Secure = true });

//            return Redirect("http://localhost:3000/");
//        }
//        catch (ApplicationException ex)
//        {
//            return BadRequest(ex.Message);
//        }
//    }
//    catch (Exception ex)
//    {
//        // Log the exception details here if necessary
//        return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred during Google callback: {ex.Message}");
//    }
//}

//[HttpGet("current-user")]
//[Authorize]
//public async Task<IActionResult> GetCurrentUser()
//{
//    try
//    {
//        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
//        if (!int.TryParse(userIdString, out var userId))
//        {
//            return BadRequest("Invalid user ID in token");
//        }

//        var user = await _userService.GetUserByIdAsync(userId);
//        if (user == null) return NotFound();

//        return Ok(user);
//    }
//    catch (Exception ex)
//    {
//        // Log the exception details here if necessary
//        return StatusCode(StatusCodes.Status500InternalServerError,
//            $"An error occurred while retrieving the current user: {ex.Message}");
//    }
//}
