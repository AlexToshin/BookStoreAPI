using BookStore.Core.Entities;

namespace BookStore.Core.Models
{
    public class Book
    {
        public const int MAX_TITLE_LENGTH = 250;
        public Book(Guid id, string title, string description, decimal price, Guid authorId, Author author, Guid categoryId, Category category, string imageUrl = null)
        {
            Id = id;
            Title = title;
            Description = description;
            Price = price;
            AuthorId = authorId;
            Author = author;
            CategoryId = categoryId;
            Category = category;
            ImageUrl = imageUrl;
        }

        public Guid Id { get; }
        public string Title { get;  } = string.Empty;
        public string Description { get;  } = string.Empty;
        public decimal Price { get;  }

        public Guid AuthorId { get; }
        public Author? Author { get; }

        public Guid CategoryId { get; }
        public Category? Category { get; }

        public string? ImageUrl { get; }

        public BookEntity ToEntity()
        {
            var entity = new BookEntity();
            entity.Id = Id;
            entity.Title = Title;
            entity.Description = Description;
            entity.Price = Price;
            entity.AuthorId = AuthorId;
            entity.Author = Author?.ToEntity();
            entity.CategoryId = CategoryId;
            entity.Category = Category?.ToEntity();
            entity.ImageUrl = ImageUrl;

            return entity;
        }
        

        public static (Book book, string Error) Create(Guid id, string title, string description, decimal price, Guid authorId, Author author, Guid categoryId, Category category, string imageUrl = null)
        {
            var error = string.Empty;

            if (string.IsNullOrEmpty(title) || title.Length > MAX_TITLE_LENGTH )
            {
                error = "Title cannot be empty or longer then 250 symbols";
            }

            if (price < 0)
            {
                error = $"{nameof(price)} can't be less then 0";
            }

            var book = new Book(id, title, description, price, authorId, author, categoryId, category, imageUrl);

            return (book, error);
        }
    }
}
