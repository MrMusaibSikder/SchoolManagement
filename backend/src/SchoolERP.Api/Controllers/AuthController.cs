using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SchoolERP.Application.Features.Authentication.DTOs;
using SchoolERP.Application.Features.Authentication.Interfaces;

namespace SchoolERP.Api.Controllers;

/// <summary>
/// Authentication endpoints: login, registration, refresh token, logout and
/// the change/forgot/reset password flows.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Authenticates a user and returns a JWT access token plus a refresh token.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(request);

        if (result == null)
        {
            return Unauthorized(new
            {
                Message = "Invalid username/email or password."
            });
        }

        return Ok(result);
    }

    /// <summary>Registers a new user under an existing role.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);

        return Ok(result);
    }

    /// <summary>Exchanges a valid refresh token for a new access/refresh token pair.</summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(RefreshTokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var result = await _authService.RefreshTokenAsync(request);

        if (result is null)
        {
            return Unauthorized(new
            {
                Message = "The refresh token is invalid, expired, or has been revoked."
            });
        }

        return Ok(result);
    }

    /// <summary>
    /// Logs the current user out by revoking the supplied refresh token, or every
    /// active refresh token belonging to the user if none is supplied.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        await _authService.LogoutAsync(userId.Value, request);
        return NoContent();
    }

    /// <summary>Changes the password for the currently authenticated user.</summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        await _authService.ChangePasswordAsync(userId.Value, request);
        return NoContent();
    }

    /// <summary>
    /// Requests a password reset token/email for the given address. Always
    /// returns 204 regardless of whether the email is registered, to avoid
    /// leaking which accounts exist.
    /// </summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
    {
        await _authService.ForgotPasswordAsync(request);
        return NoContent();
    }

    /// <summary>Completes a password reset using a token issued by <c>forgot-password</c>.</summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        await _authService.ResetPasswordAsync(request);
        return NoContent();
    }

    private int? GetCurrentUserId()
    {
        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }
}