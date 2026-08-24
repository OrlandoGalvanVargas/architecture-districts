using FacilityOS.API.DTOs.Users;
using MediatR;

namespace FacilityOS.API.Features.Auth.Me;

public record MeQuery : IRequest<UserResponse>;