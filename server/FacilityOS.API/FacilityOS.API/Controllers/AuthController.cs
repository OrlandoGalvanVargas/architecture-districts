using FacilityOS.API.DTOs.Auth;
using FacilityOS.API.Features.Auth.Login;
using FacilityOS.API.Features.Auth.Logout;
using FacilityOS.API.Features.Auth.Me;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FacilityOS.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest Request)
        {
            var result = await _mediator.Send(new LoginCommand(Request));
            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> Me()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new MeQuery(userId));

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout([FromBody] LogoutRequestBoyd body)
        {
            await _mediator.Send(new LogoutCommand(body.RefreshToken));
            return NoContent();
        }
    }

    public record LogoutRequestBoyd(string RefreshToken);
}
