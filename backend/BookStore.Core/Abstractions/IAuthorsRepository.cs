using BookStore.Core.Entities;
using BookStore.Core.Models;

namespace BookStore.Core.Repositories
{
    public interface IAuthorsRepository
    {
        Task<Guid> Create(Author author);
        Task<Guid> Delete(Guid id);
        Task<List<Author>> Get();
        Task<Guid> Update(Guid id, string name, string surname);

        Task<Author> GetByIdAsync(Guid id);
    }
}