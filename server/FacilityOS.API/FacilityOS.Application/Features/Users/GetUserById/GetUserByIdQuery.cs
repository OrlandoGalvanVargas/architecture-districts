using FacilityOS.Application.DTOs.Users;
using MediatR;

namespace FacilityOS.Application.Features.Users.GetUserById;

public record GetUserByIdQuery(int Id) : IRequest<UserResponse>;