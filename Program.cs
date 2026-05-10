using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Api.Data;

Env.Load("./.env");

var builder = WebApplication.CreateBuilder(args);

// ✅ Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // your Angular app
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    ));

var app = builder.Build();
app.UseCors("AllowFrontend"); // ✅ MUST come before MapControllers()

app.MapControllers();

app.Run();