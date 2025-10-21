# 🔑 User Login Endpoint

## Resumen

El endpoint de **login** permite a usuarios existentes autenticarse usando su **email O username** + contraseña. Al autenticarse exitosamente, reciben un token JWT y sus datos de usuario completos.

---

## 📍 Endpoint

### POST `/auth/login`

**Descripción**: Autentica un usuario existente y devuelve un token JWT con información del usuario.

**Rate Limiting**: 5 intentos por minuto (previene ataques de fuerza bruta)

**Autenticación**: No requiere autenticación (endpoint público)

---

## 📥 Request Body

```json
{
  "usernameOrEmail": "juanperez",     // Puede ser username O email
  "password": "SecurePass123!"
}
```

### Ejemplos Válidos

**Con Username:**
```json
{
  "usernameOrEmail": "juanperez",
  "password": "SecurePass123!"
}
```

**Con Email:**
```json
{
  "usernameOrEmail": "juan.perez@example.com",
  "password": "SecurePass123!"
}
```

### Validaciones

#### UsernameOrEmail
- ✅ **Requerido**
- ✅ Longitud: 3-255 caracteres
- ✅ Puede ser email válido O username
- ✅ Case-insensitive (no distingue mayúsculas/minúsculas)

#### Password
- ✅ **Requerido**
- ✅ Longitud: 8-100 caracteres

---

## 📤 Response

### Éxito (200 OK)

