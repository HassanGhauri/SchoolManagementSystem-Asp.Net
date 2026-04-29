using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Api.Data;

Env.Load("./.env");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    ));

var app = builder.Build();

app.MapControllers();

app.Run();