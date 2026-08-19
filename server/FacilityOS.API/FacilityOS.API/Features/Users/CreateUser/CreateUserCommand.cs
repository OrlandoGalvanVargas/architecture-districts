using FacilityOS.API.DTOs.Users;
using MediatR;

namespace FacilityOS.API.Features.Users.CreateUser;

public record CreateUserCommand(CreateUserRequest Request) : IRequest<UserResponse>;