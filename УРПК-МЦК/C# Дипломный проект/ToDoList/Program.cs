using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using ToDoList.DAL;
using ToDoList.DAL.Interfaces;
using ToDoList.DAL.Repositories;
using ToDoList.Domain.Entity;
using ToDoList.Service.Implementations;
using ToDoList.Service.Interfaces;
using ToDoList.Domain.Extenstions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ToDoList.Domain.Enum;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddScoped<IBaseRepository<TaskEntity>, BaseRepository>();
builder.Services.AddScoped<IBaseRepository<DepartamentEntity>, DepRepository>();
builder.Services.AddScoped<IBaseRepository<UserEntity>, UserRepository>();

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IDepService, DepartamentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<JWTService>();

builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection(nameof(JWTOptions)));

var connectionString = builder.Configuration.GetConnectionString("MSSQL");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
builder.Services.AddHttpContextAccessor();
// Добавление аутентификации и авторизации
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTOptions:SecretKey"]))
        };
        
        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["tasty-cookies"];

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireClaim("Admin", "true");
    }); 
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=LoginPage}/{id?}");

app.MapControllerRoute(
    name: "tasks",
    pattern: "{controller=Task}/{action=Tasks}/{id?}");

app.MapControllerRoute(
    name: "regUsers",
    pattern: "{controller=User}/{action=RegUsers}/{id?}");

app.MapControllerRoute(
    name: "users",
    pattern: "{controller=User}/{action=Users}/{id?}");

app.MapControllerRoute(
    name: "dep",
    pattern: "{controller=Dep}/{action=Dep}/{id?}");

app.UseCookiePolicy(new CookiePolicyOptions()
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Применяем миграции
    dbContext.Database.Migrate();

    // Проверяем, есть ли уже суперадмин в базе данных
    if (!dbContext.Users.Any(u => u.Id == 1))
    {
        // Генерируем ключ и вектор инициализации
        byte[] key = new byte[16];
        byte[] iv = new byte[16];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
            rng.GetBytes(iv);
        }
        // Шифруем данные суперадмина
        byte[] EnFIO = UserService.Encrypt("superAdmin", key, iv);
        byte[] EnLogin = UserService.Encrypt("superAd170", key, iv);
        byte[] EnPass = UserService.Encrypt("superAd170", key, iv);

        // Создаем суперадмина с зашифрованными данными
        var user = new UserEntity
        {
            EnFIO = EnFIO,
            EnLogin = EnLogin,
            EnPass = EnPass,
            EncryptionKey = key,
            InitializationVector = iv,
            role = Role.Administrator
        };

        try
        {
            // Создание пользователя "superAdmin" при инициализации базы данных
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Обработка исключений
            Console.WriteLine($"Ошибка при создании суперадмина: {ex.Message}");
        }
    }
}

;
app.Run();


    


