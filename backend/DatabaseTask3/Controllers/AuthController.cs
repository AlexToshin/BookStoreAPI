using BookStore.Core.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("Получен запрос на регистрацию пользователя: {Username}", request.Username);
            
            // По умолчанию регистрируем обычного пользователя с ролью "user"
            var (user, error) = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
            
            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogWarning("Ошибка при регистрации пользователя {Username}: {Error}", request.Username, error);
                return BadRequest(error);
            }

            _logger.LogInformation("Пользователь {Username} успешно зарегистрирован с ролью user", request.Username);
            return Ok(new { user.Id, user.Username, user.Email, user.Role });
        }
        
        [HttpPost("register-admin")]
        [Authorize(Roles = "admin")] // Только администраторы могут создавать других администраторов
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("Получен запрос на регистрацию администратора от пользователя {Username}", User.Identity.Name);
            
            var (user, error) = await _authService.RegisterAsync(request.Username, request.Email, request.Password, "admin");
            
            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogWarning("Ошибка при регистрации администратора {Username}: {Error}", request.Username, error);
                return BadRequest(error);
            }

            _logger.LogInformation("Администратор {Username} успешно зарегистрирован", request.Username);
            return Ok(new { user.Id, user.Username, user.Email, user.Role });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Получен запрос на вход от пользователя: {Username}", request.Username);
            
            var (user, token, error) = await _authService.LoginAsync(request.Username, request.Password);
            
            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogWarning("Ошибка при входе пользователя {Username}: {Error}", request.Username, error);
                return BadRequest(error);
            }

            _logger.LogInformation("Пользователь {Username} успешно вошел с ролью {Role}", request.Username, user.Role);
            return Ok(new { user.Id, user.Username, user.Email, user.Role, token });
        }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}