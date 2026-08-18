using FacilityOS.API.Common;
using FacilityOS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FacilityOS.API.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context, IConfiguration configuration, ILogger logger)
        {
            await context.Database.MigrateAsync();

            await SeedAdminUserAsync(context, configuration, logger);
        }

        private static async Task SeedAdminUserAsync(ApplicationDbContext context, IConfiguration configuration, ILogger logger)
        {
            var adminExists = await context.Users
                .AnyAsync(u => u.Role == AppRoles.Admin);

            if (adminExists)
            {
                logger.LogInformation("Admin user already exists, skipping seed.");
                return;
            }

            var adminPassword = configuration["Seed:AdminPassword"];
            var adminEmail = configuration["Seed:AdminEmail"] ?? "admin@facilityos.com";
            var adminName = configuration["Seed:AdminName"] ?? "System Administrator";

            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                logger.LogWarning("Seed:AdminPassword is not configured. Skipping admin user creation.");
                return;
            }

            var workFactor = configuration.GetValue<int>("BCrypt:WorkFactor", 12);
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword, workFactor);

            var admin = new User
            {
                Name = adminName,
                Email = adminEmail,
                PasswordHash = passwordHash,
                Role = AppRoles.Admin,
                EntityType = UserEntityType.Global,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();

            logger.LogInformation("Admin user created: {Email}", adminEmail);
        }
    }
}