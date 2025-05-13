using BookStore.API.Contracts;
using BookStore.Application.Services;
using BookStore.Core.Abstractions;
using BookStore.Core.Models;
using BookStore.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using BookStore.DataAccess;
using BookStore.DataAccess.Repositories;
using BookStore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize] // Все методы требуют аутентификации
    public class BooksController : ControllerBase
    {
        private readonly IBooksService _booksService;
        private readonly IAuthorsRepository _authorsRepository;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<BooksController> _logger;
        
        public BooksController(
            IBooksService booksService, 
            IAuthorsRepository authorsRepository, 
            ICategoriesRepository categoriesRepository,
            IWebHostEnvironment environment,
            ILogger<BooksController> logger)
        {
            _booksService = booksService;
            _authorsRepository = authorsRepository;
            _categoriesRepository = categoriesRepository;
            _environment = environment;
            _logger = logger;
        }
        [HttpGet]
        [AllowAnonymous] // Просмотр книг доступен всем
        public async Task<ActionResult<List<BooksResponse>>> GetBooks()
        {
            _logger.LogInformation("Запрос на получение списка всех книг");
            var books = await _booksService.GetAllBooks();
            _logger.LogDebug("Получено {Count} книг из базы данных", books.Count);

            var response = books.Select(b => new BooksResponse(
                b.Id, 
                b.Title, 
                b.Description, 
                b.Price, 
                b.AuthorId, 
                b.Author, 
                b.CategoryId, 
                b.Category,
                b.ImageUrl
            ));

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous] // Просмотр деталей книги доступен всем
        public async Task<ActionResult<BooksResponse>> GetBookById(Guid id)
        {
            _logger.LogInformation("Запрос на получение книги с ID: {BookId}", id);
            var book = await _booksService.GetBookById(id);

            if (book == null)
            {
                _logger.LogWarning("Книга с ID {BookId} не найдена", id);
                return NotFound();
            }

            _logger.LogDebug("Найдена книга: {BookTitle} (ID: {BookId})", book.Title, book.Id);
            var response = new BooksResponse(
                book.Id, 
                book.Title, 
                book.Description, 
                book.Price, 
                book.AuthorId, 
                book.Author, 
                book.CategoryId, 
                book.Category,
                book.ImageUrl
            );

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "admin")] // Только администраторы могут создавать книги
        public async Task<ActionResult<Guid>> CreateBook([FromBody] BooksRequest request)
        {
            _logger.LogInformation("Запрос на создание новой книги: {BookTitle}", request.Title);
            
            var author = await _authorsRepository.GetByIdAsync(request.AuthorId);
            if (author == null)
            {
                _logger.LogWarning("Не удалось создать книгу: автор с ID {AuthorId} не существует", request.AuthorId);
                return BadRequest($"Author with id {request.AuthorId} doesn't exist");
            }

            var category = await _categoriesRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
            {
                _logger.LogWarning("Не удалось создать книгу: категория с ID {CategoryId} не существует", request.CategoryId);
                return BadRequest($"Category with id {request.CategoryId} doesn't exist");
            }

            var (book, error) = Book.Create(
                Guid.NewGuid(), 
                request.Title, 
                request.Description, 
                request.Price, 
                request.AuthorId, 
                author, 
                request.CategoryId, 
                category, 
                request.ImageUrl);

            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogWarning("Не удалось создать книгу: {Error}", error);
                return BadRequest(error);
            }

            var bookId = await _booksService.CreateBook(book);
            _logger.LogInformation("Книга успешно создана: {BookTitle} (ID: {BookId})", book.Title, bookId);

            return Ok(bookId);
        }

        [HttpPost("upload-image")]
        [Authorize(Roles = "admin")] // Только администраторы могут загружать изображения
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            _logger.LogInformation("Запрос на загрузку изображения. Имя файла: {FileName}, размер: {FileSize} байт", 
                file?.FileName, file?.Length);
                
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Загрузка изображения не удалась: файл не выбран или пустой");
                return BadRequest("Файл не выбран или пустой");
            }

            // Проверяем, что файл - изображение
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            
            if (!allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("Загрузка изображения не удалась: недопустимый формат файла {FileExtension}", extension);
                return BadRequest("Недопустимый формат файла. Разрешены только изображения (.jpg, .jpeg, .png, .gif)");
            }

            // Создаем уникальное имя файла
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "books");
            
            // Убеждаемся, что папка существует
            if (!Directory.Exists(uploadsFolder))
            {
                _logger.LogInformation("Создание директории для изображений: {DirectoryPath}", uploadsFolder);
                Directory.CreateDirectory(uploadsFolder);
            }
                
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            try
            {
                // Сохраняем файл
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Возвращаем относительный путь к файлу
                var fileUrl = $"/images/books/{uniqueFileName}";
                _logger.LogInformation("Изображение успешно загружено: {ImageUrl}", fileUrl);
                return Ok(new { imageUrl = fileUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении изображения");
                return StatusCode(500, "Ошибка при сохранении изображения");
            }
        }

        [HttpPut("{id:guid}/image")]
        [Authorize(Roles = "admin")] // Только администраторы могут обновлять изображения
        public async Task<IActionResult> UpdateBookImage(Guid id, IFormFile file)
        {
            // Проверяем существование книги
            var book = await _booksService.GetBookById(id);
            if (book == null)
                return NotFound($"Книга с ID {id} не найдена");

            // Удаляем старое изображение, если оно есть
            if (!string.IsNullOrEmpty(book.ImageUrl))
            {
                var oldImagePath = Path.Combine(_environment.WebRootPath, book.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            // Загружаем новое изображение
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран или пустой");

            // Проверяем, что файл - изображение
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            
            if (!allowedExtensions.Contains(extension))
                return BadRequest("Недопустимый формат файла. Разрешены только изображения (.jpg, .jpeg, .png, .gif)");

            // Создаем уникальное имя файла
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "books");
            
            // Убеждаемся, что папка существует
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);
                
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Сохраняем файл
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Обновляем путь к изображению в базе данных
            var fileUrl = $"/images/books/{uniqueFileName}";
            
            await _booksService.UpdateBook(
                id, 
                book.Title, 
                book.Description, 
                book.Price, 
                book.AuthorId, 
                book.Author?.ToEntity(), 
                book.CategoryId, 
                book.Category?.ToEntity(), 
                fileUrl
            );

            return Ok(new { imageUrl = fileUrl });
        }

        [HttpDelete("{id:guid}/image")]
        public async Task<IActionResult> DeleteBookImage(Guid id)
        {
            // Проверяем существование книги
            var book = await _booksService.GetBookById(id);
            if (book == null)
                return NotFound($"Книга с ID {id} не найдена");

            // Проверяем наличие изображения
            if (string.IsNullOrEmpty(book.ImageUrl))
                return BadRequest("У книги нет изображения");

            // Удаляем файл изображения
            var imagePath = Path.Combine(_environment.WebRootPath, book.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            // Обновляем информацию о книге, убирая ссылку на изображение
            await _booksService.UpdateBook(
                id, 
                book.Title, 
                book.Description, 
                book.Price, 
                book.AuthorId, 
                book.Author?.ToEntity(), 
                book.CategoryId, 
                book.Category?.ToEntity(), 
                null
            );

            return Ok(new { message = "Изображение успешно удалено" });
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "admin")] // Только администраторы могут обновлять книги
        public async Task<ActionResult<Guid>> UpdateBook(Guid id, [FromBody] BooksRequest request)
        {
            var author = await _authorsRepository.GetByIdAsync(request.AuthorId);
            var category = await _categoriesRepository.GetByIdAsync(request.CategoryId);

            if (author == null)
            {
                return BadRequest("Author not found.");
            }

            if (category == null)
            {
                return BadRequest("Category not found.");
            }

            var bookId = await _booksService.UpdateBook(
                id, 
                request.Title, 
                request.Description, 
                request.Price, 
                request.AuthorId, 
                author.ToEntity(), 
                request.CategoryId, 
                category.ToEntity(),
                request.ImageUrl
            );

            return Ok(bookId);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "admin")] // Только администраторы могут удалять книги
        public async Task<ActionResult<Guid>> DeleteBook(Guid id)
        {
            return Ok(await _booksService.DeleteBook(id));            
        }
    }
}
