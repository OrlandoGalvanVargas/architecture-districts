using FacilityOS.API.DTOs.Users;
using MediatR;

namespace FacilityOS.API.Features.Users.UpdateUser;

public record UpdateUserCommand(int Id, UpdateUserRequest Request) : IRequest<UserResponse>;