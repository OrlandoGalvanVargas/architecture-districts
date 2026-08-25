using FacilityOS.API.DTOs.Districts;
using FacilityOS.API.Features.Districts.CreateDistrict;
using FacilityOS.API.Features.Districts.DeleteDistrict;
using FacilityOS.API.Features.Districts.GetDistrictById;
using FacilityOS.API.Features.Districts.GetDistricts;
using FacilityOS.API.Features.Districts.UpdateDistrict;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FacilityOS.API.Controllers;

[ApiController]
[Route("api/districts")]
[Authorize]
[EnableRateLimiting("global")]
public class DistrictsController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "SchoolAdminOrAbove")]
    public async Task<ActionResult<List<DistrictResponse>>> GetAll()
    {
        var result = await Mediator.Send(new GetDistrictsQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "SchoolAdminOrAbove")]
    public async Task<ActionResult<DistrictResponse>> GetById(int id)
    {
        var result = await Mediator.Send(new GetDistrictByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<DistrictResponse>> Create([FromBody] CreateDistrictRequest request)
    {
        var result = await Mediator.Send(new CreateDistrictCommand(request));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "DistrictAdminOrAbove")]
    public async Task<ActionResult<DistrictResponse>> Update(int id, [FromBody] UpdateDistrictRequest request)
    {
        var result = await Mediator.Send(new UpdateDistrictCommand(id, request));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteDistrictCommand(id));
        return NoContent();
    }
}