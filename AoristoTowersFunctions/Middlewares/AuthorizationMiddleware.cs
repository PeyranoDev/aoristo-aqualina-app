using AoristoTowersFunctions.Helpers;
using Common.Models.Responses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AoristoTowersFunctions.Middleware
{
    /// <summary>
    /// Atributo personalizado para decorar las funciones que requieren autorización.
    /// Funciona en conjunto con JwtAuthMiddleware.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class AuthorizeAttribute : Attribute
    {
        /// <summary>
        /// Una lista de roles separados por comas (ej. "Admin,Security") que están permitidos.
        /// Si se deja vacío, solo requiere un token válido sin importar el rol.
        /// </summary>
        public string? Roles { get; set; }
    }

    /// <summary>
    /// Middleware que intercepta las peticiones HTTP, valida el token JWT y verifica los roles.
    /// </summary>
    public class JwtAuthMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var authorizeAttribute = context.FunctionDefinition.InputBindings
                .Where(binding => binding.Value.Type == "AuthorizeAttribute")
                .Select(binding => binding.Value)
                .OfType<AuthorizeAttribute>()
                .FirstOrDefault();

            // Si la función no tiene el atributo, no requiere autorización, así que continuamos.
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

            // Intenta obtener el token del header "Authorization".
            if (!request.Headers.TryGetValues("Authorization", out var authHeaderValues) ||
                !authHeaderValues.Any() ||
                !authHeaderValues.First().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                // Si no hay token, la petición no está autorizada.
                await context.SetHttpResponse(request, HttpStatusCode.Unauthorized);
                return;
            }

            var token = authHeaderValues.First().Substring("Bearer ".Length).Trim();
            var jwtOptions = context.InstanceServices.GetRequiredService<JwtOptions>();
            var logger = context.GetLogger<JwtAuthMiddleware>();

            try
            {
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(jwtOptions.Key);

                // Valida el token. Si algo falla (firma, expiración, etc.), lanzará una excepción.
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ClockSkew = TimeSpan.Zero // No permite tolerancia en la expiración.
                }, out SecurityToken validatedToken);

                // Verificamos si la función requiere roles específicos.
                if (!string.IsNullOrEmpty(authorizeAttribute.Roles))
                {
                    var requiredRoles = authorizeAttribute.Roles.Split(',').Select(r => r.Trim().ToLowerInvariant()).ToList();
                    var userHasRole = requiredRoles.Any(role => claimsPrincipal.IsInRole(role.ToLowerInvariant()));

                    if (!userHasRole)
                    {
                        logger.LogWarning("Acceso denegado por rol. Usuario: {User}, Roles requeridos: {Roles}", claimsPrincipal.Identity?.Name, authorizeAttribute.Roles);
                        await context.SetHttpResponse(request, HttpStatusCode.Forbidden, ApiResponse<object>.Fail("Access denied. Insufficient permissions."));
                        return;
                    }
                }

                // ¡Éxito! El usuario está autenticado y tiene los roles necesarios.
                // Guardamos el ClaimsPrincipal en el contexto para que la función pueda usarlo.
                context.Features.Set(claimsPrincipal);

                await next(context);
            }
            catch (Exception ex)
            {
                // Si la validación del token falla por cualquier motivo.
                logger.LogWarning(ex, "Validación de token JWT fallida.");
                await context.SetHttpResponse(request, HttpStatusCode.Unauthorized, ApiResponse<object>.Unauthorized("Invalid token."));
            }
        }
    }
}
