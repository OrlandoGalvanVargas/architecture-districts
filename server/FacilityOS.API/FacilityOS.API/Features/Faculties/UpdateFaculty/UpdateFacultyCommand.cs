using FacilityOS.API.DTOs.Faculties;
using MediatR;

namespace FacilityOS.API.Features.Faculties.UpdateFaculty;

public record UpdateFacultyCommand(int Id, UpdateFacultyRequest Request) : IRequest<FacultyResponse>;