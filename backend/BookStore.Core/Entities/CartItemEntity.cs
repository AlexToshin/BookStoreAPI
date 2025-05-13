using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Core.Entities
{
    public class CartItemEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }
        public Guid BookId { get; set; }
        public BookEntity Book { get; set; }
        public int Quantity { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
