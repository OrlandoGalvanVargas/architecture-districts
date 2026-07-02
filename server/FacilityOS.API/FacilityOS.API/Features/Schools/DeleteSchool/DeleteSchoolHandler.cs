using FacilityOS.API.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace FacilityOS.API.Features.Schools.DeleteSchool
{
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
                return false;

            _context.Remove(school);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
