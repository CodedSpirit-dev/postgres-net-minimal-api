# Security Guide

## Critical Security Configuration

### 1. JWT Secret Key Configuration

**IMPORTANT:** The JWT secret key must be at least 32 characters (256 bits) for production use.

#### Development (User Secrets)
```bash
cd postgres-net-minimal-api
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "your-secret-key-at-least-32-characters-long-for-hs256"
```

#### Production (Environment Variables)
```bash
export Jwt__Key="your-production-secret-key-minimum-32-characters"
export ConnectionStrings__DefaultConnection="Host=your-db-host;Port=5432;Database=your_db;Username=your_user;Password=your_password"
```

Or using appsettings.Production.json (DO NOT commit this file to git):
```json
{
  "Jwt": {
    "Key": "YOUR_PRODUCTION_KEY_HERE"
  },
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING_HERE"
  }
}
```

### 2. Database Connection String

Never hardcode database credentials in appsettings.json. Use:
- **Development:** User Secrets
- **Production:** Environment variables or Azure Key Vault / AWS Secrets Manager

### 3. CORS Configuration

The default CORS policy is configured for development. For production:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://your-production-domain.com",
      "https://www.your-production-domain.com"
    ]
  }
}
```

### 4. Rate Limiting

The API includes rate limiting to prevent abuse:
- **Login endpoint:** 5 requests per minute
- **Global API:** 100 requests per minute

Adjust these in `Program.cs` based on your needs.

### 5. Password Requirements

Passwords must meet the following criteria:
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one digit
- At least one special character
- Cannot be a common weak password

### 6. Default Test Accounts

**IMPORTANT:** Change or delete these accounts in production:

```
Admin Account:
- Email: admin@example.com
- Password: AdminPassword123!

User Account:
- Email: john.doe@example.com
- Password: UserPassword123!
```

To disable these accounts, remove the seed data from `AppDbContext.cs` and create a new migration.

## Security Features Implemented

✅ **Mass Assignment Protection** - Input DTOs prevent privilege escalation
✅ **Rate Limiting** - Brute force protection on login endpoint
✅ **Password Hashing** - BCrypt with salt
✅ **Password Strength Validation** - Enforced complexity requirements
✅ **JWT Authentication** - Secure token-based authentication
✅ **Role-Based Authorization** - Admin/User/Guest roles
✅ **CORS Protection** - Configurable allowed origins
✅ **Input Validation** - Data annotations and service-level validation
✅ **SQL Injection Protection** - Parameterized queries via EF Core
✅ **Exception Handling** - Global error handler prevents information leakage

## Security Checklist for Production

- [ ] Set secure JWT secret key (minimum 32 characters)
- [ ] Configure production database connection string
- [ ] Update CORS allowed origins to production domains only
- [ ] Change or remove default seed user accounts
- [ ] Enable HTTPS redirection
- [ ] Review and adjust rate limiting policies
- [ ] Implement token blacklisting for logout (if required)
- [ ] Set up monitoring and logging (Serilog, Application Insights)
- [ ] Configure health checks
- [ ] Review and test all authorization policies
- [ ] Perform security audit and penetration testing
- [ ] Set up SSL/TLS certificates
- [ ] Configure firewall rules
- [ ] Enable database encryption at rest
- [ ] Set up automated backups
- [ ] Document incident response procedures

## Reporting Security Vulnerabilities

If you discover a security vulnerability, please email security@example.com with details. Do not create public GitHub issues for security vulnerabilities.

## Additional Resources

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [.NET Security Best Practices](https://docs.microsoft.com/en-us/aspnet/core/security/)
- [JWT Best Practices](https://tools.ietf.org/html/rfc8725)
