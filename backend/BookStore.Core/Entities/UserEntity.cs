using System;
using System.Collections.Generic;
using BookStore.Core.Models;

namespace BookStore.Core.Entities
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "user"; // По умолчанию - обычный пользователь
        
        // Коллекция элементов корзины пользователя
        public ICollection<CartItemEntity> CartItems { get; set; } = new List<CartItemEntity>();

        public User ToUser()
        {
            return new User(Id, Username, Email, Role);
        }
    }
}