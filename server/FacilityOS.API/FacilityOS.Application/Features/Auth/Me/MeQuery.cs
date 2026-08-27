using FacilityOS.Application.DTOs.Users;
using MediatR;

namespace FacilityOS.Application.Features.Auth.Me;

public record MeQuery : IRequest<UserResponse>;