# Blog System Documentation

## Overview

This is a complete, production-ready blog system built with .NET 9, C# 13+, and PostgreSQL. The system includes all features needed for a modern blog platform with focus on performance, security, and SEO.

---

## 🎯 Features

### Core Functionality
- ✅ **Post Management** - Create, read, update, delete blog posts with rich content
- ✅ **Categories** - Organize posts by categories
- ✅ **Tags** - Multiple tags per post for better organization
- ✅ **Comments** - Nested comments with moderation system
- ✅ **User Profiles** - Extended user profiles with social links
- ✅ **Search & Filtering** - Advanced search with multiple filters
- ✅ **SEO Optimization** - Auto-generated slugs, meta tags, excerpts
- ✅ **Statistics** - Comprehensive blog analytics

### Security Features
- ✅ **HTML Sanitization** - Prevents XSS attacks
- ✅ **Comment Moderation** - All comments require approval
- ✅ **Role-Based Access** - Admin/User roles for content management
- ✅ **Input Validation** - Comprehensive DTO validation

### Performance Features
- ✅ **Pagination** - All list endpoints support pagination
- ✅ **Async/Await** - All operations are asynchronous
- ✅ **No Tracking** - Read queries use AsNoTracking for performance
- ✅ **Composite Indexes** - Optimized database indexes
- ✅ **Eager Loading** - Efficient data loading with Include()

---

## 📊 Database Schema

### Entities

#### **Post**
- Id (Guid - PK)
- Title, Content, Slug
- Excerpt, FeaturedImageUrl
- MetaTitle, MetaDescription (SEO)
- AuthorId (FK to User)
- CategoryId (FK to Category)
- IsPublished, PublishedAt
- ViewCount, CommentCount
- CreatedAt, UpdatedAt

#### **Category**
- Id (Guid - PK)
- Name, Slug, Description
- CreatedAt, UpdatedAt

#### **Tag**
- Id (Guid - PK)
- Name, Slug
- CreatedAt

#### **PostTag** (Many-to-Many)
- PostId (FK to Post)
- TagId (FK to Tag)
- CreatedAt

#### **Comment**
- Id (Guid - PK)
- PostId (FK to Post)
- AuthorId (FK to User)
- ParentId (FK to Comment - for nested comments)
- Content
- IsApproved
- CreatedAt, UpdatedAt

#### **Profile**
- Id (Guid - PK)
- UserId (FK to User - One-to-One)
- Bio, AvatarUrl, WebsiteUrl
- TwitterHandle, GitHubHandle, LinkedInHandle
- CreatedAt, UpdatedAt

### Relationships
- **User** 1:N **Post** (Author)
- **Category** 1:N **Post**
- **Post** N:N **Tag** (through PostTag)
- **Post** 1:N **Comment**
- **User** 1:N **Comment** (Author)
- **Comment** 1:N **Comment** (Nested replies)
- **User** 1:1 **Profile**

---

## 🛠️ API Endpoints

### Posts (`/api/posts`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/posts` | Public | Get all posts (paginated) |
| POST | `/api/posts/search` | Public | Advanced search with filters |
| GET | `/api/posts/{id}` | Public | Get post by ID |
| GET | `/api/posts/slug/{slug}` | Public | Get post by slug |
| POST | `/api/posts` | Required | Create new post |
| PUT | `/api/posts/{id}` | Required | Update post |
| DELETE | `/api/posts/{id}` | Admin | Delete post |
| GET | `/api/posts/popular/most-viewed` | Public | Get most viewed posts |
| GET | `/api/posts/popular/most-commented` | Public | Get most commented posts |

### Categories (`/api/categories`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/categories` | Public | Get all categories |
| GET | `/api/categories/{id}` | Public | Get category by ID |
| GET | `/api/categories/slug/{slug}` | Public | Get category by slug |
| POST | `/api/categories` | Admin | Create category |
| PUT | `/api/categories/{id}` | Admin | Update category |
| DELETE | `/api/categories/{id}` | Admin | Delete category |

### Tags (`/api/tags`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/tags` | Public | Get all tags |
| GET | `/api/tags/{id}` | Public | Get tag by ID |
| GET | `/api/tags/slug/{slug}` | Public | Get tag by slug |
| POST | `/api/tags` | Admin | Create tag |
| DELETE | `/api/tags/{id}` | Admin | Delete tag |

### Comments (`/api/comments`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/comments/post/{postId}` | Public | Get comments by post |
| GET | `/api/comments/{id}` | Public | Get comment by ID |
| GET | `/api/comments/pending` | Admin | Get pending comments |
| POST | `/api/comments` | Required | Create comment |
| PUT | `/api/comments/{id}` | Required | Update comment |
| DELETE | `/api/comments/{id}` | Admin | Delete comment |
| POST | `/api/comments/{id}/approve` | Admin | Approve comment |
| POST | `/api/comments/{id}/reject` | Admin | Reject comment |

### Profiles (`/api/profiles`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/profiles/user/{userId}` | Public | Get profile by user ID |
| GET | `/api/profiles/me` | Required | Get current user's profile |
| POST | `/api/profiles` | Required | Create/update profile |
| DELETE | `/api/profiles` | Required | Delete profile |

### Statistics (`/api/statistics`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/statistics` | Public | Get comprehensive blog statistics |

---

## 🔧 Utilities & Helpers

### SlugHelper
Generates SEO-friendly URL slugs from titles:
```csharp
var slug = SlugHelper.GenerateSlug("My Awesome Blog Post");
// Result: "my-awesome-blog-post"

var uniqueSlug = await SlugHelper.GenerateUniqueSlugAsync(
    baseSlug,
    checkExistsFunction,
    excludeId);
// Ensures uniqueness by appending numbers if needed
```

