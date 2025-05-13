using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace BookStore.API.Filters
{
    /// <summary>
    /// Фильтр для логирования времени выполнения запросов API
    /// </summary>
    public class RequestPerformanceFilter : IActionFilter
    {
        private readonly ILogger<RequestPerformanceFilter> _logger;
        private Stopwatch _stopwatch;

        public RequestPerformanceFilter(ILogger<RequestPerformanceFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();
            
            // Логируем информацию о начале запроса
            _logger.LogInformation(
                "Начало выполнения запроса {Method} {Path}",
                context.HttpContext.Request.Method,
                context.HttpContext.Request.Path);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();
            
            var elapsed = _stopwatch.ElapsedMilliseconds;
            var statusCode = context.HttpContext.Response.StatusCode;
            
            // Логируем информацию о завершении запроса и его производительности
            if (elapsed > 500) // Если запрос выполнялся дольше 500 мс
            {
                _logger.LogWarning(
                    "Длительное выполнение запроса {Method} {Path} завершено со статусом {StatusCode}. Время выполнения: {ElapsedMs} мс",
                    context.HttpContext.Request.Method,
                    context.HttpContext.Request.Path,
                    statusCode,
                    elapsed);
            }
            else
            {
                _logger.LogInformation(
                    "Запрос {Method} {Path} завершен со статусом {StatusCode}. Время выполнения: {ElapsedMs} мс",
                    context.HttpContext.Request.Method,
                    context.HttpContext.Request.Path,
                    statusCode,
                    elapsed);
            }
        }
    }
} 