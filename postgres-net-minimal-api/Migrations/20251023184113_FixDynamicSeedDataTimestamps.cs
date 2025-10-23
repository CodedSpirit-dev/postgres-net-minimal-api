using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace postgres_net_minimal_api.Migrations
{
    /// <inheritdoc />
    public partial class FixDynamicSeedDataTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "ApplicationModules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationModules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ResourceKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ActionKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Bio = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AvatarUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TwitterHandle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    GitHubHandle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LinkedInHandle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Slug = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: false),
                    Excerpt = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FeaturedImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MetaTitle = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    MetaDescription = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    CommentCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Posts_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModuleFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    FeatureId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleFeatures_ApplicationModules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "ApplicationModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModuleFeatures_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeatureActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FeatureId = table.Column<int>(type: "integer", nullable: false),
                    ActionId = table.Column<int>(type: "integer", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeatureActions_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeatureActions_PermissionActions_ActionId",
                        column: x => x.ActionId,
                        principalTable: "PermissionActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostTags",
                columns: table => new
                {
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostTags", x => new { x.PostId, x.TagId });
                    table.ForeignKey(
                        name: "FK_PostTags_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    FeatureActionId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_FeatureActions_FeatureActionId",
                        column: x => x.FeatureActionId,
                        principalTable: "FeatureActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_UserRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ApplicationModules",
                columns: new[] { "Id", "CreatedAt", "Description", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "User and role management features", 1, true, "User Management" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Blog posts, categories, tags, and comments", 2, true, "Blog System" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System administration and settings", 3, true, "Administration" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c1111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Posts about technology and programming", "Technology", "technology", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c2222222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Step-by-step tutorials and guides", "Tutorials", "tutorials", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "CreatedAt", "Description", "DisplayOrder", "Name", "ResourceKey" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "User accounts management", 1, "Users", "Users" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Role management", 2, "Roles", "Roles" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "User profiles management", 3, "Profiles", "Profiles" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Blog posts management", 4, "Posts", "Posts" },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Blog categories management", 5, "Categories", "Categories" },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Blog tags management", 6, "Tags", "Tags" },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Blog comments management", 7, "Comments", "Comments" },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permission management", 8, "Permissions", "Permissions" },
                    { 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System statistics", 9, "Statistics", "Statistics" }
                });

            migrationBuilder.InsertData(
                table: "PermissionActions",
                columns: new[] { "Id", "ActionKey", "CreatedAt", "Description", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, "View", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "View/read resources", 1, "View" },
                    { 2, "Create", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Create new resources", 2, "Create" },
                    { 3, "Edit", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Edit existing resources", 3, "Edit" },
                    { 4, "Delete", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Delete resources", 4, "Delete" },
                    { 5, "Publish", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Publish content", 5, "Publish" },
                    { 6, "Unpublish", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Unpublish content", 6, "Unpublish" },
                    { 7, "Approve", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Approve pending content", 7, "Approve" },
                    { 8, "Reject", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Reject pending content", 8, "Reject" },
                    { 9, "Moderate", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Moderate and review content", 9, "Moderate" },
                    { 10, "Manage", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Full management access", 10, "Manage" }
                });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "AvatarUrl", "Bio", "CreatedAt", "GitHubHandle", "LinkedInHandle", "TwitterHandle", "UpdatedAt", "UserId", "WebsiteUrl" },
                values: new object[,]
                {
                    { new Guid("91111111-1111-1111-1111-111111111111"), null, "System administrator and technical writer. Passionate about .NET technologies and modern software architecture.", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin-developer", null, "@admin_dev", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("a1111111-1111-1111-1111-111111111111"), "https://example.com/admin" },
                    { new Guid("92222222-2222-2222-2222-222222222222"), null, "Software developer specializing in C# and PostgreSQL. Love building scalable web applications.", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "johndoe", "https://linkedin.com/in/johndoe", "@johndoe_dev", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("b2222222-2222-2222-2222-222222222222"), "https://johndoe.dev" }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "CreatedAt", "Name", "Slug" },
                values: new object[,]
                {
                    { new Guid("71111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "C#", "csharp" },
                    { new Guid("72222222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), ".NET", "dotnet" },
                    { new Guid("73333333-3333-3333-3333-333333333333"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PostgreSQL", "postgresql" }
                });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "Description",
                value: "System administrator");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "Description",
                value: "Standard user");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "Description",
                value: "Guest user with limited access");

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { new Guid("10000000-0000-0000-0000-000000000000"), "Super administrator with unrestricted access to everything", "SuperAdmin" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111111"),
                columns: new[] { "Email", "HashedPassword", "UserName" },
                values: new object[] { "admin@admin.com", "$2a$11$xKJAr8qR9Z5VyN3J9YZ.qOh4L0U8XZ0F8Y0W2N6Q5M3F6Y7Z8Q9Kf", "admin" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b2222222-2222-2222-2222-222222222222"),
                columns: new[] { "Email", "HashedPassword", "UserName" },
                values: new object[] { "user@example.com", "$2a$11$xKJAr8qR9Z5VyN3J9YZ.qOh4L0U8XZ0F8Y0W2N6Q5M3F6Y7Z8Q9Kg", "user" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfBirth", "Email", "FirstName", "HashedPassword", "LastName", "MiddleName", "MotherMaidenName", "RoleId", "UserName" },
                values: new object[] { new Guid("c3333333-3333-3333-3333-333333333333"), new DateOnly(2000, 1, 1), "guest@example.com", "Guest", "$2a$11$xKJAr8qR9Z5VyN3J9YZ.qOh4L0U8XZ0F8Y0W2N6Q5M3F6Y7Z8Q9Kh", "User", null, null, new Guid("33333333-3333-3333-3333-333333333333"), "guest" });

            migrationBuilder.InsertData(
                table: "FeatureActions",
                columns: new[] { "Id", "ActionId", "CreatedAt", "FeatureId", "IsEnabled" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true },
                    { 2, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true },
                    { 3, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true },
                    { 4, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true },
                    { 5, 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true },
                    { 6, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true },
                    { 7, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true },
                    { 8, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true },
                    { 9, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true },
                    { 10, 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true },
                    { 11, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true },
                    { 12, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true },
                    { 13, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true },
                    { 14, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true },
                    { 15, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true },
                    { 16, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true },
                    { 17, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true },
                    { 18, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true },
                    { 19, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true },
                    { 20, 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true },
                    { 21, 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true },
                    { 22, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, true },
                    { 23, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, true },
                    { 24, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, true },
                    { 25, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, true },
                    { 26, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, true },
                    { 27, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, true },
                    { 28, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, true },
                    { 29, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, true },
                    { 30, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, true },
                    { 31, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, true },
                    { 32, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, true },
                    { 33, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, true },
                    { 34, 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, true },
                    { 35, 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, true },
                    { 36, 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, true },
                    { 37, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, true },
                    { 38, 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, true },
                    { 39, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, true }
                });

            migrationBuilder.InsertData(
                table: "ModuleFeatures",
                columns: new[] { "Id", "CreatedAt", "FeatureId", "ModuleId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1 },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 1 },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 1 },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 2 },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 2 },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 2 },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, 2 },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 3 },
                    { 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, 3 }
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "AuthorId", "CategoryId", "CommentCount", "Content", "CreatedAt", "Excerpt", "FeaturedImageUrl", "IsPublished", "MetaDescription", "MetaTitle", "PublishedAt", "Slug", "Title", "UpdatedAt", "ViewCount" },
                values: new object[,]
                {
                    { new Guid("10571111-1111-1111-1111-111111111111"), new Guid("a1111111-1111-1111-1111-111111111111"), new Guid("c1111111-1111-1111-1111-111111111111"), 0, "<p>In this tutorial, we'll explore how to build a modern web API using .NET 9 and PostgreSQL. We'll cover Entity Framework Core 9, minimal APIs, and best practices for database design.</p><p>PostgreSQL is a powerful, open-source relational database that pairs perfectly with .NET for building scalable applications.</p>", new DateTime(2024, 12, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Learn how to build modern web APIs using .NET 9 and PostgreSQL with this comprehensive guide.", "https://images.example.com/net9-postgres.jpg", true, null, null, new DateTime(2024, 12, 25, 0, 0, 0, 0, DateTimeKind.Utc), "getting-started-with-net9-and-postgresql", "Getting Started with .NET 9 and PostgreSQL", new DateTime(2024, 12, 25, 0, 0, 0, 0, DateTimeKind.Utc), 1250 },
                    { new Guid("10572222-2222-2222-2222-222222222222"), new Guid("b2222222-2222-2222-2222-222222222222"), new Guid("c2222222-2222-2222-2222-222222222222"), 0, "<p>Role-Based Access Control (RBAC) is essential for securing modern applications. In this post, we dive deep into implementing granular permissions using the Resource-Action pattern.</p><p>We'll cover instance-level permissions, ownership validation, and how to combine type-level and instance-level authorization.</p>", new DateTime(2024, 12, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Implementing fine-grained role-based access control with resource-action permissions in modern .NET applications.", "https://images.example.com/rbac-pattern.jpg", true, null, null, new DateTime(2024, 12, 29, 0, 0, 0, 0, DateTimeKind.Utc), "advanced-rbac-patterns-aspnet-core", "Advanced RBAC Patterns in ASP.NET Core", new DateTime(2024, 12, 29, 0, 0, 0, 0, DateTimeKind.Utc), 867 },
                    { new Guid("10573333-3333-3333-3333-333333333333"), new Guid("b2222222-2222-2222-2222-222222222222"), new Guid("c2222222-2222-2222-2222-222222222222"), 0, "<p>Minimal APIs in .NET provide a streamlined way to build HTTP APIs with minimal ceremony. In this tutorial, we'll build a complete blog system from scratch.</p><p>Topics covered: Posts, Categories, Tags, Comments, SEO-friendly slugs, and more!</p>", new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), "Step-by-step guide to creating a complete blog system using .NET 9 Minimal APIs.", "https://images.example.com/minimal-api-blog.jpg", true, null, null, new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), "building-blog-system-minimal-apis", "Building a Blog System with Minimal APIs", new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), 423 },
                    { new Guid("10574444-4444-4444-4444-444444444444"), new Guid("a1111111-1111-1111-1111-111111111111"), new Guid("c1111111-1111-1111-1111-111111111111"), 0, "<p>This is a draft post exploring performance optimization techniques including caching, database query optimization, and async patterns.</p>", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exploring various performance optimization strategies for .NET applications (Work in Progress).", null, false, null, null, null, "draft-performance-optimization-techniques", "Draft: Performance Optimization Techniques", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0 }
                });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "AvatarUrl", "Bio", "CreatedAt", "GitHubHandle", "LinkedInHandle", "TwitterHandle", "UpdatedAt", "UserId", "WebsiteUrl" },
                values: new object[] { new Guid("93333333-3333-3333-3333-333333333333"), null, "Guest user exploring the platform features.", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("c3333333-3333-3333-3333-333333333333"), null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfBirth", "Email", "FirstName", "HashedPassword", "LastName", "MiddleName", "MotherMaidenName", "RoleId", "UserName" },
                values: new object[] { new Guid("50000000-0000-0000-0000-000000000000"), new DateOnly(1985, 1, 1), "superadmin@admin.com", "Super", "$2a$11$xKJAr8qR9Z5VyN3J9YZ.qOh4L0U8XZ0F8Y0W2N6Q5M3F6Y7Z8Q9Ke", "Administrator", null, null, new Guid("10000000-0000-0000-0000-000000000000"), "superadmin" });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "AuthorId", "Content", "CreatedAt", "IsApproved", "ParentId", "PostId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c0001111-1111-1111-1111-111111111111"), new Guid("b2222222-2222-2222-2222-222222222222"), "Great article! The integration between EF Core 9 and PostgreSQL has really improved. Thanks for sharing!", new DateTime(2024, 12, 26, 0, 0, 0, 0, DateTimeKind.Utc), true, null, new Guid("10571111-1111-1111-1111-111111111111"), new DateTime(2024, 12, 26, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c0003333-3333-3333-3333-333333333333"), new Guid("a1111111-1111-1111-1111-111111111111"), "This is exactly what I was looking for! The Resource-Action pattern makes so much sense for complex authorization scenarios.", new DateTime(2024, 12, 30, 0, 0, 0, 0, DateTimeKind.Utc), true, null, new Guid("10572222-2222-2222-2222-222222222222"), new DateTime(2024, 12, 30, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c0004444-4444-4444-4444-444444444444"), new Guid("a1111111-1111-1111-1111-111111111111"), "Minimal APIs are really powerful for this use case. Would love to see more about SEO optimization!", new DateTime(2024, 12, 31, 12, 0, 0, 0, DateTimeKind.Utc), true, null, new Guid("10573333-3333-3333-3333-333333333333"), new DateTime(2024, 12, 31, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c0005555-5555-5555-5555-555555555555"), new Guid("b2222222-2222-2222-2222-222222222222"), "I'm planning to implement this for my project. Do you have the source code available on GitHub?", new DateTime(2024, 12, 31, 22, 0, 0, 0, DateTimeKind.Utc), false, null, new Guid("10573333-3333-3333-3333-333333333333"), new DateTime(2024, 12, 31, 22, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "PostTags",
                columns: new[] { "PostId", "TagId", "CreatedAt" },
                values: new object[,]
                {
                    { new Guid("10571111-1111-1111-1111-111111111111"), new Guid("72222222-2222-2222-2222-222222222222"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("10571111-1111-1111-1111-111111111111"), new Guid("73333333-3333-3333-3333-333333333333"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("10572222-2222-2222-2222-222222222222"), new Guid("71111111-1111-1111-1111-111111111111"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("10572222-2222-2222-2222-222222222222"), new Guid("72222222-2222-2222-2222-222222222222"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("10573333-3333-3333-3333-333333333333"), new Guid("71111111-1111-1111-1111-111111111111"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("10573333-3333-3333-3333-333333333333"), new Guid("72222222-2222-2222-2222-222222222222"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("10574444-4444-4444-4444-444444444444"), new Guid("71111111-1111-1111-1111-111111111111"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("10574444-4444-4444-4444-444444444444"), new Guid("72222222-2222-2222-2222-222222222222"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "AvatarUrl", "Bio", "CreatedAt", "GitHubHandle", "LinkedInHandle", "TwitterHandle", "UpdatedAt", "UserId", "WebsiteUrl" },
                values: new object[] { new Guid("90000000-0000-0000-0000-000000000000"), null, "Super Administrator with unrestricted access to all system features. Responsible for platform management, user moderation, and system-wide configurations.", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "superadmin", null, "@platform_admin", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("50000000-0000-0000-0000-000000000000"), "https://platform.example.com" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "FeatureActionId", "RoleId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 11, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 12, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 13, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 13, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 14, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 14, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 15, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 15, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 16, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 16, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 17, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 18, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 18, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 19, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 19, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 20, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 20, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 21, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 21, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 22, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 22, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 23, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 23, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 24, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 24, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 25, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 25, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 26, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 26, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 27, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 27, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 28, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 28, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 29, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 29, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 30, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 30, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 31, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 31, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 32, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 32, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 33, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 33, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 34, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 34, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 35, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 35, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 36, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 36, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 37, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 37, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 38, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 38, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 39, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 39, new Guid("10000000-0000-0000-0000-000000000000") },
                    { 40, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 41, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 42, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 43, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 44, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 45, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 46, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 47, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 48, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 49, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 50, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 11, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 51, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 12, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 52, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 13, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 53, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 14, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 54, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 15, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 55, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 16, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 56, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 57, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 18, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 58, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 19, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 59, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 20, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 60, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 21, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 61, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 22, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 62, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 23, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 63, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 24, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 64, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 25, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 65, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 26, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 66, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 27, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 67, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 28, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 68, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 29, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 69, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 30, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 70, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 31, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 71, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 32, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 72, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 33, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 73, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 34, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 74, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 35, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 75, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 36, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 76, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 37, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 77, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 38, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 78, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 39, new Guid("11111111-1111-1111-1111-111111111111") },
                    { 79, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 11, new Guid("22222222-2222-2222-2222-222222222222") },
                    { 80, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 13, new Guid("22222222-2222-2222-2222-222222222222") },
                    { 81, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 15, new Guid("22222222-2222-2222-2222-222222222222") },
                    { 82, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 16, new Guid("22222222-2222-2222-2222-222222222222") },
                    { 83, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, new Guid("22222222-2222-2222-2222-222222222222") },
                    { 84, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 18, new Guid("22222222-2222-2222-2222-222222222222") },
                    { 85, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 19, new Guid("22222222-2222-2222-2222-222222222222") },
                    { 86, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 20, new Guid("22222222-2222-2222-2222-222222222222") },
                    { 87, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 22, new Guid("22222222-2222-2222-2222-222222222222") },
                    { 88, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 26, new Guid("22222222-2222-2222-2222-222222222222") },
                    { 89, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 30, new Guid("22222222-2222-2222-2222-222222222222") },
                    { 90, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 31, new Guid("22222222-2222-2222-2222-222222222222") },
                    { 91, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 32, new Guid("22222222-2222-2222-2222-222222222222") },
                    { 92, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 33, new Guid("22222222-2222-2222-2222-222222222222") },
                    { 93, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 39, new Guid("22222222-2222-2222-2222-222222222222") },
                    { 94, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 15, new Guid("33333333-3333-3333-3333-333333333333") },
                    { 95, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 22, new Guid("33333333-3333-3333-3333-333333333333") },
                    { 96, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 26, new Guid("33333333-3333-3333-3333-333333333333") },
                    { 97, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 30, new Guid("33333333-3333-3333-3333-333333333333") }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "AuthorId", "Content", "CreatedAt", "IsApproved", "ParentId", "PostId", "UpdatedAt" },
                values: new object[] { new Guid("c0002222-2222-2222-2222-222222222222"), new Guid("a1111111-1111-1111-1111-111111111111"), "Thank you! I'm glad you found it helpful. Let me know if you have any questions.", new DateTime(2024, 12, 26, 2, 0, 0, 0, DateTimeKind.Utc), true, new Guid("c0001111-1111-1111-1111-111111111111"), new Guid("10571111-1111-1111-1111-111111111111"), new DateTime(2024, 12, 26, 2, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId_LastName",
                table: "Users",
                columns: new[] { "RoleId", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Slug",
                table: "Categories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_IsApproved",
                table: "Comments",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentId",
                table: "Comments",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId_IsApproved",
                table: "Comments",
                columns: new[] { "PostId", "IsApproved" });

            migrationBuilder.CreateIndex(
                name: "IX_FeatureAction_FeatureId_ActionId",
                table: "FeatureActions",
                columns: new[] { "FeatureId", "ActionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeatureActions_ActionId",
                table: "FeatureActions",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_Feature_ResourceKey",
                table: "Features",
                column: "ResourceKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModuleFeature_ModuleId_FeatureId",
                table: "ModuleFeatures",
                columns: new[] { "ModuleId", "FeatureId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModuleFeatures_FeatureId",
                table: "ModuleFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionAction_ActionKey",
                table: "PermissionActions",
                column: "ActionKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CategoryId",
                table: "Posts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_IsPublished",
                table: "Posts",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Published_PublishedAt",
                table: "Posts",
                columns: new[] { "IsPublished", "PublishedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_PublishedAt",
                table: "Posts",
                column: "PublishedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Slug",
                table: "Posts",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostTags_TagId",
                table: "PostTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_UserId",
                table: "Profiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleId_FeatureActionId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "FeatureActionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_FeatureActionId",
                table: "RolePermissions",
                column: "FeatureActionId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Slug",
                table: "Tags",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "ModuleFeatures");

            migrationBuilder.DropTable(
                name: "PostTags");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "ApplicationModules");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "FeatureActions");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropTable(
                name: "PermissionActions");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId_LastName",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c3333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "Description",
                value: "Administrador del sistema");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "Description",
                value: "Usuario estándar");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "Description",
                value: "Invitado, acceso limitado");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111111"),
                columns: new[] { "Email", "HashedPassword", "UserName" },
                values: new object[] { "admin@example.com", "$2a$11$67rVUGbvftj.YcqUIiQGSeg47kWVtGJXZR8ZbESPh7VG5glAAtDqe", "Admin" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b2222222-2222-2222-2222-222222222222"),
                columns: new[] { "Email", "HashedPassword", "UserName" },
                values: new object[] { "john.doe@example.com", "yo$2a$11$5ByfQRylb6t1ucfmqMASC.OGbS4Qp7sPq4Dpc1YC24oiG6usM26PK", "User" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }
    }
}
