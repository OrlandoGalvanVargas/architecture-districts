using FacilityOS.API.DTOs.Schools;
using FacilityOS.API.Features.Schools.CreateSchool;
using FacilityOS.API.Features.Schools.DeleteSchool;
using FacilityOS.API.Features.Schools.GetSchoolById;
using FacilityOS.API.Features.Schools.GetSchools;
using FacilityOS.API.Features.Schools.UpdateSchool;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacilityOS.API.Controllers
{
    [ApiController]
    [Route("/api/schools")]
    [Authorize]
    public class SchoolsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SchoolsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<SchoolResponse>>> GetSchools(
            [FromQuery] int? districtId,
            [FromQuery] string? level,
            [FromQuery] string? type,
            [FromQuery] bool? isActive,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageSize > 50) pageSize = 50;

            var result = await _mediator.Send(new GetSchoolsQuery(
                districtId, level, type, isActive, page, pageSize));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SchoolResponse>> GetById(int id)
        {
            var result = await _mediator.Send(new GetSchoolByIdQuery(id));
            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<SchoolResponse>> Create([FromBody] CreateSchoolRequest request)
        {
            var result = await _mediator.Send(new CreateSchoolCommand(request));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SchoolResponse>> Update(int id, [FromBody] UpdateSchoolRequest request)
        {
            var result = await _mediator.Send(new UpdateSchoolCommand(id, request));
            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delelete(int id)
        {
            var deleted = await _mediator.Send(new DeleteSchoolCommand(id));
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
