using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace API.Controllers
{
    [Route("api/v1/authentication")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthenticationService authenticationService, IConfiguration configuration)
        {
            _authenticationService = authenticationService;
            _configuration = configuration;
        }


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

        /// <summary>
        /// Signs up a user using Google authentication.
        /// </summary>
        [HttpPost("google-signup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StudentSignupByGoogle([FromBody] string id_token)
        {
            try
            {
                var checkToken = await _authenticationService.UserGetInfoSignUpByGoogle(id_token);
                return Ok(checkToken);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}