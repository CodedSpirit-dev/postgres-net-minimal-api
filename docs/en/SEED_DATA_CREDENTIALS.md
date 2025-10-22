# 🔑 Seed Data - Credenciales de Desarrollo

## ⚠️ IMPORTANTE

**Estas credenciales son SOLO para desarrollo y testing.**
**NUNCA uses estas credenciales en producción.**

---

## 👥 Usuarios por Defecto

El sistema incluye 4 usuarios de ejemplo con diferentes niveles de permisos.

### 1. SuperAdmin (Acceso Total)

**Credenciales:**
```
Username: superadmin
Email:    superadmin@admin.com
Password: yosuperadmin123
Rol:      SuperAdmin
```

**Permisos:**
- ✅ **TODOS los permisos del sistema**
- ✅ Crear, editar, eliminar usuarios
- ✅ Gestionar roles y permisos
- ✅ Publicar y moderar contenido
- ✅ Aprobar/rechazar comentarios
- ✅ Acceso a estadísticas
- ✅ Gestión completa del blog

**Uso recomendado:**
- Configuración inicial del sistema
- Gestión de usuarios y roles
- Tareas administrativas críticas
- Desarrollo y testing de funciones admin

---

### 2. Admin (Administrador)

**Credenciales:**
```
Username: admin
Email:    admin@admin.com
Password: yoadmin123
Rol:      Admin
```

**Permisos:**
- ✅ Ver usuarios
- ✅ Gestión completa de posts
- ✅ Gestión de categorías y tags
- ✅ Moderar comentarios (aprobar/rechazar)
- ✅ Ver estadísticas
- ❌ NO puede modificar permisos
- ❌ NO puede eliminar usuarios

**Uso recomendado:**
- Gestión de contenido
- Moderación de comentarios
- Testing de permisos de admin

---

### 3. User (Usuario Estándar)

**Credenciales:**
```
Username: user
Email:    user@example.com
Password: youser123
Rol:      User
```

**Permisos:**
- ✅ Ver su propio perfil
- ✅ Crear posts propios
- ✅ Editar sus propios posts
- ✅ Eliminar sus propios posts
- ✅ Publicar/despublicar sus propios posts
- ✅ Crear comentarios
- ✅ Editar sus propios comentarios
- ✅ Eliminar sus propios comentarios
- ❌ NO puede editar posts de otros
- ❌ NO puede aprobar comentarios
- ❌ NO puede gestionar usuarios

**Uso recomendado:**
- Testing de funcionalidad de usuario normal
- Creación de contenido
- Verificar permisos de instancia-level

---

### 4. Guest (Invitado)

**Credenciales:**
```
Username: guest
Email:    guest@example.com
Password: yoguest123
Rol:      Guest
```

**Permisos:**
- ✅ Ver contenido público
- ❌ NO puede crear posts
- ❌ NO puede comentar
- ❌ NO puede modificar nada

**Uso recomendado:**
- Testing de permisos mínimos
- Verificar acceso de solo lectura

---

## 🔐 Patrón de Contraseñas

Todas las contraseñas siguen el patrón:

```
yo + nombre_del_rol + 123
```

**Ejemplos:**
- SuperAdmin → `yosuperadmin123`
- Admin → `yoadmin123`
- User → `youser123`
- Guest → `yoguest123`

**Nota:** El prefijo "yo" facilita recordar que son contraseñas de desarrollo.

---

## 📋 Tabla Resumen

| Rol | Username | Email | Password | Permisos |
|-----|----------|-------|----------|----------|
| **SuperAdmin** | superadmin | superadmin@admin.com | yosuperadmin123 | TODOS |
| **Admin** | admin | admin@admin.com | yoadmin123 | Gestión + Moderación |
| **User** | user | user@example.com | youser123 | Crear contenido propio |
| **Guest** | guest | guest@example.com | yoguest123 | Solo lectura |

---

## 🚀 Uso Rápido

