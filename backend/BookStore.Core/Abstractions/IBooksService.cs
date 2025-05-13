using BookStore.Core.Entities;
using BookStore.Core.Models;
using BookStore.Core.Repositories;

namespace BookStore.Application.Services
{
    public interface IBooksService
    {

        Task<List<Book>> GetAllBooks();
        Task<Book> GetBookById(Guid id);
        Task<Guid> CreateBook(Book book);
        Task<Guid> UpdateBook(Guid id, string title, string description, decimal price, Guid authorId, AuthorEntity author, Guid categoryId, CategoryEntity category, string imageUrl = null);
        Task<Guid> DeleteBook(Guid id);
        
    }
}
