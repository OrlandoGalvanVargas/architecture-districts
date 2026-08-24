using FacilityOS.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FacilityOS.API.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.Property(x => x.Role)
            .HasMaxLength(50)
            .HasDefaultValue("User");

        builder.Property(x => x.EntityType)
            .HasConversion<string>();

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // OPTIMIZADO: Se eliminó el bloque HasMany de aquí para centralizarlo en RefreshTokenConfiguration

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
