using FacilityOS.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FacilityOS.API.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
            .IsRequired();

        builder.HasIndex(x => x.Token)
            .IsUnique();

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // SOLUCIÓN AL WARNING: Mapeo explícito de la relación configurada como Opcional
        // Esto cambia el INNER JOIN por un LEFT JOIN y blinda tu auditoría
        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .IsRequired(false) // <- La clave de la solución
            .OnDelete(DeleteBehavior.Cascade); // Mantiene la eliminación en cascada si se borra físicamente al usuario

        builder.Ignore(x => x.IsExpired);
        builder.Ignore(x => x.IsActive);
    }
}
