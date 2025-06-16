using AoristoTowersFunctions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Net;
using System.Reflection; 
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AoristoTowersFunctions.Middleware
{
    /// <summary>
    /// Atributo personalizado para decorar las funciones que requieren autorización.
    /// Funciona en conjunto con AuthorizationMiddleware.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class AuthorizeAttribute : Attribute
    {
        public string? Roles { get; set; }
    }

    /// <summary>
    /// Middleware que intercepta, valida el token JWT (autenticación) y verifica
    /// los roles del usuario contra los requeridos por la función (autorización).
    /// </summary>
    public class AuthorizationMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly ILogger<AuthorizationMiddleware> _logger;

        public AuthorizationMiddleware(ILogger<AuthorizationMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            // --- CORRECCIÓN CLAVE: Usamos Reflexión para obtener el atributo ---
            var targetMethod = GetTargetFunctionMethod(context);
            var authorizeAttribute = targetMethod?.GetCustomAttribute<AuthorizeAttribute>();

            // Si la función no está decorada con nuestro atributo, no requiere autorización.
            if (authorizeAttribute == null)
            {
                await next(context);
                return;
            }

            var request = await context.GetHttpRequestDataAsync();
            if (request == null)
            {
                await next(context);
                return;
            }

            if (!request.Headers.TryGetValues("Authorization", out var authHeaderValues) ||
                !authHeaderValues.Any() ||
                !authHeaderValues.First().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                await SetErrorResponse(request, HttpStatusCode.Unauthorized, "Authorization header missing or invalid.");
                return;
            }

            var token = authHeaderValues.First().Substring("Bearer ".Length).Trim();
            var jwtOptions = context.InstanceServices.GetRequiredService<JwtOptions>();

            try
            {
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(jwtOptions.Key);

                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                if (!string.IsNullOrEmpty(authorizeAttribute.Roles))
                {
                    var requiredRoles = authorizeAttribute.Roles.Split(',').Select(r => r.Trim());
                    if (!requiredRoles.Any(role => claimsPrincipal.IsInRole(role)))
                    {
                        await SetErrorResponse(request, HttpStatusCode.Forbidden, "Access denied. Insufficient permissions.");
                        return;
                    }
                }

                context.Features.Set(claimsPrincipal);
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Token validation failed.");
                await SetErrorResponse(request, HttpStatusCode.Unauthorized, "Invalid token.");
            }
        }

        /// <summary>
        /// Usa reflexión para obtener la información del método (MethodInfo) de la función que se está ejecutando.
        /// </summary>
        private MethodInfo? GetTargetFunctionMethod(FunctionContext context)
        {
            // El EntryPoint contiene el nombre completo del método, ej: "MiNamespace.MiClase.MiMetodo"
            var entryPoint = context.FunctionDefinition.EntryPoint;
            if (string.IsNullOrEmpty(entryPoint)) return null;

            // Cargamos el ensamblado que contiene la función
            var assemblyPath = context.FunctionDefinition.PathToAssembly;
            var assembly = Assembly.LoadFrom(assemblyPath);

            // Separamos el nombre de la clase del nombre del método
            var typeName = entryPoint.Substring(0, entryPoint.LastIndexOf('.'));
            var methodName = entryPoint.Substring(entryPoint.LastIndexOf('.') + 1);

            var type = assembly.GetType(typeName);
            return type?.GetMethod(methodName);
        }

        private async Task SetErrorResponse(HttpRequestData request, HttpStatusCode statusCode, string message)
        {
            var response = request.CreateResponse(statusCode);
            // Es buena práctica devolver un cuerpo JSON incluso para errores.
            await response.WriteAsJsonAsync(new { message });
            request.FunctionContext.GetInvocationResult().Value = response;
        }
    }
}
