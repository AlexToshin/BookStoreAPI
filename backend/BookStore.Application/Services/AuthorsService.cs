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
    public class AuthorsService : IAuthorsService
    {
        private readonly IAuthorsRepository _authorsRepository;
        public AuthorsService(IAuthorsRepository authorsRepository)
        {
            _authorsRepository = authorsRepository;
        }

        public async Task<List<Author>> GetAllAuthors()
        {
            return await _authorsRepository.Get();
        }

        public async Task<Author> GetAuthorById(Guid id)
        {
            return await _authorsRepository.GetByIdAsync(id);
        }

        public async Task<Guid> CreateAuthor(Author author)
        {
            return await _authorsRepository.Create(author);
        }

        public async Task<Guid> UpdateAuthor(Guid id, string name, string surname)
        {
            return await _authorsRepository.Update(id, name, surname);
        }

        public async Task<Guid> DeleteAuthor(Guid id)
        {
            return await _authorsRepository.Delete(id);
        }
    }
}
