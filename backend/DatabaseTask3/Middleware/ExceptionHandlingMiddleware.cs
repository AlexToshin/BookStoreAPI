using System.Net;
using System.Text.Json;
using BookStore.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlingMiddleware(
            RequestDelegate next, 
            ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Необработанное исключение: {ExceptionMessage}", exception.Message);

            var statusCode = HttpStatusCode.InternalServerError; // 500 по умолчанию
            var errorMessage = "Произошла внутренняя ошибка сервера.";
            var detailedMessage = exception.Message;
            object errorData = null;

            // Проверяем, является ли исключение нашим пользовательским исключением
            if (exception is BookStoreException bookStoreException)
            {
                statusCode = bookStoreException.StatusCode;
                errorMessage = bookStoreException.Message;
                errorData = bookStoreException.ErrorData;
            }
            // Если не пользовательское, определяем тип стандартного исключения
            else if (exception is ArgumentException || exception is FormatException || exception is JsonException)
            {
                statusCode = HttpStatusCode.BadRequest; // 400
                errorMessage = "Ошибка в запросе. Проверьте правильность данных.";
            }
            else if (exception is KeyNotFoundException || exception is FileNotFoundException)
            {
                statusCode = HttpStatusCode.NotFound; // 404
                errorMessage = "Запрашиваемый ресурс не найден.";
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized; // 401
                errorMessage = "Доступ запрещен. Необходима авторизация.";
            }
            else if (exception is DbUpdateException)
            {
                statusCode = HttpStatusCode.BadRequest; // 400
                errorMessage = "Ошибка при обновлении базы данных.";
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                status = (int)statusCode,
                title = errorMessage,
                detail = _env.IsDevelopment() ? detailedMessage : null,
                data = errorData,
                traceId = context.TraceIdentifier
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }

    // Extension method для удобного подключения middleware
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
} 