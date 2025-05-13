using BookStore.Core.Entities;
using BookStore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Core.Abstractions
{
    public interface IBooksRepository
    {
        Task<Guid> Create(Book book);
        Task<Guid> Update(Guid id, string title, string description, decimal price, Guid authorId, AuthorEntity author, Guid categoryId, CategoryEntity category, string imageUrl = null);
        Task<Guid> Delete(Guid id);

        Task<List<Book>> Get();
        Task<Book> GetByIdAsync(Guid id);
    }
}
