using FacilityOS.API.Common;
using FacilityOS.API.DTOs.Schools;
using MediatR;

namespace FacilityOS.API.Features.Schools.GetSchools;
    public record GetSchoolsQuery(
        int? DistrictId, 
        string? Search,
        string? Level, 
        string? Type, 
        bool? IsActive, 
        int Page, 
        int PageSize) : IRequest<PagedResult<SchoolResponse>>;   

