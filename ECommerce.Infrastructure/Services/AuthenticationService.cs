using ECommerce.Application.DTO;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ECommerce.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;

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

        public async Task<AuthenticationResult> RegisterAsync(RegisterRequest request)
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
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            // Generate Tokens
            var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!);
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);

            return new AuthenticationResult
            {
                Success = true,
                Token = accessToken,
                RefreshToken = refreshToken.Token,
                User = new UserDto { Identifier = user.Id, Email = user.Email!, FullName = user.FullName }
            };
        }

        public async Task<AuthenticationResult> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new List<string> { "Invalid email or password" }
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new List<string> { "Invalid email or password" }
                };
            }

            // Generate Tokens
            var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!);
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);

            return new AuthenticationResult
            {
                Success = true,
                Token = accessToken,
                RefreshToken = refreshToken.Token,
                User = new UserDto { Identifier = user.Id, Email = user.Email!, FullName = user.FullName }
            };
        }

        public async Task LogoutAsync(string userId)
        {
            await _refreshTokenService.RevokeAllUserTokensAsync(userId);
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            return result.Succeeded;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return false;

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            return result.Succeeded;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            return await _refreshTokenService.RevokeRefreshTokenAsync(token);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token)
        {
            var storedToken = await _refreshTokenService.GetByTokenAsync(token);

            if (storedToken == null || !storedToken.IsActive)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new List<string> { "Invalid or expired refresh token" }
                };
            }

            var user = await _userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new List<string> { "User not found" }
                };
            }

            // Token Rotation: Revoke the used refresh token
            await _refreshTokenService.RevokeRefreshTokenAsync(token);

            // Generate new tokens
            var newAccessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!);
            var newRefreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);

            return new AuthenticationResult
            {
                Success = true,
                Token = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                User = new UserDto { Identifier = user.Id, Email = user.Email!, FullName = user.FullName }
            };
        }

    }
}
