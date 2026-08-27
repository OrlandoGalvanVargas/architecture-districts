using FacilityOS.Application.DTOs.Faculties;
using MediatR;

namespace FacilityOS.Application.Features.Faculties.UpdateFaculty;

public record UpdateFacultyCommand(int Id, UpdateFacultyRequest Request) : IRequest<FacultyResponse>;