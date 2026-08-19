using MediatR;

namespace FacilityOS.API.Features.Users.DeleteUser;

public record DeleteUserCommand(int Id) : IRequest<bool>;