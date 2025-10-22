# Endpoint de Registro

## 🌐 Idioma / Language

**Español** | [English](../en/REGISTRATION_ENDPOINT.md)

---

## 📝 Estado de Traducción

**⚠️ Traducción en progreso**

Por favor, consulta la [versión en inglés](../en/REGISTRATION_ENDPOINT.md) para documentación completa.

---

## Resumen Rápido

**Endpoint**: `POST /auth/register`  
**Autenticación**: No requerida  
**Rate Limit**: 5 requests/minuto  
**Rol asignado**: User (automático)

### Ejemplo

```bash
curl -X POST http://localhost:5000/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "nuevouser",
    "firstName": "Juan",
    "lastName": "Pérez",
    "dateOfBirth": "1990-05-15",
    "email": "juan@example.com",
    "password": "MiPassword123!"
  }'
```

### Respuesta (201 Created)

Retorna JWT token + datos de usuario automáticamente.

---

## Documentación Relacionada

- **[Documentación Completa en Inglés](../en/REGISTRATION_ENDPOINT.md)**
- **[Endpoint de Login](./LOGIN_ENDPOINT.md)**
- **[📖 Índice](./INDEX.md)**

---

**[⬅️ Volver al Índice](./INDEX.md)**
