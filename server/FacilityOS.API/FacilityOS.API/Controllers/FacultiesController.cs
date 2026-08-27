using FacilityOS.Application.Common;
using FacilityOS.Application.DTOs.Faculties;
using FacilityOS.Application.Features.Faculties.CreateFaculty;
using FacilityOS.Application.Features.Faculties.DeleteFaculty;
using FacilityOS.Application.Features.Faculties.GetFaculties;
using FacilityOS.Application.Features.Faculties.GetFacultyById;
using FacilityOS.Application.Features.Faculties.UpdateFaculty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FacilityOS.API.Controllers;

[Route("api/faculties")]
[Authorize]
[EnableRateLimiting("global")]
public class FacultiesController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "SchoolAdminOrAbove")]
    public async Task<ActionResult<PagedResult<FacultyResponse>>> GetFaculties(
        [FromQuery] string? search,
        [FromQuery] int? districtId,
        [FromQuery] int? schoolId,
        [FromQuery] bool? isActive,
        [FromQuery] bool? hasBeacon,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (pageSize > 50) pageSize = 50;

        var query = new GetFacultiesQuery(search, districtId, schoolId, isActive, hasBeacon, page, pageSize);
        var result = await Mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "SchoolAdminOrAbove")]
    public async Task<ActionResult<FacultyResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetFacultyByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "SchoolAdminOrAbove")]
    public async Task<ActionResult<FacultyResponse>> Create([FromBody] CreateFacultyRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new CreateFacultyCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "SchoolAdminOrAbove")]
    public async Task<ActionResult<FacultyResponse>> Update(int id, [FromBody] UpdateFacultyRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new UpdateFacultyCommand(id, request), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "DistrictAdminOrAbove")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteFacultyCommand(id), cancellationToken);
        return NoContent();
    }
}
