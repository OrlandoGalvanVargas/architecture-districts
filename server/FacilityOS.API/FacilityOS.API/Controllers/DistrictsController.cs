using FacilityOS.API.DTOs.Districts;
using FacilityOS.API.Features.Districts.CreateDistrict;
using FacilityOS.API.Features.Districts.DeleteDistrict;
using FacilityOS.API.Features.Districts.GetDistrictById;
using FacilityOS.API.Features.Districts.GetDistricts;
using FacilityOS.API.Features.Districts.UpdateDistrict;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FacilityOS.API.Controllers;

[ApiController]
[Route("api/districts")]
[Authorize]
[EnableRateLimiting("global")]
public class DistrictsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DistrictsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "SchoolAdminOrAbove")]
    public async Task<ActionResult<List<DistrictResponse>>> GetAll()
    {
        var result = await _mediator.Send(new GetDistrictsQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "SchoolAdminOrAbove")]
    public async Task<ActionResult<DistrictResponse>> GetById(int id)
    {
        var result = await _mediator.Send(new GetDistrictByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<DistrictResponse>> Create([FromBody] CreateDistrictRequest request)
    {
        var result = await _mediator.Send(new CreateDistrictCommand(request));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "DistrictAdminOrAbove")]
    public async Task<ActionResult<DistrictResponse>> Update(int id, [FromBody] UpdateDistrictRequest request)
    {
        var result = await _mediator.Send(new UpdateDistrictCommand(id, request));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteDistrictCommand(id));
        return NoContent();
    }
}