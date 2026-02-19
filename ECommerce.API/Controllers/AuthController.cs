using ECommerce.Application.DTO.Auth.Requests;
using ECommerce.Application.DTO.Auth.Responses;
using ECommerce.Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// Authentication operations.
    /// </summary>
    public class AuthController : BaseApiController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Register a new user.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authenticationService.RegisterAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Authenticate user and return tokens.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authenticationService.LoginAsync(request);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        /// <summary>
        /// Refresh JWT token.
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest(new AuthenticationResponse { Success = false, Errors = new List<string> { "Refresh token is required" } });

            var result = await _authenticationService.RefreshTokenAsync(request.Token);
            return result.Success ? Ok(result) : BadRequest(result);
        }


    }
}
