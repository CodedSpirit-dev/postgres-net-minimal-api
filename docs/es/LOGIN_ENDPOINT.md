# Endpoint de Login

## 🌐 Idioma / Language

**Español** | [English](../en/LOGIN_ENDPOINT.md)

---

## 📝 Estado de Traducción

**⚠️ Traducción en progreso**

Por favor, consulta la [versión en inglés](../en/LOGIN_ENDPOINT.md) para documentación completa.

---

## Resumen Rápido

**Endpoint**: `POST /auth/login`  
**Autenticación**: No requerida  
**Rate Limit**: 5 requests/minuto

### Request

```json
{
  "usernameOrEmail": "admin",
  "password": "yoadmin123"
}
```

### Ejemplo

```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{"usernameOrEmail":"admin","password":"yoadmin123"}'
```

### Respuesta (200 OK)

```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": { ... }
}
```

---

## Credenciales de Prueba

Ver [SEED_DATA_CREDENTIALS.md](./SEED_DATA_CREDENTIALS.md) para todas las credenciales de desarrollo.

---

## Documentación Relacionada

- **[Documentación Completa en Inglés](../en/LOGIN_ENDPOINT.md)** 
- **[Credenciales de Prueba](./SEED_DATA_CREDENTIALS.md)**
- **[📖 Índice](./INDEX.md)**

---

**[⬅️ Volver al Índice](./INDEX.md)**
