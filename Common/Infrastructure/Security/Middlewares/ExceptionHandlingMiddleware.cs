using Common.Exceptions;
using Common.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Common.Infrastructure.Security.Middlewares
{
    /// <summary>
    /// Middleware que intercepta todas las excepciones no manejadas
    /// y devuelve un ApiResponse&lt;object&gt; con formato JSON.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                // Excepciones derivadas de AppException (NotFound, Conflict, etc.)
                _logger.LogWarning(ex, "AppException capturada en middleware");
                await HandleAppExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                // Cualquier otra excepción inesperada
                _logger.LogError(ex, "Excepción no controlada en middleware");
                await HandleUnknownExceptionAsync(context, ex);
            }
        }

        private static async Task HandleAppExceptionAsync(HttpContext context, AppException ex)
        {
            // Establece el status code definido en la excepción
            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.Fail(ex.Message);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }

        private static async Task HandleUnknownExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.Fail("Ha ocurrido un error inesperado.");
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}