using BookStore.Core.Entities;
using BookStore.Core.Models;
using System;
using System.Threading.Tasks;

namespace BookStore.Core.Abstractions
{
    public interface IUsersRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<UserEntity?> GetUserEntityByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<Guid> CreateAsync(User user, string passwordHash);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsAnyUserAsync();
    }
}