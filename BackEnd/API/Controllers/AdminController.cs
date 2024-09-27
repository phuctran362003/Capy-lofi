using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Commons;

namespace API.Controllers
{
    [Route("api/v1/admin/")]
    [ApiController]
    [Authorize(Policy = "AdminPolicy")]
    public class AdminController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult TestAdminAccess()
        {
            return Ok(new ApiResult<string>
            {
                Data = "You are an Admin!",
                Message = "Access granted to Admin role",
                Success = true
            });
        }
    }
}
