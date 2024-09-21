using Domain.DTOs.Request;
using Microsoft.AspNetCore.Mvc;
using Repository.Commons;
using Service.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : Controller
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IAuthenService _authenService;
    private readonly IConfiguration _configuration;

    public AuthController(IUserService userService, ITokenService tokenService, IAuthenService authenService, IConfiguration configuration)
    {
        _userService = userService;
        _tokenService = tokenService;
        _authenService = authenService;
        _configuration = configuration;
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
                if (result.Message == "OTP has expired")
                {
                    return BadRequest(ApiResult<string>.Error(null, result.Message));  // Handles expired OTP
                }
                else
                {
                    return Unauthorized(ApiResult<string>.Error(null, result.Message));  // Handles other errors
                }
            }

            var user = result.Data;

            // Generate tokens using TokenService
            var tokens = _tokenService.GenerateTokens(user);

            // Update the refresh token in the database
            await _authenService.UpdateRefreshTokenAsync(user, tokens.RefreshToken);

            // Set JWT cookie
            Response.Cookies.Append("jwt", tokens.AccessToken, new CookieOptions { HttpOnly = true, Secure = true });

            // Retrieve expiration times from the configuration
            var accessTokenExpirationMinutes = _configuration["JwtSettings:AccessTokenExpirationMinutes"];
            var refreshTokenExpirationMinutes = _configuration["JwtSettings:RefreshTokenExpirationMinutes"];

            var successResult = ApiResult<object>.Succeed(new
            {
                accessToken = tokens.AccessToken,
                refreshToken = tokens.RefreshToken,
                accessTokenExpiration = accessTokenExpirationMinutes,    // Add expiration info to the response
                refreshTokenExpiration = refreshTokenExpirationMinutes   // Add expiration info to the response
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



