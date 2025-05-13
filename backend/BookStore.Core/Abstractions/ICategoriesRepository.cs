using BookStore.Core.Models;

namespace BookStore.DataAccess.Repositories
{
    public interface ICategoriesRepository
    {
        Task<Guid> Create(Category category);
        Task<Guid> Delete(Guid id);
        Task<List<Category>> Get();
        Task<Category> GetByIdAsync(Guid id);
        Task<Guid> Update(Guid id, string name, string description);
    }
}