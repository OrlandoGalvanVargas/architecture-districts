using FacilityOS.API.Common;
using FacilityOS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<School> Schools { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.EntityType).HasConversion<string>(); 

                entity.HasMany(u => u.RefreshTokens)
                    .WithOne(rt => rt.User)
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Token).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.Ignore(rt => rt.IsExpired);
                entity.Ignore(rt => rt.IsActive);
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<School>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.HasIndex(s => s.SchoolCode).IsUnique();
                entity.Property(s => s.Level).HasConversion<string>();
                entity.Property(s => s.Type).HasConversion<string>();

                entity.HasOne(s => s.District)
                    .WithMany(s => s.Schools)
                    .HasForeignKey(s => s.DistrictId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<District>().HasData(
                new District
                {
                    Id = 1,
                    Name = "Los Angeles Unified School District",
                    Code = "LAUSD-001",
                    State = "CA",
                    City = "Los Angeles",
                    ZipCode = "90012",
                    Address = "333 S Beaudry Ave",
                    Description = "Largest school district in California",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new District
                {
                    Id = 2,
                    Name = "San Diego Unified School District",
                    Code = "SDUSD-002",
                    State = "CA",
                    City = "San Diego",
                    ZipCode = "92101",
                    Address = "4100 Normal St",
                    Description = "Second largest district in San Diego County",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}