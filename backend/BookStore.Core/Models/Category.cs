using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.Core.Entities;

namespace BookStore.Core.Models
{
    public  class Category
    {
        public const int MAX_TITLE_LENGTH = 250;
        public Category(Guid id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
            Books = new List<Book>();

        }

        public Guid Id { get; }
        public string Name { get; } = string.Empty;
        public string Description { get; } = string.Empty;
        public List<Book> Books { get; set; }

        public CategoryEntity ToEntity()
        {
            var books = new List<BookEntity>();
            foreach (var book in Books)
            {
                books.Add(book.ToEntity());
            }
            var entity = new CategoryEntity();
            entity.Id = Id;
            entity.Name = Name;
            entity.Description = Description;
            entity.Books = books;

            return entity;
        }


        public static (Category category, string Error) Create(Guid id, string name, string description)
        {
            var error = string.Empty;

            if (string.IsNullOrEmpty(name) || name.Length > MAX_TITLE_LENGTH)
            {
                error = "Name cannot be empty or longer then 250 symbols";
            }

            var category = new Category(id, name, description);

            return (category, error);
        }
    }
}
