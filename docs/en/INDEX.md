# Documentation - PostgreSQL .NET 9 Minimal API

Master index for all project documentation.

---

## 🌐 Language / Idioma

**English** | [Español](../es/INDEX.md)

---

## 📚 Table of Contents

### 🏗️ Arquitectura y Diseño

- **[Blog System](./BLOG_SYSTEM.md)** - Descripción completa del sistema de blog con RBAC granular
- **[Security](./SECURITY.md)** - Consideraciones de seguridad y mejores prácticas
- **[Improvements](./IMPROVEMENTS.md)** - Mejoras sugeridas y roadmap del proyecto
- **[Code Review Report](./CODE_REVIEW_REPORT.md)** - Reporte de revisión de código y análisis

---

### 🔐 Autenticación y Autorización

#### Endpoints de Autenticación

- **[Registration Endpoint](./REGISTRATION_ENDPOINT.md)** - `POST /auth/register`
  - Registro público de usuarios
  - Asignación automática del rol "User"
  - Retorna JWT token + datos del usuario
  - Rate limited (5 requests/minuto)

- **[Login Endpoint](./LOGIN_ENDPOINT.md)** - `POST /auth/login`
  - Autenticación de usuarios
  - Login con username o email
  - Retorna JWT token + datos completos del usuario
  - Rate limited (5 requests/minuto)

- **[Change Password Endpoint](./CHANGE_PASSWORD_ENDPOINT.md)** - `POST /auth/change-password`
  - Cambio de contraseña para usuarios autenticados
  - Requiere verificación de contraseña actual
  - Confirmación de nueva contraseña
  - Autenticación JWT requerida

---

### 👤 Gestión de Usuarios

- **[Update Profile Endpoint](./UPDATE_PROFILE_ENDPOINT.md)** - `PUT /users/me`
  - Actualización de perfil propio
  - Usuarios pueden editar: username, nombre, email, fecha de nacimiento
  - **Restricción de seguridad**: Los usuarios NO pueden cambiar su propio rol
  - Validación de unicidad de email y username

---

### 🗄️ Datos de Desarrollo

- **[Seed Data Credentials](./SEED_DATA_CREDENTIALS.md)** - Credenciales de usuarios pre-configurados
  - SuperAdmin, Admin, User, Guest
  - Patrón de contraseñas: `yo{role}123`
  - Ejemplos de uso con cURL y otras herramientas
  - Permisos y capacidades de cada rol

---

## 🚀 Inicio Rápido

### Para Desarrolladores

1. **Configuración Inicial**: Lee [README.md](../README.md) en la raíz del proyecto
2. **Sistema de Blog**: Revisa [BLOG_SYSTEM.md](./BLOG_SYSTEM.md) para entender la arquitectura
3. **Credenciales de Prueba**: Consulta [SEED_DATA_CREDENTIALS.md](./SEED_DATA_CREDENTIALS.md)

### Para Testing de API

1. **Registro**: [REGISTRATION_ENDPOINT.md](./REGISTRATION_ENDPOINT.md)
2. **Login**: [LOGIN_ENDPOINT.md](./LOGIN_ENDPOINT.md)
3. **Cambio de Contraseña**: [CHANGE_PASSWORD_ENDPOINT.md](./CHANGE_PASSWORD_ENDPOINT.md)
4. **Actualizar Perfil**: [UPDATE_PROFILE_ENDPOINT.md](./UPDATE_PROFILE_ENDPOINT.md)

---

## 📋 Guías por Caso de Uso

### Caso 1: Implementar Autenticación en Cliente

```
1. Implementar registro → REGISTRATION_ENDPOINT.md
2. Implementar login → LOGIN_ENDPOINT.md
3. Almacenar JWT token
4. Usar token en headers: Authorization: Bearer {token}
```

### Caso 2: Gestión de Perfil de Usuario

```
1. Login y obtener token → LOGIN_ENDPOINT.md
2. Ver perfil → GET /users/{id} (ver README principal)
3. Actualizar perfil → UPDATE_PROFILE_ENDPOINT.md
4. Cambiar contraseña → CHANGE_PASSWORD_ENDPOINT.md
```

### Caso 3: Testing con Usuarios de Desarrollo

```
1. Ver credenciales → SEED_DATA_CREDENTIALS.md
2. Login como SuperAdmin, Admin, User, o Guest
3. Probar diferentes niveles de permisos
```

---

## 🔒 Seguridad

Para información detallada sobre seguridad, consulta:
- **[SECURITY.md](./SECURITY.md)** - Guía completa de seguridad
- Autenticación JWT en todos los documentos de endpoints
- RBAC y permisos granulares en [BLOG_SYSTEM.md](./BLOG_SYSTEM.md)

---

## 🛠️ Mejoras Futuras

Consulta **[IMPROVEMENTS.md](./IMPROVEMENTS.md)** para:
- Roadmap del proyecto
- Mejoras sugeridas
- Nuevas características planificadas

---

## 📝 Reportes y Análisis

- **[Code Review Report](./CODE_REVIEW_REPORT.md)** - Análisis de código y recomendaciones

---

## 🎯 Stack Tecnológico

- **.NET 9** con **C# 13**
- **PostgreSQL** con **Npgsql**
- **Entity Framework Core 9**
- **Minimal APIs** (ASP.NET Core)
- **JWT Bearer Authentication**
- **BCrypt** para hashing de contraseñas
- **RBAC Granular** (Resource-Action permissions)

---

## 📞 Soporte

Si necesitas ayuda con algún aspecto específico:
1. Revisa el documento correspondiente en este índice
2. Consulta el README.md principal en la raíz del proyecto
3. Revisa los ejemplos de código en cada documento de endpoint

---

**Última actualización**: Octubre 2025
**Proyecto**: PostgreSQL .NET 9 Minimal API - Blog System with Granular RBAC
