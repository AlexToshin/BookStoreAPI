using BookStore.API.Controllers;
using BookStore.API.Filters;
using BookStore.API.Middleware;
using BookStore.Application.Services;
using BookStore.Core.Abstractions;
using BookStore.Core.Abstractions.Repositories;
using BookStore.Core.Repositories;
using BookStore.DataAccess;
using BookStore.DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Diagnostics;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System;

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/bookstore-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Запуск приложения");
    
    var builder = WebApplication.CreateBuilder(args);

    // Добавляем Serilog в приложение
    builder.Host.UseSerilog();
    
    // Add services to the container.

    // Настройка CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp",
            builder =>
            {
                builder
                    .AllowAnyOrigin() // Разрешаем запросы с любого источника во время разработки
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
    });

    // Настройка размера для загрузки файлов
    builder.Services.Configure<IISServerOptions>(options =>
    {
        options.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
    });

    builder.Services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
    });

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Добавляем фильтр для отслеживания производительности API
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<RequestPerformanceFilter>();
    });

    // Настройка базы данных
    builder.Services.AddDbContext<BookStoreDbContext>(
        options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("BookStoreDbContext"));
        });

    // Настройка JWT аутентификации
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        });

    // Настройка политик авторизации
    builder.Services.AddAuthorization(options =>
    {
        // Политика для администраторов
        options.AddPolicy("RequireAdminRole", policy =>
            policy.RequireRole("admin"));

        // Политика для всех аутентифицированных пользователей
        options.AddPolicy("RequireAuthenticatedUser", policy =>
            policy.RequireAuthenticatedUser());
            
        // Политика только для чтения (для обычных пользователей)
        options.AddPolicy("ReadOnlyAccess", policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole("admin") || 
                (context.User.Identity.IsAuthenticated && 
                 context.Resource is Endpoint endpoint &&
                 (endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods.All(m => m == "GET") ?? false))));
    });

    // Регистрация сервисов
    builder.Services.AddScoped<IBooksService, BooksService>();
    builder.Services.AddScoped<IBooksRepository, BooksRepository>();
    builder.Services.AddScoped<IAuthorsService, AuthorsService>();
    builder.Services.AddScoped<IAuthorsRepository, AuthorsRepository>();
    builder.Services.AddScoped<ICategoriesService, CategoriesService>();
    builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IUsersRepository, UsersRepository>();
    builder.Services.AddScoped<ICartService, CartService>();
    builder.Services.AddScoped<ICartRepository, CartRepository>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors("AllowReactApp");

    // Добавляем глобальную обработку исключений
    app.UseExceptionHandling();

    // Добавляем обслуживание статических файлов
    app.UseStaticFiles();
    
    // Добавляем поддержку для файлов SPA в папке wwwroot
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
        RequestPath = ""
    });

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    // Добавляем middleware для перенаправления на основе ролей
    app.UseRoleBasedRedirect();

    app.MapControllers();
    
    // Добавляем поддержку SPA - все неизвестные запросы перенаправляем на index.html
    app.MapFallbackToFile("index.html");
    
    // Автоматически открываем браузер при запуске приложения
    if (!app.Environment.IsDevelopment())
    {
        var url = "http://localhost:5059";
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
            Log.Information("Браузер открыт по адресу: {Url}", url);
        }
        catch (Exception ex)
        {
            Log.Warning("Не удалось автоматически открыть браузер: {Error}", ex.Message);
            Log.Information("Пожалуйста, откройте браузер и перейдите по адресу: {Url}", url);
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Приложение завершило работу из-за ошибки.");
}
finally
{
    Log.CloseAndFlush();
}