```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI1NTBlODQwMC1lMjliLTQxZDQtYTcxNi00NDY2NTU0NDAwMDAiLCJlbWFpbCI6Imp1YW4ucGVyZXpAZXhhbXBsZS5jb20iLCJ1c2VybmFtZSI6Imp1YW5wZXJleiIsInJvbGUiOiJVc2VyIiwibmJmIjoxNzA5ODUzNjAwLCJleHAiOjE3MDk4NTcyMDAsImlhdCI6MTcwOTg1MzYwMCwiaXNzIjoieW91ci1hcGktaXNzdWVyIiwiYXVkIjoieW91ci1hcGktYXVkaWVuY2UifQ.signature",
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

### Error - Credenciales Inválidas (401 Unauthorized)

```json
{
  "success": false,
  "message": "Invalid username/email or password"
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

### 1. **Protección contra Fuerza Bruta**
- ✅ Rate limiting: Máximo 5 intentos por minuto
- ✅ Previene ataques automatizados de fuerza bruta
- ✅ Bloqueo temporal después de exceder el límite

### 2. **Mensajes de Error Genéricos**
- ✅ No revela si el email/username existe
- ✅ Mensaje genérico: "Invalid username/email or password"
- ✅ Previene enumeración de usuarios

### 3. **Hash BCrypt**
- ✅ Contraseñas hasheadas con BCrypt
- ✅ Nunca se comparan contraseñas en texto plano
- ✅ Verificación segura con BCrypt.Verify

### 4. **Token JWT Seguro**
- ✅ Token firmado con clave secreta
- ✅ Incluye claims: UserId, Email, Username, Role
- ✅ Expiración configurable (default: 1 hora)
- ✅ HTTPS requerido en producción

---

## 🚀 Ejemplos de Uso

### cURL

**Login con Email:**
```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "juan.perez@example.com",
    "password": "SecurePass123!"
  }'
```

**Login con Username:**
```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "juanperez",
    "password": "SecurePass123!"
  }'
```

### JavaScript (Fetch)

```javascript
const loginUser = async (usernameOrEmail, password) => {
  try {
    const response = await fetch('http://localhost:5000/auth/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        usernameOrEmail,
        password
      })
    });

    const data = await response.json();

    if (response.ok && data.success) {
      // Guardar token en localStorage o cookie segura
      localStorage.setItem('authToken', data.token);

      // Guardar datos del usuario
      localStorage.setItem('user', JSON.stringify(data.user));

      console.log('Login exitoso:', data.user);
      return data;
    } else {
      console.error('Error de login:', data.message);
      throw new Error(data.message);
    }
  } catch (error) {
    console.error('Error de conexión:', error);
    throw error;
  }
};

// Uso
loginUser('juanperez', 'SecurePass123!')
  .then(data => {
    console.log('Usuario autenticado:', data.user.userName);
  })
  .catch(error => {
    alert('Error: ' + error.message);
  });
```

### Axios (JavaScript/TypeScript)

```typescript
import axios from 'axios';

interface LoginRequest {
  usernameOrEmail: string;
  password: string;
}

interface LoginResponse {
  success: boolean;
  message: string;
  token: string;
  user: {
    id: string;
    userName: string;
    email: string;
    firstName: string;
    lastName: string;
    role: {
      name: string;
    };
  };
}

const login = async (credentials: LoginRequest): Promise<LoginResponse> => {
  try {
    const { data } = await axios.post<LoginResponse>(
      'http://localhost:5000/auth/login',
      credentials
    );

    if (data.success) {
      // Guardar token
      localStorage.setItem('authToken', data.token);

      // Configurar header de autorización para futuras peticiones
      axios.defaults.headers.common['Authorization'] = `Bearer ${data.token}`;

      console.log('Login exitoso:', data.user);
    }

    return data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      if (error.response?.status === 401) {
        throw new Error('Credenciales inválidas');
      } else if (error.response?.status === 429) {
        throw new Error('Demasiados intentos. Espera un minuto.');
      }
    }
    throw new Error('Error de conexión');
  }
};

// Uso
login({
  usernameOrEmail: 'juanperez',
  password: 'SecurePass123!'
})
  .then(response => {
    console.log(`Bienvenido ${response.user.firstName}!`);
  })
  .catch(error => {
    console.error(error.message);
  });
```

### React Hook Example

```typescript
import { useState } from 'react';
import axios from 'axios';

const useLogin = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const login = async (usernameOrEmail: string, password: string) => {
    setLoading(true);
    setError(null);

    try {
      const { data } = await axios.post('http://localhost:5000/auth/login', {
        usernameOrEmail,
        password
      });

      if (data.success) {
        // Guardar en localStorage
        localStorage.setItem('authToken', data.token);
        localStorage.setItem('user', JSON.stringify(data.user));

        // Configurar header de autorización
        axios.defaults.headers.common['Authorization'] = `Bearer ${data.token}`;

        return data;
      }
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || 'Error de conexión';
      setError(errorMessage);
      throw new Error(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  return { login, loading, error };
};

// Componente de Login
const LoginForm = () => {
  const { login, loading, error } = useLogin();
  const [usernameOrEmail, setUsernameOrEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      const data = await login(usernameOrEmail, password);
      console.log('Login exitoso:', data.user);
      // Redirigir al dashboard
      window.location.href = '/dashboard';
    } catch (error) {
      // El error ya está manejado en el hook
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <input
        type="text"
        placeholder="Email o Username"
        value={usernameOrEmail}
        onChange={(e) => setUsernameOrEmail(e.target.value)}
        required
      />
      <input
        type="password"
        placeholder="Contraseña"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        required
      />
      <button type="submit" disabled={loading}>
        {loading ? 'Iniciando sesión...' : 'Iniciar Sesión'}
      </button>
      {error && <p style={{ color: 'red' }}>{error}</p>}
    </form>
  );
};
```

### C# (.NET Client)

```csharp
using System.Net.Http.Json;

public class AuthClient
{
    private readonly HttpClient _httpClient;

    public AuthClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:5000");
    }

    public async Task<LoginResponse> LoginAsync(string usernameOrEmail, string password)
    {
        var request = new LoginRequest
        {
            UsernameOrEmail = usernameOrEmail,
            Password = password
        };

        var response = await _httpClient.PostAsJsonAsync("/auth/login", request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (result?.Success == true)
            {
                // Guardar token para futuras peticiones
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);

                Console.WriteLine($"Bienvenido {result.User.FirstName}!");
                return result;
            }
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new UnauthorizedAccessException(error?.Message ?? "Credenciales inválidas");
        }
        else if ((int)response.StatusCode == 429)
        {
            throw new Exception("Demasiados intentos. Espera un minuto.");
        }

        throw new Exception("Error al iniciar sesión");
    }
}

// DTOs
public record LoginRequest
{
    public string UsernameOrEmail { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public record LoginResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public UserData User { get; init; } = null!;
}

public record UserData
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public RoleData Role { get; init; } = null!;
}

public record RoleData
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

// Uso
var authClient = new AuthClient(new HttpClient());

try
{
    var result = await authClient.LoginAsync("juanperez", "SecurePass123!");
    Console.WriteLine($"Token: {result.Token}");
    Console.WriteLine($"Usuario: {result.User.UserName}");
    Console.WriteLine($"Rol: {result.User.Role.Name}");
}
catch (UnauthorizedAccessException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

---

## 🔄 Flujo de Login

```
1. Usuario envía POST /auth/login con credenciales
   ↓
2. Sistema normaliza username/email a lowercase
   ↓
3. Sistema busca usuario por email O username (case-insensitive)
   ↓
4. Si no existe usuario → 401 Unauthorized
   ↓
5. Sistema verifica password con BCrypt
   ↓
6. Si password incorrecto → 401 Unauthorized
   ↓
7. Sistema genera token JWT con claims del usuario
   ↓
8. Sistema devuelve token + datos completos del usuario
   ↓
9. Cliente guarda token y está autenticado
   ↓
10. Cliente incluye token en header Authorization: Bearer {token}
```

---

## 🎯 Casos de Uso

### 1. Login Exitoso con Email
```json
POST /auth/login
{
  "usernameOrEmail": "juan.perez@example.com",
  "password": "SecurePass123!"
}

→ 200 OK + Token JWT + Datos de usuario
```

### 2. Login Exitoso con Username
```json
POST /auth/login
{
  "usernameOrEmail": "juanperez",
  "password": "SecurePass123!"
}

→ 200 OK + Token JWT + Datos de usuario
```

### 3. Credenciales Incorrectas
```json
POST /auth/login
{
  "usernameOrEmail": "juanperez",
  "password": "WrongPassword123!"
}

→ 401 Unauthorized: "Invalid username/email or password"
```

### 4. Usuario No Existe
```json
POST /auth/login
{
  "usernameOrEmail": "noexiste@example.com",
  "password": "AnyPassword123!"
}

→ 401 Unauthorized: "Invalid username/email or password"
```

### 5. Rate Limit Excedido
```json
// 6to intento en menos de 1 minuto
POST /auth/login
{ ... }

→ 429 Too Many Requests
```

---

## 🔐 JWT Token

### Claims Incluidos

El token JWT incluye los siguientes claims:

```json
{
  "sub": "550e8400-e29b-41d4-a716-446655440000",  // User ID
  "email": "juan.perez@example.com",
  "username": "juanperez",
  "role": "User",                                  // Role name
  "nbf": 1709853600,                              // Not Before
  "exp": 1709857200,                              // Expiration
  "iat": 1709853600,                              // Issued At
  "iss": "your-api-issuer",                       // Issuer
  "aud": "your-api-audience"                      // Audience
}
```

### Configuración

Configurar en `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "CHANGE_THIS_super-secret-key-at-least-32-characters-long",
    "Issuer": "your-api-issuer",
    "Audience": "your-api-audience",
    "ExpirationInHours": 1
  }
}
```

### Uso del Token

En todas las peticiones autenticadas, incluir el header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Ejemplo con Fetch:**
```javascript
fetch('http://localhost:5000/api/posts', {
  headers: {
    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
  }
});
```

**Ejemplo con Axios:**
```javascript
axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
```

---

## ⚡ Diferencias entre Login y Registro

| Característica | Login | Registro |
|---|---|---|
| **Endpoint** | POST /auth/login | POST /auth/register |
| **Requiere cuenta** | Sí | No |
| **Datos requeridos** | Email/Username + Password | Todos los campos de usuario |
| **Asignación de rol** | No aplica | Automático ("User") |
| **Validaciones** | Credenciales | Email único, username único, password fuerte |
| **Respuesta** | Token + Usuario | Token + Usuario |
| **Rate limit** | 5/minuto | 5/minuto |

---

## 🧪 Testing

### Test de Login Exitoso con Email
```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "admin@admin.com",
    "password": "yoadmin123"
  }'
