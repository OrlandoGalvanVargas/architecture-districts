using FacilityOS.API.DTOs.Districts;
using FacilityOS.API.Features.Districts.CreateDistrict;
using FacilityOS.API.Features.Districts.GetDistricts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FacilityOS.API.Controllers
{

    [ApiController]
    [Route("api/districts")]
    public class DistrictsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DistrictsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<DistrictResponse>>> GetAll()
        {
            var result = await _mediator.Send(new GetDistrictsQuery());
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<DistrictResponse>> Create([FromBody] CreateDistrictRequest request)
        {
            var result = await _mediator.Send(new CreateDistrictCommand(request));
            return CreatedAtAction(nameof(GetAll), result);
        }
    }
}
