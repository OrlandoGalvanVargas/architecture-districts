using FacilityOS.API.DTOs.Schools;
using MediatR;

namespace FacilityOS.API.Features.Schools.GetSchools
{
    public record GetSchoolsQuery(
        int? DistrictId, 
        string? Search,
        string? Level, 
        string? Type, 
        bool? IsActive, 
        int Page, 
        int PageSize) : IRequest<PagedResult<SchoolResponse>>;

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }    

}
