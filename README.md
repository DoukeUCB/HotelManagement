# Hotel Management API

Sistema de gestión de reservas de hotel desarrollado en C# con .NET 9.0, implementando una arquitectura de 3 capas y operaciones CRUD para detalles de reservas.

## 🏗️ Arquitectura

El proyecto sigue una arquitectura de 3 capas:

### 1. Capa de Presentación
- **Controllers**: Controladores de API REST
- **Middleware**: Manejo centralizado de errores

### 2. Capa de Aplicación
- **Services**: Lógica de negocio
- **Validators**: Validación de datos
- **Exceptions**: Excepciones personalizadas
- **DTOs**: Objetos de transferencia de datos

### 3. Capa de Datos
- **Repositories**: Acceso a datos
- **Models**: Entidades del dominio
- **Config**: Configuración de DbContext

## 📋 Requisitos Previos

- .NET 9.0 SDK o superior (compatible con .NET 8.0)
- MySQL 8.0 o superior
- Git (opcional)

## 📦 Dependencias del Proyecto

### Dependencias Principales

| Paquete | Versión | Propósito |
|---------|---------|-----------|
| **Microsoft.EntityFrameworkCore** | 8.0.11 | ORM principal para mapeo objeto-relacional y gestión de base de datos |
| **Microsoft.EntityFrameworkCore.Design** | 8.0.11 | Herramientas de diseño para migraciones y scaffolding de EF Core |
| **Pomelo.EntityFrameworkCore.MySql** | 8.0.2 | Provider de MySQL para Entity Framework Core |
| **Swashbuckle.AspNetCore** | 6.8.1 | Generación automática de documentación OpenAPI/Swagger |
| **DotNetEnv** | 3.1.1 | Carga de variables de entorno desde archivos .env |

### ¿Por qué estas dependencias?

#### Entity Framework Core (9.0.0)
- **ORM moderno y eficiente**: Permite trabajar con la base de datos usando objetos C# en lugar de SQL directo
- **LINQ support**: Consultas type-safe y expresivas
- **Change tracking**: Seguimiento automático de cambios en entidades
- **Migraciones**: Versionamiento del esquema de base de datos

#### Pomelo MySQL Provider
- **Optimizado para MySQL**: Mejor rendimiento que el provider oficial de MySQL
- **Open Source**: Comunidad activa y actualizaciones frecuentes
- **Compatibilidad**: Soporte completo para características específicas de MySQL como BINARY(16) para UUIDs

#### Swashbuckle (Swagger)
- **Documentación interactiva**: UI web para explorar y probar endpoints
- **OpenAPI standard**: Genera especificación OpenAPI 3.0
- **Desarrollo ágil**: Facilita pruebas sin necesidad de herramientas externas

#### DotNetEnv
- **Configuración segura**: Mantiene credenciales fuera del código fuente
- **Portabilidad**: Fácil configuración entre diferentes entornos
- **Estándar de industria**: Compatible con el patrón de 12-factor apps

### Instalación de Dependencias

Para instalar todas las dependencias del proyecto, ejecute:

```bash
# Restaurar todos los paquetes NuGet
dotnet restore

# O instalarlas manualmente una por una
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.11
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 8.0.2
dotnet add package Swashbuckle.AspNetCore --version 6.8.1
dotnet add package DotNetEnv --version 3.1.1
```

> **Nota sobre versiones**: Aunque el proyecto utiliza .NET 9.0, se emplean las versiones estables de EF Core 8.0.11 y Pomelo 8.0.2 para garantizar máxima compatibilidad y estabilidad en producción.

## 🚀 Instalación

### 1. Clonar el repositorio o descargar el código

```bash
git clone <url-del-repositorio>
cd HotelManagement
```

### 2. Configurar la base de datos

Ejecutar el script SQL ubicado en `Database/schema.sql` en su servidor MySQL:

```bash
# Opción 1: Usando el cliente MySQL
mysql -u root -p < Database/schema.sql

# Opción 2: Conectarse primero y luego ejecutar
mysql -u root -p
mysql> source Database/schema.sql;
mysql> quit;
```

El script creará:
- La base de datos `HotelDB`
- Todas las tablas necesarias (Cliente, Tipo_Habitacion, Huesped, Reserva, Habitacion, Detalle_Reserva)
- Relaciones y constraints entre tablas

### 3. Configurar variables de entorno

Editar el archivo `.env` con sus credenciales de base de datos:

```env
DB_SERVER=localhost
DB_PORT=3306
DB_NAME=HotelDB
DB_USER=root
DB_PASSWORD=su_contraseña
```

### 4. Restaurar paquetes NuGet

```bash
dotnet restore
```

### 5. Compilar el proyecto

```bash
dotnet build
```

## Ejecución

```bash
dotnet run
```

La aplicación estará disponible en:
- **Swagger UI**: http://localhost:5000

## Endpoints de la API