```

**Resultado Esperado**: 200 OK con token JWT y datos del usuario

### Test de Login Exitoso con Username
```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "Admin",
    "password": "yoadmin123"
  }'
```

**Resultado Esperado**: 200 OK con token JWT y datos del usuario

### Test de Credenciales Incorrectas
```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "admin@admin.com",
    "password": "wrongpassword"
  }'
```

**Resultado Esperado**: 401 Unauthorized

### Test de Rate Limiting
```bash
# Ejecutar 6 veces en menos de 1 minuto
for i in {1..6}; do
  curl -X POST http://localhost:5000/auth/login \
    -H "Content-Type: application/json" \
    -d '{"usernameOrEmail":"test","password":"test12345"}';
  echo "";
done
```

**Resultado Esperado**: Primeros 5 → 401, Sexto → 429

---

## 🛡️ Mejores Prácticas de Seguridad

### En el Cliente

1. **Almacenamiento Seguro del Token**
   ```javascript
   // ❌ NO: Cookie sin flags de seguridad
   document.cookie = `token=${token}`;

   // ✅ SÍ: Cookie con httpOnly y secure
   // (esto debe hacerse desde el servidor)

   // ✅ ALTERNATIVA: localStorage (para SPAs)
   localStorage.setItem('authToken', token);
   ```

2. **HTTPS Siempre en Producción**
   ```javascript
   // Verificar protocolo en producción
   if (window.location.protocol !== 'https:' &&
       window.location.hostname !== 'localhost') {
     window.location.href = 'https:' + window.location.href.substring(window.location.protocol.length);
   }
   ```

3. **Validar Expiración del Token**
   ```javascript
   function isTokenExpired(token) {
     const payload = JSON.parse(atob(token.split('.')[1]));
     return Date.now() >= payload.exp * 1000;
   }

   if (isTokenExpired(token)) {
     // Redirigir a login
     window.location.href = '/login';
   }
   ```

4. **Logout Seguro**
   ```javascript
   function logout() {
     // Limpiar localStorage
     localStorage.removeItem('authToken');
     localStorage.removeItem('user');

     // Limpiar headers de axios
     delete axios.defaults.headers.common['Authorization'];

     // Redirigir a login
     window.location.href = '/login';
   }
   ```

---

## 📝 Notas Importantes

1. **Soporta Email Y Username**: El campo `usernameOrEmail` acepta ambos formatos

2. **Case-insensitive**: La búsqueda no distingue mayúsculas/minúsculas

3. **Mensaje genérico de error**: No revela si el usuario existe o no (seguridad)

4. **Rate limiting compartido**: Login y registro comparten el límite de 5/minuto

5. **Token incluye rol**: El JWT contiene el rol del usuario en los claims

6. **Devuelve datos completos**: Similar al registro, devuelve usuario completo

---

## 🔮 Extensiones Futuras

### 1. Refresh Tokens
```csharp
POST /auth/refresh
{
  "refreshToken": "..."
}

→ Nuevo access token sin requerir credenciales
```

### 2. Remember Me
```csharp
{
  "usernameOrEmail": "...",
  "password": "...",
  "rememberMe": true  // Token con expiración extendida
}
```

### 3. Autenticación Multi-Factor (2FA)
```csharp
POST /auth/login
→ Returns: { "requiresTwoFactor": true, "tempToken": "..." }

POST /auth/verify-2fa
{
  "tempToken": "...",
  "code": "123456"
}
→ Returns: Final token
```

### 4. Device Tracking
```csharp
// Registrar dispositivo en login
{
  "usernameOrEmail": "...",
  "password": "...",
  "device": {
    "name": "iPhone 14",
    "fingerprint": "..."
  }
}
```

---

**Implementado en**: Branch `claude/net9-postgres-continue-011CULuK32NnLRSQN34ToRgQ`
**Archivos**:
- `Auth/Controllers/AuthEndpoints.cs`
- `Services/IAuthService.cs`
- `Services/AuthService.cs`
**Fecha**: 2025-10-21
