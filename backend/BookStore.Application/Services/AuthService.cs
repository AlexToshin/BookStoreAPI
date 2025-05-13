using BookStore.Core.Abstractions;
using BookStore.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace BookStore.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUsersRepository usersRepository, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _usersRepository = usersRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<(User user, string token, string error)> LoginAsync(string username, string password)
        {
            _logger.LogInformation("Попытка входа пользователя: {Username}", username);
            
            var user = await _usersRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("Неудачная попытка входа: пользователь {Username} не найден", username);
                return (new User(Guid.Empty, string.Empty, string.Empty), string.Empty, "User not found");
            }

            var userEntity = await _usersRepository.GetUserEntityByUsernameAsync(username);
            if (userEntity == null || !BC.Verify(password, userEntity.PasswordHash))
            {
                _logger.LogWarning("Неудачная попытка входа: неверный пароль для пользователя {Username}", username);
                return (new User(Guid.Empty, string.Empty, string.Empty), string.Empty, "Invalid password");
            }

            var token = GenerateJwtToken(user);
            _logger.LogInformation("Успешный вход пользователя: {Username}, UserId: {UserId}, Role: {Role}", username, user.Id, user.Role);
            return (user, token, string.Empty);
        }

        public async Task<(User user, string error)> RegisterAsync(string username, string email, string password, string role = "user")
        {
            _logger.LogInformation("Попытка регистрации нового пользователя: {Username}, {Email}, Role: {Role}", username, email, role);
            
            // Проверка на первого пользователя - он автоматически становится администратором
            var usersExist = await _usersRepository.ExistsAnyUserAsync();
            if (!usersExist)
            {
                _logger.LogInformation("Регистрация первого пользователя с повышенными правами (admin): {Username}", username);
                role = "admin";
            }
            // Проверяем, что роль соответствует одному из возможных значений
            else if (role != "user" && role != "admin")
            {
                role = "user"; // По умолчанию - обычный пользователь
            }
            
            if (await _usersRepository.ExistsByUsernameAsync(username))
            {
                _logger.LogWarning("Регистрация не удалась: пользователь {Username} уже существует", username);
                return (new User(Guid.Empty, string.Empty, string.Empty), "Username already exists");
            }

            if (await _usersRepository.ExistsByEmailAsync(email))
            {
                return (new User(Guid.Empty, string.Empty, string.Empty), "Email already exists");
            }

            var (user, error) = User.Create(Guid.NewGuid(), username, email, role);
            if (!string.IsNullOrEmpty(error))
            {
                return (new User(Guid.Empty, string.Empty, string.Empty), error);
            }

            var passwordHash = BC.HashPassword(password);
            await _usersRepository.CreateAsync(user, passwordHash);

            return (user, string.Empty);
        }

        public string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role) // Добавляем роль пользователя
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}