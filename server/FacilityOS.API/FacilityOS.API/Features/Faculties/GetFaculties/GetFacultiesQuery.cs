using FacilityOS.API.Common;
using FacilityOS.API.DTOs.Faculties;
using MediatR;

namespace FacilityOS.API.Features.Faculties.GetFaculties;

public record GetFacultiesQuery(
    string? Search = null,
    int? DistrictId = null,
    int? SchoolId = null,
    bool? IsActive = null,
    bool? HasBeacon = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<PagedResult<FacultyResponse>>;