using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Core.Abstractions.Repositories
{
    public interface ICartRepository
    {
        Task<IEnumerable<CartItemEntity>> GetCartItemsByUserIdAsync(Guid userId);
        Task<CartItemEntity> GetCartItemAsync(Guid userId, Guid bookId);
        Task<CartItemEntity> GetCartItemByIdAsync(Guid id);
        Task AddCartItemAsync(CartItemEntity cartItem);
        Task UpdateCartItemAsync(CartItemEntity cartItem);
        Task RemoveCartItemAsync(Guid id);
        Task ClearCartAsync(Guid userId);
    }
}
