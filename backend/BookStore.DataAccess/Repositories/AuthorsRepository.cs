using BookStore.Core.Entities;
using BookStore.Core.Models;
using BookStore.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repositories
{
    public class AuthorsRepository : IAuthorsRepository
    {
        private readonly BookStoreDbContext _context;
        public AuthorsRepository(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<Author>> Get()
        {
            var authorEntities = await _context.Authors
                .AsNoTracking()
                .ToListAsync();

            var authors = authorEntities.Select(a => Author.Create(a.Id, a.Name, a.Surname).author).ToList();

            return authors;
        }

        public async Task<Author> GetByIdAsync(Guid id)
        {
            var authorEntity = await _context.Authors
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            return authorEntity != null ? Author.Create(authorEntity.Id, authorEntity.Name, authorEntity.Surname).author : null;
        }


        public async Task<Guid> Create(Author author)
        {

            AuthorEntity authorEntity = author.ToEntity();
            await _context.Authors.AddAsync(authorEntity);
            await _context.SaveChangesAsync();

            return authorEntity.Id;
        }

        public async Task<Guid> Update(Guid id, string name, string surname)
        {
            await _context.Authors
                .Where(a => a.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(a => a.Name, a => name)
                    .SetProperty(a => a.Surname, a => surname)
                  
                    );

            return id;
        }

        public async Task<Guid> Delete(Guid id)
        {
            await _context.Authors
                .Where(a => a.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }
    }
}
