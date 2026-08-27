using FacilityOS.Application.DTOs.Auth;
using MediatR;

namespace FacilityOS.Application.Features.Auth.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResult>;