using BookStore.Core.Models;
using BookStore.Core.Entities;
using BookStore.Core.Abstractions;
using BookStore.Core.Repositories;
using BookStore.DataAccess.Repositories;
namespace BookStore.Application.Services
{
    public class BooksService : IBooksService
    {
        private readonly IBooksRepository _booksRepository;
        private readonly IAuthorsRepository _authorsRepository;
        private readonly ICategoriesRepository _categoriesRepository;

        public BooksService(IBooksRepository booksRepository, IAuthorsRepository authorsRepository, ICategoriesRepository categoriesRepository)
        {
            _booksRepository = booksRepository;
            _authorsRepository = authorsRepository;
            _categoriesRepository = categoriesRepository;
        }           

        public async Task<List<Book>> GetAllBooks()
        {
            return await _booksRepository.Get();
        }

        public async Task<Book> GetBookById(Guid id)
        {
            return await _booksRepository.GetByIdAsync(id);
        }

        public async Task<Guid> CreateBook(Book book)
        {
            return await _booksRepository.Create(book);
        }

        public async Task<Guid> UpdateBook(Guid id, string title, string description, decimal price, Guid authorId, AuthorEntity author, Guid categoryId, CategoryEntity category, string imageUrl = null)
        {
            return await _booksRepository.Update(id, title, description, price, authorId, author, categoryId, category, imageUrl);
        }

        public async Task<Guid> DeleteBook(Guid id)
        {
            return await _booksRepository.Delete(id);
        }
    }
}
