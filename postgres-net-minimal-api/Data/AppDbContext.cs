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
        var now = DateTime.UtcNow;

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
        var approveActionId = 5;
        var manageActionId = 6;

        modelBuilder.Entity<PermissionAction>().HasData(
            new PermissionAction { Id = viewActionId, Name = "View", ActionKey = "View", Description = "View/read resources", DisplayOrder = 1, CreatedAt = now },
            new PermissionAction { Id = createActionId, Name = "Create", ActionKey = "Create", Description = "Create new resources", DisplayOrder = 2, CreatedAt = now },
            new PermissionAction { Id = editActionId, Name = "Edit", ActionKey = "Edit", Description = "Edit existing resources", DisplayOrder = 3, CreatedAt = now },
            new PermissionAction { Id = deleteActionId, Name = "Delete", ActionKey = "Delete", Description = "Delete resources", DisplayOrder = 4, CreatedAt = now },
            new PermissionAction { Id = approveActionId, Name = "Approve", ActionKey = "Approve", Description = "Approve/reject resources", DisplayOrder = 5, CreatedAt = now },
            new PermissionAction { Id = manageActionId, Name = "Manage", ActionKey = "Manage", Description = "Full management access", DisplayOrder = 6, CreatedAt = now }
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

        // Permissions permissions
        AddFeatureAction(permissionsFeatureId, viewActionId);
        AddFeatureAction(permissionsFeatureId, manageActionId);

        // Statistics permissions
        AddFeatureAction(statisticsFeatureId, viewActionId);

        // 6. Seed RolePermissions (Assign permissions to Admin role)
        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var guestRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        var rolePermissionId = 1;

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

        // User role gets limited permissions
        var userPermissions = new[]
        {
            featureActionMap[$"{profilesFeatureId}_{viewActionId}"],
            featureActionMap[$"{profilesFeatureId}_{editActionId}"],
            featureActionMap[$"{postsFeatureId}_{viewActionId}"],
            featureActionMap[$"{postsFeatureId}_{createActionId}"],
            featureActionMap[$"{postsFeatureId}_{editActionId}"],
            featureActionMap[$"{categoriesFeatureId}_{viewActionId}"],
            featureActionMap[$"{tagsFeatureId}_{viewActionId}"],
            featureActionMap[$"{commentsFeatureId}_{viewActionId}"],
            featureActionMap[$"{commentsFeatureId}_{createActionId}"],
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