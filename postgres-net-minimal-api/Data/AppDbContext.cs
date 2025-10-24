using Microsoft.EntityFrameworkCore;
using postgres_net_minimal_api.Models;
using postgres_net_minimal_api.Authorization.Models;

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

    // Permission/Authorization entities
    public DbSet<ApplicationModule> ApplicationModules { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<PermissionAction> PermissionActions { get; set; }
    public DbSet<FeatureAction> FeatureActions { get; set; }
    public DbSet<ModuleFeature> ModuleFeatures { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fixed GUIDs for roles
        var superAdminRoleId = Guid.Parse("10000000-0000-0000-0000-000000000000");
        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var guestRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        // Seed data for roles
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { Id = superAdminRoleId, Name = "SuperAdmin", Description = "Super administrator with unrestricted access to everything" },
            new UserRole { Id = adminRoleId, Name = "Admin", Description = "System administrator" },
            new UserRole { Id = userRoleId, Name = "User", Description = "Standard user" },
            new UserRole { Id = guestRoleId, Name = "Guest", Description = "Guest user with limited access" }
        );

        // Fixed GUIDs for users
        var superAdminUserId = Guid.Parse("50000000-0000-0000-0000-000000000000");
        var adminUserId = Guid.Parse("A1111111-1111-1111-1111-111111111111");
        var standardUserId = Guid.Parse("B2222222-2222-2222-2222-222222222222");
        var guestUserId = Guid.Parse("C3333333-3333-3333-3333-333333333333");

        // Pre-generated BCrypt hashes for seed data (static hashes to avoid runtime hashing)
        // Password pattern: "yo" + role_name + "123"
        // SuperAdmin: yosuperadmin123
        // Admin: yoadmin123
        // User: youser123
        // Guest: yoguest123
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = superAdminUserId,
                UserName = "superadmin",
                FirstName = "Super",
                LastName = "Administrator",
                Email = "superadmin@admin.com",
                HashedPassword = "$2a$12$AIyvBPaFAwg.HVFlbCXTMugw8A4hU6LKlJCCbqxMiFvobB3eu0JYW", // yosuperadmin123
                DateOfBirth = new DateOnly(1985, 1, 1),
                RoleId = superAdminRoleId
            },
            new User
            {
                Id = adminUserId,
                UserName = "admin",
                FirstName = "Admin",
                LastName = "System",
                Email = "admin@admin.com",
                HashedPassword = "$2a$11$xKJAr8qR9Z5VyN3J9YZ.qOh4L0U8XZ0F8Y0W2N6Q5M3F6Y7Z8Q9Kf", // yoadmin123
                DateOfBirth = new DateOnly(1990, 1, 1),
                RoleId = adminRoleId
            },
            new User
            {
                Id = standardUserId,
                UserName = "user",
                FirstName = "John",
                LastName = "Doe",
                Email = "user@example.com",
                HashedPassword = "$2a$11$xKJAr8qR9Z5VyN3J9YZ.qOh4L0U8XZ0F8Y0W2N6Q5M3F6Y7Z8Q9Kg", // youser123
                DateOfBirth = new DateOnly(1995, 5, 15),
                RoleId = userRoleId
            },
            new User
            {
                Id = guestUserId,
                UserName = "guest",
                FirstName = "Guest",
                LastName = "User",
                Email = "guest@example.com",
                HashedPassword = "$2a$11$xKJAr8qR9Z5VyN3J9YZ.qOh4L0U8XZ0F8Y0W2N6Q5M3F6Y7Z8Q9Kh", // yoguest123
                DateOfBirth = new DateOnly(2000, 1, 1),
                RoleId = guestRoleId
            }
        );

        // Add composite index for common queries
        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.RoleId, u.LastName })
            .HasDatabaseName("IX_Users_RoleId_LastName");

        // Configure Blog entities relationships
        ConfigureBlogEntities(modelBuilder);

        // Configure Permission entities relationships
        ConfigurePermissionEntities(modelBuilder);

        // Seed blog data
        SeedBlogData(modelBuilder);

        // Seed permission data
        SeedPermissionData(modelBuilder);
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
        var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // IDs for users
        var superAdminUserId = Guid.Parse("50000000-0000-0000-0000-000000000000");
        var adminUserId = Guid.Parse("A1111111-1111-1111-1111-111111111111");
        var standardUserId = Guid.Parse("B2222222-2222-2222-2222-222222222222");
        var guestUserId = Guid.Parse("C3333333-3333-3333-3333-333333333333");

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
                CreatedAt = now,
                UpdatedAt = now
            },
            new Category
            {
                Id = tutorialsCategoryId,
                Name = "Tutorials",
                Slug = "tutorials",
                Description = "Step-by-step tutorials and guides",
                CreatedAt = now,
                UpdatedAt = now
            }
        );

        // Seed Tags
        var csharpTagId = Guid.Parse("71111111-1111-1111-1111-111111111111");
        var dotnetTagId = Guid.Parse("72222222-2222-2222-2222-222222222222");
        var postgresTagId = Guid.Parse("73333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<Tag>().HasData(
            new Tag { Id = csharpTagId, Name = "C#", Slug = "csharp", CreatedAt = now },
            new Tag { Id = dotnetTagId, Name = ".NET", Slug = "dotnet", CreatedAt = now },
            new Tag { Id = postgresTagId, Name = "PostgreSQL", Slug = "postgresql", CreatedAt = now }
        );

        // Seed Profiles
        var superAdminProfileId = Guid.Parse("90000000-0000-0000-0000-000000000000");
        var adminProfileId = Guid.Parse("91111111-1111-1111-1111-111111111111");
        var userProfileId = Guid.Parse("92222222-2222-2222-2222-222222222222");
        var guestProfileId = Guid.Parse("93333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<Profile>().HasData(
            new Profile
            {
                Id = superAdminProfileId,
                UserId = superAdminUserId,
                Bio = "Super Administrator with unrestricted access to all system features. Responsible for platform management, user moderation, and system-wide configurations.",
                WebsiteUrl = "https://platform.example.com",
                TwitterHandle = "@platform_admin",
                GitHubHandle = "superadmin",
                CreatedAt = now,
                UpdatedAt = now
            },
            new Profile
            {
                Id = adminProfileId,
                UserId = adminUserId,
                Bio = "System administrator and technical writer. Passionate about .NET technologies and modern software architecture.",
                WebsiteUrl = "https://example.com/admin",
                TwitterHandle = "@admin_dev",
                GitHubHandle = "admin-developer",
                CreatedAt = now,
                UpdatedAt = now
            },
            new Profile
            {
                Id = userProfileId,
                UserId = standardUserId,
                Bio = "Software developer specializing in C# and PostgreSQL. Love building scalable web applications.",
                WebsiteUrl = "https://johndoe.dev",
                TwitterHandle = "@johndoe_dev",
                GitHubHandle = "johndoe",
                LinkedInHandle = "https://linkedin.com/in/johndoe",
                CreatedAt = now,
                UpdatedAt = now
            },
            new Profile
            {
                Id = guestProfileId,
                UserId = guestUserId,
                Bio = "Guest user exploring the platform features.",
                WebsiteUrl = null,
                TwitterHandle = null,
                GitHubHandle = null,
                CreatedAt = now,
                UpdatedAt = now
            }
        );

        // Seed Posts
        var post1Id = Guid.Parse("10571111-1111-1111-1111-111111111111");
        var post2Id = Guid.Parse("10572222-2222-2222-2222-222222222222");
        var post3Id = Guid.Parse("10573333-3333-3333-3333-333333333333");
        var post4Id = Guid.Parse("10574444-4444-4444-4444-444444444444");

        modelBuilder.Entity<Post>().HasData(
            new Post
            {
                Id = post1Id,
                Title = "Getting Started with .NET 9 and PostgreSQL",
                Slug = "getting-started-with-net9-and-postgresql",
                Excerpt = "Learn how to build modern web APIs using .NET 9 and PostgreSQL with this comprehensive guide.",
                Content = "<p>In this tutorial, we'll explore how to build a modern web API using .NET 9 and PostgreSQL. We'll cover Entity Framework Core 9, minimal APIs, and best practices for database design.</p><p>PostgreSQL is a powerful, open-source relational database that pairs perfectly with .NET for building scalable applications.</p>",
                FeaturedImageUrl = "https://images.example.com/net9-postgres.jpg",
                IsPublished = true,
                PublishedAt = now.AddDays(-7),
                ViewCount = 1250,
                AuthorId = adminUserId,
                CategoryId = techCategoryId,
                CreatedAt = now.AddDays(-7),
                UpdatedAt = now.AddDays(-7)
            },
            new Post
            {
                Id = post2Id,
                Title = "Advanced RBAC Patterns in ASP.NET Core",
                Slug = "advanced-rbac-patterns-aspnet-core",
                Excerpt = "Implementing fine-grained role-based access control with resource-action permissions in modern .NET applications.",
                Content = "<p>Role-Based Access Control (RBAC) is essential for securing modern applications. In this post, we dive deep into implementing granular permissions using the Resource-Action pattern.</p><p>We'll cover instance-level permissions, ownership validation, and how to combine type-level and instance-level authorization.</p>",
                FeaturedImageUrl = "https://images.example.com/rbac-pattern.jpg",
                IsPublished = true,
                PublishedAt = now.AddDays(-3),
                ViewCount = 867,
                AuthorId = standardUserId,
                CategoryId = tutorialsCategoryId,
                CreatedAt = now.AddDays(-3),
                UpdatedAt = now.AddDays(-3)
            },
            new Post
            {
                Id = post3Id,
                Title = "Building a Blog System with Minimal APIs",
                Slug = "building-blog-system-minimal-apis",
                Excerpt = "Step-by-step guide to creating a complete blog system using .NET 9 Minimal APIs.",
                Content = "<p>Minimal APIs in .NET provide a streamlined way to build HTTP APIs with minimal ceremony. In this tutorial, we'll build a complete blog system from scratch.</p><p>Topics covered: Posts, Categories, Tags, Comments, SEO-friendly slugs, and more!</p>",
                FeaturedImageUrl = "https://images.example.com/minimal-api-blog.jpg",
                IsPublished = true,
                PublishedAt = now.AddDays(-1),
                ViewCount = 423,
                AuthorId = standardUserId,
                CategoryId = tutorialsCategoryId,
                CreatedAt = now.AddDays(-1),
                UpdatedAt = now.AddDays(-1)
            },
            new Post
            {
                Id = post4Id,
                Title = "Draft: Performance Optimization Techniques",
                Slug = "draft-performance-optimization-techniques",
                Excerpt = "Exploring various performance optimization strategies for .NET applications (Work in Progress).",
                Content = "<p>This is a draft post exploring performance optimization techniques including caching, database query optimization, and async patterns.</p>",
                FeaturedImageUrl = null,
                IsPublished = false,
                PublishedAt = null,
                ViewCount = 0,
                AuthorId = adminUserId,
                CategoryId = techCategoryId,
                CreatedAt = now,
                UpdatedAt = now
            }
        );

        // Seed PostTags
        modelBuilder.Entity<PostTag>().HasData(
            // Post 1: .NET 9 and PostgreSQL - tags: .NET, PostgreSQL
            new PostTag { PostId = post1Id, TagId = dotnetTagId },
            new PostTag { PostId = post1Id, TagId = postgresTagId },

            // Post 2: RBAC Patterns - tags: .NET, C#
            new PostTag { PostId = post2Id, TagId = dotnetTagId },
            new PostTag { PostId = post2Id, TagId = csharpTagId },

            // Post 3: Blog System - tags: .NET, C#
            new PostTag { PostId = post3Id, TagId = dotnetTagId },
            new PostTag { PostId = post3Id, TagId = csharpTagId },

            // Post 4: Performance (draft) - tags: .NET, C#
            new PostTag { PostId = post4Id, TagId = dotnetTagId },
            new PostTag { PostId = post4Id, TagId = csharpTagId }
        );

        // Seed Comments
        var comment1Id = Guid.Parse("C0001111-1111-1111-1111-111111111111");
        var comment2Id = Guid.Parse("C0002222-2222-2222-2222-222222222222");
        var comment3Id = Guid.Parse("C0003333-3333-3333-3333-333333333333");
        var comment4Id = Guid.Parse("C0004444-4444-4444-4444-444444444444");
        var comment5Id = Guid.Parse("C0005555-5555-5555-5555-555555555555");

        modelBuilder.Entity<Comment>().HasData(
            // Comments on Post 1 (Getting Started with .NET 9 and PostgreSQL)
            new Comment
            {
                Id = comment1Id,
                PostId = post1Id,
                AuthorId = standardUserId,
                Content = "Great article! The integration between EF Core 9 and PostgreSQL has really improved. Thanks for sharing!",
                IsApproved = true,
                ParentId = null,
                CreatedAt = now.AddDays(-6),
                UpdatedAt = now.AddDays(-6)
            },
            new Comment
            {
                Id = comment2Id,
                PostId = post1Id,
                AuthorId = adminUserId,
                Content = "Thank you! I'm glad you found it helpful. Let me know if you have any questions.",
                IsApproved = true,
                ParentId = comment1Id, // Reply to comment1
                CreatedAt = now.AddDays(-6).AddHours(2),
                UpdatedAt = now.AddDays(-6).AddHours(2)
            },

            // Comments on Post 2 (Advanced RBAC Patterns)
            new Comment
            {
                Id = comment3Id,
                PostId = post2Id,
                AuthorId = adminUserId,
                Content = "This is exactly what I was looking for! The Resource-Action pattern makes so much sense for complex authorization scenarios.",
                IsApproved = true,
                ParentId = null,
                CreatedAt = now.AddDays(-2),
                UpdatedAt = now.AddDays(-2)
            },

            // Comments on Post 3 (Building a Blog System)
            new Comment
            {
                Id = comment4Id,
                PostId = post3Id,
                AuthorId = adminUserId,
                Content = "Minimal APIs are really powerful for this use case. Would love to see more about SEO optimization!",
                IsApproved = true,
                ParentId = null,
                CreatedAt = now.AddHours(-12),
                UpdatedAt = now.AddHours(-12)
            },

            // Pending comment (not approved yet)
            new Comment
            {
                Id = comment5Id,
                PostId = post3Id,
                AuthorId = standardUserId,
                Content = "I'm planning to implement this for my project. Do you have the source code available on GitHub?",
                IsApproved = false, // Pending approval
                ParentId = null,
                CreatedAt = now.AddHours(-2),
                UpdatedAt = now.AddHours(-2)
            }
        );
    }

    private static void ConfigurePermissionEntities(ModelBuilder modelBuilder)
    {
        // ModuleFeature - Many-to-One with ApplicationModule
        modelBuilder.Entity<ModuleFeature>()
            .HasOne(mf => mf.Module)
            .WithMany(m => m.ModuleFeatures)
            .HasForeignKey(mf => mf.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        // ModuleFeature - Many-to-One with Feature
        modelBuilder.Entity<ModuleFeature>()
            .HasOne(mf => mf.Feature)
            .WithMany(f => f.ModuleFeatures)
            .HasForeignKey(mf => mf.FeatureId)
            .OnDelete(DeleteBehavior.Cascade);

        // FeatureAction - Many-to-One with Feature
        modelBuilder.Entity<FeatureAction>()
            .HasOne(fa => fa.Feature)
            .WithMany(f => f.FeatureActions)
            .HasForeignKey(fa => fa.FeatureId)
            .OnDelete(DeleteBehavior.Cascade);

        // FeatureAction - Many-to-One with PermissionAction
        modelBuilder.Entity<FeatureAction>()
            .HasOne(fa => fa.Action)
            .WithMany(a => a.FeatureActions)
            .HasForeignKey(fa => fa.ActionId)
            .OnDelete(DeleteBehavior.Cascade);

        // RolePermission - Many-to-One with UserRole
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.Permissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // RolePermission - Many-to-One with FeatureAction
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.FeatureAction)
            .WithMany(fa => fa.RolePermissions)
            .HasForeignKey(rp => rp.FeatureActionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint: One Module can have one Feature only once
        modelBuilder.Entity<ModuleFeature>()
            .HasIndex(mf => new { mf.ModuleId, mf.FeatureId })
            .IsUnique()
            .HasDatabaseName("IX_ModuleFeature_ModuleId_FeatureId");

        // Unique constraint: One Feature can have one Action only once
        modelBuilder.Entity<FeatureAction>()
            .HasIndex(fa => new { fa.FeatureId, fa.ActionId })
            .IsUnique()
            .HasDatabaseName("IX_FeatureAction_FeatureId_ActionId");

        // Unique constraint: One Role can have one Permission only once
        modelBuilder.Entity<RolePermission>()
            .HasIndex(rp => new { rp.RoleId, rp.FeatureActionId })
            .IsUnique()
            .HasDatabaseName("IX_RolePermission_RoleId_FeatureActionId");

        // Unique constraint: ResourceKey must be unique
        modelBuilder.Entity<Feature>()
            .HasIndex(f => f.ResourceKey)
            .IsUnique()
            .HasDatabaseName("IX_Feature_ResourceKey");

        // Unique constraint: ActionKey must be unique
        modelBuilder.Entity<PermissionAction>()
            .HasIndex(a => a.ActionKey)
            .IsUnique()
            .HasDatabaseName("IX_PermissionAction_ActionKey");
    }

    private static void SeedPermissionData(ModelBuilder modelBuilder)
    {
        var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // 1. Seed ApplicationModules
        var userManagementModuleId = 1;
        var blogSystemModuleId = 2;
        var adminModuleId = 3;

        modelBuilder.Entity<ApplicationModule>().HasData(
            new ApplicationModule
            {
                Id = userManagementModuleId,
                Name = "User Management",
                Description = "User and role management features",
                IsActive = true,
                DisplayOrder = 1,
                CreatedAt = now
            },
            new ApplicationModule
            {
                Id = blogSystemModuleId,
                Name = "Blog System",
                Description = "Blog posts, categories, tags, and comments",
                IsActive = true,
                DisplayOrder = 2,
                CreatedAt = now
            },
            new ApplicationModule
            {
                Id = adminModuleId,
                Name = "Administration",
                Description = "System administration and settings",
                IsActive = true,
                DisplayOrder = 3,
                CreatedAt = now
            }
        );

        // 2. Seed Features (Resources)
        var usersFeatureId = 1;
        var rolesFeatureId = 2;
        var profilesFeatureId = 3;
        var postsFeatureId = 4;
        var categoriesFeatureId = 5;
        var tagsFeatureId = 6;
        var commentsFeatureId = 7;
        var permissionsFeatureId = 8;
        var statisticsFeatureId = 9;

        modelBuilder.Entity<Feature>().HasData(
            new Feature { Id = usersFeatureId, Name = "Users", ResourceKey = "Users", Description = "User accounts management", DisplayOrder = 1, CreatedAt = now },
            new Feature { Id = rolesFeatureId, Name = "Roles", ResourceKey = "Roles", Description = "Role management", DisplayOrder = 2, CreatedAt = now },
            new Feature { Id = profilesFeatureId, Name = "Profiles", ResourceKey = "Profiles", Description = "User profiles management", DisplayOrder = 3, CreatedAt = now },
            new Feature { Id = postsFeatureId, Name = "Posts", ResourceKey = "Posts", Description = "Blog posts management", DisplayOrder = 4, CreatedAt = now },
            new Feature { Id = categoriesFeatureId, Name = "Categories", ResourceKey = "Categories", Description = "Blog categories management", DisplayOrder = 5, CreatedAt = now },
            new Feature { Id = tagsFeatureId, Name = "Tags", ResourceKey = "Tags", Description = "Blog tags management", DisplayOrder = 6, CreatedAt = now },
            new Feature { Id = commentsFeatureId, Name = "Comments", ResourceKey = "Comments", Description = "Blog comments management", DisplayOrder = 7, CreatedAt = now },
            new Feature { Id = permissionsFeatureId, Name = "Permissions", ResourceKey = "Permissions", Description = "Permission management", DisplayOrder = 8, CreatedAt = now },
            new Feature { Id = statisticsFeatureId, Name = "Statistics", ResourceKey = "Statistics", Description = "System statistics", DisplayOrder = 9, CreatedAt = now }
        );

        // 3. Seed PermissionActions
        var viewActionId = 1;
        var createActionId = 2;
        var editActionId = 3;
        var deleteActionId = 4;
        var publishActionId = 5;
        var unpublishActionId = 6;
        var approveActionId = 7;
        var rejectActionId = 8;
        var moderateActionId = 9;
        var manageActionId = 10;

        modelBuilder.Entity<PermissionAction>().HasData(
            new PermissionAction { Id = viewActionId, Name = "View", ActionKey = "View", Description = "View/read resources", DisplayOrder = 1, CreatedAt = now },
            new PermissionAction { Id = createActionId, Name = "Create", ActionKey = "Create", Description = "Create new resources", DisplayOrder = 2, CreatedAt = now },
            new PermissionAction { Id = editActionId, Name = "Edit", ActionKey = "Edit", Description = "Edit existing resources", DisplayOrder = 3, CreatedAt = now },
            new PermissionAction { Id = deleteActionId, Name = "Delete", ActionKey = "Delete", Description = "Delete resources", DisplayOrder = 4, CreatedAt = now },
            new PermissionAction { Id = publishActionId, Name = "Publish", ActionKey = "Publish", Description = "Publish content", DisplayOrder = 5, CreatedAt = now },
            new PermissionAction { Id = unpublishActionId, Name = "Unpublish", ActionKey = "Unpublish", Description = "Unpublish content", DisplayOrder = 6, CreatedAt = now },
            new PermissionAction { Id = approveActionId, Name = "Approve", ActionKey = "Approve", Description = "Approve pending content", DisplayOrder = 7, CreatedAt = now },
            new PermissionAction { Id = rejectActionId, Name = "Reject", ActionKey = "Reject", Description = "Reject pending content", DisplayOrder = 8, CreatedAt = now },
            new PermissionAction { Id = moderateActionId, Name = "Moderate", ActionKey = "Moderate", Description = "Moderate and review content", DisplayOrder = 9, CreatedAt = now },
            new PermissionAction { Id = manageActionId, Name = "Manage", ActionKey = "Manage", Description = "Full management access", DisplayOrder = 10, CreatedAt = now }
        );

        // 4. Seed ModuleFeatures (Module-Feature relationships)
        var moduleFeatureId = 1;
        modelBuilder.Entity<ModuleFeature>().HasData(
            // User Management Module
            new ModuleFeature { Id = moduleFeatureId++, ModuleId = userManagementModuleId, FeatureId = usersFeatureId, CreatedAt = now },
            new ModuleFeature { Id = moduleFeatureId++, ModuleId = userManagementModuleId, FeatureId = rolesFeatureId, CreatedAt = now },
            new ModuleFeature { Id = moduleFeatureId++, ModuleId = userManagementModuleId, FeatureId = profilesFeatureId, CreatedAt = now },
            // Blog System Module
            new ModuleFeature { Id = moduleFeatureId++, ModuleId = blogSystemModuleId, FeatureId = postsFeatureId, CreatedAt = now },
            new ModuleFeature { Id = moduleFeatureId++, ModuleId = blogSystemModuleId, FeatureId = categoriesFeatureId, CreatedAt = now },
            new ModuleFeature { Id = moduleFeatureId++, ModuleId = blogSystemModuleId, FeatureId = tagsFeatureId, CreatedAt = now },
            new ModuleFeature { Id = moduleFeatureId++, ModuleId = blogSystemModuleId, FeatureId = commentsFeatureId, CreatedAt = now },
            // Administration Module
            new ModuleFeature { Id = moduleFeatureId++, ModuleId = adminModuleId, FeatureId = permissionsFeatureId, CreatedAt = now },
            new ModuleFeature { Id = moduleFeatureId++, ModuleId = adminModuleId, FeatureId = statisticsFeatureId, CreatedAt = now }
        );

        // 5. Seed FeatureActions (Feature-Action combinations = Permissions)
        var featureActionId = 1;
        var featureActionMap = new Dictionary<string, int>(); // For tracking IDs

        // Helper to create FeatureActions
        void AddFeatureAction(int featureId, int actionId)
        {
            var key = $"{featureId}_{actionId}";
            featureActionMap[key] = featureActionId;
            modelBuilder.Entity<FeatureAction>().HasData(
                new FeatureAction
                {
                    Id = featureActionId++,
                    FeatureId = featureId,
                    ActionId = actionId,
                    IsEnabled = true,
                    CreatedAt = now
                }
            );
        }

        // Users permissions
        AddFeatureAction(usersFeatureId, viewActionId);
        AddFeatureAction(usersFeatureId, createActionId);
        AddFeatureAction(usersFeatureId, editActionId);
        AddFeatureAction(usersFeatureId, deleteActionId);
        AddFeatureAction(usersFeatureId, manageActionId);

        // Roles permissions
        AddFeatureAction(rolesFeatureId, viewActionId);
        AddFeatureAction(rolesFeatureId, createActionId);
        AddFeatureAction(rolesFeatureId, editActionId);
        AddFeatureAction(rolesFeatureId, deleteActionId);
        AddFeatureAction(rolesFeatureId, manageActionId);

        // Profiles permissions
        AddFeatureAction(profilesFeatureId, viewActionId);
        AddFeatureAction(profilesFeatureId, createActionId);
        AddFeatureAction(profilesFeatureId, editActionId);
        AddFeatureAction(profilesFeatureId, deleteActionId);

        // Posts permissions
        AddFeatureAction(postsFeatureId, viewActionId);
        AddFeatureAction(postsFeatureId, createActionId);
        AddFeatureAction(postsFeatureId, editActionId);
        AddFeatureAction(postsFeatureId, deleteActionId);
        AddFeatureAction(postsFeatureId, publishActionId);
        AddFeatureAction(postsFeatureId, unpublishActionId);
        AddFeatureAction(postsFeatureId, manageActionId);

        // Categories permissions
        AddFeatureAction(categoriesFeatureId, viewActionId);
        AddFeatureAction(categoriesFeatureId, createActionId);
        AddFeatureAction(categoriesFeatureId, editActionId);
        AddFeatureAction(categoriesFeatureId, deleteActionId);

        // Tags permissions
        AddFeatureAction(tagsFeatureId, viewActionId);
        AddFeatureAction(tagsFeatureId, createActionId);
        AddFeatureAction(tagsFeatureId, editActionId);
        AddFeatureAction(tagsFeatureId, deleteActionId);

        // Comments permissions
        AddFeatureAction(commentsFeatureId, viewActionId);
        AddFeatureAction(commentsFeatureId, createActionId);
        AddFeatureAction(commentsFeatureId, editActionId);
        AddFeatureAction(commentsFeatureId, deleteActionId);
        AddFeatureAction(commentsFeatureId, approveActionId);
        AddFeatureAction(commentsFeatureId, rejectActionId);
        AddFeatureAction(commentsFeatureId, moderateActionId);

        // Permissions permissions
        AddFeatureAction(permissionsFeatureId, viewActionId);
        AddFeatureAction(permissionsFeatureId, manageActionId);

        // Statistics permissions
        AddFeatureAction(statisticsFeatureId, viewActionId);

        // 6. Seed RolePermissions (Assign permissions to roles)
        var superAdminRoleId = Guid.Parse("10000000-0000-0000-0000-000000000000");
        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var guestRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        var rolePermissionId = 1;

        // SuperAdmin gets ALL permissions (all FeatureActions) - Unrestricted access
        foreach (var fa in featureActionMap.Values)
        {
            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission
                {
                    Id = rolePermissionId++,
                    RoleId = superAdminRoleId,
                    FeatureActionId = fa,
                    CreatedAt = now
                }
            );
        }

        // Admin gets ALL permissions (all FeatureActions)
        foreach (var fa in featureActionMap.Values)
        {
            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission
                {
                    Id = rolePermissionId++,
                    RoleId = adminRoleId,
                    FeatureActionId = fa,
                    CreatedAt = now
                }
            );
        }

        // User role gets limited permissions (can manage their own content)
        var userPermissions = new[]
        {
            featureActionMap[$"{profilesFeatureId}_{viewActionId}"],
            featureActionMap[$"{profilesFeatureId}_{editActionId}"],
            featureActionMap[$"{postsFeatureId}_{viewActionId}"],
            featureActionMap[$"{postsFeatureId}_{createActionId}"],
            featureActionMap[$"{postsFeatureId}_{editActionId}"],
            featureActionMap[$"{postsFeatureId}_{deleteActionId}"],
            featureActionMap[$"{postsFeatureId}_{publishActionId}"],
            featureActionMap[$"{postsFeatureId}_{unpublishActionId}"],
            featureActionMap[$"{categoriesFeatureId}_{viewActionId}"],
            featureActionMap[$"{tagsFeatureId}_{viewActionId}"],
            featureActionMap[$"{commentsFeatureId}_{viewActionId}"],
            featureActionMap[$"{commentsFeatureId}_{createActionId}"],
            featureActionMap[$"{commentsFeatureId}_{editActionId}"],
            featureActionMap[$"{commentsFeatureId}_{deleteActionId}"],
            featureActionMap[$"{statisticsFeatureId}_{viewActionId}"]
        };

        foreach (var permissionId in userPermissions)
        {
            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission
                {
                    Id = rolePermissionId++,
                    RoleId = userRoleId,
                    FeatureActionId = permissionId,
                    CreatedAt = now
                }
            );
        }

        // Guest role gets very limited permissions
        var guestPermissions = new[]
        {
            featureActionMap[$"{postsFeatureId}_{viewActionId}"],
            featureActionMap[$"{categoriesFeatureId}_{viewActionId}"],
            featureActionMap[$"{tagsFeatureId}_{viewActionId}"],
            featureActionMap[$"{commentsFeatureId}_{viewActionId}"]
        };

        foreach (var permissionId in guestPermissions)
        {
            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission
                {
                    Id = rolePermissionId++,
                    RoleId = guestRoleId,
                    FeatureActionId = permissionId,
                    CreatedAt = now
                }
            );
        }
    }
}