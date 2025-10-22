# 🔐 User Registration Endpoint

## Resumen

Se ha implementado un endpoint de **registro de usuarios** público que permite a nuevos usuarios registrarse en la plataforma. Los usuarios registrados reciben automáticamente el rol **"User"** y un token JWT para acceso inmediato.

---

## 📍 Endpoint

### POST `/auth/register`

**Descripción**: Registra un nuevo usuario con el rol predeterminado "User" y devuelve un token JWT.

**Rate Limiting**: 5 intentos por minuto (mismo límite que login para prevenir abuso)

**Autenticación**: No requiere autenticación (endpoint público)

---

## 📥 Request Body

```json
{
  "userName": "juanperez",
  "firstName": "Juan",
  "middleName": "Carlos",        // Opcional
  "lastName": "Pérez",
  "motherMaidenName": "García",  // Opcional
  "dateOfBirth": "1990-05-15",   // Formato: YYYY-MM-DD
  "email": "juan.perez@example.com",
  "password": "SecurePass123!"
}
```

### Validaciones

#### Username
- ✅ **Requerido**
- ✅ Longitud: 3-20 caracteres
- ✅ Debe ser único en el sistema

#### FirstName
- ✅ **Requerido**
- ✅ Longitud: 1-100 caracteres

#### LastName
- ✅ **Requerido**
- ✅ Longitud: 1-100 caracteres

#### Email
- ✅ **Requerido**
- ✅ Formato de email válido
- ✅ Máximo 255 caracteres
- ✅ Debe ser único en el sistema

#### Password
- ✅ **Requerido**
- ✅ Longitud mínima: 8 caracteres
- ✅ Máximo 100 caracteres
- ✅ Debe cumplir requisitos de seguridad del sistema:
  - Al menos una letra mayúscula
  - Al menos una letra minúscula
  - Al menos un número
  - Al menos un carácter especial

#### DateOfBirth
- ✅ **Requerido**
- ✅ Formato: YYYY-MM-DD (ISO 8601)

---

## 📤 Response

### Éxito (201 Created)

```json
{
  "success": true,
  "message": "User registered successfully",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "userName": "juanperez",
    "firstName": "Juan",
    "middleName": "Carlos",
    "lastName": "Pérez",
    "motherMaidenName": "García",
    "dateOfBirth": "1990-05-15",
    "email": "juan.perez@example.com",
    "role": {
      "id": "00000000-0000-0000-0000-000000000003",
      "name": "User",
      "description": "Standard user with basic permissions"
    }
  }
}
```

### Error - Email/Username Duplicado (400 Bad Request)

```json
{
  "success": false,
  "message": "Email address is already registered",
  "token": null,
  "user": null
}
```

o

```json
{
  "success": false,
  "message": "Username is already taken",
  "token": null,
  "user": null
}
```

### Error - Contraseña Débil (400 Bad Request)

```json
{
  "success": false,
  "message": "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character",
  "token": null,
  "user": null
}
```

### Error - Rol User No Existe (400 Bad Request)

```json
{
  "success": false,
  "message": "Default user role not found. Please contact system administrator",
  "token": null,
  "user": null
}
```

### Error - Rate Limit Excedido (429 Too Many Requests)

```json
{
  "error": "Too many requests. Please try again later."
}
```

---

## 🔒 Seguridad Implementada

### 1. **Prevención de Escalada de Privilegios**
- ❌ El usuario **NO puede especificar** su propio rol
- ✅ El rol "User" se asigna **automáticamente en el servidor**
- ✅ Solo administradores pueden asignar roles diferentes

### 2. **Validación de Contraseñas**
- ✅ Hash BCrypt automático (el password nunca se almacena en texto plano)
- ✅ Validación de complejidad en el servidor
- ✅ Longitud mínima de 8 caracteres

### 3. **Rate Limiting**
- ✅ Máximo 5 intentos por minuto
- ✅ Previene ataques de fuerza bruta
- ✅ Previene registro masivo automatizado

