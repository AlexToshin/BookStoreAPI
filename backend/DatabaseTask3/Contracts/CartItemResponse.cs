using System;

namespace BookStore.API.Contracts
{
    public record CartItemResponse(
        Guid Id,
        Guid BookId,
        string BookTitle,
        string BookDescription,
        decimal BookPrice,
        string BookImageUrl,
        string AuthorName,
        string CategoryName,
        int Quantity,
        DateTime DateAdded
    );
}
