# Endpoint de Cambio de Contraseña

## 🌐 Idioma / Language

**Español** | [English](../en/CHANGE_PASSWORD_ENDPOINT.md)

---

## 📝 Estado de Traducción

**⚠️ Traducción en progreso**

Por favor, consulta la [versión en inglés](../en/CHANGE_PASSWORD_ENDPOINT.md) para documentación completa mientras completamos la traducción al español.

---

## Resumen Rápido

**Endpoint**: `POST /auth/change-password`  
**Autenticación**: Requerida (JWT Bearer token)  
**Rate Limit**: No

### Request Body

```json
{
  "currentPassword": "tu_contraseña_actual",
  "newPassword": "tu_nueva_contraseña",
  "confirmNewPassword": "tu_nueva_contraseña"
}
```

### Ejemplo con cURL

```bash
curl -X POST http://localhost:5000/auth/change-password \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TU_JWT_TOKEN" \
  -d '{
    "currentPassword": "youser123",
    "newPassword": "NuevaContraseña123!",
    "confirmNewPassword": "NuevaContraseña123!"
  }'
```

### Respuesta Exitosa (200 OK)

```json
{
  "success": true,
  "message": "Password changed successfully"
}
```

### Errores Comunes

| Código | Descripción |
|--------|-------------|
| 400    | Contraseña actual incorrecta o validación fallida |
| 401    | Token JWT no válido o faltante |

---

## Documentación Relacionada

- **[Documentación Completa en Inglés](../en/CHANGE_PASSWORD_ENDPOINT.md)** - Complete documentation
- **[Endpoint de Login](./LOGIN_ENDPOINT.md)** - Para obtener un nuevo token JWT
- **[Endpoint de Actualización de Perfil](./UPDATE_PROFILE_ENDPOINT.md)** - Para actualizar información del perfil
- **[Credenciales de Prueba](./SEED_DATA_CREDENTIALS.md)** - Credenciales de usuarios de desarrollo
- **[📖 Índice de Documentación](./INDEX.md)** - Volver al índice principal

---

**[⬅️ Volver al Índice de Documentación](./INDEX.md)**
