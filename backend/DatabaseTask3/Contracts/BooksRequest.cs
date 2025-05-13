using BookStore.Core.Models;

namespace BookStore.API.Contracts
{
    public record BooksRequest(
        string Title,
        string Description,
        decimal Price,
        Guid AuthorId,
        Guid CategoryId,
        string? ImageUrl = null
        );
}
