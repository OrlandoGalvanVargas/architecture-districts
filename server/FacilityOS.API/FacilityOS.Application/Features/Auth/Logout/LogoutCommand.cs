using MediatR;

namespace FacilityOS.Application.Features.Auth.Logout;

public record LogoutCommand(string? RefreshToken) : IRequest;