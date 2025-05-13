using BookStore.Core.Models;
using System;
using System.Threading.Tasks;

namespace BookStore.Core.Abstractions
{
    public interface IAuthService
    {
        Task<(User user, string token, string error)> LoginAsync(string username, string password);
        Task<(User user, string error)> RegisterAsync(string username, string email, string password, string role = "user");
        string GenerateJwtToken(User user);
    }
}