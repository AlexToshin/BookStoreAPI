using BookStore.API.Contracts;
using BookStore.Application.Services;
using BookStore.Core.Entities;
using BookStore.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims; // добавлено пространство имен для ClaimsPrincipal
using System.Security.Principal; // добавлено пространство имен для IPrincipal

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize] // Все методы требуют аутентификации
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesService _categoriesService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoriesService categoriesService, ILogger<CategoriesController> logger)
        {
            _categoriesService = categoriesService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous] // Просмотр списка категорий доступен всем
        public async Task<ActionResult<List<CategoriesResponse>>> GetCategories()
        {
            _logger.LogInformation("Запрос на получение списка всех категорий");
            var categories = await _categoriesService.GetAllCategories();
            var response = categories.Select(a => new CategoriesResponse(a.Id, a.Name, a.Description, a.ToEntity().Books));
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous] // Просмотр деталей категории доступен всем
        public async Task<ActionResult<CategoriesResponse>> GetCategoryById(Guid id)
        {
            _logger.LogInformation("Запрос на получение категории с ID: {CategoryId}", id);
            var category = await _categoriesService.GetCategoryById(id);
            
            if (category == null)
            {
                _logger.LogWarning("Категория с ID {CategoryId} не найдена", id);
                return NotFound();
            }

            var response = new CategoriesResponse(category.Id, category.Name, category.Description, category.ToEntity().Books);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "admin")] // Только администраторы могут создавать категории
        public async Task<ActionResult<Guid>> CreateCategory([FromBody] CategoriesRequest request)
        {
            _logger.LogInformation("Запрос на создание новой категории: {Name}", request.Name);
            var (category, error) = Category.Create(Guid.NewGuid(), request.Name, request.Description);

            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogWarning("Ошибка при создании категории {Name}: {Error}", request.Name, error);
                return BadRequest(error);
            }

            var categoryId = await _categoriesService.CreateCategory(category);
            return Ok(categoryId);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "admin")] // Только администраторы могут обновлять категории
        public async Task<ActionResult<Guid>> UpdateCategory(Guid id, [FromBody] CategoriesRequest request)
        {
            _logger.LogInformation("Запрос на обновление категории с ID {CategoryId}: {Name}", id, request.Name);
            var categoryId = await _categoriesService.UpdateCategory(id, request.Name, request.Description);
            return Ok(categoryId);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "admin")] // Только администраторы могут удалять категории
        public async Task<ActionResult<Guid>> DeleteCategory(Guid id)
        {
            _logger.LogInformation("Запрос на удаление категории с ID: {CategoryId}", id);
            return Ok(await _categoriesService.DeleteCategory(id));
        }
    }
}
