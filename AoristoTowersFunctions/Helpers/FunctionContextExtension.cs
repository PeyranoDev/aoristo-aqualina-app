using Common.Models.Responses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace AoristoTowersFunctions.Helpers
{
    /// <summary>
    /// Métodos de extensión para simplificar el trabajo con el contexto de las funciones.
    /// </summary>
    public static class FunctionContextExtensions
    {
        /// <summary>
        /// Obtiene el ClaimsPrincipal del usuario autenticado desde el contexto de la función.
        /// </summary>
        public static ClaimsPrincipal? GetUser(this FunctionContext context)
        {
            var principal = context.Features.Get<ClaimsPrincipal>();
            return principal;
        }

        /// <summary>
        /// Obtiene el ID del usuario desde los claims del token JWT.
        /// </summary>
        public static int GetUserId(this FunctionContext context)
        {
            var principal = GetUser(context);
            var userIdClaim = principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        /// <summary>
        /// Obtiene el rol del usuario desde los claims del token JWT.
        /// </summary>
        public static string GetUserRole(this FunctionContext context)
        {
            var principal = GetUser(context);
            return principal?.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        }

        /// <summary>
        /// Adjunta una respuesta HTTP al contexto de la función para detener la ejecución.
        /// Usado por el middleware para devolver errores de 401/403.
        /// </summary>
        public static async Task SetHttpResponse(this FunctionContext context, HttpRequestData request, HttpStatusCode statusCode, object? responseBody = null)
        {
            var response = request.CreateResponse();
            response.StatusCode = statusCode;

            if (responseBody != null)
            {
                await response.WriteAsJsonAsync(responseBody);
            }

            context.Features.Set(response);
        }
    }

    /// <summary>
    /// Métodos de extensión para simplificar la creación de respuestas HTTP.
    /// </summary>
    public static class HttpRequestDataExtensions
    {
        /// <summary>
        /// Crea una respuesta HTTP con un cuerpo JSON y el código de estado especificado.
        /// </summary>
        public static async Task<HttpResponseData> CreateJsonResponse<T>(this HttpRequestData req, HttpStatusCode status, ApiResponse<T> body)
        {
            var response = req.CreateResponse(status);
            await response.WriteAsJsonAsync(body, cancellationToken: default); 
            return response;
        }
    }
}
