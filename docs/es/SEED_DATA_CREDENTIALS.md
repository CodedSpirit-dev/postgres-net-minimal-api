# Credenciales de Datos de Prueba (Seed Data)

## 🌐 Idioma / Language

**Español** | [English](../en/SEED_DATA_CREDENTIALS.md)

---

## Descripción General

Este documento contiene las credenciales de todos los usuarios pre-configurados en la base de datos para testing y desarrollo. Estos usuarios se crean automáticamente al ejecutar las migraciones de Entity Framework Core.

⚠️ **IMPORTANTE**: Estas credenciales son SOLO para desarrollo. NUNCA usar en producción.

---

## 👥 Usuarios Pre-configurados

### 1. SuperAdmin

**Credenciales**:
- **Username**: `superadmin`
- **Email**: `superadmin@admin.com`
- **Password**: `yosuperadmin123`
- **Rol**: SuperAdmin

**Capacidades**:
- ✅ Acceso total al sistema
- ✅ Gestionar todos los usuarios (crear, editar, eliminar, cambiar roles)
- ✅ Gestionar todos los roles y permisos
- ✅ Moderar todo el contenido
- ✅ Ver estadísticas del sistema
- ✅ Acceso completo a la API

**Ejemplo de Login**:
```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "superadmin",
    "password": "yosuperadmin123"
  }'
```

---

### 2. Admin

**Credenciales**:
- **Username**: `admin`
- **Email**: `admin@admin.com`
- **Password**: `yoadmin123`
- **Rol**: Admin

**Capacidades**:
- ✅ Gestionar usuarios (crear, editar, NO eliminar)
- ✅ Moderar contenido (aprobar/rechazar posts, comentarios)
- ✅ Ver estadísticas
- ✅ NO puede cambiar roles de otros usuarios
- ✅ NO puede eliminar usuarios

**Ejemplo de Login**:
```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "admin",
    "password": "yoadmin123"
  }'
```

---

### 3. User (Usuario Estándar)

**Credenciales**:
- **Username**: `user`
- **Email**: `user@example.com`
- **Password**: `youser123`
- **Rol**: User

**Capacidades**:
- ✅ Crear posts propios
- ✅ Editar posts propios
- ✅ Eliminar posts propios
- ✅ Crear comentarios
- ✅ Editar comentarios propios
- ✅ Ver contenido público
- ✅ Actualizar su propio perfil
- ✅ Cambiar su propia contraseña
- ❌ NO puede editar/eliminar contenido de otros usuarios
- ❌ NO puede moderar contenido
- ❌ NO puede ver usuarios o estadísticas del sistema

**Ejemplo de Login**:
```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "user",
    "password": "youser123"
  }'
```

---

### 4. Guest (Invitado)

**Credenciales**:
- **Username**: `guest`
- **Email**: `guest@example.com`
- **Password**: `yoguest123`
- **Rol**: Guest

**Capacidades**:
- ✅ Ver contenido público
- ✅ Ver posts publicados
- ✅ Ver comentarios aprobados
- ❌ NO puede crear contenido
- ❌ NO puede comentar
- ❌ NO puede editar nada

**Ejemplo de Login**:
```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "guest",
    "password": "yoguest123"
  }'
```

---

## 🔑 Patrón de Contraseñas

Todas las contraseñas de desarrollo siguen el patrón:

```
yo{nombre_del_rol}123
```

**Ejemplos**:
- SuperAdmin: `yosuperadmin123`
- Admin: `yoadmin123`
- User: `youser123`
- Guest: `yoguest123`

Este patrón facilita recordar las contraseñas durante el desarrollo.

---

## 🧪 Ejemplos de Testing

### Test 1: Login con Email

```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "admin@admin.com",
    "password": "yoadmin123"
  }'
```

**Respuesta Esperada**:
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "...",
    "userName": "admin",
    "email": "admin@admin.com",
    "role": {
      "name": "Admin"
    }
  }
}
```

### Test 2: Login con Username

```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "user",
    "password": "youser123"
  }'
```

### Test 3: Cambiar Contraseña

```bash
# 1. Primero hacer login
TOKEN=$(curl -s -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{"usernameOrEmail":"user","password":"youser123"}' \
  | jq -r '.token')

# 2. Cambiar contraseña
curl -X POST http://localhost:5000/auth/change-password \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "currentPassword": "youser123",
    "newPassword": "MyNewPass123!",
    "confirmNewPassword": "MyNewPass123!"
  }'
