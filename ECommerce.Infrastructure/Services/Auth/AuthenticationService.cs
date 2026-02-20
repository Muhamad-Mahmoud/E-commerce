using ECommerce.Application.DTO.Auth.Requests;
using ECommerce.Application.DTO.Auth.Responses;
using ECommerce.Application.Interfaces.Services.Auth;
using ECommerce.Domain.Exceptions;
using ECommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Infrastructure.Services.Auth
{
    /// <summary>
    /// Service for handling user authentication and account management.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
        /// </summary>
        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        public async Task<Result<AuthenticationResponse>> RegisterAsync(RegisterRequest request)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var firstError = result.Errors.FirstOrDefault();
                return Result.Failure<AuthenticationResponse>(new Error("Auth.RegistrationError", firstError?.Description ?? "Registration failed", 400));
            }

            // Assign Default Role
            await _userManager.AddToRoleAsync(user, "User");

            // Generate Tokens
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles);
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);

            return Result.Success(new AuthenticationResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken.Token,
                User = new UserResponse { Identifier = user.Id, Email = user.Email!, FullName = user.FullName, Roles = roles }
            });
        }

        /// <summary>
        /// Authenticates a user and returns authentication tokens.
        /// </summary>
        public async Task<Result<AuthenticationResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<AuthenticationResponse>(DomainErrors.Authentication.InvalidCredentials);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
            {
                return Result.Failure<AuthenticationResponse>(DomainErrors.Authentication.InvalidCredentials);
            }

            // Generate Tokens
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles);
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);

            return Result.Success(new AuthenticationResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken.Token,
                User = new UserResponse { Identifier = user.Id, Email = user.Email!, FullName = user.FullName, Roles = roles }
            });
        }

        /// <summary>
        /// Changes the current user's password.
        /// </summary>
        public async Task<Result> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return Result.Failure(DomainErrors.Authentication.UserNotFound);

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            return result.Succeeded ? Result.Success() : Result.Failure(new Error("Auth.ChangePasswordError", result.Errors.FirstOrDefault()?.Description ?? "Failed to change password", 400));
        }

        /// <summary>
        /// Resets a user's password using a reset token.
        /// </summary>
        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return Result.Failure(DomainErrors.Authentication.UserNotFound);

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            return result.Succeeded ? Result.Success() : Result.Failure(new Error("Auth.ResetPasswordError", result.Errors.FirstOrDefault()?.Description ?? "Failed to reset password", 400));
        }

        /// <summary>
        /// Refreshes the authentication tokens using a refresh token.
        /// </summary>
        public async Task<Result<AuthenticationResponse>> RefreshTokenAsync(string token)
        {
            var storedToken = await _refreshTokenService.GetByTokenAsync(token);

            if (storedToken == null || !storedToken.IsActive)
            {
                return Result.Failure<AuthenticationResponse>(DomainErrors.Authentication.InvalidRefreshToken);
            }

            var user = await _userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
            {
                return Result.Failure<AuthenticationResponse>(DomainErrors.Authentication.UserNotFound);
            }

            // Token Rotation: Revoke the used refresh token
            await _refreshTokenService.RevokeRefreshTokenAsync(token);

            // Generate new tokens
            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles);
            var newRefreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);

            return Result.Success(new AuthenticationResponse
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                User = new UserResponse { Identifier = user.Id, Email = user.Email!, FullName = user.FullName, Roles = roles }
            });
        }
    }
}
