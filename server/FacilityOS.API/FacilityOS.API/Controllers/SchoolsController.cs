using FacilityOS.Application.Common;
using FacilityOS.Application.DTOs.Schools;
using FacilityOS.Application.Features.Schools.CreateSchool;
using FacilityOS.Application.Features.Schools.DeleteSchool;
using FacilityOS.Application.Features.Schools.GetSchoolById;
using FacilityOS.Application.Features.Schools.GetSchools;
using FacilityOS.Application.Features.Schools.UpdateSchool;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FacilityOS.API.Controllers;

[ApiController]
[Route("api/schools")]
[Authorize]
[EnableRateLimiting("global")]
public class SchoolsController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "SchoolAdminOrAbove")]
    public async Task<ActionResult<PagedResult<SchoolResponse>>> GetSchools(
        [FromQuery] int? districtId,
        [FromQuery] string? search,
        [FromQuery] string? level,
        [FromQuery] string? type,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageSize > 50) pageSize = 50;

        var result = await Mediator.Send(new GetSchoolsQuery(
            districtId, search, level, type, isActive, page, pageSize));
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "SchoolAdminOrAbove")]
    public async Task<ActionResult<SchoolResponse>> GetById(int id)
    {
        var result = await Mediator.Send(new GetSchoolByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "DistrictAdminOrAbove")]
    public async Task<ActionResult<SchoolResponse>> Create([FromBody] CreateSchoolRequest request)
    {
        var result = await Mediator.Send(new CreateSchoolCommand(request));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "SchoolAdminOrAbove")]
    public async Task<ActionResult<SchoolResponse>> Update(int id, [FromBody] UpdateSchoolRequest request)
    {
        var result = await Mediator.Send(new UpdateSchoolCommand(id, request));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteSchoolCommand(id));
        return NoContent();
    }
}