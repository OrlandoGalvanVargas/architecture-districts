using FacilityOS.Application.Common;
using FacilityOS.Application.DTOs.Beacons;
using FacilityOS.Application.Features.Beacons.CreateBeacon;
using FacilityOS.Application.Features.Beacons.DeleteBeacon;
using FacilityOS.Application.Features.Beacons.GetBeaconById;
using FacilityOS.Application.Features.Beacons.GetBeacons;
using FacilityOS.Application.Features.Beacons.UpdateBeacon;
using FacilityOS.Domain.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FacilityOS.API.Controllers;

[Route("api/beacons")]
[Authorize]
[EnableRateLimiting("global")]
public class BeaconsController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "SchoolAdminOrAbove")]
    public async Task<ActionResult<PagedResult<BeaconResponse>>> GetBeacons(
        [FromQuery] string? search,
        [FromQuery] BeaconType? type,
        [FromQuery] BeaconStatus? status,
        [FromQuery] int? districtId,
        [FromQuery] int? schoolId,
        [FromQuery] bool? isAssigned,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (pageSize > 50) pageSize = 50;

        var query = new GetBeaconsQuery(search, type, status, districtId, schoolId, isAssigned, page, pageSize);
        var result = await Mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "SchoolAdminOrAbove")]
    public async Task<ActionResult<BeaconResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetBeaconByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<BeaconResponse>> Create([FromBody] CreateBeaconRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new CreateBeaconCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<BeaconResponse>> Update(int id, [FromBody] UpdateBeaconRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new UpdateBeaconCommand(id, request), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteBeaconCommand(id), cancellationToken);
        return NoContent();
    }
}
