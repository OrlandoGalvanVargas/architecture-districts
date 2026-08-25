using FacilityOS.API.DTOs.Auth;
using MediatR;

namespace FacilityOS.API.Features.Auth.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<LoginResponse>;
