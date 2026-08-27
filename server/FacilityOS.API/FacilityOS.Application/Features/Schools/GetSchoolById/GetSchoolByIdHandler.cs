using FacilityOS.Application.Common.Exceptions;
using FacilityOS.Application.Common.Mapping;
using FacilityOS.Application.DTOs.Schools;
using FacilityOS.Application.Services;
using FacilityOS.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Application.Features.Schools.GetSchoolById;

public class GetSchoolByIdHandler : IRequestHandler<GetSchoolByIdQuery, SchoolResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public GetSchoolByIdHandler(IApplicationDbContext context, IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<SchoolResponse> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
    {
        var schoolResponse = await _context.Schools
            .AsNoTracking()
            .Where(s => s.Id == request.Id)
            .ProjectToResponse()
            .FirstOrDefaultAsync(cancellationToken);

        if (schoolResponse is null)
            throw new NotFoundException(nameof(School), request.Id);

        var canAccess = await _authService.CanAccessSchoolAsync(request.Id, cancellationToken);
        if (!canAccess)
            throw new ForbiddenException("You do not have permission to view this school.");

        return schoolResponse;
    }
}
