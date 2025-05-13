using BookStore.Core.Models;
using BookStore.Core.Entities;

namespace BookStore.API.Contracts
{
    public record AuthorsResponse(
        Guid Id,
        string Name,
        string Surname,
        List<BookEntity> Books
        );
}
