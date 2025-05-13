using BookStore.Core.Models;
using BookStore.Core.Entities;

namespace BookStore.API.Contracts
{
    public record CategoriesResponse(
        Guid Id,
        string Name,
        string Description,
        List<BookEntity> Books
        );
}
