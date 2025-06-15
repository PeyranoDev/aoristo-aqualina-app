using Data.Repositories.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class ScheduledTasks
{
    private readonly ILogger<ScheduledTasks> _logger;
    private readonly ITokenRepository _tokenRepo;
    private readonly IRequestRepository _requestRepo;

    public ScheduledTasks(ILogger<ScheduledTasks> logger, ITokenRepository tokenRepo, IRequestRepository requestRepo)
    {
        _logger = logger;
        _tokenRepo = tokenRepo;
        _requestRepo = requestRepo;
    }

    [Function("CleanupTask")]
    public async Task Run([TimerTrigger("0 0 */6 * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"Ejecutando tareas de limpieza programadas: {DateTime.UtcNow}");

        var tokenExpiration = TimeSpan.FromDays(30);
        var requestExpiration = TimeSpan.FromDays(90);

        try
        {
            var tokensCleaned = await _tokenRepo.DeleteExpiredTokensAsync(tokenExpiration);
            _logger.LogInformation(tokensCleaned ? "Tokens expirados eliminados." : "No se encontraron tokens para limpiar.");

            var requestsCleaned = await _requestRepo.DeleteOldRequestsAsync(requestExpiration);
            _logger.LogInformation(requestsCleaned ? "Solicitudes antiguas eliminadas." : "No se encontraron solicitudes para limpiar.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió un error durante las tareas de limpieza.");
        }
    }
}