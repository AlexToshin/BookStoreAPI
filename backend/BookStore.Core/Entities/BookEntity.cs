using BookStore.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Core.Entities
{ 
    //свойства для бд
    public class BookEntity 
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set;  }

        public Guid AuthorId { get; set; }
        public AuthorEntity? Author { get; set; }

        public Guid CategoryId { get; set; }
        public CategoryEntity? Category { get; set; }
        
        public string? ImageUrl { get; set; }

        public Book ToBook(bool includeRelations = true)
        {
            Author? author = null;
            Category? category = null;
            
            if (includeRelations)
            {
                // Избегаем циклической зависимости, не включая книги автора
                author = Author != null ? new Author(Author.Id, Author.Name, Author.Surname) : null;
                category = Category?.ToCategory();
            }
            
            var (book, error) = Book.Create(Id, Title, Description, Price, AuthorId, author, CategoryId, category, ImageUrl);

            if (!string.IsNullOrEmpty(error))
            {
                throw new ArgumentException(error);
            }

            return book;
        }
    }
}
