using BookStore.Core.Models;

namespace BookStore.API.Contracts
{
    public record BooksResponse(
        Guid Id,
        string Title,
        string Description, 
        decimal Price,
        Guid AuthorId,
        Author? Author,
        Guid CategoryId,
        Category? Category,
        string? ImageUrl = null
        );
}
