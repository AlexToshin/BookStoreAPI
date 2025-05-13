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
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly BookStoreDbContext _context;
        public CategoriesRepository(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> Get()
        {
            var categoryEntities = await _context.Categories
                .AsNoTracking()
                .ToListAsync();

            var categories = categoryEntities.Select(c => Category.Create(c.Id, c.Name, c.Description).category).ToList();

            return categories;
        }

        public async Task<Category> GetByIdAsync(Guid id)
        {
            var categoryEntity = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return categoryEntity != null ? Category.Create(categoryEntity.Id, categoryEntity.Name, categoryEntity.Description).category : null;
        }


        public async Task<Guid> Create(Category category)
        {

            CategoryEntity categoryEntity = category.ToEntity();
            await _context.Categories.AddAsync(categoryEntity);
            await _context.SaveChangesAsync();

            return categoryEntity.Id;
        }

        public async Task<Guid> Update(Guid id, string name, string description)
        {
            await _context.Categories
                .Where(c => c.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.Name, c => name)
                    .SetProperty(c => c.Description, c => description)

                    );

            return id;
        }

        public async Task<Guid> Delete(Guid id)
        {
            await _context.Categories
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }
    }
}
