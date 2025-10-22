# Guía de Seguridad

## 🌐 Idioma / Language

**Español** | [English](../en/SECURITY.md)

---

## 📝 Estado de Traducción

**⚠️ Traducción en progreso**

Por favor, consulta la [versión en inglés](../en/SECURITY.md) para documentación completa.

---

## 🔐 Configuración Crítica de Seguridad

### 1. Clave Secreta JWT

⚠️ **IMPORTANTE**: La clave secreta JWT debe tener al menos 32 caracteres (256 bits) para producción.

```bash
# Desarrollo (User Secrets)
dotnet user-secrets set "Jwt:Key" "tu-clave-secreta-minimo-32-caracteres"

# Producción (Variables de Entorno)
export Jwt__Key="tu-clave-produccion-minimo-32-caracteres"
```

### 2. Credenciales de Base de Datos

NUNCA hardcodear credenciales en appsettings.json. Usar:
- **Desarrollo:** User Secrets
- **Producción:** Variables de entorno o Azure Key Vault / AWS Secrets Manager

### 3. Características de Seguridad Implementadas

✅ Mass Assignment Protection  
✅ Rate Limiting  
✅ Password Hashing (BCrypt)  
✅ JWT Authentication  
✅ Role-Based Authorization  
✅ CORS Protection  
✅ Input Validation  
✅ SQL Injection Protection  

### 4. Checklist para Producción

- [ ] Configurar clave JWT segura (mínimo 32 caracteres)
- [ ] Configurar connection string de producción
- [ ] Actualizar orígenes CORS permitidos
- [ ] Cambiar o eliminar cuentas de seed por defecto
- [ ] Habilitar redirección HTTPS
- [ ] Revisar políticas de rate limiting
- [ ] Configurar health checks
- [ ] Revisar y probar políticas de autorización
- [ ] Realizar audit de seguridad

---

## Documentación Relacionada

- **[Documentación Completa en Inglés](../en/SECURITY.md)**
- **[Credenciales de Desarrollo](./SEED_DATA_CREDENTIALS.md)** 
- **[📖 Índice](./INDEX.md)**

---

**[⬅️ Volver al Índice](./INDEX.md)**
