using MediatR;

namespace FacilityOS.Application.Features.Users.DeleteUser;

public record DeleteUserCommand(int Id) : IRequest;
