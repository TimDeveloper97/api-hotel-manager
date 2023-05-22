using HotelAPI.Contracts;
using HotelAPI.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace HotelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthManager authManager, ILogger<AccountController> logger)
        {
            this._authManager = authManager;
            this._logger = logger;
        }

        // POST: api/Account/register
        [HttpPost]
        [Route("registor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Register([FromBody] ApiUserDto apiUserDto)
        {
            _logger.LogInformation($"Registration attempt for {apiUserDto.Email}");
            try
            {
                var errors = await _authManager.Register(apiUserDto);
                if (errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Ok(apiUserDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Somethong went wrong in the {nameof(Register)} - usser registration attempt for {apiUserDto.Email}");
                return Problem($"Something went wrong in the {nameof(Register)}", statusCode: 500);
            }
        }

        // POST: api/Account/login
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation($"Login attempt for {loginDto.Email}");
            try
            {
                var authResponse = await _authManager.Login(loginDto);
                if (authResponse == null) return Unauthorized();

                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Somethong went wrong in the {nameof(Login)} - usser Login attempt for {loginDto.Email}");
                return Problem($"Something went wrong in the {nameof(Login)}", statusCode: 500);
            }
        }

        // POST: api/Account/refreshToken
        [HttpPost]
        [Route("refreshToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RefreshTOken([FromBody] AuthResponseDto authResponseDto)
        {
            var authResponse = await _authManager.VerifyRefreshToken(authResponseDto);
            if (authResponse == null) return Unauthorized();

            return Ok(authResponse);
        }
    }
}
