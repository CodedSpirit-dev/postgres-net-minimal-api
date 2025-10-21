using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Models;

namespace postgres_net_minimal_api.Data;

/// <summary>
/// Database context for the application using PostgreSQL
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // User management
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Profile> Profiles { get; set; }

    // Blog entities
    public DbSet<Post> Posts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fixed GUIDs for roles
        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var guestRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        // Seed data for roles
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { Id = adminRoleId, Name = "Admin", Description = "System administrator" },
            new UserRole { Id = userRoleId, Name = "User", Description = "Standard user" },
            new UserRole { Id = guestRoleId, Name = "Guest", Description = "Guest user with limited access" }
        );

        // Fixed GUIDs for users
        var adminUserId = Guid.Parse("A1111111-1111-1111-1111-111111111111");
        var standardUserId = Guid.Parse("B2222222-2222-2222-2222-222222222222");

        // Pre-generated BCrypt hashes for seed data (static hashes to avoid runtime hashing)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminUserId,
                UserName = "Admin",
                FirstName = "Admin",
                LastName = "System",
                Email = "admin@example.com",
                HashedPassword = "$2a$11$67rVUGbvftj.YcqUIiQGSeg47kWVtGJXZR8ZbESPh7VG5glAAtDqe",
                DateOfBirth = new DateOnly(1990, 1, 1),
                RoleId = adminRoleId
            },
            new User
            {
                Id = standardUserId,
                UserName = "User",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                HashedPassword = "yo$2a$11$5ByfQRylb6t1ucfmqMASC.OGbS4Qp7sPq4Dpc1YC24oiG6usM26PK",
                DateOfBirth = new DateOnly(1995, 5, 15),
                RoleId = userRoleId
            }
        );

        // Add composite index for common queries
        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.RoleId, u.LastName })
            .HasDatabaseName("IX_Users_RoleId_LastName");

        // Configure Blog entities relationships
        ConfigureBlogEntities(modelBuilder);

        // Seed blog data
        SeedBlogData(modelBuilder);
    }

    private static void ConfigureBlogEntities(ModelBuilder modelBuilder)
    {
        // Profile - One-to-One with User
        modelBuilder.Entity<Profile>()
            .HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<Profile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Post - Many-to-One with User (Author)
        modelBuilder.Entity<Post>()
            .HasOne(p => p.Author)
            .WithMany()
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Post - Many-to-One with Category
        modelBuilder.Entity<Post>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Posts)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // PostTag - Composite Primary Key
        modelBuilder.Entity<PostTag>()
            .HasKey(pt => new { pt.PostId, pt.TagId });

        // PostTag - Many-to-One with Post
        modelBuilder.Entity<PostTag>()
            .HasOne(pt => pt.Post)
            .WithMany(p => p.PostTags)
            .HasForeignKey(pt => pt.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // PostTag - Many-to-One with Tag
        modelBuilder.Entity<PostTag>()
            .HasOne(pt => pt.Tag)
            .WithMany(t => t.PostTags)
            .HasForeignKey(pt => pt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        // Comment - Many-to-One with Post
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Comment - Many-to-One with User (Author)
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Author)
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Comment - Self-referencing for nested comments
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Performance indexes
        modelBuilder.Entity<Post>()
            .HasIndex(p => new { p.IsPublished, p.PublishedAt })
            .HasDatabaseName("IX_Posts_Published_PublishedAt");

        modelBuilder.Entity<Comment>()
            .HasIndex(c => new { c.PostId, c.IsApproved })
            .HasDatabaseName("IX_Comments_PostId_IsApproved");
    }

    private static void SeedBlogData(ModelBuilder modelBuilder)
    {
        var techCategoryId = Guid.Parse("C1111111-1111-1111-1111-111111111111");
        var tutorialsCategoryId = Guid.Parse("C2222222-2222-2222-2222-222222222222");

        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = techCategoryId,
                Name = "Technology",
                Slug = "technology",
                Description = "Posts about technology and programming",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = tutorialsCategoryId,
                Name = "Tutorials",
                Slug = "tutorials",
                Description = "Step-by-step tutorials and guides",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        // Seed Tags
        var csharpTagId = Guid.Parse("T1111111-1111-1111-1111-111111111111");
        var dotnetTagId = Guid.Parse("T2222222-2222-2222-2222-222222222222");
        var postgresTagId = Guid.Parse("T3333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<Tag>().HasData(
            new Tag { Id = csharpTagId, Name = "C#", Slug = "csharp", CreatedAt = DateTime.UtcNow },
            new Tag { Id = dotnetTagId, Name = ".NET", Slug = "dotnet", CreatedAt = DateTime.UtcNow },
            new Tag { Id = postgresTagId, Name = "PostgreSQL", Slug = "postgresql", CreatedAt = DateTime.UtcNow }
        );
    }
}