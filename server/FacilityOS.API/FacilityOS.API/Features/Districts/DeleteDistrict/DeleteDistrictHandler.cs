using FacilityOS.API.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Features.Districts.DeleteDistrict
{
    public class DeleteDistrictHandler : IRequestHandler<DeleteDistrictCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public DeleteDistrictHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteDistrictCommand command, CancellationToken cancellationToken)
        {
            var district = await _context.Districts.FirstOrDefaultAsync(d => d.Id == command.Id, cancellationToken);

            if (district is null)
                return false;

            _context.Districts.Remove(district);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
