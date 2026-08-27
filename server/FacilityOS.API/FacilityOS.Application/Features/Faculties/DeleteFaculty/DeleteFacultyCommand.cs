using MediatR;

namespace FacilityOS.Application.Features.Faculties.DeleteFaculty;

public record DeleteFacultyCommand(int Id) : IRequest;