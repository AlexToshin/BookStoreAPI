using System;
using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Contracts
{
    public record CartItemRequest(
        [Required] Guid BookId,
        [Required] [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")] int Quantity
    );
}
