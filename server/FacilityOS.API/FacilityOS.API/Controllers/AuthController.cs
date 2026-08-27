using FacilityOS.Application.Common.Mapping;
using FacilityOS.Application.Common.Settings;
using FacilityOS.Application.DTOs.Auth;
using FacilityOS.Application.DTOs.Users;
using FacilityOS.Application.Features.Auth.Login;
using FacilityOS.Application.Features.Auth.Logout;
using FacilityOS.Application.Features.Auth.Me;
using FacilityOS.Application.Features.Auth.RefreshToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FacilityOS.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ApiControllerBase
{
    private readonly JwtSettings _jwtSettings;

    public AuthController(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await Mediator.Send(new LoginCommand(request));
        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(result.ToLoginResponse());
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["X-Refresh-Token"];

        if (string.IsNullOrWhiteSpace(refreshToken))
            return Unauthorized("Refresh token cookie is missing.");

        var result = await Mediator.Send(new RefreshTokenCommand(refreshToken), cancellationToken);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(result.ToLoginResponse());
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> Me(CancellationToken ct)
    {
        var result = await Mediator.Send(new MeQuery(), ct);
        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["X-Refresh-Token"];

        await Mediator.Send(new LogoutCommand(refreshToken), cancellationToken);

        Response.Cookies.Delete("X-Refresh-Token");

        return NoContent();
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        Response.Cookies.Append("X-Refresh-Token", refreshToken, cookieOptions);
    }
}