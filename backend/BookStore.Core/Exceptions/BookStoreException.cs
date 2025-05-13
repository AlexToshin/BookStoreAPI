using System.Net;

namespace BookStore.Core.Exceptions
{
    /// <summary>
    /// Базовое исключение для всех ошибок в приложении BookStore
    /// </summary>
    public abstract class BookStoreException : Exception
    {
        /// <summary>
        /// HTTP-статус код, соответствующий ошибке
        /// </summary>
        public HttpStatusCode StatusCode { get; }
        
        /// <summary>
        /// Дополнительные данные об ошибке
        /// </summary>
        public object ErrorData { get; }

        protected BookStoreException(string message, HttpStatusCode statusCode, object errorData = null)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorData = errorData;
        }
    }

    /// <summary>
    /// Исключение, связанное с ресурсом, который не был найден
    /// </summary>
    public class NotFoundException : BookStoreException
    {
        public NotFoundException(string message = "Запрашиваемый ресурс не найден")
            : base(message, HttpStatusCode.NotFound)
        {
        }

        public NotFoundException(string entityName, object entityId)
            : base($"Сущность '{entityName}' с идентификатором '{entityId}' не найдена", HttpStatusCode.NotFound)
        {
        }
    }

    /// <summary>
    /// Исключение для ошибок валидации данных
    /// </summary>
    public class ValidationException : BookStoreException
    {
        public ValidationException(string message = "Ошибка валидации данных")
            : base(message, HttpStatusCode.BadRequest)
        {
        }

        public ValidationException(string message, object validationErrors)
            : base(message, HttpStatusCode.BadRequest, validationErrors)
        {
        }
    }

    /// <summary>
    /// Исключение, связанное с отсутствием прав доступа
    /// </summary>
    public class ForbiddenException : BookStoreException
    {
        public ForbiddenException(string message = "Доступ запрещен")
            : base(message, HttpStatusCode.Forbidden)
        {
        }
    }

    /// <summary>
    /// Исключение, связанное с конфликтом данных
    /// </summary>
    public class ConflictException : BookStoreException
    {
        public ConflictException(string message = "Конфликт данных")
            : base(message, HttpStatusCode.Conflict)
        {
        }
    }

    /// <summary>
    /// Исключение для некорректной работы с файлами
    /// </summary>
    public class FileProcessingException : BookStoreException
    {
        public FileProcessingException(string message = "Ошибка при обработке файла")
            : base(message, HttpStatusCode.BadRequest)
        {
        }
    }
} 