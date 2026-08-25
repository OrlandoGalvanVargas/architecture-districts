using FacilityOS.API.DTOs.Faculties;
using MediatR;

namespace FacilityOS.API.Features.Faculties.CreateFaculty;

public record CreateFacultyCommand(CreateFacultyRequest Request) : IRequest<FacultyResponse>;