using BookStore.Core.Abstractions;
using BookStore.Core.Entities;
using BookStore.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly BookStoreDbContext _context;

        public UsersRepository(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var userEntity = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);

            return userEntity?.ToUser();
        }

        public async Task<UserEntity?> GetUserEntityByUsernameAsync(string username)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var userEntity = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            return userEntity?.ToUser();
        }

        public async Task<Guid> CreateAsync(User user, string passwordHash)
        {
            var userEntity = user.ToEntity();
            userEntity.PasswordHash = passwordHash;
            
            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();

            return userEntity.Id;
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        
        public async Task<bool> ExistsAnyUserAsync()
        {
            return await _context.Users.AnyAsync();
        }
    }
}