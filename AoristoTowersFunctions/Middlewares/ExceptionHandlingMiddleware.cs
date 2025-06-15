using Common.Exceptions;
using Common.Models.Responses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System;
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
        var apiResponse = ApiResponse<object>.Fail("Ha ocurrido un error inesperado.");

        if (exception is AppException appException)
        {
            _logger.LogWarning("App controlled exception: {Message}", appException.Message);
            statusCode = (HttpStatusCode)appException.StatusCode;
            apiResponse = ApiResponse<object>.Fail(appException.Message);
        }

        var response = httpRequest.CreateResponse(statusCode);
        await response.WriteAsJsonAsync(apiResponse);

        var invocationResult = context.GetInvocationResult();
        invocationResult.Value = response;
    }
}