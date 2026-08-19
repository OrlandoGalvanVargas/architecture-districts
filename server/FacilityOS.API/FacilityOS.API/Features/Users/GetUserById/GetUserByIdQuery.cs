using FacilityOS.API.DTOs.Users;
using MediatR;

namespace FacilityOS.API.Features.Users.GetUserById;

public record GetUserByIdQuery(int Id) : IRequest<UserResponse>;