using Domain.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Commons;
using Service.Interfaces;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/v1/profile")]
[Authorize]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("displayName")]
    public async Task<IActionResult> UpdateDisplayName(string newDisplayName)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current logged-in user's ID
        var result = await _userService.UpdateUserDisplayNameAsync(int.Parse(userId), newDisplayName);

        if (!result.Success)
        {
            return BadRequest(new ApiResult<User>
            {
                Data = null,
                Message = "Error",
                Success = false
            });
        }

        return Ok(new ApiResult<string>
        {
            Data = newDisplayName,
            Message = "Update displayName successfully",
            Success = true
        });
    }

    [HttpGet("current-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLoggedInUserInfo()
    {
        try
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return NotFound(new ApiResult<string>
                {
                    Success = false,
                    Data = null,
                    Message = "User ID claim not found."
                });
            }

            var userId = int.Parse(userIdClaim.Value);

            var userResult = await _userService.GetUserByIdAsync(userId);
            if (!userResult.Success || userResult.Data == null)
            {
                return NotFound(new ApiResult<string>
                {
                    Success = false,
                    Data = null,
                    Message = "User not found."
                });
            }

            var userDto = new UserDto
            {
                Id = userResult.Data.Id,
                Name = userResult.Data.Name,
                DisplayName = userResult.Data.DisplayName,
                Email = userResult.Data.Email,
                ProfileInfo = userResult.Data.ProfileInfo,
                Coins = userResult.Data.Coins ?? 0,
            };

            return Ok(new ApiResult<UserDto>
            {
                Success = true,
                Data = userDto,
                Message = "User retrieved successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResult<string>.Fail(ex));
        }
    }
}