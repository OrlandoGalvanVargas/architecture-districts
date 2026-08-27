using FacilityOS.Application.DTOs.Faculties;
using MediatR;

namespace FacilityOS.Application.Features.Faculties.CreateFaculty;

public record CreateFacultyCommand(CreateFacultyRequest Request) : IRequest<FacultyResponse>;