# API PG - Gestión de Usuarios y Roles

Una API REST desarrollada con .NET 8 para la gestión de usuarios y roles, integrada con PostgreSQL.

## 🚀 Características

- **Gestión de Usuarios**: CRUD completo para usuarios con autenticación
- **Gestión de Roles**: Sistema de roles y permisos
- **Base de Datos**: Integración con PostgreSQL usando Entity Framework Core
- **Documentación**: Swagger/OpenAPI integrado
- **Seguridad**: Hash de contraseñas con SHA256 y salt
- **Validación**: Validaciones de datos con Data Annotations
- **CORS**: Configurado para desarrollo

## 🛠️ Tecnologías Utilizadas

- **.NET 8** - Framework principal
- **ASP.NET Core Web API** - Para crear la API REST
- **Entity Framework Core 9.0** - ORM para base de datos
- **PostgreSQL** - Base de datos principal
- **Npgsql** - Proveedor de PostgreSQL para Entity Framework
- **Swagger/OpenAPI** - Documentación de la API
- **Newtonsoft.Json** - Serialización JSON

## 📋 Requisitos Previos

- .NET 8 SDK
- PostgreSQL Server
- Visual Studio Code (recomendado)

## 🔧 Configuración

### 1. Base de Datos PostgreSQL

Asegúrate de tener PostgreSQL instalado y ejecutándose. La aplicación está configurada para usar:

- **Host**: localhost
- **Puerto**: 5432
- **Base de datos**: Montessori
- **Usuario**: postgres
- **Contraseña**: Mayerly123

### 2. Instalación

```bash
# Clonar el repositorio (si aplica)
git clone [url-del-repositorio]

# Navegar al directorio del proyecto
cd ApiPG

# Restaurar dependencias
dotnet restore

# Aplicar migraciones a la base de datos
dotnet ef database update

# Ejecutar la aplicación
dotnet run
```

### 3. Configuración de Conexión

Si necesitas cambiar la configuración de la base de datos, edita el archivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=Montessori;Username=postgres;Password=Mayerly123"
  }
}
```

## 📚 Uso de la API

### Endpoints Principales

#### Usuarios
- `GET /api/users` - Obtener todos los usuarios
- `GET /api/users/{id}` - Obtener usuario por ID
- `GET /api/users/username/{username}` - Obtener usuario por nombre de usuario
- `POST /api/users` - Crear nuevo usuario
- `PUT /api/users/{id}` - Actualizar usuario
- `DELETE /api/users/{id}` - Eliminar usuario (soft delete)

#### Roles
- `GET /api/roles` - Obtener todos los roles
- `GET /api/roles/{id}` - Obtener rol por ID
- `POST /api/roles` - Crear nuevo rol
- `PUT /api/roles/{id}` - Actualizar rol
- `DELETE /api/roles/{id}` - Eliminar rol (soft delete)

### Ejemplos de Uso

#### Crear Usuario
```json
POST /api/users
{
  "firstName": "Juan",
  "lastName": "Pérez",
  "email": "juan.perez@email.com",
  "username": "jperez",
  "password": "password123",
  "roleId": 2
}
```

#### Crear Rol
```json
POST /api/roles
{
  "name": "Editor",
  "description": "Can edit content"
}
```

## 📖 Documentación

Una vez que ejecutes la aplicación, puedes acceder a:

- **Swagger UI**: `http://localhost:5273` (página principal)
- **API Health Check**: `http://localhost:5273/health`

## 🗄️ Base de Datos

### Tablas Principales

#### Users
- **Id**: Identificador único
- **FirstName**: Nombre
- **LastName**: Apellido
- **Email**: Correo electrónico (único)
- **Username**: Nombre de usuario (único)
- **PasswordHash**: Hash de la contraseña
- **RoleId**: Referencia al rol
- **CreatedAt**: Fecha de creación
- **UpdatedAt**: Fecha de última actualización
- **IsActive**: Estado activo/inactivo

#### Roles
- **Id**: Identificador único
- **Name**: Nombre del rol (único)
- **Description**: Descripción del rol
- **CreatedAt**: Fecha de creación
- **IsActive**: Estado activo/inactivo

### Datos de Prueba

La aplicación incluye datos de prueba:

**Roles:**
- Administrator (ID: 1)
- User (ID: 2) 
- Viewer (ID: 3)
- Manager (ID: 4)

**Usuarios:**
- admin / admin123 (Administrator)
- jperez / user123 (User)
- mgarcia / user123 (Viewer)

## 🔐 Seguridad

- Las contraseñas se almacenan hasheadas con SHA256 y salt personalizado
- Validación de datos en todos los endpoints
- Soft delete para mantener integridad referencial
- Índices únicos para email y username

## 🚧 Desarrollo

### Comandos Útiles

```bash
# Crear nueva migración
dotnet ef migrations add NombreMigracion

# Aplicar migraciones
dotnet ef database update

# Eliminar última migración
dotnet ef migrations remove

# Compilar proyecto
dotnet build

# Ejecutar tests (si los hay)
dotnet test
```

## 📝 Notas

- La aplicación usa soft delete, por lo que los registros eliminados mantienen `IsActive = false`
- El sistema de roles es extensible para agregar más funcionalidades de autorización
- La configuración CORS permite todos los orígenes en desarrollo

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo licencia MIT. Ver el archivo `LICENSE` para más detalles.

---

**Desarrollado con ❤️ usando .NET 8 y PostgreSQL**
