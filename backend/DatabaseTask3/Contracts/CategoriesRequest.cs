using BookStore.Core.Models;
using BookStore.Core.Entities;
namespace BookStore.API.Contracts
{
    public record CategoriesRequest(
        string Name,
        string Description
        );
}
