using FacilityOS.API.DTOs.Faculties;
using MediatR;

namespace FacilityOS.API.Features.Faculties.GetFacultyById;

public record GetFacultyByIdQuery(int Id) : IRequest<FacultyResponse>;