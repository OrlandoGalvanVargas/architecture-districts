using FacilityOS.Application.DTOs.Auth;
using MediatR;

namespace FacilityOS.Application.Features.Auth.Login;

public record LoginCommand(LoginRequest Request) : IRequest<AuthResult>;
