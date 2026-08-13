using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Schools;
using FacilityOS.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Schools.GetSchools
{
    public class GetSchoolsHandler : IRequestHandler<GetSchoolsQuery, PagedResult<SchoolResponse>>
    {
        private readonly ApplicationDbContext _context;

        public GetSchoolsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<SchoolResponse>> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Schools.Include(s => s.District).AsQueryable();

            if (request.DistrictId.HasValue)
                query = query.Where(s => s.DistrictId == request.DistrictId.Value);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchTerm = request.Search.Trim().ToLower();
                query = query.Where(s => s.Name.ToLower().Contains(searchTerm) ||
                                         s.SchoolCode.ToLower().Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(request.Level) && Enum.TryParse<SchoolLevel>(request.Level, true, out var level))
                query = query.Where(s => s.Level == level);

            if (!string.IsNullOrEmpty(request.Type) && Enum.TryParse<SchoolType>(request.Type, true, out var type))
                query = query.Where(s => s.Type == type);
                
            if (request.IsActive.HasValue)
                query = query.Where(s => s.IsActive == request.IsActive.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(s => s.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new SchoolResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    SchoolCode = s.SchoolCode,
                    Level = s.Level.ToString(),
                    Type = s.Type.ToString(),
                    Address = s.Address,
                    City = s.City,
                    State = s.State,
                    ZipCode = s.ZipCode,
                    Phone = s.Phone,
                    ContactEmail = s.ContactEmail,
                    StudentCapacity = s.StudentCapacity,
                    IsActive = s.IsActive,
                    DistrictId = s.DistrictId,
                    DistrictName = s.District.Name,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<SchoolResponse>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
