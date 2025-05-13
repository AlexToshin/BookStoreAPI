using System;
using BookStore.Core.Entities;

namespace BookStore.Core.Models
{
    public class User
    {
        public const int MAX_USERNAME_LENGTH = 50;
        public const int MAX_EMAIL_LENGTH = 100;

        public User(Guid id, string username, string email, string role = "user")
        {
            Id = id;
            Username = username;
            Email = email;
            Role = role;
        }

        public Guid Id { get; }
        public string Username { get; } = string.Empty;
        public string Email { get; } = string.Empty;
        public string Role { get; } = "user";

        public UserEntity ToEntity()
        {
            return new UserEntity
            {
                Id = Id,
                Username = Username,
                Email = Email,
                Role = Role
            };
        }

        public static (User user, string Error) Create(Guid id, string username, string email, string role = "user")
        {
            var error = string.Empty;

            if (string.IsNullOrEmpty(username) || username.Length > MAX_USERNAME_LENGTH)
            {
                error = "Username cannot be empty or longer than 50 symbols";
            }
            else if (string.IsNullOrEmpty(email) || email.Length > MAX_EMAIL_LENGTH)
            {
                error = "Email cannot be empty or longer than 100 symbols";
            }
            else if (!email.Contains("@"))
            {
                error = "Invalid email format";
            }

            var user = new User(id, username, email, role);
            return (user, error);
        }
    }
}