### 4. **Validación de Unicidad**
- ✅ Verifica que el email no esté registrado
- ✅ Verifica que el username no esté en uso
- ✅ Case-insensitive (no distingue mayúsculas/minúsculas)

---

## 🚀 Ejemplos de Uso

### cURL

```bash
curl -X POST http://localhost:5000/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "juanperez",
    "firstName": "Juan",
    "lastName": "Pérez",
    "dateOfBirth": "1990-05-15",
    "email": "juan.perez@example.com",
    "password": "SecurePass123!"
  }'
```

### JavaScript (Fetch)

```javascript
const response = await fetch('http://localhost:5000/auth/register', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    userName: 'juanperez',
    firstName: 'Juan',
    lastName: 'Pérez',
    dateOfBirth: '1990-05-15',
    email: 'juan.perez@example.com',
    password: 'SecurePass123!'
  })
});

const data = await response.json();

if (data.success) {
  // Guardar el token en localStorage o cookie segura
  localStorage.setItem('authToken', data.token);
  console.log('Usuario registrado:', data.user);
} else {
  console.error('Error de registro:', data.message);
}
```

### Axios (JavaScript/TypeScript)

```typescript
import axios from 'axios';

try {
  const { data } = await axios.post('http://localhost:5000/auth/register', {
    userName: 'juanperez',
    firstName: 'Juan',
    lastName: 'Pérez',
    dateOfBirth: '1990-05-15',
    email: 'juan.perez@example.com',
    password: 'SecurePass123!'
  });

  if (data.success) {
    // Guardar token
    localStorage.setItem('authToken', data.token);
    console.log('Registro exitoso:', data.user);
  }
} catch (error) {
  if (error.response?.status === 400) {
    console.error('Error de validación:', error.response.data.message);
  } else if (error.response?.status === 429) {
    console.error('Demasiados intentos. Espera un minuto.');
  }
}
```

### C# (.NET Client)

```csharp
using System.Net.Http.Json;

var client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

var request = new
{
    userName = "juanperez",
    firstName = "Juan",
    lastName = "Pérez",
    dateOfBirth = new DateOnly(1990, 5, 15),
    email = "juan.perez@example.com",
    password = "SecurePass123!"
};

var response = await client.PostAsJsonAsync("/auth/register", request);

if (response.IsSuccessStatusCode)
{
    var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
    Console.WriteLine($"Token: {result.Token}");
    Console.WriteLine($"Usuario: {result.User.UserName}");
}
else
{
    var error = await response.Content.ReadFromJsonAsync<RegisterResponse>();
    Console.WriteLine($"Error: {error.Message}");
}
```

---

## 🔄 Flujo de Registro

```
1. Cliente envía POST /auth/register
   ↓
2. Sistema valida datos de entrada (longitud, formato, etc.)
   ↓
3. Sistema verifica unicidad de email y username
   ↓
4. Sistema valida complejidad de la contraseña
   ↓
5. Sistema verifica que el rol "User" existe
   ↓
6. Sistema hashea la contraseña con BCrypt
   ↓
7. Sistema crea el usuario con rol "User" asignado
   ↓
8. Sistema genera token JWT automáticamente
   ↓
9. Sistema devuelve token + datos del usuario
   ↓
10. Cliente guarda el token y está autenticado
```

---

## 🎯 Casos de Uso

### 1. Registro Normal
```json
POST /auth/register
{
  "userName": "maria2024",
  "firstName": "María",
  "lastName": "González",
  "dateOfBirth": "1995-03-20",
  "email": "maria@example.com",
  "password": "MyP@ssw0rd!"
}

→ 201 Created + Token JWT
```

### 2. Email Duplicado
```json
POST /auth/register
{
  "email": "maria@example.com",  // Ya existe
  ...
}

→ 400 Bad Request: "Email address is already registered"
```

### 3. Username Duplicado
```json
POST /auth/register
{
  "userName": "maria2024",  // Ya existe
  ...
}

→ 400 Bad Request: "Username is already taken"
```

### 4. Contraseña Débil
```json
POST /auth/register
{
  "password": "12345678",  // Solo números
  ...
}

→ 400 Bad Request: "Password must contain..."
```

