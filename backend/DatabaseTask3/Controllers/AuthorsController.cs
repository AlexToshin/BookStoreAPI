using BookStore.Application.Services;
using BookStore.API.Contracts;
using Microsoft.AspNetCore.Mvc;
using BookStore.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;
using BookStore.Core.Entities;
using System;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize] // Все методы требуют аутентификации
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorsService _service;
        private readonly ILogger<AuthorsController> _logger;

        public AuthorsController(IAuthorsService service, ILogger<AuthorsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous] // Просмотр списка авторов доступен всем
        public async Task<ActionResult<List<AuthorsResponse>>> GetAuthors()
        {
            _logger.LogInformation("Запрос на получение списка всех авторов");
            var authors = await _service.GetAllAuthors();
            var response = authors.Select(a => new AuthorsResponse(a.Id, a.Name, a.Surname, a.ToEntity().Books));
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous] // Просмотр деталей автора доступен всем
        public async Task<ActionResult<AuthorsResponse>> GetAuthorById(Guid id)
        {
            _logger.LogInformation("Запрос на получение автора с ID: {AuthorId}", id);
            var author = await _service.GetAuthorById(id);
            if (author == null)
            {
                _logger.LogWarning("Автор с ID {AuthorId} не найден", id);
                return NotFound($"Автор с ID {id} не найден");
            }
            
            var response = new AuthorsResponse(author.Id, author.Name, author.Surname, author.ToEntity().Books);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "admin")] // Только администраторы могут создавать авторов
        public async Task<ActionResult<Guid>> CreateAuthor([FromBody] AuthorsRequest request)
        {
            _logger.LogInformation("Запрос на создание нового автора: {Name} {Surname}", request.Name, request.Surname);
            var (author, error) = Author.Create(Guid.NewGuid(), request.Name, request.Surname);

            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogWarning("Ошибка при создании автора {Name} {Surname}: {Error}", request.Name, request.Surname, error);
                return BadRequest(error);
            }

            var authorId = await _service.CreateAuthor(author);

            return Ok(authorId);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "admin")] // Только администраторы могут обновлять авторов
        public async Task<ActionResult<Guid>> UpdateAuthor(Guid id, [FromBody] AuthorsRequest request)
        {
            _logger.LogInformation("Запрос на обновление автора с ID {AuthorId}: {Name} {Surname}", id, request.Name, request.Surname);
            var authorId = await _service.UpdateAuthor(id, request.Name, request.Surname);

            return Ok(authorId);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "admin")] // Только администраторы могут удалять авторов
        public async Task<ActionResult<Guid>> DeleteAuthor(Guid id)
        {
            _logger.LogInformation("Запрос на удаление автора с ID: {AuthorId}", id);
            return Ok(await _service.DeleteAuthor(id));
        }
    }
}
