using FacilityOS.Application.Common.Exceptions;
using FacilityOS.Application.Common.Mapping;
using FacilityOS.Application.DTOs.Schools;
using FacilityOS.Application.Services;
using FacilityOS.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Application.Features.Schools.UpdateSchool;

public class UpdateSchoolHandler : IRequestHandler<UpdateSchoolCommand, SchoolResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public UpdateSchoolHandler(IApplicationDbContext context, IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<SchoolResponse> Handle(UpdateSchoolCommand command, CancellationToken cancellationToken)
    {
        var canManage = await _authService.CanManageSchoolAsync(command.Id, cancellationToken);
        if (!canManage)
            throw new ForbiddenException("You do not have permission to modify this school.");

        var school = await _context.Schools
            .Include(s => s.District)
            .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

        if (school is null)
            throw new NotFoundException(nameof(School), command.Id);

        var req = command.Request;

        if (school.DistrictId != req.DistrictId)
        {
            var canMove = await _authService.CanCreateSchoolInDistrictAsync(req.DistrictId, cancellationToken);
            if (!canMove)
                throw new ForbiddenException("You do not have permission to move a school to the target district.");

            var targetDistrict = await _context.Districts
                .FirstOrDefaultAsync(d => d.Id == req.DistrictId, cancellationToken);

            if (targetDistrict is null)
                throw new NotFoundException(nameof(District), req.DistrictId);
        }

        var codeTaken = await _context.Schools
            .AnyAsync(s => s.SchoolCode.ToLower() == req.SchoolCode.ToLower().Trim() && s.Id != command.Id, cancellationToken);

        if (codeTaken)
            throw new ConflictException($"A school with the code '{req.SchoolCode}' already exists.");

        school.Update(
            req.Name.Trim(),
            req.SchoolCode.ToUpper().Trim(),
            req.Level,
            req.Type,
            req.Address.Trim(),
            req.City.Trim(),
            req.State.ToUpper().Trim(),
            req.ZipCode.Trim(),
            req.StudentCapacity,
            req.Phone?.Trim(),
            req.ContactEmail?.ToLower().Trim()
        );

        if (req.IsActive != school.IsActive)
        {
            if (req.IsActive)
                school.Activate();
            else
                school.Deactivate();
        }

        if (school.DistrictId != req.DistrictId)
            school.MoveToDistrict(req.DistrictId);

        await _context.SaveChangesAsync(cancellationToken);

        return school.ToResponse();
    }
}
