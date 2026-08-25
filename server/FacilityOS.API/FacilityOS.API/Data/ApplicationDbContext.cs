using FacilityOS.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FacilityOS.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<School> Schools { get; set; }
    public DbSet<Beacon> Beacons { get; set; }      
    public DbSet<Faculty> Faculties { get; set; }    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