### HtmlSanitizer
Sanitizes HTML content to prevent XSS attacks:
```csharp
var sanitized = HtmlSanitizer.Sanitize(userInput);
// Removes dangerous tags: <script>, <iframe>, event handlers, etc.

var plainText = HtmlSanitizer.StripHtml(html, maxLength: 160);
// Extracts plain text from HTML

var excerpt = HtmlSanitizer.GenerateExcerpt(html, 160);
// Generates excerpt for preview
```

### SearchNormalizer
Normalizes search queries for better matching:
```csharp
var normalized = SearchNormalizer.Normalize("Búsqueda con áçcentos");
// Result: "busqueda con accentos" (lowercase, no accents)

var terms = SearchNormalizer.GetSearchTerms(query);
// Splits into individual search terms

var tsquery = SearchNormalizer.ToPostgresFullTextQuery(query);
// Generates PostgreSQL full-text search query
```

---

## 📝 Advanced Search

The search endpoint (`POST /api/posts/search`) supports multiple filters:

```json
{
  "query": "dotnet core",
  "categoryId": "uuid-here",
  "tagIds": ["uuid1", "uuid2"],
  "authorId": "uuid-here",
  "isPublished": true,
  "publishedAfter": "2024-01-01",
  "publishedBefore": "2024-12-31",
  "orderBy": "publishedAt",
  "descending": true,
  "page": 1,
  "pageSize": 20
}
```

**Order By Options:**
- `publishedAt` (default)
- `title`
- `viewCount`
- `commentCount`

---

## 🎨 Usage Examples

### Creating a Blog Post

```bash
POST /api/posts
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Getting Started with .NET 9",
  "content": "<p>This is my blog post content...</p>",
  "excerpt": "A guide to .NET 9",
  "featuredImageUrl": "https://example.com/image.jpg",
  "categoryId": "uuid-here",
  "tagIds": ["uuid1", "uuid2"],
  "metaTitle": "Getting Started with .NET 9 - My Blog",
  "metaDescription": "Complete guide to getting started with .NET 9",
  "isPublished": true,
  "publishedAt": "2024-01-15T10:00:00Z"
}
```

**Auto-Generated:**
- Slug: `getting-started-with-net-9`
- Excerpt (if not provided): First 160 characters of content
- Meta fields (if not provided): Defaults to title/excerpt

### Adding a Comment

```bash
POST /api/comments
Authorization: Bearer {token}
Content-Type: application/json

{
  "postId": "uuid-here",
  "parentId": null,
  "content": "Great post! Very helpful."
}
```

**Note:** Comments require Admin approval before being visible.

### Searching Posts

```bash
POST /api/posts/search
Content-Type: application/json

{
  "query": "dotnet postgresql",
  "categoryId": "technology-category-uuid",
  "tagIds": ["csharp-tag-uuid", "dotnet-tag-uuid"],
  "isPublished": true,
  "orderBy": "viewCount",
  "descending": true,
  "page": 1,
  "pageSize": 10
}
```

---

## 🔐 Security Best Practices

1. **HTML Sanitization**: All post content and comments are sanitized to prevent XSS
2. **Comment Moderation**: Comments require approval by default
3. **Input Validation**: All DTOs have validation attributes
4. **Role-Based Access**: Admin role required for moderation and deletion
5. **Mass Assignment Protection**: Separate DTOs for input/output

---

## 🚀 Performance Optimizations

1. **Pagination**: All list endpoints support pagination (max 100 items per page)
2. **AsNoTracking**: Read-only queries don't track entities
3. **Eager Loading**: Related data loaded efficiently with Include()
4. **Composite Indexes**:
   - `(IsPublished, PublishedAt)` on Posts
   - `(PostId, IsApproved)` on Comments
   - `(RoleId, LastName)` on Users
5. **View Count**: Incremented asynchronously (fire-and-forget)

---

## 📈 Statistics Available

The statistics endpoint provides:
- Total posts (published/draft)
- Total comments (approved/pending)
- Total categories and tags
- Top 10 most viewed posts
- Top 10 most commented posts
- Post count by category

---

## 🗄️ Seed Data

The system comes with pre-seeded data:

**Categories:**
- Technology (slug: `technology`)
- Tutorials (slug: `tutorials`)

**Tags:**
- C# (slug: `csharp`)
- .NET (slug: `dotnet`)
- PostgreSQL (slug: `postgresql`)

---

## 🔄 Migrations

To create and apply migrations:

```bash
cd postgres-net-minimal-api

# Create initial migration
dotnet ef migrations add BlogSystemInitial

# Update database
dotnet ef database update
```

---

## ✅ Testing Checklist

- [ ] Create a blog post
- [ ] Add categories and tags
- [ ] Search posts with filters
- [ ] Add comments to a post
- [ ] Moderate comments (approve/reject)
- [ ] Create user profile
- [ ] View statistics
- [ ] Test pagination on all endpoints
- [ ] Verify SEO metadata generation
- [ ] Test nested comments
- [ ] Verify HTML sanitization

---

## 📚 Additional Resources

- [Swagger UI](http://localhost:5175/swagger) - Interactive API documentation
- [PostgreSQL Full-Text Search](https://www.postgresql.org/docs/current/textsearch.html)
- [.NET 9 Documentation](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9)

---

**The blog system is production-ready and follows all .NET 9 best practices with PostgreSQL optimizations!** 🎉
