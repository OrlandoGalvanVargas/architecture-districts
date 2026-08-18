using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Schools;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Schools.GetSchoolById;

public class GetSchoolByIdHandler : IRequestHandler<GetSchoolByIdQuery, SchoolResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public GetSchoolByIdHandler(ApplicationDbContext context, IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<SchoolResponse> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
    {
        var school = await _context.Schools
            .AsNoTracking()
            .Include(s => s.District)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (school is null)
            throw new NotFoundException(nameof(School), request.Id);

        var canAccess = await _authService.CanAccessSchoolAsync(request.Id, cancellationToken);
        if (!canAccess)
            throw new ForbiddenException("You do not have permission to view this school.");

        return new SchoolResponse
        {
            Id = school.Id,
            Name = school.Name,
            SchoolCode = school.SchoolCode,
            Level = school.Level.ToString(),
            Type = school.Type.ToString(),
            Address = school.Address,
            City = school.City,
            State = school.State,
            ZipCode = school.ZipCode,
            Phone = school.Phone,
            ContactEmail = school.ContactEmail,
            StudentCapacity = school.StudentCapacity,
            IsActive = school.IsActive,
            DistrictId = school.DistrictId,
            DistrictName = school.District.Name,
            CreatedAt = school.CreatedAt,
            UpdatedAt = school.UpdatedAt,
        };
    }
}