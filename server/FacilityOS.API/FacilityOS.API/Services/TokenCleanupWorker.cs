using FacilityOS.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FacilityOS.API.Services;

public class TokenCleanupWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TokenCleanupWorker> _logger;

    private readonly TimeSpan _executionInterval = TimeSpan.FromHours(24);

    public TokenCleanupWorker(IServiceProvider serviceProvider, ILogger<TokenCleanupWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 Token Cleanup Background Service has started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("🧹 Starting scheduled refresh token purge...");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var utcNow = DateTime.UtcNow;

                    int deletedRows = await context.RefreshTokens
                        .Where(rt => rt.IsRevoked || rt.ExpiresAt < utcNow)
                        .ExecuteDeleteAsync(stoppingToken);

                    _logger.LogInformation("✅ Purge completed successfully. Removed {Count} dead refresh tokens from database.", deletedRows);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ An error occurred while purging expired refresh tokens.");
            }

            _logger.LogInformation("💤 Token Cleanup Service is going to sleep for 24 hours.");
            await Task.Delay(_executionInterval, stoppingToken);
        }
    }
}
