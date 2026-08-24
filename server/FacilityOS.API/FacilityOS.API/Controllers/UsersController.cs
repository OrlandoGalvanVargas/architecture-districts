using FacilityOS.API.DTOs.Schools;
using FacilityOS.API.DTOs.Users;
using FacilityOS.API.Features.Schools.GetSchools;
using FacilityOS.API.Features.Users.CreateUser;
using FacilityOS.API.Features.Users.DeleteUser;
using FacilityOS.API.Features.Users.GetUserById;
using FacilityOS.API.Features.Users.GetUsers;
using FacilityOS.API.Features.Users.UpdateUser;
using FacilityOS.API.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FacilityOS.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
[EnableRateLimiting("global")]
public class UsersController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "SchoolAdminOrAbove")]
    public async Task<ActionResult<PagedResult<UserResponse>>> GetUsers(
        [FromQuery] string? search,
        [FromQuery] string? role,
        [FromQuery] UserEntityType? entityType,
        [FromQuery] int? entityId,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageSize > 50) pageSize = 50;

        var result = await Mediator.Send(new GetUsersQuery(
            search, role, entityType, entityId, isActive, page, pageSize));

        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "SchoolAdminOrAbove")]
    public async Task<ActionResult<UserResponse>> GetById(int id)
    {
        var result = await Mediator.Send(new GetUserByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "SchoolAdminOrAbove")]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
    {
        var result = await Mediator.Send(new CreateUserCommand(request));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "SchoolAdminOrAbove")]
    public async Task<ActionResult<UserResponse>> Update(int id, [FromBody] UpdateUserRequest request)
    {
        var result = await Mediator.Send(new UpdateUserCommand(id, request));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "DistrictAdminOrAbove")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }
}