```

### Test 4: Actualizar Perfil

```bash
# 1. Login y obtener token
TOKEN=$(curl -s -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{"usernameOrEmail":"user","password":"youser123"}' \
  | jq -r '.token')

# 2. Actualizar perfil
curl -X PUT http://localhost:5000/users/me \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "userName": "user",
    "firstName": "Juan",
    "lastName": "Pérez",
    "dateOfBirth": "1990-05-15",
    "email": "user@example.com"
  }'
```

---

## 🔄 Regenerar Datos de Prueba

Si necesitas regenerar la base de datos con los datos de seed:

### Opción 1: Eliminar y Recrear DB

```bash
# Eliminar la base de datos
dotnet ef database drop --force

# Recrear con seed data
dotnet ef database update
```

### Opción 2: Nueva Migración

```bash
# Crear nueva migración
dotnet ef migrations add UpdateSeedDataCredentials

# Aplicar migración
dotnet ef database update
```

---

## 🛡️ Seguridad

### ⚠️ Advertencias Importantes

1. **NUNCA uses estas credenciales en producción**
2. **Cambia TODAS las contraseñas antes de deploy**
3. **Los hashes BCrypt están pre-generados para desarrollo**
4. **El prefijo 'yo' es solo para desarrollo**

### 🔒 Para Producción

Antes de ir a producción:

1. **Eliminar todos los usuarios de seed**
   ```csharp
   // Comentar o eliminar todo el bloque de seed data en AppDbContext.cs
   ```

2. **Crear el primer SuperAdmin manualmente**
   ```bash
   POST /auth/register
   # Luego cambiar su rol a SuperAdmin via SQL
   ```

3. **Usar contraseñas fuertes**
   - Mínimo 12 caracteres
   - Mayúsculas, minúsculas, números, símbolos
   - Sin patrones predecibles

4. **Habilitar 2FA** (cuando esté implementado)

5. **Implementar políticas de contraseñas**
   - Expiración periódica
   - Historial de contraseñas
   - Bloqueo después de intentos fallidos

---

## 🧪 Testing de Permisos

### Test 1: SuperAdmin puede todo

```bash
# Login como SuperAdmin
TOKEN=$(curl -s -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{"usernameOrEmail":"superadmin","password":"yosuperadmin123"}' \
  | jq -r '.token')

# Listar todos los usuarios (requiere permisos)
curl -X GET http://localhost:5000/api/users \
  -H "Authorization: Bearer $TOKEN"
```

### Test 2: User NO puede listar usuarios

```bash
# Login como User
USER_TOKEN=$(curl -s -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{"usernameOrEmail":"user","password":"youser123"}' \
  | jq -r '.token')

# Intentar listar usuarios (debe fallar con 403)
curl -X GET http://localhost:5000/api/users \
  -H "Authorization: Bearer $USER_TOKEN"
```

---

## 📊 Jerarquía de Roles

```
SuperAdmin (Nivel 0) - TODOS los permisos
    ↓
Admin (Nivel 1) - Gestión + Moderación
    ↓
User (Nivel 2) - Crear contenido propio
    ↓
Guest (Nivel 3) - Solo lectura
```

---

## 📝 Notas de Desarrollo

### IDs Fijos

Los usuarios tienen GUIDs fijos para facilitar testing:

```csharp
SuperAdmin: S0000000-0000-0000-0000-000000000000
Admin:      A1111111-1111-1111-1111-111111111111
User:       B2222222-2222-2222-2222-222222222222
Guest:      C3333333-3333-3333-3333-333333333333
```

### Consistencia

Todos los usuarios siguen el mismo patrón:
- Username en lowercase
- Email con dominio coherente
- Password con patrón "yo{role}123"
- Perfiles completos con información realista

---

## Documentación Relacionada

- **[Endpoint de Login](./LOGIN_ENDPOINT.md)** - Usa estas credenciales para probar login
- **[Endpoint de Registro](./REGISTRATION_ENDPOINT.md)** - Crear nuevos usuarios de prueba
- **[Sistema de Blog](./BLOG_SYSTEM.md)** - Sistema completo de permisos RBAC
- **[Seguridad](./SECURITY.md)** - Mejores prácticas de seguridad
- **[📖 Índice de Documentación](./INDEX.md)** - Volver al índice principal

---

**Fecha de creación**: 2025-10-21
**Última actualización**: 2025-10-22
**Versión del sistema**: .NET 9 + PostgreSQL

---

**[⬅️ Volver al Índice de Documentación](./INDEX.md)**
