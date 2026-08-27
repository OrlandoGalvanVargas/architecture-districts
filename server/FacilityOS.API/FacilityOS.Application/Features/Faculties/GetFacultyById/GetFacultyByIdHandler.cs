using FacilityOS.Application.Common.Exceptions;
using FacilityOS.Application.Common.Mapping;
using FacilityOS.Application.DTOs.Faculties;
using FacilityOS.Application.Services;
using FacilityOS.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Application.Features.Faculties.GetFacultyById;

public class GetFacultyByIdQueryHandler : IRequestHandler<GetFacultyByIdQuery, FacultyResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IResourceAuthorizationService _authService;

    public GetFacultyByIdQueryHandler(
        IApplicationDbContext context,
        IResourceAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<FacultyResponse> Handle(GetFacultyByIdQuery request, CancellationToken cancellationToken)
    {
        var facultyResponse = await _context.Faculties
            .AsNoTracking()
            .Where(f => f.Id == request.Id)
            .ProjectToResponse()
            .FirstOrDefaultAsync(cancellationToken);

        if (facultyResponse is null)
            throw new NotFoundException(nameof(Faculty), request.Id);

        var canAccess = await _authService.CanAccessFacultyAsync(request.Id, cancellationToken);
        if (!canAccess)
            throw new ForbiddenException("You do not have permission to view this faculty.");

        return facultyResponse;
    }
}
