using FacilityOS.API.DTOs.Auth;
using MediatR;

namespace FacilityOS.API.Features.Auth.Login;

public record LoginCommand(LoginRequest Request) : IRequest<LoginResponse>;
