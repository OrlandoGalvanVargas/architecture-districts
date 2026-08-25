using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Common.Mapping; 
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Schools;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Schools.CreateSchool;

public class CreateSchoolHandler : IRequestHandler<CreateSchoolCommand, SchoolResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public CreateSchoolHandler(ApplicationDbContext context, IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<SchoolResponse> Handle(CreateSchoolCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;

        var canCreate = await _authService.CanCreateSchoolInDistrictAsync(req.DistrictId, cancellationToken);
        if (!canCreate)
            throw new ForbiddenException("You do not have permission to create a school in this district.");

        var district = await _context.Districts
            .FirstOrDefaultAsync(d => d.Id == req.DistrictId, cancellationToken);

        if (district is null)
            throw new NotFoundException(nameof(District), req.DistrictId);

        var codeExists = await _context.Schools
            .AnyAsync(s => s.SchoolCode.ToLower() == req.SchoolCode.ToLower().Trim(), cancellationToken);

        if (codeExists)
            throw new ConflictException($"A school with code '{req.SchoolCode}' already exists.");

        var school = req.ToEntity();

        _context.Schools.Add(school);
        await _context.SaveChangesAsync(cancellationToken);

        return school.ToResponse();
    }
}
