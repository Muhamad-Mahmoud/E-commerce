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
            return HandleResult(await _authenticationService.RegisterAsync(request));
        }

        /// <summary>
        /// Authenticate user and return tokens.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            return HandleResult(await _authenticationService.LoginAsync(request));
        }

        /// <summary>
        /// Refresh JWT token.
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            return HandleResult(await _authenticationService.RefreshTokenAsync(request.Token));
        }


    }
}
