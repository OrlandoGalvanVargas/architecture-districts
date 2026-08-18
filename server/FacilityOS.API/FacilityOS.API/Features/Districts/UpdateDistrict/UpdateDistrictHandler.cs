using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Data;
using FacilityOS.API.DTOs.Districts;
using FacilityOS.API.Models;
using FacilityOS.API.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Districts.UpdateDistrict;

public class UpdateDistrictHandler : IRequestHandler<UpdateDistrictCommand, DistrictResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public UpdateDistrictHandler(ApplicationDbContext context, IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<DistrictResponse> Handle(UpdateDistrictCommand command, CancellationToken cancellationToken)
    {
        var district = await _context.Districts.FirstOrDefaultAsync(d => d.Id == command.Id, cancellationToken);

        if (district is null)
            throw new NotFoundException(nameof(District), command.Id);

        var canAccess = await _authService.CanAccessDistrictAsync(command.Id, cancellationToken);
        if (!canAccess)
            throw new ForbiddenException("You do not have permission to modify this district.");
            
        var req = command.Request;

        var codeTaken = await _context.Districts.AnyAsync(d => d.Code == req.Code && d.Id != command.Id, cancellationToken);
        if (codeTaken)
            throw new InvalidOperationException($"A district with the code '{req.Code}' already exists.");

        district.Name = req.Name;
        district.Code = req.Code;
        district.State = req.State;
        district.City = req.City;
        district.ZipCode = req.ZipCode;
        district.Address = req.Address;
        district.Description = req.Description;
        district.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new DistrictResponse
        {
            Id = district.Id,
            Name = district.Name,
            Code = district.Code,
            State = district.State,
            City = district.City,
            ZipCode = district.ZipCode,
            Address = district.Address,
            Description = district.Description,
            SchoolCount = district.Schools.Count,
            CreatedAt = district.CreatedAt,
            UpdatedAt = district.UpdatedAt
        };
    }
}