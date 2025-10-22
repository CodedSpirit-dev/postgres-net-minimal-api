# API Minimal PostgreSQL .NET

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-336791?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Habilitado-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![License](https://img.shields.io/badge/Licencia-MIT-green?style=for-the-badge)

---

## 🌐 Language / Idioma

[English](./README.md) | **Español**

📖 **Documentación**: [English](./docs/en/INDEX.md) | [Español](./docs/es/INDEX.md)

---

Una API REST lista para producción construida con .NET 9, PostgreSQL y Entity Framework Core para gestión de usuarios y roles. Incluye autenticación JWT, soporte Docker y documentación completa de la API.

## 📑 Tabla de Contenidos

- [Características](#-características)
- [Requisitos Previos](#-requisitos-previos)
- [Inicio Rápido](#-inicio-rápido)
- [Instalación y Configuración](#️-instalación-y-configuración)
- [Documentación de la API](#-documentación-de-la-api)
- [Estructura del Proyecto](#️-estructura-del-proyecto)
- [Comandos de Desarrollo](#-comandos-de-desarrollo)
- [Lista de Tareas](#-lista-de-tareas)
- [Seguridad](#-seguridad-y-variables-de-entorno)
- [Solución de Problemas](#-solución-de-problemas)
- [Tecnologías Utilizadas](#️-tecnologías-utilizadas)

## 🚀 Características

- ✅ **Autenticación JWT** - Autenticación basada en tokens con hashing de contraseñas BCrypt
- ✅ **Gestión de Usuarios y Roles** - Operaciones CRUD completas para usuarios y roles
- ✅ **Base de Datos PostgreSQL** - Base de datos relacional lista para producción
- ✅ **Entity Framework Core** - Enfoque code-first con migraciones
- ✅ **Soporte Docker** - Configuración Docker Compose para despliegue fácil
- ✅ **Variables de Entorno** - Configuración segura con archivos .env
- ✅ **Swagger/OpenAPI** - Documentación interactiva de la API
- ✅ **Configuración CORS** - Compartición de recursos de origen cruzado habilitada
- ✅ **Minimal API** - Patrón moderno de API minimal de .NET 9
- ✅ **Datos de Prueba** - Usuarios y roles pre-configurados para testing

## 📋 Requisitos Previos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (recomendado)
- [Git](https://git-scm.com/)
- Opcional: [DBeaver](https://dbeaver.io/) o [pgAdmin](https://www.pgadmin.org/) para gestión de base de datos

## ⚡ Inicio Rápido

```bash
# 1. Clonar repositorio
git clone <repository-url>
cd postgres-net-minimal-api

# 2. Configurar entorno
cp .env.example .env

# 3. Iniciar PostgreSQL
docker-compose up -d

# 4. Ejecutar migraciones
dotnet ef database update

# 5. Ejecutar aplicación
dotnet run
```

Acceder a la API en: **http://localhost:5174**
Swagger UI en: **http://localhost:5174**

## 🛠️ Instalación y Configuración

### 1. Clonar Repositorio
```bash
git clone <repository-url>
cd postgres-net-minimal-api
```

### 2. Instalar Dependencias
```bash
dotnet restore
```

### 3. Configurar Variables de Entorno
Copiar el archivo de ejemplo y configurar tus credenciales:
```bash
cp .env.example .env
```

Editar el archivo `.env` con tu configuración:
```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=tu_contraseña_segura
POSTGRES_DB=postgres_db
POSTGRES_PORT=5433

JWT_KEY=tu_clave_jwt_secreta_minimo_32_caracteres
JWT_ISSUER=tu_api_minimal
JWT_AUDIENCE=clientes_de_tu_api
```

### 4. Configurar Base de Datos PostgreSQL

#### Opción A: Usando Docker Compose (Recomendado)
```bash
docker-compose up -d
```

Esto iniciará PostgreSQL en un contenedor con la configuración de tu archivo `.env`.

#### Opción B: Usando Docker Run
```bash
docker run --name postgres17_minimal_api \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=postgres_db \
  -p 5433:5432 \
  -v postgres17_data:/var/lib/postgresql/data \
  -d postgres:17
```

#### Opción C: Instalación Local de PostgreSQL
Instalar PostgreSQL localmente y crear una base de datos llamada `postgres_db`.

### 5. Ejecutar Migraciones de Base de Datos

Instalar herramientas de EF Core si no están instaladas:
```bash
dotnet tool install --global dotnet-ef
```

Aplicar migraciones para crear tablas:
```bash
dotnet ef database update
```

La migración creará:
- Tabla `Users` con datos de prueba (Admin y User)
- Tabla `UserRoles` con datos de prueba (SuperAdmin, Admin, User, Guest)

### 6. Ejecutar la Aplicación
```bash
dotnet run
```

La API estará disponible en: `http://localhost:5174`

### 7. Probar con Datos de Prueba

La migración crea automáticamente usuarios y roles de prueba:

**Usuarios:**
- **SuperAdmin**
  - Email: `superadmin@admin.com`
  - Contraseña: `yosuperadmin123`
  - Rol: SuperAdmin

- **Admin**
  - Email: `admin@admin.com`
  - Contraseña: `yoadmin123`
  - Rol: Admin

- **Usuario Estándar**
  - Email: `user@example.com`
  - Contraseña: `youser123`
  - Rol: User

- **Invitado**
  - Email: `guest@example.com`
  - Contraseña: `yoguest123`
  - Rol: Guest

**Roles:**
- SuperAdmin - Acceso completo al sistema
- Admin - Gestión y moderación
- User - Acceso de usuario estándar
- Guest - Acceso limitado

## 📚 Documentación de la API

### 📖 Documentación Completa

Para documentación completa y detallada del proyecto, consulta la **[Carpeta de Documentación (docs/)](./docs/es/INDEX.md)**:

- **[📚 Índice Maestro de Documentación](./docs/es/INDEX.md)** - Guía completa con enlaces a toda la documentación
- **[🔐 Autenticación y Endpoints](./docs/es/INDEX.md#-autenticación-y-autorización)** - Login, registro, cambio de contraseña
- **[📝 Sistema de Blog](./docs/es/BLOG_SYSTEM.md)** - Arquitectura RBAC granular
- **[👥 Credenciales de Desarrollo](./docs/es/SEED_DATA_CREDENTIALS.md)** - Usuarios y contraseñas de prueba
- **[🔒 Seguridad](./docs/es/SECURITY.md)** - Mejores prácticas de seguridad

**English**: See [English Documentation](./docs/en/INDEX.md)

### Swagger UI
Acceder a la documentación interactiva de la API en: **http://localhost:5174**

El Swagger UI te permite:
- Explorar todos los endpoints disponibles
- Probar llamadas a la API directamente desde el navegador
- Ver esquemas de request/response
- Autenticarte con tokens JWT

### Resumen de Endpoints de la API

#### Endpoints de Autenticación

| Método | Endpoint | Descripción | Autenticación Requerida | Documentación |
|--------|----------|-------------|------------------------|---------------|
| POST | `/auth/register` | Registrar nuevo usuario (rol "User" automático) | No | [📄 Docs](./docs/es/REGISTRATION_ENDPOINT.md) |
| POST | `/auth/login` | Autenticar usuario y obtener token JWT | No | [📄 Docs](./docs/es/LOGIN_ENDPOINT.md) |
| POST | `/auth/change-password` | Cambiar contraseña de usuario | Sí | [📄 Docs](./docs/es/CHANGE_PASSWORD_ENDPOINT.md) |
| POST | `/auth/logout` | Logout (descartar token en cliente) | Sí | - |

#### Endpoints de Gestión de Usuarios

| Método | Endpoint | Descripción | Autenticación Requerida | Documentación |
|--------|----------|-------------|------------------------|---------------|
| GET | `/users` | Obtener todos los usuarios con paginación | No | - |
| GET | `/users/{id}` | Obtener usuario por ID | No | - |
| POST | `/users` | Crear nuevo usuario (registro público) | No | - |
| PUT | `/users/me` | Actualizar perfil propio | Sí | [📄 Docs](./docs/es/UPDATE_PROFILE_ENDPOINT.md) |
| PUT | `/users/{id}` | Actualizar usuario (Solo Admin) | Sí (Admin) | - |
| DELETE | `/users/{id}` | Eliminar usuario (Solo Admin) | Sí (Admin) | - |

#### Endpoints de Gestión de Roles

| Método | Endpoint | Descripción | Autenticación Requerida |
|--------|----------|-------------|------------------------|
| GET | `/roles` | Obtener todos los roles | No |
| GET | `/roles/{id}` | Obtener rol por ID | No |

**Credenciales de prueba**: Ver [SEED_DATA_CREDENTIALS.md](./docs/es/SEED_DATA_CREDENTIALS.md)

## 🗂️ Estructura del Proyecto

```
postgres-net-minimal-api/
├── Controllers/
│   ├── AuthEndpoints.cs
│   └── UsersEndpoints.cs
├── Data/
│   └── AppDbContext.cs
├── Models/
│   ├── User.cs
│   └── UserRole.cs
├── Migrations/
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── docker-compose.yml          # Configuración Docker Compose
├── .env                         # Variables de entorno (no en git)
├── .env.example                 # Plantilla de entorno
├── .gitignore                   # Reglas de ignore de Git
├── README.md                    # Documentación en inglés
├── README-ES.md                 # Documentación en español
└── docs/
    ├── en/                      # Documentación en inglés
    └── es/                      # Documentación en español
```

## 🔧 Comandos de Desarrollo

### Gestión de PostgreSQL con Docker

#### Usando Docker Compose
```bash
# Iniciar contenedor PostgreSQL
docker-compose up -d

# Detener contenedor PostgreSQL
docker-compose down

# Ver logs del contenedor
docker-compose logs -f postgres

# Reiniciar contenedor PostgreSQL
docker-compose restart

# Conectar al CLI de PostgreSQL
docker-compose exec postgres psql -U postgres -d postgres_db
```

### Comandos de Migración de Base de Datos

#### Instalar Herramientas de EF Core
```bash
dotnet tool install --global dotnet-ef
```

#### Aplicar Migraciones
```bash
dotnet ef database update
```

#### Ver Migraciones Aplicadas
```bash
dotnet ef migrations list
```

#### Crear Nueva Migración
```bash
dotnet ef migrations add NombreMigracion
```

#### Remover Última Migración
```bash
dotnet ef migrations remove
```

#### Resetear Base de Datos (Limpieza Completa)
```bash
dotnet ef database drop --force && rm -rf Migrations/ && dotnet ef migrations add InitialCreate && dotnet ef database update
```

## 📝 Lista de Tareas

### ✅ Completado
- [x] Generación y validación de tokens JWT
- [x] Hashing de contraseñas con BCrypt
- [x] Endpoints de gestión de usuarios y roles
- [x] Documentación Swagger/OpenAPI
- [x] Containerización con Docker Compose
- [x] Configuración de variables de entorno
- [x] Migraciones de base de datos y datos de prueba
- [x] Configuración CORS
- [x] Funcionalidad de cambio de contraseña
- [x] Actualización de perfil de usuario
- [x] Registro público de usuarios
- [x] Documentación bilingüe (Inglés/Español)

### Alta Prioridad
- [ ] Agregar middleware de autenticación adecuado a endpoints protegidos
- [ ] Agregar validación de entrada y manejo de errores comprensivo
- [ ] Implementar autorización basada en roles con políticas
- [ ] Agregar logging de requests/responses

### Prioridad Media
- [ ] Crear endpoints dedicados de gestión de roles (POST, PUT, DELETE)
- [ ] Agregar funcionalidad de reseteo de contraseña
- [ ] Implementar verificación de email
- [ ] Agregar logging estructurado con Serilog
- [ ] Agregar búsqueda y filtrado

### Prioridad Baja
- [ ] Implementar rate limiting global
- [ ] Agregar pruebas unitarias e de integración
- [ ] Agregar endpoints de health check
- [ ] Implementar audit logging
- [ ] Agregar versionado de API
- [ ] Crear Dockerfile para la API

### Iteraciones Futuras
- [ ] Agregar mecanismo de refresh tokens
- [ ] Implementar OAuth2/OpenID Connect
- [ ] Agregar autenticación de dos factores (2FA)
- [ ] Crear dashboard de administración
- [ ] Agregar capacidades de subida de archivos
- [ ] Implementar notificaciones en tiempo real con SignalR
- [ ] Agregar caché con Redis
- [ ] Implementar trabajos en segundo plano con Hangfire

## 🔒 Seguridad y Variables de Entorno

### Archivo de Entorno (.env)
- ⚠️ **NUNCA commitear `.env` al control de versiones**
- El archivo `.env` contiene credenciales sensibles
- Siempre usar `.env.example` como plantilla para nuevos desarrolladores
- Usar credenciales diferentes para desarrollo y producción

### Prioridad de Configuración
La aplicación carga la configuración en este orden (las fuentes posteriores sobreescriben las anteriores):
1. `appsettings.json` (configuración base)
2. `appsettings.{Environment}.json` (específico del entorno)
3. Archivo `.env` (cargado por DotNetEnv)
4. Variables de entorno
5. Argumentos de línea de comandos

### Despliegue en Producción
Para entornos de producción:
- Usar variables de entorno en lugar del archivo `.env`
- Almacenar secrets en vaults seguros (Azure Key Vault, AWS Secrets Manager, etc.)
- Nunca usar contraseñas por defecto
- Generar claves JWT fuertes (mínimo 32 caracteres)

## ❓ Solución de Problemas

### Problemas Comunes

**Problema: No se puede conectar a PostgreSQL**
```bash
# Verificar si el contenedor está corriendo
docker ps

# Verificar logs del contenedor
docker-compose logs postgres

# Reiniciar contenedor
docker-compose restart
```

**Problema: Herramientas de EF Core no encontradas**
```bash
# Instalar globalmente
dotnet tool install --global dotnet-ef

# Verificar instalación
dotnet ef --version
```

**Problema: Falla la migración**
```bash
# Eliminar y recrear base de datos
dotnet ef database drop --force
dotnet ef database update
```

**Problema: Puerto 5433 ya en uso**
```bash
# Verificar qué está usando el puerto
lsof -i :5433  # Mac/Linux
netstat -ano | findstr :5433  # Windows

# Cambiar puerto en archivo .env
POSTGRES_PORT=5434
```

## 🤝 Contribuir

Este es un proyecto de demostración para desarrollo futuro. Siéntete libre de:
1. Fork el repositorio
2. Crear ramas de características
3. Enviar pull requests
4. Reportar issues y sugerencias

## 📄 Licencia

Este proyecto está licenciado bajo la Licencia MIT - ver el archivo LICENSE para detalles.

## 🛠️ Tecnologías Utilizadas

### Backend
- **.NET 9** - Última versión del framework .NET
- **ASP.NET Core Minimal API** - Framework de API ligero
- **Entity Framework Core 9** - ORM para operaciones de base de datos
- **Npgsql** - Proveedor PostgreSQL para EF Core

### Base de Datos
- **PostgreSQL 17** - Base de datos relacional avanzada de código abierto
- **Docker** - Plataforma de containerización

### Seguridad
- **JWT Bearer Authentication** - Autenticación basada en tokens
- **BCrypt.Net** - Librería de hashing de contraseñas

### Herramientas y Librerías
- **Swashbuckle (Swagger)** - Documentación de API
- **DotNetEnv** - Gestión de variables de entorno

## 🔗 Recursos Adicionales

- [Documentación de .NET 9](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Documentación de PostgreSQL](https://www.postgresql.org/docs/)
- [BCrypt.Net](https://github.com/BcryptNet/bcrypt.net)
- [Documentación de Docker](https://docs.docker.com/)
- [JWT.IO](https://jwt.io/) - Depurador JWT

## 📊 Estadísticas del Proyecto

- **Lenguaje:** C# 13
- **Framework:** .NET 9.0
- **Base de Datos:** PostgreSQL 17
- **Arquitectura:** Patrón Minimal API
- **Líneas de Código:** ~1,500+ (excluyendo migraciones)

---

**Nota:** Este proyecto es una base lista para producción con autenticación JWT, soporte Docker y documentación comprensiva de la API. Revisa la lista de tareas para mejoras planificadas.
