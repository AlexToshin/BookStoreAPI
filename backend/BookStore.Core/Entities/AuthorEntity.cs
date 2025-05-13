    using BookStore.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace BookStore.Core.Entities
    {
        public class AuthorEntity
        {
        public AuthorEntity()
        {
            Books = new List<BookEntity>();
        }
        public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;

            public List<BookEntity> Books { get; set; }

            public Author ToAuthor(bool includeBooks = true)
            {
                var author = new Author(Id, Name, Surname);
                
                if (includeBooks && Books != null)
                {
                    var books = new List<Book>();
                    foreach (var book in Books)
                    {
                        // Используем параметр includeRelations=false, чтобы избежать циклической зависимости
                        books.Add(book.ToBook(false));
                    }
                    author.Books = books;
                }
                
                return author;
            }
        }
    }
