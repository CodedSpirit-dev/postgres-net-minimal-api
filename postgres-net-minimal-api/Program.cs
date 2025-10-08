using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using postgres_net_minimal_api.Controllers;
using postgres_net_minimal_api.Data;
using DotNetEnv;

// Load environment variables from .env file if it exists
var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3005",    // Tu frontend
                    "http://localhost:3000",    // Common React port
                    "http://localhost:5173",    // Common Vite port
                    "http://localhost:8080"     // Common Vue CLI port
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // Permite credentials
        });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendAll",
        policy =>
        {
            policy.SetIsOriginAllowed(_ => true)  // ✅ ESTA LÍNEA
                .AllowAnyHeader()               // ✅ ESTA LÍNEA  
                .AllowAnyMethod()               // ✅ ESTA LÍNEA
                .AllowCredentials();            // ✅ ESTA LÍNEA
        });
});

// Configure JWT Authentication
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
        };
    });

builder.Services.AddAuthorization();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "PostgreSQL Minimal API",
        Description = "Una API web ASP.NET Core para gestión de usuarios y roles con PostgreSQL",
        Contact = new OpenApiContact
        {
            Name = "Tu Nombre",
            Email = "tu.email@ejemplo.com"
        }
    });

    // JWT Authentication configuration for Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. \r\n\r\n " +
                      "Ingresa 'Bearer' [espacio] y luego tu token en el campo de texto a continuación.\r\n\r\n" +
                      "Ejemplo: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PostgreSQL Minimal API v1");
        c.RoutePrefix = string.Empty; // Swagger UI en la raíz
    });
}

// Use CORS before other middleware
app.UseCors("AllowFrontendAll");

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapUsersEndpoints();
app.MapRolesEndpoints();
app.MapAuthEndpoints();

app.Run();