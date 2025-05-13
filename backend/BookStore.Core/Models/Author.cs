using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.Core.Entities;

namespace BookStore.Core.Models
{
    public  class Author
    {
        public const int MAX_TITLE_LENGTH = 250;
        public Author(Guid id, string name, string surname)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Books = new List<Book>();

        }

        public Guid Id { get; }
        public string Name { get; } = string.Empty;
        public string Surname { get; } = string.Empty;

        public List<Book> Books { get; set; }

        public AuthorEntity ToEntity()
        {
            var books = new List<BookEntity>();
            foreach (var book in Books)
            {
                books.Add(book.ToEntity());
            }
            var entity = new AuthorEntity();
            entity.Id = Id;
            entity.Name = Name;
            entity.Surname = Surname;
            entity.Books = books;
            return entity;
        }


        public static (Author author, string Error) Create(Guid id, string name, string surname)
        {
            var error = string.Empty;

            if (string.IsNullOrEmpty(name) || name.Length > MAX_TITLE_LENGTH)
            {
                error = "Name cannot be empty or longer then 250 symbols";
            }

            var author = new Author(id, name, surname);

            return (author, error);
        }
    }
}
