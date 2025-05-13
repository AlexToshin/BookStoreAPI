using BookStore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Core.Abstractions
{
    public interface ICartService
    {
        Task<IEnumerable<CartItem>> GetCartItemsAsync(Guid userId);
        Task<Guid> AddToCartAsync(Guid userId, Guid bookId, int quantity);
        Task UpdateCartItemQuantityAsync(Guid userId, Guid itemId, int quantity);
        Task RemoveFromCartAsync(Guid userId, Guid itemId);
        Task ClearCartAsync(Guid userId);
    }
}
