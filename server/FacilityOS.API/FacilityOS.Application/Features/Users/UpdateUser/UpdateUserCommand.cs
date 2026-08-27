using FacilityOS.Application.DTOs.Users;
using MediatR;

namespace FacilityOS.Application.Features.Users.UpdateUser;

public record UpdateUserCommand(int Id, UpdateUserRequest Request) : IRequest<UserResponse>;