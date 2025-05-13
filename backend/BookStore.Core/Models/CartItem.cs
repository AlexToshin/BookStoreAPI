using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Core.Models
{
    public class CartItem
    {
        public Guid Id { get; }
        public Guid UserId { get; }
        public Guid BookId { get; }
        public Book Book { get; }
        public int Quantity { get; }
        public DateTime DateAdded { get; }

        private CartItem(Guid id, Guid userId, Guid bookId, Book book, int quantity, DateTime dateAdded)
        {
            Id = id;
            UserId = userId;
            BookId = bookId;
            Book = book;
            Quantity = quantity;
            DateAdded = dateAdded;
        }

        public static (CartItem cartItem, string error) Create(
            Guid id, 
            Guid userId, 
            Guid bookId, 
            Book book, 
            int quantity, 
            DateTime? dateAdded = null)
        {
            if (userId == Guid.Empty)
            {
                return (null, "User ID cannot be empty");
            }

            if (bookId == Guid.Empty)
            {
                return (null, "Book ID cannot be empty");
            }

            if (book == null)
            {
                return (null, "Book cannot be null");
            }

            if (quantity <= 0)
            {
                return (null, "Quantity must be greater than zero");
            }

            var actualDateAdded = dateAdded ?? DateTime.UtcNow;

            return (new CartItem(id, userId, bookId, book, quantity, actualDateAdded), string.Empty);
        }

        public CartItem UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero");
            }

            return new CartItem(Id, UserId, BookId, Book, newQuantity, DateAdded);
        }
    }
}
