using FacilityOS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FacilityOS.API.Data.Configurations;

public class BeaconConfiguration : IEntityTypeConfiguration<Beacon>
{
    public void Configure(EntityTypeBuilder<Beacon> builder)
    {
        builder.ToTable("Beacons");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DeviceName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.SerialNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.SerialNumber)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.HasOne(x => x.District)
            .WithMany(d => d.Beacons)
            .HasForeignKey(x => x.DistrictId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.School)
            .WithMany(s => s.Beacons)
            .HasForeignKey(x => x.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Faculty)
            .WithOne(f => f.Beacon)
            .HasForeignKey<Beacon>(x => x.FacultyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.DistrictId);
        builder.HasIndex(x => x.SchoolId);

        builder.HasIndex(x => x.FacultyId)
            .IsUnique()
            .HasFilter("[FacultyId] IS NOT NULL");

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
