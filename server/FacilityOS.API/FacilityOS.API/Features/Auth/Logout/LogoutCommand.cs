using MediatR;

namespace FacilityOS.API.Features.Auth.Logout
{
    public record LogoutCommand(string RefreshToken) : IRequest<bool>;
}
