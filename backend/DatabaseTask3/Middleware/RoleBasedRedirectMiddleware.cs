using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BookStore.API.Middleware
{
    public class RoleBasedRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RoleBasedRedirectMiddleware> _logger;

        public RoleBasedRedirectMiddleware(RequestDelegate next, ILogger<RoleBasedRedirectMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Проверка авторизации
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var path = context.Request.Path.Value?.ToLower() ?? string.Empty;
                var method = context.Request.Method;
                var userRole = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "user";

                _logger.LogInformation("Пользователь с ролью {Role} запрашивает доступ к {Path} с методом {Method}", 
                    userRole, path, method);

                // Проверка доступа для обычных пользователей
                if (userRole == "user")
                {
                    // Запрещаем доступ к методам создания, редактирования и удаления
                    if ((method == "POST" || method == "PUT" || method == "DELETE") && 
                        (path.Contains("/books") || path.Contains("/authors") || path.Contains("/categories")))
                    {
                        _logger.LogWarning("Отказ в доступе пользователю с ролью {Role} к {Path} с методом {Method}", 
                            userRole, path, method);
                            
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("Forbidden: Requires admin role");
                        return;
                    }
                }
            }

            // Продолжаем выполнение следующего middleware в конвейере
            await _next(context);
        }
    }

    // Метод расширения для удобного добавления middleware в конвейер
    public static class RoleBasedRedirectMiddlewareExtensions
    {
        public static IApplicationBuilder UseRoleBasedRedirect(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RoleBasedRedirectMiddleware>();
        }
    }
}
