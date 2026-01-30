using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using postgres_net_minimal_api.Controllers;
using postgres_net_minimal_api.Data;
using postgres_net_minimal_api.Services;
using postgres_net_minimal_api.Authorization;
using postgres_net_minimal_api.Authorization.Endpoints;
using postgres_net_minimal_api.Authorization.Services;
using postgres_net_minimal_api.Blog.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using DotNetEnv;

// Load environment variables from .env file if it exists
var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
}

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

// Register strongly-typed configuration
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

// Add services to the container
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
        npgsqlOptions.CommandTimeout(30);
    });

    // Use NoTracking by default for better performance (opt-in when needed)
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

// Register SOLID pattern components (Repository Pattern - DIP + DRY)
builder.Services.AddScoped(typeof(postgres_net_minimal_api.Common.Interfaces.IRepository<>), typeof(postgres_net_minimal_api.Common.Repository.GenericRepository<>));
builder.Services.AddScoped(typeof(postgres_net_minimal_api.Common.Interfaces.IQueryableRepository<>), typeof(postgres_net_minimal_api.Common.Repository.GenericRepository<>));

// Register validators (SRP - Single Responsibility Principle)
builder.Services.AddSingleton<postgres_net_minimal_api.Common.Validation.IValidator<string>, postgres_net_minimal_api.Common.Validation.PasswordStrengthValidator>();
builder.Services.AddSingleton<postgres_net_minimal_api.Common.Validation.EmailFormatValidator>();
builder.Services.AddSingleton<postgres_net_minimal_api.Common.Validation.UsernameFormatValidator>();

// Register application services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IPasswordValidator, PasswordValidator>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Register blog services
builder.Services.AddScoped<postgres_net_minimal_api.Blog.Services.IPostService, postgres_net_minimal_api.Blog.Services.PostService>();
builder.Services.AddScoped<postgres_net_minimal_api.Blog.Services.ICategoryService, postgres_net_minimal_api.Blog.Services.CategoryService>();
builder.Services.AddScoped<postgres_net_minimal_api.Blog.Services.ITagService, postgres_net_minimal_api.Blog.Services.TagService>();
builder.Services.AddScoped<postgres_net_minimal_api.Blog.Services.ICommentService, postgres_net_minimal_api.Blog.Services.CommentService>();
builder.Services.AddScoped<postgres_net_minimal_api.Blog.Services.IProfileService, postgres_net_minimal_api.Blog.Services.ProfileService>();
builder.Services.AddScoped<postgres_net_minimal_api.Blog.Services.IBlogStatisticsService, postgres_net_minimal_api.Blog.Services.BlogStatisticsService>();

// Register memory cache for permissions
builder.Services.AddMemoryCache();

// Register permission services
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IPermissionChecker, PermissionChecker>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// Register ownership checkers for instance-level permissions
builder.Services.AddScoped<PostOwnershipChecker>();
builder.Services.AddScoped<CommentOwnershipChecker>();
builder.Services.AddScoped<ProfileOwnershipChecker>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendAll",
        policy =>
        {
            policy.SetIsOriginAllowed(_ => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// Configure Rate Limiting (NEW in .NET 8+)
builder.Services.AddRateLimiter(options =>
{
    // Rate limit for login endpoint to prevent brute force attacks
    options.AddFixedWindowLimiter("login", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 5;
        opt.QueueLimit = 0;
    });

    // Global rate limit for API
    options.AddFixedWindowLimiter("api", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 100;
        opt.QueueLimit = 0;
    });

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(
            new { error = "Too many requests. Please try again later." },
            cancellationToken);
    };
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

// Add OpenAPI (Built-in .NET 10 support)
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Maps the OpenAPI endpoint at /openapi/v1.json
}

// Global exception handling middleware
app.UseExceptionHandler("/error");

// Use CORS before other middleware
app.UseCors("AllowFrontendAll");

// Add rate limiting middleware
app.UseRateLimiter();

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

// Error endpoint for global exception handling
app.Map("/error", () => Results.Problem("An error occurred processing your request"));

// Map user management endpoints
app.MapUsersEndpoints();
app.MapRolesEndpoints();
app.MapAuthEndpoints();

// Map blog endpoints
app.MapPostsEndpoints();
app.MapCategoriesEndpoints();
app.MapTagsEndpoints();
app.MapCommentsEndpoints();
app.MapProfilesEndpoints();
app.MapBlogStatisticsEndpoints();

// Map permission endpoints
app.MapPermissionEndpoints();

app.Run();