using ECommerce.Application.DTO.Auth;
using ECommerce.Application.DTO.Categories;
using ECommerce.Application.DTO.Products;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// Handles user authentication and authorization operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="request">The user registration details.</param>
        /// <returns>Authentication result with JWT tokens.</returns>
        /// <response code="200">User registered successfully, returns authentication result with tokens.</response>
        /// <response code="400">Invalid request data or registration failed.</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authenticationService.RegisterAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Authenticates a user and returns JWT tokens.
        /// </summary>
        /// <param name="request">The user credentials (email and password).</param>
        /// <returns>Authentication result with JWT and refresh tokens.</returns>
        /// <response code="200">Login successful, returns authentication result with tokens.</response>
        /// <response code="401">Invalid credentials.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authenticationService.LoginAsync(request);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Refreshes the JWT token using a valid refresh token.
        /// </summary>
        /// <param name="request">The refresh token request.</param>
        /// <returns>New JWT and refresh tokens.</returns>
        /// <response code="200">Token refreshed successfully, returns new tokens.</response>
        /// <response code="400">Invalid or missing refresh token.</response>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest(new AuthenticationResult
                {
                    Success = false,
                    Errors = new List<string> { "Refresh token is required" }
                });

            var result = await _authenticationService.RefreshTokenAsync(request.Token);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        /// <summary>
        /// Logs out the authenticated user by invalidating their refresh token.
        /// </summary>
        /// <returns>Success message.</returns>
        /// <response code="200">User logged out successfully.</response>
        /// <response code="401">Unauthorized - User not authenticated.</response>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _authenticationService.LogoutAsync(userId);
            return Ok(new { message = "Logged out successfully" });
        }
    }
}