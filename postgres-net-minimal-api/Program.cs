using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using postgres_net_minimal_api.Data;
using postgres_net_minimal_api.Controllers;

var builder = WebApplication.CreateBuilder(args);

// 👇 Registrar servicios ANTES de builder.Build()
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configuración de DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de autenticación con JWT (opcional, si lo necesitas)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build(); // 👈 Aquí ya NO puedes registrar más servicios

// 👇 Configuración de middleware y endpoints DESPUÉS de builder.Build()
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication(); // 👈 Asegúrate de usar UseAuthentication antes de UseAuthorization
app.UseAuthorization();

app.MapControllers();
app.MapUsersEndpoints();
app.MapAuthEndpoints();
app.MapRolesEndpoints();

app.Run();