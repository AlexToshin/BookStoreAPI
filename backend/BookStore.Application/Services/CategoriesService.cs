using BookStore.Core.Entities;
using BookStore.Core.Models;
using BookStore.Core.Repositories;
using BookStore.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Application.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ICategoriesRepository _categoriesRepository;
        public CategoriesService(ICategoriesRepository categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _categoriesRepository.Get();
        }

        public async Task<Category> GetCategoryById(Guid id)
        {
            return await _categoriesRepository.GetByIdAsync(id);
        }

        public async Task<Guid> CreateCategory(Category category)
        {
            return await _categoriesRepository.Create(category);
        }

        public async Task<Guid> UpdateCategory(Guid id, string name, string description)
        {
            return await _categoriesRepository.Update(id, name, description);
        }

        public async Task<Guid> DeleteCategory(Guid id)
        {
            return await _categoriesRepository.Delete(id);
        }
    }
}
