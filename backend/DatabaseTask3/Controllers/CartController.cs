using BookStore.API.Contracts;
using BookStore.Core.Abstractions;
using BookStore.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize] // Требуется аутентификация для всех методов
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemResponse>>> GetCart()
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Запрос на получение корзины пользователя {UserId}", userId);

            try
            {
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                
                var response = cartItems.Select(item => new CartItemResponse(
                    item.Id,
                    item.BookId,
                    item.Book.Title,
                    item.Book.Description,
                    item.Book.Price,
                    item.Book.ImageUrl,
                    item.Book.Author?.Name ?? "Неизвестный автор",
                    item.Book.Category?.Name ?? "Без категории",
                    item.Quantity,
                    item.DateAdded
                ));

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении корзины пользователя {UserId}", userId);
                return StatusCode(500, "Произошла ошибка при получении корзины");
            }
        }

        [HttpPost("items")]
        public async Task<ActionResult<Guid>> AddToCart([FromBody] CartItemRequest request)
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Запрос на добавление книги {BookId} в корзину пользователя {UserId}", request.BookId, userId);

            try
            {
                var cartItemId = await _cartService.AddToCartAsync(userId, request.BookId, request.Quantity);
                return Ok(cartItemId);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Ошибка при добавлении книги {BookId} в корзину пользователя {UserId}", request.BookId, userId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении книги {BookId} в корзину пользователя {UserId}", request.BookId, userId);
                return StatusCode(500, "Произошла ошибка при добавлении товара в корзину");
            }
        }

        [HttpPut("items/{id:guid}")]
        public async Task<ActionResult> UpdateCartItemQuantity(Guid id, [FromBody] CartItemRequest request)
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Запрос на обновление количества для элемента корзины {CartItemId} пользователя {UserId}", id, userId);

            try
            {
                await _cartService.UpdateCartItemQuantityAsync(userId, id, request.Quantity);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Ошибка при обновлении количества для элемента корзины {CartItemId}", id);
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Попытка обновить элемент корзины другого пользователя. UserId: {UserId}, CartItemId: {CartItemId}", userId, id);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении количества для элемента корзины {CartItemId}", id);
                return StatusCode(500, "Произошла ошибка при обновлении количества товара");
            }
        }

        [HttpDelete("items/{id:guid}")]
        public async Task<ActionResult> RemoveFromCart(Guid id)
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Запрос на удаление элемента корзины {CartItemId} пользователя {UserId}", id, userId);

            try
            {
                await _cartService.RemoveFromCartAsync(userId, id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Ошибка при удалении элемента корзины {CartItemId}", id);
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Попытка удалить элемент корзины другого пользователя. UserId: {UserId}, CartItemId: {CartItemId}", userId, id);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении элемента корзины {CartItemId}", id);
                return StatusCode(500, "Произошла ошибка при удалении товара из корзины");
            }
        }

        [HttpDelete]
        public async Task<ActionResult> ClearCart()
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Запрос на очистку корзины пользователя {UserId}", userId);

            try
            {
                await _cartService.ClearCartAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при очистке корзины пользователя {UserId}", userId);
                return StatusCode(500, "Произошла ошибка при очистке корзины");
            }
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return userId;
        }
    }
}
