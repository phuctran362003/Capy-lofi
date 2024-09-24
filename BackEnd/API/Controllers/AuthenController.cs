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



    /// <summary>
    /// Này dành cho Admin, sau này scope to hơn thì cho user tạo password luôn 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(string username, string password)
    {
        var result = await _authenService.LoginAsync(username, password);
        if (!result.Success)
        {
            return BadRequest(new ApiResult<string>
            {
                Success = false,
                Data = null,
                Message = "Invalid request."
            });
        }

        var user = result.Data;

        return await GenerateTokensAndSetCookies(user);
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
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
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
                    return BadRequest(ApiResult<string>.Error(null, result.Message));
                }
                else
                {
                    return Unauthorized(ApiResult<string>.Error(null, result.Message));
                }
            }

            var user = result.Data;

            return await GenerateTokensAndSetCookies(user);
        }
        catch (Exception ex)
        {
            var errorResult = ApiResult<string>.Fail(ex);
            return StatusCode(StatusCodes.Status500InternalServerError, errorResult);
        }
    }


    private async Task<IActionResult> GenerateTokensAndSetCookies(User user)
    {
        var tokens = _tokenService.GenerateTokens(user);

        await _authenService.UpdateRefreshTokenAsync(user, tokens.RefreshToken);

        Response.Cookies.Append("jwt", tokens.AccessToken, new CookieOptions { HttpOnly = true, Secure = true });

        var accessTokenExpirationMinutes = _configuration["JwtSettings:AccessTokenExpirationMinutes"];
        var refreshTokenExpirationMinutes = _configuration["JwtSettings:RefreshTokenExpirationMinutes"];

        var successResult = ApiResult<object>.Succeed(new
        {
            accessToken = tokens.AccessToken,
            refreshToken = tokens.RefreshToken,
            accessTokenExpiration = accessTokenExpirationMinutes,
            refreshTokenExpiration = refreshTokenExpirationMinutes
        }, "Tokens generated successfully.");

        return Ok(successResult);
    }


}



