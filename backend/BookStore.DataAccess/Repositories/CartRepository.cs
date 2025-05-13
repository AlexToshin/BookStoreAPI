using BookStore.Core.Abstractions.Repositories;
using BookStore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly BookStoreDbContext _context;

        public CartRepository(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartItemEntity>> GetCartItemsByUserIdAsync(Guid userId)
        {
            return await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Book)
                    .ThenInclude(b => b.Author)
                .Include(c => c.Book)
                    .ThenInclude(b => b.Category)
                .OrderByDescending(c => c.DateAdded)
                .ToListAsync();
        }

        public async Task<CartItemEntity> GetCartItemAsync(Guid userId, Guid bookId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);
        }

        public async Task<CartItemEntity> GetCartItemByIdAsync(Guid id)
        {
            return await _context.CartItems
                .Include(c => c.Book)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddCartItemAsync(CartItemEntity cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(CartItemEntity cartItem)
        {
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCartItemAsync(Guid id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(Guid userId)
        {
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }
    }
}
