using BookStore.Core.Entities;
using BookStore.Core.Models;

namespace BookStore.Application.Services
{
    public interface IAuthorsService
    {
        Task<Guid> CreateAuthor(Author author);
        Task<Guid> DeleteAuthor(Guid id);
        Task<List<Author>> GetAllAuthors();
        Task<Author> GetAuthorById(Guid id);
        Task<Guid> UpdateAuthor(Guid id, string name, string surname);
    }
}