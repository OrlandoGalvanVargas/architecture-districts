using FacilityOS.API.DTOs.Auth;
using MediatR;

namespace FacilityOS.API.Features.Auth.Me
{
    public record MeQuery(int UserId) : IRequest<UserDto?>;
}
