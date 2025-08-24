# PostgreSQL .NET Minimal API

A REST API built with .NET 9, PostgreSQL, and Entity Framework Core for user and role management. This project serves as a **mockup foundation** for future iterations and development.

## 🚀 Features

- ✅ BCrypt authentication
- ✅ User and role management
- ✅ PostgreSQL database
- ✅ Entity Framework Core with migrations
- ✅ Minimal API endpoints
- ✅ Data validation
- ✅ Circular reference prevention
- ✅ Clean architecture foundation

## 📋 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/)

## 🛠️ Installation & Setup

### 1. Clone Repository
```bash
git clone <repository-url>
cd postgres-net-minimal-api
```

### 2. Install Dependencies
```bash
dotnet restore
```

### 3. Setup PostgreSQL Database

#### Option A: Using Docker (Recommended)
```bash
docker run --name postgres17_minimal_api \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=postgres_db \
  -p 5433:5432 \
  -v postgres17_data:/var/lib/postgresql/data \
  -d postgres:17
```

#### Option B: Local PostgreSQL Installation
Install PostgreSQL locally and create a database named `postgres_db`.

### 4. Configure Database Connection
Update the connection string in `appsettings.json`:

**For Docker setup:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=postgres_db;Username=postgres;Password=postgres"
  }
}
```

**For local PostgreSQL:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=postgres_db;Username=your_username;Password=your_password"
  }
}
```

### 5. Reset Database & Run Migrations
```bash
dotnet ef database drop --force && rm -rf Migrations/ && dotnet ef migrations add InitialCreate && dotnet ef database update
```

### 6. Run the Application
```bash
dotnet run
```

The API will be available at: `http://localhost:5174`

## 📚 API Documentation

### Authentication Endpoints

#### POST `/auth/login`
Authenticate user and get access token.

**Request:**
```json
{
  "email": "admin@example.com",
  "password": "Admin@123"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
  "user": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "firstName": "Juan",
    "lastName": "Pérez",
    "email": "admin@example.com",
    "role": {
      "id": "456e7890-e89b-12d3-a456-426614174001",
      "name": "Admin"
    }
  },
  "expiresAt": "2025-08-24T10:30:00Z"
}
```

**Response (401 Unauthorized):**
```json
{
  "error": "Invalid credentials"
}
```

### User Endpoints

#### GET `/users`
Get all users with their roles.

**Response:**
```json
[
  {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "firstName": "John",
    "lastName": "Doe",
    "middleName": null,
    "email": "john@example.com",
    "dateOfBirth": "1990-05-15",
    "motherMaidenName": "Smith",
    "role": {
      "id": "456e7890-e89b-12d3-a456-426614174001",
      "name": "Admin",
      "description": "Administrator role"
    }
  }
]
```

#### GET `/users/{id}`
Get user by ID.

**Response (200 OK):**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "firstName": "John",
  "lastName": "Doe",
  "middleName": null,
  "email": "john@example.com",
  "dateOfBirth": "1990-05-15",
  "motherMaidenName": "Smith",
  "role": {
    "id": "456e7890-e89b-12d3-a456-426614174001",
    "name": "Admin",
    "description": "Administrator role"
  }
}
```

**Response (404 Not Found):**
```json
{
  "error": "User not found"
}
```

#### PUT `/users/{id}`
Update user information.

**Request:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "hashedPassword": "NewPassword123",
  "roleId": "456e7890-e89b-12d3-a456-426614174001"
}
```

**Response:** `204 No Content`

#### DELETE `/users/{id}`
Delete user by ID.

**Response:** `204 No Content`

## 🧪 Testing with Postman

### Setup Collection
1. Create a new collection named "PostgreSQL .NET API"
2. Set base URL variable: `{{baseUrl}}` = `http://localhost:5174`

### Test Scenarios

#### 1. Login Test
```
POST {{baseUrl}}/auth/login
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "Admin@123"
}
```

#### 2. Get All Users
```
GET {{baseUrl}}/users
```

#### 3. Get User by ID
```
GET {{baseUrl}}/users/{{userId}}
```

#### 4. Update User
```
PUT {{baseUrl}}/users/{{userId}}
Content-Type: application/json

{
  "firstName": "Updated Name",
  "lastName": "Updated Last",
  "email": "updated@example.com",
  "hashedPassword": "NewPassword123",
  "roleId": "role-guid-here"
}
```

#### 5. Delete User
```
DELETE {{baseUrl}}/users/{{userId}}
```

## 🗂️ Project Structure

```
postgres-net-minimal-api/
├── Controllers/
│   ├── AuthEndpoints.cs
│   └── UsersEndpoints.cs
├── Data/
│   └── AppDbContext.cs
├── Models/
│   ├── User.cs
│   └── UserRole.cs
├── Migrations/
├── Program.cs
├── appsettings.json
└── README.md
```

## 🔧 Development Commands

### Docker PostgreSQL Management
```bash
# Start PostgreSQL container
docker start postgres17_minimal_api

# Stop PostgreSQL container
docker stop postgres17_minimal_api

# View container logs
docker logs postgres17_minimal_api

# Connect to PostgreSQL CLI
docker exec -it postgres17_minimal_api psql -U postgres -d postgres_db
```

### Database Migration Commands

### Reset Database (Complete Clean)
```bash
dotnet ef database drop --force && rm -rf Migrations/ && dotnet ef migrations add InitialCreate && dotnet ef database update
```

### Add New Migration
```bash
dotnet ef migrations add MigrationName
```

### Update Database
```bash
dotnet ef database update
```

### Remove Last Migration
```bash
dotnet ef migrations remove
```

## 📝 TODO List

### High Priority
- [ ] Implement JWT token generation and validation
- [ ] Add proper authentication middleware
- [ ] Create user registration endpoint
- [ ] Add input validation and error handling
- [ ] Implement role-based authorization

### Medium Priority
- [ ] Add Username field to User model (nullable)
- [ ] Create user roles management endpoints
- [ ] Add password reset functionality
- [ ] Implement email verification
- [ ] Add logging and monitoring
- [ ] Create user profile endpoints

### Low Priority
- [ ] Add Swagger/OpenAPI documentation
- [ ] Implement rate limiting
- [ ] Add unit and integration tests
- [ ] Create Docker containerization
- [ ] Add health check endpoints
- [ ] Implement audit logging

### Future Iterations
- [ ] Add refresh token mechanism
- [ ] Implement OAuth2/OpenID Connect
- [ ] Add two-factor authentication
- [ ] Create admin dashboard
- [ ] Add file upload capabilities
- [ ] Implement real-time notifications

## 🤝 Contributing

This is a mockup project for future development. Feel free to:
1. Fork the repository
2. Create feature branches
3. Submit pull requests
4. Report issues and suggestions

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🔗 Additional Resources

- [.NET 9 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [BCrypt.Net](https://github.com/BcryptNet/bcrypt.net)

---

**Note:** This project is intended as a foundation mockup for future iterations. The authentication system uses placeholder tokens and should be replaced with proper JWT implementation in production.