using BookStore.Core.Models;
using BookStore.Core.Entities;
using BookStore.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repositories
{
    public class BooksRepository : IBooksRepository
    {
        private readonly BookStoreDbContext _context;
        public BooksRepository(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> Get()
        {
            var bookEntities = await _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.Category)
                .ToListAsync();

            var books = bookEntities.Select(b =>
            {
                // Создаем автора и категорию без связанных книг, чтобы избежать циклической зависимости
                var author = b.Author != null 
                    ? new Author(
                        b.Author.Id,
                        b.Author.Name,
                        b.Author.Surname
                      ) 
                    : null;
                
                var category = b.Category != null 
                    ? new Category(
                        b.Category.Id,
                        b.Category.Name,
                        b.Category.Description
                      ) 
                    : null;
                
                return Book.Create(
                    b.Id, 
                    b.Title, 
                    b.Description, 
                    b.Price, 
                    b.AuthorId, 
                    author, 
                    b.CategoryId, 
                    category,
                    b.ImageUrl
                ).book;
            }).ToList();

            return books;
        }

        public async Task<Book> GetByIdAsync(Guid id)
        {
            var bookEntity = await _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bookEntity == null)
            {
                return null;
            }

            // Создаем автора и категорию без связанных книг, чтобы избежать циклической зависимости
            var author = bookEntity.Author != null 
                ? new Author(
                    bookEntity.Author.Id,
                    bookEntity.Author.Name,
                    bookEntity.Author.Surname
                  ) 
                : null;
                
            var category = bookEntity.Category != null 
                ? new Category(
                    bookEntity.Category.Id,
                    bookEntity.Category.Name,
                    bookEntity.Category.Description
                  ) 
                : null;
            
            return Book.Create(
                bookEntity.Id,
                bookEntity.Title,
                bookEntity.Description,
                bookEntity.Price,
                bookEntity.AuthorId,
                author,
                bookEntity.CategoryId,
                category,
                bookEntity.ImageUrl
            ).book;
        }

        public async Task<Guid> Create(Book book)
        {
            var bookEntity = new BookEntity
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Price = book.Price,
                AuthorId = book.AuthorId,
                CategoryId = book.CategoryId,
                ImageUrl = book.ImageUrl
            };
            await _context.Books.AddAsync(bookEntity);
            await _context.SaveChangesAsync();

            return bookEntity.Id;
        }

        public async Task<Guid> Update(Guid id, string title, string description, decimal price, Guid authorId, AuthorEntity author, Guid categoryId, CategoryEntity category, string imageUrl = null)
        {
            await _context.Books
                .Where(b => b.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.Title, b => title)
                    .SetProperty(b => b.Description, b => description)
                    .SetProperty(b => b.Price, b => price)
                    .SetProperty(b => b.AuthorId, b => authorId)
                    .SetProperty(b => b.CategoryId, b => categoryId)
                    .SetProperty(b => b.ImageUrl, b => imageUrl)
                    );     

            return id;
        }

        public async Task<Guid> Delete(Guid id)
        {
            await _context.Books
                .Where(b => b.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }
    }
}
