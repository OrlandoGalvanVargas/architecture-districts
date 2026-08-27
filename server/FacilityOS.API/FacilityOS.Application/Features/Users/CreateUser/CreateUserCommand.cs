using FacilityOS.Application.DTOs.Users;
using MediatR;

namespace FacilityOS.Application.Features.Users.CreateUser;

public record CreateUserCommand(CreateUserRequest Request) : IRequest<UserResponse>;