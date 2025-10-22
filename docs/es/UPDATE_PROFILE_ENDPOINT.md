# Endpoint de Actualización de Perfil

## 🌐 Idioma / Language

**Español** | [English](../en/UPDATE_PROFILE_ENDPOINT.md)

---

## 📝 Estado de Traducción

**⚠️ Traducción en progreso**

Por favor, consulta la [versión en inglés](../en/UPDATE_PROFILE_ENDPOINT.md) para documentación completa mientras completamos la traducción al español.

---

## Resumen Rápido

**Endpoint**: `PUT /users/me`  
**Autenticación**: Requerida (JWT Bearer token)  
**Descripción**: Permite a usuarios autenticados actualizar su propio perfil

### Campos Actualizables

- ✅ Username
- ✅ Nombre (First, Middle, Last)
- ✅ Email
- ✅ Fecha de nacimiento
- ❌ Rol (NO se puede cambiar - seguridad)

### Ejemplo con cURL

```bash
curl -X PUT http://localhost:5000/users/me \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TU_JWT_TOKEN" \
  -d '{
    "userName": "juanperez",
    "firstName": "Juan",
    "lastName": "Pérez",
    "dateOfBirth": "1990-05-15",
    "email": "juan.perez@example.com"
  }'
```

---

## Documentación Relacionada

- **[Documentación Completa en Inglés](../en/UPDATE_PROFILE_ENDPOINT.md)** - Complete documentation
- **[Endpoint de Cambio de Contraseña](./CHANGE_PASSWORD_ENDPOINT.md)** - Para cambiar contraseña
- **[Endpoint de Login](./LOGIN_ENDPOINT.md)** - Para obtener un token JWT
- **[📖 Índice de Documentación](./INDEX.md)** - Volver al índice principal

---

**[⬅️ Volver al Índice de Documentación](./INDEX.md)**
