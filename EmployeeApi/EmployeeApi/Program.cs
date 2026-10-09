using Scalar.AspNetCore;
using EmployeeApi.Data;
using EmployeeApi.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Add services to the container.   
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();    // UI to view and test the API

}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");      // ?? must come BEFORE UseAuthorization & MapControllers


app.UseAuthorization();

app.MapControllers();

app.Run();
