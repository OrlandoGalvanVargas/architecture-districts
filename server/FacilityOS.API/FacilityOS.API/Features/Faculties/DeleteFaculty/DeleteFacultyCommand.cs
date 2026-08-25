using MediatR;

namespace FacilityOS.API.Features.Faculties.DeleteFaculty;

public record DeleteFacultyCommand(int Id) : IRequest;