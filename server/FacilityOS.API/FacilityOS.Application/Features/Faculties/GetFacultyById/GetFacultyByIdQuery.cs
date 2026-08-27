using FacilityOS.Application.DTOs.Faculties;
using MediatR;

namespace FacilityOS.Application.Features.Faculties.GetFacultyById;

public record GetFacultyByIdQuery(int Id) : IRequest<FacultyResponse>;