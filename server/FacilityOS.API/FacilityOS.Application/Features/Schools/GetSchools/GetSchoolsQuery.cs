using FacilityOS.Application.Common;
using FacilityOS.Application.DTOs.Schools;
using MediatR;

namespace FacilityOS.Application.Features.Schools.GetSchools;

public record GetSchoolsQuery(
    int? DistrictId,
    string? Search,
    string? Level,
    string? Type,
    bool? IsActive,
    int Page,
    int PageSize) : IRequest<PagedResult<SchoolResponse>>;

