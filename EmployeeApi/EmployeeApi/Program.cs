using Scalar.AspNetCore;
using EmployeeApi.Data;
using EmployeeApi.Repositories;
using Microsoft.EntityFrameworkCore;
using EmployeeApi.Services;
using EmployeeApi.Middleware;
using EmployeeApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// ================= SERVICES (Dependency Injection) =================
builder.Services.AddControllers();

// Add services to the container.   
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// JWT settings – validated at startup so a missing key fails fast
builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .Validate(o => o.Key.Length >= 32, "Jwt:Key must be at least 32 characters (set it in User Secrets)")
    .ValidateOnStart();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(allowedOrigins)   // only our Angular app
              .AllowAnyHeader()              // Content-Type, Authorization...
              .AllowAnyMethod();             // GET, POST, PUT, DELETE
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
// ================= MIDDLEWARE PIPELINE (order matters!) =================
app.UseExceptionHandler();        // FIRST, so it wraps everything after it

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();    // UI to view and test the API

}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");      //  must come BEFORE UseAuthorization & MapControllers


app.UseAuthorization();

app.MapControllers();

app.Run();
