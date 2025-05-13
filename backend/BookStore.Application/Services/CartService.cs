using BookStore.Core.Abstractions;
using BookStore.Core.Abstractions.Repositories;
using BookStore.Core.Entities;
using BookStore.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IBooksRepository _booksRepository;
        private readonly ILogger<CartService> _logger;

        public CartService(
            ICartRepository cartRepository, 
            IBooksRepository booksRepository, 
            ILogger<CartService> logger)
        {
            _cartRepository = cartRepository;
            _booksRepository = booksRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(Guid userId)
        {
            _logger.LogInformation("Получение элементов корзины для пользователя {UserId}", userId);
            
            var cartItemEntities = await _cartRepository.GetCartItemsByUserIdAsync(userId);
            var cartItems = new List<CartItem>();

            foreach (var entity in cartItemEntities)
            {
                var book = entity.Book.ToBook();
                var (cartItem, error) = CartItem.Create(
                    entity.Id,
                    entity.UserId,
                    entity.BookId,
                    book,
                    entity.Quantity,
                    entity.DateAdded);

                if (string.IsNullOrEmpty(error))
                {
                    cartItems.Add(cartItem);
                }
                else
                {
                    _logger.LogWarning("Ошибка при создании элемента корзины: {Error}", error);
                }
            }

            return cartItems;
        }

        public async Task<Guid> AddToCartAsync(Guid userId, Guid bookId, int quantity)
        {
            _logger.LogInformation("Добавление книги {BookId} в корзину пользователя {UserId}", bookId, userId);
            
            // Проверяем, существует ли уже такой элемент в корзине
            var existingCartItem = await _cartRepository.GetCartItemAsync(userId, bookId);
            
            if (existingCartItem != null)
            {
                // Если элемент уже существует, увеличиваем количество
                existingCartItem.Quantity += quantity;
                await _cartRepository.UpdateCartItemAsync(existingCartItem);
                _logger.LogInformation("Обновлено количество для существующего элемента корзины {CartItemId}", existingCartItem.Id);
                return existingCartItem.Id;
            }
            
            // Получаем информацию о книге
            var bookEntity = await _booksRepository.GetByIdAsync(bookId);
            if (bookEntity == null)
            {
                _logger.LogWarning("Книга {BookId} не найдена", bookId);
                throw new ArgumentException($"Book with id {bookId} not found");
            }

            // Создаем новый элемент корзины
            var cartItem = new CartItemEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BookId = bookId,
                Quantity = quantity,
                DateAdded = DateTime.UtcNow
            };

            await _cartRepository.AddCartItemAsync(cartItem);
            _logger.LogInformation("Создан новый элемент корзины {CartItemId}", cartItem.Id);
            
            return cartItem.Id;
        }

        public async Task UpdateCartItemQuantityAsync(Guid userId, Guid itemId, int quantity)
        {
            _logger.LogInformation("Обновление количества для элемента корзины {CartItemId}", itemId);
            
            var cartItem = await _cartRepository.GetCartItemByIdAsync(itemId);
            
            if (cartItem == null)
            {
                _logger.LogWarning("Элемент корзины {CartItemId} не найден", itemId);
                throw new ArgumentException($"Cart item with id {itemId} not found");
            }

            if (cartItem.UserId != userId)
            {
                _logger.LogWarning("Попытка обновить элемент корзины другого пользователя. UserId: {UserId}, CartItemUserId: {CartItemUserId}", userId, cartItem.UserId);
                throw new UnauthorizedAccessException("You are not authorized to update this cart item");
            }

            if (quantity <= 0)
            {
                _logger.LogWarning("Некорректное количество: {Quantity}", quantity);
                throw new ArgumentException("Quantity must be greater than zero");
            }

            cartItem.Quantity = quantity;
            await _cartRepository.UpdateCartItemAsync(cartItem);
            _logger.LogInformation("Количество для элемента корзины {CartItemId} обновлено до {Quantity}", itemId, quantity);
        }

        public async Task RemoveFromCartAsync(Guid userId, Guid itemId)
        {
            _logger.LogInformation("Удаление элемента корзины {CartItemId}", itemId);
            
            var cartItem = await _cartRepository.GetCartItemByIdAsync(itemId);
            
            if (cartItem == null)
            {
                _logger.LogWarning("Элемент корзины {CartItemId} не найден", itemId);
                throw new ArgumentException($"Cart item with id {itemId} not found");
            }

            if (cartItem.UserId != userId)
            {
                _logger.LogWarning("Попытка удалить элемент корзины другого пользователя. UserId: {UserId}, CartItemUserId: {CartItemUserId}", userId, cartItem.UserId);
                throw new UnauthorizedAccessException("You are not authorized to remove this cart item");
            }

            await _cartRepository.RemoveCartItemAsync(itemId);
            _logger.LogInformation("Элемент корзины {CartItemId} удален", itemId);
        }

        public async Task ClearCartAsync(Guid userId)
        {
            _logger.LogInformation("Очистка корзины пользователя {UserId}", userId);
            await _cartRepository.ClearCartAsync(userId);
            _logger.LogInformation("Корзина пользователя {UserId} очищена", userId);
        }
    }
}
