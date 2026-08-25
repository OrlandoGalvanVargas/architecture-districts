using FacilityOS.API.DTOs.Auth;
using FacilityOS.API.DTOs.Users;
using FacilityOS.API.Features.Auth.Login;
using FacilityOS.API.Features.Auth.Logout;
using FacilityOS.API.Features.Auth.Me;
using FacilityOS.API.Features.Auth.RefreshToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacilityOS.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ApiControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await Mediator.Send(new LoginCommand(request));
        return Ok(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await Mediator.Send(new RefreshTokenCommand(request.RefreshToken));
        return Ok(result);
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
    public async Task<ActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        await Mediator.Send(new LogoutCommand(request.RefreshToken));
        return NoContent();
    }
}