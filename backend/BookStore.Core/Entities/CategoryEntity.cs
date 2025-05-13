    using BookStore.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace BookStore.Core.Entities
    {
        public class CategoryEntity
        {
            public CategoryEntity()
            {
                Books = new List<BookEntity>();
            }
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;

            public List<BookEntity> Books { get; set; }

            public Category ToCategory(bool includeBooks = true)
            {
                var category = new Category(Id, Name, Description);
                
                if (includeBooks && Books != null)
                {
                    var booksList = new List<Book>();
                    foreach (var book in Books)
                    {
                        booksList.Add(book.ToBook(false));
                    }
                    category.Books = booksList;
                }
                
                return category;
            }
        }
    }
