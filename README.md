# PostgreSQL .NET Minimal API

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-336791?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

A production-ready REST API built with .NET 9, PostgreSQL, and Entity Framework Core for user and role management. Features JWT authentication, Docker support, and comprehensive API documentation.

## 📑 Table of Contents

- [Features](#-features)
- [Prerequisites](#-prerequisites)
- [Quick Start](#-quick-start)
- [Installation & Setup](#️-installation--setup)
- [API Documentation](#-api-documentation)
- [Project Structure](#️-project-structure)
- [Development Commands](#-development-commands)
- [TODO List](#-todo-list)
- [Security](#-security--environment-variables)
- [Troubleshooting](#-troubleshooting)
- [Technologies Used](#️-technologies-used)

## 🚀 Features

- ✅ **JWT Authentication** - Token-based authentication with BCrypt password hashing
- ✅ **User & Role Management** - Complete CRUD operations for users and roles
- ✅ **PostgreSQL Database** - Production-ready relational database
- ✅ **Entity Framework Core** - Code-first approach with migrations
- ✅ **Docker Support** - Docker Compose configuration for easy deployment
- ✅ **Environment Variables** - Secure configuration with .env files
- ✅ **Swagger/OpenAPI** - Interactive API documentation
- ✅ **CORS Configuration** - Cross-origin resource sharing enabled
- ✅ **Minimal API** - Modern .NET 9 minimal API pattern
- ✅ **Seed Data** - Pre-configured users and roles for testing

## 📋 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (recommended)
- [Git](https://git-scm.com/)
- Optional: [DBeaver](https://dbeaver.io/) or [pgAdmin](https://www.pgadmin.org/) for database management

## ⚡ Quick Start

```bash
# 1. Clone repository
git clone <repository-url>
cd postgres-net-minimal-api

# 2. Setup environment
cp .env.example .env

# 3. Start PostgreSQL
docker-compose up -d

# 4. Run migrations
dotnet ef database update

# 5. Run application
dotnet run
```

Access the API at: **http://localhost:5174**
Swagger UI at: **http://localhost:5174**

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

### 3. Setup Environment Variables
Copy the example environment file and configure your credentials:
```bash
cp .env.example .env
```

Edit `.env` file with your configuration:
```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_secure_password
POSTGRES_DB=postgres_db
POSTGRES_PORT=5433

JWT_KEY=your_jwt_secret_key_at_least_32_characters_long
JWT_ISSUER=tu_api_minimal
JWT_AUDIENCE=clientes_de_tu_api
```

### 4. Setup PostgreSQL Database

#### Option A: Using Docker Compose (Recommended)
```bash
docker-compose up -d
```

This will start PostgreSQL in a container with the configuration from your `.env` file.

#### Option B: Using Docker Run
```bash
docker run --name postgres17_minimal_api \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=postgres_db \
  -p 5433:5432 \
  -v postgres17_data:/var/lib/postgresql/data \
  -d postgres:17
```

#### Option C: Local PostgreSQL Installation
Install PostgreSQL locally and create a database named `postgres_db`.

### 5. Run Database Migrations

Install EF Core tools if not already installed:
```bash
dotnet tool install --global dotnet-ef
```

Apply migrations to create database tables:
```bash
dotnet ef database update
```

The migration will create:
- `Users` table with seed data (Admin and User)
- `UserRoles` table with seed data (Admin, User, Guest)

### 6. Run the Application
```bash
dotnet run
```

The API will be available at: `http://localhost:5174`

### 7. Test with Seed Data

The migration automatically creates test users and roles:

**Users:**
- **Admin User**
  - Email: `admin@example.com`
  - Password: `Admin@123`
  - Role: Admin

- **Standard User**
  - Email: `john.doe@example.com`
  - Password: `User@123`
  - Role: User

**Roles:**
- Admin - Full system access
- User - Standard user access
- Guest - Limited access

## 📚 API Documentation

### 📖 Complete Documentation

Para documentación completa y detallada del proyecto, consulta la **[Carpeta de Documentación (docs/)](./docs/INDEX.md)**:

- **[Índice Maestro de Documentación](./docs/INDEX.md)** - Guía completa con enlaces a toda la documentación
- **[Autenticación y Endpoints](./docs/INDEX.md#-autenticación-y-autorización)** - Login, registro, cambio de contraseña
- **[Sistema de Blog](./docs/BLOG_SYSTEM.md)** - Arquitectura RBAC granular
- **[Credenciales de Desarrollo](./docs/SEED_DATA_CREDENTIALS.md)** - Usuarios y contraseñas de prueba
- **[Seguridad](./docs/SECURITY.md)** - Mejores prácticas de seguridad

### Swagger UI
Access interactive API documentation at: **http://localhost:5174**

The Swagger UI allows you to:
- Explore all available endpoints
- Test API calls directly from the browser
- View request/response schemas
- Authenticate with JWT tokens

### API Endpoints Overview

#### Authentication Endpoints

| Method | Endpoint | Description | Auth Required | Documentación |
|--------|----------|-------------|---------------|---------------|
| POST | `/auth/register` | Register new user (auto role "User") | No | [📄 Docs](./docs/REGISTRATION_ENDPOINT.md) |
| POST | `/auth/login` | Authenticate user and get JWT token | No | [📄 Docs](./docs/LOGIN_ENDPOINT.md) |
| POST | `/auth/change-password` | Change user password | Yes | [📄 Docs](./docs/CHANGE_PASSWORD_ENDPOINT.md) |
| POST | `/auth/logout` | Logout (client-side token discard) | Yes | - |

#### User Management Endpoints

| Method | Endpoint | Description | Auth Required | Documentación |
|--------|----------|-------------|---------------|---------------|
| GET | `/users` | Get all users with pagination | No | - |
| GET | `/users/{id}` | Get user by ID | No | - |
| POST | `/users` | Create new user (public registration) | No | - |
| PUT | `/users/me` | Update own profile | Yes | [📄 Docs](./docs/UPDATE_PROFILE_ENDPOINT.md) |
| PUT | `/users/{id}` | Update user (Admin only) | Yes (Admin) | - |
| DELETE | `/users/{id}` | Delete user (Admin only) | Yes (Admin) | - |

#### Role Management Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/roles` | Get all roles | No |
| GET | `/roles/{id}` | Get role by ID | No |

**Credenciales de prueba**: Ver [SEED_DATA_CREDENTIALS.md](./docs/SEED_DATA_CREDENTIALS.md)

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
├── appsettings.Development.json
├── docker-compose.yml          # Docker Compose configuration
├── .env                         # Environment variables (not in git)
├── .env.example                 # Environment template
├── .gitignore                   # Git ignore rules
└── README.md
```

## 🔧 Development Commands

### Docker PostgreSQL Management

#### Using Docker Compose
```bash
# Start PostgreSQL container
docker-compose up -d

# Stop PostgreSQL container
docker-compose down

# View container logs
docker-compose logs -f postgres

# Restart PostgreSQL container
docker-compose restart

# Connect to PostgreSQL CLI
docker-compose exec postgres psql -U postgres -d postgres_db
```

#### Using Docker (Legacy)
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

### Database Tools (DBeaver, pgAdmin, etc.)

To connect to PostgreSQL using database management tools:

**Connection Parameters:**
- **Host:** `localhost`
- **Port:** `5433`
- **Database:** `postgres_db`
- **Username:** `postgres`
- **Password:** `postgres`

**DBeaver Quick Setup:**
1. New Connection → PostgreSQL
2. Enter connection parameters above
3. Test Connection
4. Download PostgreSQL driver if prompted
5. Finish

**Verify container is running:**
```bash
docker ps
```
You should see port mapping: `0.0.0.0:5433->5432/tcp`

### Database Migration Commands

#### Install EF Core Tools
```bash
dotnet tool install --global dotnet-ef
```

#### Apply Migrations
```bash
dotnet ef database update
```

#### View Applied Migrations
```bash
dotnet ef migrations list
```

#### Create New Migration
```bash
dotnet ef migrations add MigrationName
```

#### Remove Last Migration
```bash
dotnet ef migrations remove
```

#### Reset Database (Complete Clean)
```bash
dotnet ef database drop --force && rm -rf Migrations/ && dotnet ef migrations add InitialCreate && dotnet ef database update
```

## 📝 TODO List

### ✅ Completed
- [x] JWT token generation and validation
- [x] BCrypt password hashing
- [x] User and role management endpoints
- [x] Swagger/OpenAPI documentation
- [x] Docker containerization with Docker Compose
- [x] Environment variables configuration
- [x] Database migrations and seed data
- [x] CORS configuration

### High Priority
- [ ] Add proper authentication middleware to protected endpoints
- [ ] Create user registration endpoint
- [ ] Add comprehensive input validation and error handling
- [ ] Implement role-based authorization with policies
- [ ] Add request/response logging

### Medium Priority
- [ ] Create dedicated roles management endpoints (POST, PUT, DELETE)
- [ ] Add password reset functionality
- [ ] Implement email verification
- [ ] Add structured logging with Serilog
- [ ] Create user profile endpoints
- [ ] Add pagination to GET endpoints
- [ ] Implement search and filtering

### Low Priority
- [ ] Implement rate limiting
- [ ] Add unit and integration tests
- [ ] Add health check endpoints
- [ ] Implement audit logging
- [ ] Add API versioning
- [ ] Create Dockerfile for API

### Future Iterations
- [ ] Add refresh token mechanism
- [ ] Implement OAuth2/OpenID Connect
- [ ] Add two-factor authentication (2FA)
- [ ] Create admin dashboard
- [ ] Add file upload capabilities
- [ ] Implement real-time notifications with SignalR
- [ ] Add Redis caching
- [ ] Implement background jobs with Hangfire

## 🔒 Security & Environment Variables

### Environment File (.env)
- ⚠️ **NEVER commit `.env` to version control**
- The `.env` file contains sensitive credentials
- Always use `.env.example` as a template for new developers
- Use different credentials for development and production

### Configuration Priority
The application loads configuration in this order (later sources override earlier ones):
1. `appsettings.json` (base configuration)
2. `appsettings.{Environment}.json` (environment-specific)
3. `.env` file (loaded by DotNetEnv)
4. Environment variables
5. Command-line arguments

### Production Deployment
For production environments:
- Use environment variables instead of `.env` file
- Store secrets in secure vaults (Azure Key Vault, AWS Secrets Manager, etc.)
- Never use default passwords
- Generate strong JWT keys (minimum 32 characters)

## ❓ Troubleshooting

### Common Issues

**Issue: Cannot connect to PostgreSQL**
```bash
# Check if container is running
docker ps

# Check container logs
docker-compose logs postgres

# Restart container
docker-compose restart
```

**Issue: EF Core tools not found**
```bash
# Install globally
dotnet tool install --global dotnet-ef

# Verify installation
dotnet ef --version
```

**Issue: Migration fails**
```bash
# Drop and recreate database
dotnet ef database drop --force
dotnet ef database update
```

**Issue: Port 5433 already in use**
```bash
# Check what's using the port
lsof -i :5433  # Mac/Linux
netstat -ano | findstr :5433  # Windows

# Change port in .env file
POSTGRES_PORT=5434
```

**Issue: .env file not loading**
- Ensure `.env` file is in the root directory (same level as docker-compose.yml)
- Check file permissions
- Restart the application

## 🤝 Contributing

This is a mockup project for future development. Feel free to:
1. Fork the repository
2. Create feature branches
3. Submit pull requests
4. Report issues and suggestions

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🛠️ Technologies Used

### Backend
- **.NET 9** - Latest version of the .NET framework
- **ASP.NET Core Minimal API** - Lightweight API framework
- **Entity Framework Core 9** - ORM for database operations
- **Npgsql** - PostgreSQL provider for EF Core

### Database
- **PostgreSQL 17** - Advanced open-source relational database
- **Docker** - Containerization platform

### Security
- **JWT Bearer Authentication** - Token-based authentication
- **BCrypt.Net** - Password hashing library

### Tools & Libraries
- **Swashbuckle (Swagger)** - API documentation
- **DotNetEnv** - Environment variable management

## 🔗 Additional Resources

- [.NET 9 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [BCrypt.Net](https://github.com/BcryptNet/bcrypt.net)
- [Docker Documentation](https://docs.docker.com/)
- [JWT.IO](https://jwt.io/) - JWT Debugger

## 📊 Project Stats

- **Language:** C# 11
- **Framework:** .NET 9.0
- **Database:** PostgreSQL 17
- **Architecture:** Minimal API Pattern
- **Lines of Code:** ~500+ (excluding migrations)

---

**Note:** This project is a production-ready foundation with JWT authentication, Docker support, and comprehensive API documentation. Review the TODO list for planned enhancements.