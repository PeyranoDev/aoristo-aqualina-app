using System;

namespace Common.Models.Responses
{
    /// <summary>
    /// Representa una respuesta estándar de la API que incluye información sobre el éxito de la operación, un mensaje y datos opcionales.
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indica si la operación fue exitosa.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Datos devueltos por la operación, si aplica.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Mensaje que describe el resultado de la operación.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ApiResponse{T}"/>.
        /// </summary>
        public ApiResponse(bool success, T? data, string message)
        {
            Success = success;
            Data = data;
            Message = message;
        }

        /// <summary>
        /// Crea una respuesta exitosa con datos.
        /// </summary>
        /// <param name="data">Los datos devueltos por la operación.</param>
        /// <param name="message">Mensaje opcional. Por defecto: "Operación exitosa".</param>
        public static ApiResponse<T> Ok(T data, string message = "Operación exitosa")
        {
            return new ApiResponse<T>(true, data, message);
        }

        /// <summary>
        /// Crea una respuesta exitosa sin datos (por ejemplo, para actualizaciones o eliminaciones).
        /// </summary>
        /// <param name="message">Mensaje opcional. Por defecto: "Operación realizada correctamente".</param>
        public static ApiResponse<T> NoContent(string message = "Operación realizada correctamente")
        {
            return new ApiResponse<T>(true, default, message);
        }

        /// <summary>
        /// Crea una respuesta de error genérico.
        /// </summary>
        /// <param name="message">Mensaje que describe el error. Por defecto: "Ocurrió un error".</param>
        public static ApiResponse<T> Fail(string message = "Ocurrió un error")
        {
            return new ApiResponse<T>(false, default, message);
        }

        /// <summary>
        /// Crea una respuesta para errores de validación de datos.
        /// </summary>
        /// <param name="message">Mensaje que describe el error de validación.</param>
        public static ApiResponse<T> ValidationError(string message = "Datos inválidos")
        {
            return new ApiResponse<T>(false, default, message);
        }

        /// <summary>
        /// Crea una respuesta cuando el usuario no está autorizado.
        /// </summary>
        /// <param name="message">Mensaje que describe el error de autorización.</param>
        public static ApiResponse<T> Unauthorized(string message = "No autorizado")
        {
            return new ApiResponse<T>(false, default, message);
        }

        /// <summary>
        /// Crea una respuesta cuando el recurso solicitado no se encuentra.
        /// </summary>
        /// <param name="message">Mensaje que describe el recurso no encontrado.</param>
        public static ApiResponse<T> NotFound(string message = "Recurso no encontrado")
        {
            return new ApiResponse<T>(false, default, message);
        }

        /// <summary>
        /// Crea una respuesta de error basada en una excepción.
        /// </summary>
        /// <param name="ex">La excepción capturada.</param>
        /// <param name="customMessage">Mensaje opcional que reemplaza al mensaje de la excepción.</param>
        public static ApiResponse<T> FromException(Exception ex, string? customMessage = null)
        {
            return new ApiResponse<T>(false, default, customMessage ?? ex.Message);
        }
    }
}