---

## 📊 Diferencias con ASP.NET Core Identity

| Característica | Este Sistema | ASP.NET Core Identity |
|---|---|---|
| **Complejidad** | Bajo | Alto |
| **Dependencias** | Ninguna (sistema propio) | Microsoft.AspNetCore.Identity |
| **Tablas DB** | Users, UserRoles (2 tablas) | 7+ tablas (AspNetUsers, etc.) |
| **Hash Password** | BCrypt | PBKDF2 (por defecto) |
| **Autenticación** | JWT personalizado | Cookie o JWT |
| **Confirmación Email** | No implementado | Sí (integrado) |
| **2FA** | No implementado | Sí (integrado) |
| **Reset Password** | No implementado | Sí (integrado) |
| **Control Total** | ✅ Sí | ⚠️ Limitado |

---

## ✅ Ventajas de Esta Implementación

1. **Simplicidad**: No requiere la complejidad de Identity
2. **Control Total**: Puedes modificar cualquier aspecto
3. **Menor Overhead**: Solo 2 tablas en lugar de 7+
4. **BCrypt**: Hash más seguro que PBKDF2 por defecto
5. **JWT Inmediato**: El usuario queda autenticado al registrarse
6. **Sin Dependencias**: No depende de Microsoft.AspNetCore.Identity

---

## 🛠️ Extensiones Futuras

### 1. Confirmación de Email
```csharp
// Enviar email de confirmación
await emailService.SendConfirmationEmailAsync(user.Email, confirmationToken);

// Usuario debe confirmar antes de poder login
if (!user.EmailConfirmed)
{
    return Results.Unauthorized("Please confirm your email address");
}
```

### 2. Reset de Contraseña
```csharp
POST /auth/forgot-password
POST /auth/reset-password
```

### 3. Autenticación de Dos Factores (2FA)
```csharp
POST /auth/enable-2fa
POST /auth/verify-2fa
```

### 4. OAuth / Social Login
```csharp
POST /auth/google
POST /auth/facebook
POST /auth/github
```

---

## 🧪 Testing

### Test de Registro Exitoso
```bash
curl -X POST http://localhost:5000/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "testuser",
    "firstName": "Test",
    "lastName": "User",
    "dateOfBirth": "2000-01-01",
    "email": "test@example.com",
    "password": "TestPass123!"
  }'
```

**Resultado Esperado**: 201 Created con token JWT

### Test de Email Duplicado
```bash
# Repetir el mismo request anterior
curl -X POST http://localhost:5000/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    ...
  }'
```

**Resultado Esperado**: 400 Bad Request con mensaje "Email address is already registered"

---

## Related Endpoints

- **[Login Endpoint](./LOGIN_ENDPOINT.md)** - To authenticate with existing account
- **[Change Password Endpoint](./CHANGE_PASSWORD_ENDPOINT.md)** - To change password after registration
- **[Update Profile Endpoint](./UPDATE_PROFILE_ENDPOINT.md)** - To update profile information
- **[Seed Data Credentials](./SEED_DATA_CREDENTIALS.md)** - Development user credentials
- **[📖 Documentation Index](./INDEX.md)** - Return to main documentation index

---

## 📝 Notas Importantes

1. **El token JWT se genera automáticamente**: No es necesario hacer un segundo request a `/auth/login`

2. **El rol "User" debe existir**: El sistema lanzará error si el rol no existe en la base de datos (asegúrate de tener seed data)

3. **La contraseña se hashea automáticamente**: Nunca se almacena en texto plano

4. **Rate limiting compartido con login**: Ambos endpoints comparten el límite de 5 requests/minuto

5. **Case-insensitive**: Los emails y usernames se validan sin distinguir mayúsculas/minúsculas

---

**Implementado en**: Branch `claude/net9-postgres-continue-011CULuK32NnLRSQN34ToRgQ`
**Archivo**: `Auth/Controllers/AuthEndpoints.cs`
**Fecha**: 2025-10-21

---

**[⬅️ Back to Documentation Index](./INDEX.md)**
