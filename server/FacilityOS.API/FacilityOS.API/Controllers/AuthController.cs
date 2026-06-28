using FacilityOS.API.DTOs.Auth;
using FacilityOS.API.Features.Auth.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

    }
}