### Detalle Reserva

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/DetalleReserva` | Obtener todos los detalles de reservas |
| GET | `/api/DetalleReserva/{id}` | Obtener un detalle por ID |
| GET | `/api/DetalleReserva/reserva/{reservaId}` | Obtener detalles de una reserva |
| POST | `/api/DetalleReserva` | Crear un nuevo detalle |
| PUT | `/api/DetalleReserva/{id}` | Actualizar un detalle |
| DELETE | `/api/DetalleReserva/{id}` | Eliminar un detalle |

### Ejemplos de Uso

#### Crear Detalle de Reserva (POST)

```json
{
  "reserva_ID": "123e4567-e89b-12d3-a456-426614174000",
  "habitacion_ID": "223e4567-e89b-12d3-a456-426614174001",
  "huesped_ID": "323e4567-e89b-12d3-a456-426614174002",
  "precio_Total": 250.50,
  "cantidad_Huespedes": 2
}
```

#### Actualizar Detalle de Reserva (PUT)

```json
{
  "habitacion_ID": "223e4567-e89b-12d3-a456-426614174003",
  "precio_Total": 300.00,
  "cantidad_Huespedes": 3
}
```

#### Respuesta Exitosa (200 OK)

```json
{
  "id": "423e4567-e89b-12d3-a456-426614174003",
  "reserva_ID": "123e4567-e89b-12d3-a456-426614174000",
  "habitacion_ID": "223e4567-e89b-12d3-a456-426614174001",
  "huesped_ID": "323e4567-e89b-12d3-a456-426614174002",
  "precio_Total": 250.50,
  "cantidad_Huespedes": 2,
  "numero_Habitacion": "101",
  "nombre_Huesped": "Juan Pérez",
  "fecha_Entrada": "2024-01-15T00:00:00",
  "fecha_Salida": "2024-01-20T00:00:00"
}
```

## Modelo de Datos

### Entidades Principales

- **Cliente**: Información del cliente
- **Reserva**: Datos de la reserva
- **Detalle_Reserva**: Detalles específicos de cada reserva (CRUD principal)
- **Habitacion**: Información de habitaciones
- **Huesped**: Datos de huéspedes
- **Tipo_Habitacion**: Tipos de habitaciones disponibles

## Validaciones

### Validaciones de Negocio

- **UUIDs válidos**: Todos los IDs deben ser UUIDs válidos
- **Precio Total**: Debe ser mayor a 0
- **Cantidad de Huéspedes**: Debe ser mayor a 0
- **Relaciones**: Se valida la existencia de Reserva, Habitación y Huésped

### Manejo de Errores

La API implementa un middleware centralizado que maneja:

- **404 Not Found**: Recurso no encontrado
- **400 Bad Request**: Datos inválidos o errores de validación
- **409 Conflict**: Conflictos de datos
- **500 Internal Server Error**: Errores internos del servidor

## Estructura del Proyecto

```
HotelManagement/
├── Aplicacion/
│   ├── Exceptions/
│   │   └── CustomExceptions.cs
│   └── Validators/
│       └── DetalleReservaValidator.cs
├── Controllers/
│   └── DetalleReservaController.cs
├── Datos/
│   ├── Config/
│   │   └── HotelDbContext.cs
│   └── Models/
│       ├── Cliente.cs
│       └── TipoHabitacion.cs
├── DTOs/
│   └── DetalleReservaDTO.cs
├── Models/
│   ├── DetalleReserva.cs
│   ├── Habitacion.cs
│   ├── Huesped.cs
│   └── Reserva.cs
├── Presentacion/
│   └── Middleware/
│       └── ErrorHandlingMiddleware.cs
├── Repositories/
│   ├── DetalleReservaRepository.cs
│   └── IDetalleReservaRepository.cs
├── Services/
│   ├── DetalleReservaService.cs
│   └── IDetalleReservaService.cs
├── Database/
│   └── schema.sql
├── .env
├── Program.cs
└── HotelManagement.csproj
```

## Notas Adicionales

- Los IDs se generan automáticamente como UUIDs (BINARY(16) en MySQL)
- El middleware de errores registra todos los errores en los logs
- La API implementa CORS para permitir llamadas desde cualquier origen (configurar según necesidades de producción)
- Swagger está configurado para mostrarse en la ruta raíz durante el desarrollo
- **Versión de .NET**: El proyecto usa .NET 9.0 como target framework, pero EF Core 8.0.11 LTS para máxima estabilidad

## Inicio Rápido

```bash
# 1. Clonar o descargar el proyecto
git clone <url-del-repositorio>
cd HotelManagement

# 2. Configurar .env con tus credenciales de MySQL
# Editar el archivo .env y cambiar:
# DB_PASSWORD=tu_contraseña_real

# 3. Crear la base de datos y tablas
mysql -u root -p < Database/schema.sql

# 5. Restaurar dependencias
dotnet restore

# 6. Compilar
dotnet build

# 7. Ejecutar
dotnet run

# 8. Abrir Swagger en el navegador
# http://localhost:5000
```
