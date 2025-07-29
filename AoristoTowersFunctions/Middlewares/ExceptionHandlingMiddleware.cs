using AoristoTowersFunctions.Helpers;
using Common.Exceptions;
using Common.Models.Responses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System;
using System.Linq; 
using System.Net;
using System.Text.Json; 
using System.Threading.Tasks;

public class ExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió una excepción no controlada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(FunctionContext context, Exception exception)
    {
        var httpRequest = await context.GetHttpRequestDataAsync();

        if (httpRequest == null)
            return;

        var statusCode = HttpStatusCode.InternalServerError;
        string errorMessage = "Ha ocurrido un error inesperado.";

        var innerException = (exception is AggregateException aggEx && aggEx.InnerExceptions.Any())
                             ? aggEx.InnerExceptions.First()
                             : exception;

        if (innerException is AppException appException)
        {
            _logger.LogWarning("Excepción controlada por la aplicación: {Message}", appException.Message);
            statusCode = (HttpStatusCode)appException.StatusCode;
            errorMessage = appException.Message;
        }
        else if (innerException is JsonException)
        {
            statusCode = HttpStatusCode.BadRequest;
            errorMessage = "El cuerpo de la solicitud no tiene un formato JSON válido.";
            _logger.LogWarning("Error de formato JSON: {Message}", innerException.Message);
        }
        else if (innerException is ArgumentException)
        {
            statusCode = HttpStatusCode.BadRequest;
            errorMessage = "Argumento(s) inválido(s) en la solicitud.";
            _logger.LogWarning("Error de argumento: {Message}", innerException.Message);
        }

        var apiResponse = ApiResponse<object>.Fail(errorMessage);

        var response = await httpRequest.CreateJsonResponse(statusCode, apiResponse);

        var invocationResult = context.GetInvocationResult();
        invocationResult.Value = response;
    }
}