### Login con SuperAdmin

**cURL:**
```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "superadmin",
    "password": "yosuperadmin123"
  }'
```

**JavaScript:**
```javascript
const response = await fetch('http://localhost:5000/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    usernameOrEmail: 'superadmin',
    password: 'yosuperadmin123'
  })
});

const { token, user } = await response.json();
console.log('SuperAdmin autenticado:', user);
```

### Login con Admin

```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "admin",
    "password": "yoadmin123"
  }'
```

### Login con User

```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "user",
    "password": "youser123"
  }'
```

---

## 🗂️ Contenido de Seed Data

### Posts Creados

El sistema incluye **4 posts de ejemplo**:

1. **"Getting Started with .NET 9"**
   - Autor: Admin
   - Estado: Publicado
   - Categoría: Technology
   - Tags: C#, .NET

2. **"Building RESTful APIs with PostgreSQL"**
   - Autor: Admin
   - Estado: Publicado
   - Categoría: Tutorials
   - Tags: PostgreSQL, .NET

3. **"Understanding RBAC Permissions"**
   - Autor: User (John Doe)
   - Estado: Publicado
   - Categoría: Technology
   - Tags: C#

4. **"Draft Post: Advanced Topics"**
   - Autor: User (John Doe)
   - Estado: BORRADOR (no publicado)
   - Categoría: Tutorials
   - Tags: .NET, PostgreSQL

### Comentarios Creados

El sistema incluye **5 comentarios de ejemplo**:

- 2 comentarios aprobados en "Getting Started with .NET 9"
- 1 comentario aprobado en "Building RESTful APIs"
- 1 respuesta a comentario (nested comment)
- 1 comentario PENDIENTE de aprobación

### Perfiles Creados

Todos los usuarios tienen perfiles completos con:
- Bio
- Imagen de avatar (URL de ejemplo)
- Links a redes sociales (Twitter, GitHub)

---

## 🔄 Regenerar Seed Data

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

### Test 3: Admin puede moderar comentarios

```bash
# Login como Admin
ADMIN_TOKEN=$(curl -s -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{"usernameOrEmail":"admin","password":"yoadmin123"}' \
  | jq -r '.token')

# Ver comentarios pendientes
curl -X GET http://localhost:5000/api/comments/pending \
  -H "Authorization: Bearer $ADMIN_TOKEN"

# Aprobar un comentario
curl -X POST http://localhost:5000/api/comments/{id}/approve \
  -H "Authorization: Bearer $ADMIN_TOKEN"
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

## 🔧 Configuración de Hashes BCrypt

Los hashes BCrypt en el seed data son pre-generados con:
- **Work factor**: 11 (2^11 = 2,048 iterations)
- **Salt**: Generado automáticamente por BCrypt
- **Algoritmo**: BCrypt 2a

### ⚠️ NOTA IMPORTANTE

Los hashes BCrypt mostrados en `AppDbContext.cs` son **placeholders**.
Al ejecutar la migración, los hashes reales se generarán automáticamente
con las contraseñas especificadas arriba.

Si los hashes no funcionan, puedes regenerarlos ejecutando:

```bash
# Crear migración para actualizar contraseñas
dotnet ef migrations add UpdateUserPasswords
dotnet ef database update
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

## Related Documentation

- **[Login Endpoint](./LOGIN_ENDPOINT.md)** - Use these credentials to test login
- **[Registration Endpoint](./REGISTRATION_ENDPOINT.md)** - Create new test users
- **[Blog System](./BLOG_SYSTEM.md)** - Complete RBAC permission system
- **[Security](./SECURITY.md)** - Security best practices
- **[📖 Documentation Index](./INDEX.md)** - Return to main documentation index

---

**Fecha de creación**: 2025-10-21
**Última actualización**: 2025-10-21
**Versión del sistema**: .NET 9 + PostgreSQL

---

**[⬅️ Back to Documentation Index](./INDEX.md)**
