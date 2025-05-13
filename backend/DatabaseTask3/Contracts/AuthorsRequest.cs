using BookStore.Core.Models;
using BookStore.Core.Entities;
namespace BookStore.API.Contracts
{
    public record AuthorsRequest(
        string Name,
        string Surname
        );
}
