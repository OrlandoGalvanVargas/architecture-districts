using FacilityOS.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.Application.Services;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<District> Districts { get; }
    DbSet<School> Schools { get; }
    DbSet<Beacon> Beacons { get; }
    DbSet<Faculty> Faculties { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
