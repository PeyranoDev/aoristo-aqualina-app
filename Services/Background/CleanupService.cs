using Data.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Background
{
    public class CleanupService : BackgroundService
    {
        private readonly ILogger<CleanupService> _logger;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _config;

        public CleanupService(
            IServiceProvider services,
            ILogger<CleanupService> logger,
            IConfiguration config)
        {
            _services = services;
            _logger = logger;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Cleanup Service is starting");

            var cleanupInterval = TimeSpan.FromHours(
                int.TryParse(_config.GetSection("CleanupSettings:IntervalHours")?.Value, out var intervalHours) ? intervalHours : 6);

            var tokenExpiration = TimeSpan.FromDays(
                int.TryParse(_config.GetSection("CleanupSettings:TokenExpirationDays")?.Value, out var tokenDays) ? tokenDays : 30);

            var requestExpiration = TimeSpan.FromDays(
                int.TryParse(_config.GetSection("CleanupSettings:RequestExpirationDays")?.Value, out var requestDays) ? requestDays : 90);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Running cleanup tasks...");

                    var tokensCleaned = await CleanupTokensAsync(tokenExpiration);
                    var requestsCleaned = await CleanupRequestsAsync(requestExpiration);

                    _logger.LogInformation($"Cleanup completed. Tokens: {tokensCleaned}, Requests: {requestsCleaned}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during cleanup tasks");
                }

                await Task.Delay(cleanupInterval, stoppingToken);
            }

            _logger.LogInformation("Cleanup Service is stopping");
        }

        private async Task<bool> CleanupTokensAsync(TimeSpan expiration)
        {
            using var scope = _services.CreateScope();
            var tokenRepo = scope.ServiceProvider.GetRequiredService<ITokenRepository>();
            return await tokenRepo.DeleteExpiredTokensAsync(expiration);
        }

        private async Task<bool> CleanupRequestsAsync(TimeSpan expiration)
        {
            using var scope = _services.CreateScope();
            var requestRepo = scope.ServiceProvider.GetRequiredService<IRequestRepository>();
            return await requestRepo.DeleteOldRequestsAsync(expiration);
        }
    }
}