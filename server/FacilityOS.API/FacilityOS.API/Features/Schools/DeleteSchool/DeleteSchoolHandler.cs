using FacilityOS.API.Common.Exceptions;
using FacilityOS.API.Data;
using FacilityOS.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Schools.DeleteSchool;

public class DeleteSchoolHandler : IRequestHandler<DeleteSchoolCommand, bool>
{
    private readonly ApplicationDbContext _context;

    public DeleteSchoolHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteSchoolCommand command, CancellationToken cancellationToken)
    {
        var school = await _context.Schools.FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

        if (school is null)
            throw new NotFoundException(nameof(School), command.Id);

        _context.Remove(school);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}