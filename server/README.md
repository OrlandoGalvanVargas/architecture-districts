# FacilityOS.API — Backend Service

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square)
![C#](https://img.shields.io/badge/C%23-14.0-239120?style=flat-square)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=flat-square)
![JWT](https://img.shields.io/badge/JWT-Authentication-000000?style=flat-square)
![MediatR](https://img.shields.io/badge/MediatR-CQRS-5A4FCF?style=flat-square)
![FluentValidation](https://img.shields.io/badge/FluentValidation-Validation-0078D4?style=flat-square)
![Entity Framework](https://img.shields.io/badge/EF_Core-10.0-512BD4?style=flat-square)
![Tests](https://img.shields.io/badge/Tests-xUnit-25A162?style=flat-square)

API RESTful de nivel empresarial para la administración de distritos escolares, escuelas, personal académico y dispositivos de seguridad (beacons). Construida bajo Vertical Slice Architecture, CQRS y separación estricta en tres capas de responsabilidad.

---

## Tabla de Contenidos

- [Descripción General](#descripción-general)
- [Arquitectura](#arquitectura)
- [Stack Tecnológico](#stack-tecnológico)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Módulos de Negocio](#módulos-de-negocio)
- [API Endpoints](#api-endpoints)
- [Seguridad](#seguridad)
- [Persistencia y Resiliencia de Datos](#persistencia-y-resiliencia-de-datos)
- [Manejo de Errores (RFC 7807)](#manejo-de-errores-rfc-7807)
- [Patrones de Diseño](#patrones-de-diseño)
- [Testing y Calidad de Código](#testing-y-calidad-de-código)
- [Logging y Observabilidad](#logging-y-observabilidad)
- [CI/CD y Despliegue](#cicd-y-despliegue)
- [Containerización (Docker)](#containerización-docker)
- [Configuración](#configuración)
- [Base de Datos](#base-de-datos)
- [Convenciones de Código](#convenciones-de-código)
- [Desarrollo Local](#desarrollo-local)
- [Licencia](#licencia)

---

## Descripción General

**FacilityOS.API** es el servicio backend de FacilityOS, una plataforma para la gestión integral de instalaciones educativas: distritos escolares, escuelas, usuarios administrativos, personal académico (faculty) y dispositivos de seguridad tipo beacon utilizados en protocolos de emergencia.

El backend está diseñado siguiendo estándares de producción propios de entornos SaaS multi-tenant: aislamiento de datos por jerarquía organizativa, autenticación y autorización robustas, manejo de errores estandarizado internacionalmente, persistencia de alto rendimiento y una arquitectura preparada para escalar sin degradar mantenibilidad.

### Características Principales

- Autenticación **JWT** con **refresh tokens** rotativos entregados vía cookie `HttpOnly`
- Autorización jerárquica por rol (`Admin` > `DistrictAdmin` > `SchoolAdmin`) con aislamiento **multi-tenant**
- **Soft delete** con filtros globales e índices únicos filtrados
- Auditoría automática de entidades (`CreatedAt`, `UpdatedAt`) vía interceptor de EF Core
- **Rate limiting** y cabeceras de seguridad OWASP en cada respuesta
- Validación desacoplada con **FluentValidation** integrada al pipeline de MediatR
- Manejo centralizado de errores bajo **RFC 7807** (`ProblemDetails`)
- Paginación, filtrado y búsqueda en todos los listados
- **Vertical Slice Architecture** con CQRS vía MediatR
- Suite de pruebas automatizadas (unitarias e integración) cubriendo reglas de negocio y autorización
- Migraciones aplicadas automáticamente en el arranque (Database-as-Code)

---

## Arquitectura

### Decisión: Vertical Slice Architecture sobre 3 capas físicas

El backend se organiza en **tres proyectos físicos** (ensamblados) con reglas de referencia unidireccionales estrictas. Esto evita dependencias circulares y mantiene el dominio de negocio completamente aislado de detalles de infraestructura.

```
┌─────────────────────────────────────────┐
│  FacilityOS.API                         │ Controladores, persistencia física,
│  (Infraestructura / Entry Point)        │ middlewares, mappers, Program.cs
└───────────────────┬─────────────────────┘
                    │ referencia a
                    ▼
┌─────────────────────────────────────────┐
│  FacilityOS.Application                 │ Casos de uso (Commands/Queries),
│  (Casos de Uso)                         │ DTOs, validadores, excepciones
└───────────────────┬─────────────────────┘
                    │ referencia a
                    ▼
┌─────────────────────────────────────────┐
│  FacilityOS.Domain                      │ Entidades ricas, enums, reglas
│  (C# puro, sin dependencias externas)   │ de negocio invariantes
└─────────────────────────────────────────┘
```

| Proyecto | Rol | Referencias permitidas |
|---|---|---|
| `FacilityOS.Domain` | Verdad absoluta del negocio: modelos ricos (Aggregate Roots), entidades y enums. Cero dependencias a frameworks, ORM o paquetes externos. | Ninguna |
| `FacilityOS.Application` | Orquesta los casos de uso: comandos, queries, handlers, DTOs, validadores y excepciones de negocio. | Solo `Domain` |
| `FacilityOS.API` | Capa de entrada: controladores HTTP delgados, `ApplicationDbContext`, migraciones, middlewares, mappers estáticos y configuración del host. | Solo `Application` |

### ¿Por qué Vertical Slice en lugar de capas técnicas?

Una arquitectura en capas técnicas tradicionales (`Controllers/`, `Services/`, `Repositories/`) dispersa un mismo caso de uso en múltiples carpetas: crear un distrito implica tocar controller, service, repository, DTOs y validators en ubicaciones distintas.

**Vertical Slice** agrupa todo lo relacionado a un caso de uso en una sola carpeta:

```
Features/
└── Districts/
    ├── CreateDistrict/
    │   ├── CreateDistrictCommand.cs
    │   └── CreateDistrictHandler.cs
    ├── UpdateDistrict/
    ├── GetDistricts/
    └── Validators/
        └── DistrictValidators.cs
```

**Beneficios:**

- **Descubribilidad** — todo lo de un feature vive en un solo lugar
- **Mantenibilidad** — cambios aislados sin efectos colaterales en otros módulos
- **Escalabilidad** — nuevos features se agregan sin modificar código existente
- **Bajo acoplamiento** — features independientes entre sí
- **Sin boilerplate** — se eliminan capas de indirección innecesarias

### Flujo de una petición

```
HTTP Request
    │
    ▼
Controller (delgado, solo traduce HTTP ↔ MediatR)
    │
    ▼
MediatR Pipeline
    ├── ValidationBehavior  → ejecuta validadores en paralelo (Task.WhenAll)
    │
    ▼
Handler (lógica de negocio)
    ├── Evaluación de autorización jerárquica (ICurrentUserService)
    ├── Invocación de métodos de dominio
    ├── Persistencia vía EF Core (No Tracking + proyección)
    │
    ▼
Response Mapping (mappers estáticos manuales)
    │
    ▼
HTTP Response (o ProblemDetails RFC 7807 en caso de error)
```

---

## Stack Tecnológico

| Tecnología | Versión | Uso |
|---|---|---|
| .NET | 10.0 | Runtime |
| C# | 14.0 | Lenguaje |
| ASP.NET Core | 10.0 | Web API |
| Entity Framework Core | 10.0 | ORM (con `DbContextPool`) |
| SQL Server | 2022 | Base de datos relacional |
| MediatR | 12.x | CQRS / mensajería interna |
| FluentValidation | 11.x | Validación de contratos de entrada |
| BCrypt.Net | 4.x | Hash de contraseñas |
| JWT Bearer | 10.0 | Autenticación basada en tokens |
| Serilog | 3.x | Logging estructurado |
| NetEscapades.AspNetCore.SecurityHeaders | 0.x | Cabeceras de seguridad OWASP |
| Swagger / OpenAPI | 6.x | Documentación interactiva de la API |
| xUnit | 2.x | Pruebas unitarias e integración |
| Docker | — | Containerización (build multi-stage) |

---

## Estructura del Proyecto

La solución se divide en tres proyectos físicos independientes, cada uno con una única dirección de dependencia (API → Application → Domain).

```
FacilityOS/
├── .dockerignore
├── docker-compose.local.yml
├── Dockerfile
├── FacilityOS.API/
│   ├── Common/
│   │   └── Exceptions/
│   │       └── ExceptionHandlingMiddleware.cs
│   ├── Controllers/
│   │   ├── ApiControllerBase.cs
│   │   ├── AuthController.cs
│   │   ├── BeaconsController.cs
│   │   ├── DistrictsController.cs
│   │   ├── FacultiesController.cs
│   │   ├── SchoolsController.cs
│   │   └── UsersController.cs
│   ├── Data/
│   │   ├── Configurations/
│   │   │   ├── BeaconConfiguration.cs
│   │   │   ├── DistrictConfiguration.cs
│   │   │   ├── FacultyConfiguration.cs
│   │   │   ├── RefreshTokenConfiguration.cs
│   │   │   ├── SchoolConfiguration.cs
│   │   │   └── UserConfiguration.cs
│   │   ├── Interceptors/
│   │   │   └── UpdateAuditableEntitiesInterceptor.cs
│   │   └── ApplicationDbContext.cs
│   ├── Migrations/
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── CurrentUserService.cs
│   │   ├── ResourceAuthorizationService.cs
│   │   └── TokenCleanupWorker.cs
│   ├── appsettings.json
│   ├── FacilityOS.API.http
│   └── Program.cs
│
├── FacilityOS.Application/
│   ├── Common/
│   │   ├── Behaviors/
│   │   │   └── ValidationBehavior.cs
│   │   ├── Exceptions/
│   │   │   ├── ConflictException.cs
│   │   │   ├── ForbiddenException.cs
│   │   │   └── NotFoundException.cs
│   │   ├── Mapping/
│   │   │   ├── AuthMapping.cs
│   │   │   ├── BeaconMapping.cs
│   │   │   ├── DistrictMapping.cs
│   │   │   ├── FacultyMapping.cs
│   │   │   ├── SchoolMapping.cs
│   │   │   └── UserMapping.cs
│   │   ├── Settings/
│   │   │   ├── BCryptSettings.cs
│   │   │   └── JwtSettings.cs
│   │   ├── AppConstants.cs
│   │   └── PagedResult.cs
│   ├── DTOs/
│   │   ├── Auth/
│   │   ├── Beacons/
│   │   ├── Districts/
│   │   ├── Faculties/
│   │   ├── Schools/
│   │   └── Users/
│   ├── Features/
│   │   ├── Auth/
│   │   │   ├── Login/
│   │   │   ├── Logout/
│   │   │   ├── Me/
│   │   │   ├── RefreshToken/
│   │   │   └── Validators/
│   │   ├── Beacons/
│   │   │   ├── CreateBeacon/
│   │   │   ├── DeleteBeacon/
│   │   │   ├── GetBeaconById/
│   │   │   ├── GetBeacons/
│   │   │   ├── UpdateBeacon/
│   │   │   └── Validators/
│   │   ├── Districts/
│   │   │   ├── CreateDistrict/
│   │   │   ├── DeleteDistrict/
│   │   │   ├── GetDistrictById/
│   │   │   ├── GetDistricts/
│   │   │   ├── UpdateDistrict/
│   │   │   └── Validators/
│   │   ├── Faculties/
│   │   │   ├── CreateFaculty/
│   │   │   ├── DeleteFaculty/
│   │   │   ├── GetFacultyById/
│   │   │   ├── GetFaculties/
│   │   │   ├── UpdateFaculty/
│   │   │   └── Validators/
│   │   ├── Schools/
│   │   │   ├── CreateSchool/
│   │   │   ├── DeleteSchool/
│   │   │   ├── GetSchoolById/
│   │   │   ├── GetSchools/
│   │   │   ├── UpdateSchool/
│   │   │   └── Validators/
│   │   └── Users/
│   │       ├── CreateUser/
│   │       ├── DeleteUser/
│   │       ├── GetUserById/
│   │       ├── GetUsers/
│   │       ├── UpdateUser/
│   │       └── Validators/
│   └── Services/
│       ├── IApplicationDbContext.cs
│       ├── IAuthService.cs
│       ├── ICurrentUserService.cs
│       └── IResourceAuthorizationService.cs
│
└── FacilityOS.Domain/
    └── Models/
        ├── Base/
        │   ├── AuditableEntity.cs
        │   └── BaseEntity.cs
        ├── Enums/
        │   ├── BeaconStatus.cs
        │   ├── BeaconType.cs
        │   ├── SchoolLevel.cs
        │   ├── SchoolType.cs
        │   └── UserEntityType.cs
        ├── Beacon.cs
        ├── District.cs
        ├── Faculty.cs
        ├── RefreshToken.cs
        ├── School.cs
        └── User.cs
```

> La solución incluye además un proyecto de pruebas automatizadas (`FacilityOS.Tests`) con cobertura unitaria e integración sobre handlers, validadores y reglas de autorización, no incluido en el árbol anterior por brevedad.

---

## Módulos de Negocio

### 1. Autenticación (Auth)
Login con JWT + refresh token, rotación de refresh tokens, logout con revocación y consulta del perfil autenticado.

### 2. Distritos (Districts)
CRUD completo de distritos escolares con asignación jerárquica y contadores agregados de escuelas, beacons y faculties.

### 3. Escuelas (Schools)
CRUD completo con filtros por distrito, nivel y tipo, y contadores de beacons y faculties asociados.

### 4. Usuarios (Users)
CRUD completo con roles (`Admin`, `DistrictAdmin`, `SchoolAdmin`, `User`), asignación a entidades (Global, District, School) y revocación de sesiones ante cambios críticos.

### 5. Beacons (Dispositivos de Emergencia)
CRUD restringido a rol `Admin`. Tipos: `Pendant`, `Wristband`, `Fixed`, `Mobile`. Estados: `Available`, `Assigned`, `Maintenance`, `Inactive`. Asignación flexible a distrito, escuela o faculty.

### 6. Faculties (Personal Académico)
CRUD según jerarquía de rol, asignación a distrito o escuela, asignación opcional de beacon e información profesional (título, departamento).

---

## API Endpoints

### Autenticación

| Método | Endpoint | Descripción | Auth |
|---|---|---|---|
| POST | `/api/auth/login` | Iniciar sesión | No |
| POST | `/api/auth/refresh` | Refrescar token | No (cookie) |
| GET | `/api/auth/me` | Perfil actual | Sí |
| POST | `/api/auth/logout` | Cerrar sesión | Sí |

### Distritos

| Método | Endpoint | Descripción | Auth |
|---|---|---|---|
| GET | `/api/districts` | Listar distritos | SchoolAdmin+ |
| GET | `/api/districts/{id}` | Obtener distrito | SchoolAdmin+ |
| POST | `/api/districts` | Crear distrito | Admin |
| PUT | `/api/districts/{id}` | Actualizar distrito | Admin |
| DELETE | `/api/districts/{id}` | Eliminar distrito | Admin |

### Escuelas

| Método | Endpoint | Descripción | Auth |
|---|---|---|---|
| GET | `/api/schools` | Listar escuelas | SchoolAdmin+ |
| GET | `/api/schools/{id}` | Obtener escuela | SchoolAdmin+ |
| POST | `/api/schools` | Crear escuela | DistrictAdmin+ |
| PUT | `/api/schools/{id}` | Actualizar escuela | SchoolAdmin+ |
| DELETE | `/api/schools/{id}` | Eliminar escuela | Admin |

### Usuarios

| Método | Endpoint | Descripción | Auth |
|---|---|---|---|
| GET | `/api/users` | Listar usuarios | SchoolAdmin+ |
| GET | `/api/users/{id}` | Obtener usuario | SchoolAdmin+ |
| POST | `/api/users` | Crear usuario | SchoolAdmin+ |
| PUT | `/api/users/{id}` | Actualizar usuario | SchoolAdmin+ |
| DELETE | `/api/users/{id}` | Eliminar usuario | DistrictAdmin+ |

### Beacons

| Método | Endpoint | Descripción | Auth |
|---|---|---|---|
| GET | `/api/beacons` | Listar beacons | SchoolAdmin+ |
| GET | `/api/beacons/{id}` | Obtener beacon | SchoolAdmin+ |
| POST | `/api/beacons` | Crear beacon | Admin |
| PUT | `/api/beacons/{id}` | Actualizar beacon | Admin |
| DELETE | `/api/beacons/{id}` | Eliminar beacon | Admin |

### Faculties

| Método | Endpoint | Descripción | Auth |
|---|---|---|---|
| GET | `/api/faculties` | Listar faculties | SchoolAdmin+ |
| GET | `/api/faculties/{id}` | Obtener faculty | SchoolAdmin+ |
| POST | `/api/faculties` | Crear faculty | SchoolAdmin+ |
| PUT | `/api/faculties/{id}` | Actualizar faculty | SchoolAdmin+ |
| DELETE | `/api/faculties/{id}` | Eliminar faculty | DistrictAdmin+ |

La documentación interactiva completa (parámetros, esquemas y ejemplos) está disponible vía **Swagger UI** en `/swagger` una vez levantado el servicio.

---

## Seguridad

### Autenticación y sesiones

- **JWT Bearer Token** (HS256)
- **Access Token:** vida corta (15 minutos, configurable). Viaja en el cuerpo JSON de la respuesta y se almacena únicamente en memoria en el cliente.
- **Refresh Token:** vida larga (7 días, configurable). Nunca viaja en el payload JSON; se entrega exclusivamente dentro de una cookie de servidor con las siguientes directrices OWASP:
  - `HttpOnly = true` — inaccesible desde JavaScript, mitiga robo de sesión por XSS
  - `Secure = true` — solo viaja sobre HTTPS
  - `SameSite = Strict` — bloquea envío en peticiones cross-site, mitiga CSRF
- **Rotación de un solo uso:** cada llamada a `/refresh` revoca el token actual y emite un nuevo par criptográfico.
- **Limpieza automática:** un `BackgroundService` (`TokenCleanupWorker`) elimina en segundo plano, una vez al día, las sesiones expiradas o revocadas mediante `ExecuteDeleteAsync`, sin impacto en el tráfico de usuarios.

### Autorización jerárquica y multi-tenancy

| Acción | Admin | DistrictAdmin | SchoolAdmin |
|---|---|---|---|
| Gestionar Distritos | ✅ | ❌ | ❌ |
| Gestionar Escuelas | ✅ | En su distrito | Solo su escuela |
| Gestionar Usuarios | ✅ | En su distrito | Solo su escuela |
| Gestionar Beacons | ✅ | ❌ | ❌ |
| Ver Beacons | ✅ | Su distrito | Su escuela |
| Gestionar Faculties | ✅ | Su distrito | Su escuela |

- **`ICurrentUserService`** centraliza la lectura de claims del JWT (`UserId`, `Role`, `EntityId`, `EntityType`), evitando que los controladores procesen tokens manualmente.
- **Evaluación secuencial fail-fast:** en consultas por ID se verifica primero existencia (`404` si no existe) y luego autorización jerárquica (`403` si no corresponde), evitando fugas de información y queries redundantes.
- **Queries multi-tenant atómicas:** los filtros de seguridad por rol y los filtros dinámicos de búsqueda se combinan en un único `IQueryable`, resuelto por EF Core en una sola consulta indexada.

### Seguridad de red

- **Rate limiting** — 100 solicitudes/minuto por IP (configurable)
- **CORS** — orígenes explícitamente configurados
- **Cabeceras de seguridad OWASP** en cada respuesta:
  - `X-Frame-Options: DENY` — mitiga clickjacking
  - `X-Content-Type-Options: nosniff` — previene MIME-sniffing
- **BCrypt** con factor de trabajo configurable para hash de contraseñas
- **Validación de entrada** con FluentValidation en todos los DTOs

---

## Persistencia y Resiliencia de Datos

- **`DbContextPool`** — reutiliza instancias de `ApplicationDbContext` en lugar de crear y destruir una por petición, reduciendo el costo de asignación a casi cero.
- **Proyección sin tracking** — todos los Query Handlers ejecutan `AsNoTracking()` con `.Select()` directo a los DTOs de respuesta, evitando el antipatrón de cargar grafos de entidades completos (`.Include()` masivos) y minimizando el payload transferido.
- **Resiliencia ante fallos transitorios** — `EnableRetryOnFailure` reintenta automáticamente hasta 5 veces ante microcortes de red en la infraestructura cloud.
- **Filtros globales de borrado lógico** — `HasQueryFilter(x => !x.IsDeleted)` excluye automáticamente los registros archivados de todos los listados y búsquedas.
- **Índices únicos filtrados** — las restricciones `UNIQUE` (email, número de serie, códigos maestros) se configuran con `HasFilter("[IsDeleted] = 0")`, permitiendo reutilizar valores únicos tras un soft delete.

---

## Manejo de Errores (RFC 7807)

Todas las excepciones controladas se interceptan en un middleware global (`ExceptionHandlingMiddleware`) colocado al inicio del pipeline y se traducen a respuestas `ProblemDetails` estandarizadas internacionalmente bajo RFC 7807, exponiendo `type`, `title`, `status`, `instance` y, cuando aplica, el diccionario `errors` agrupado por campo.

| Excepción de negocio | HTTP | Significado |
|---|---|---|
| `ValidationException` | 400 | Bad Request — estructura de entrada inválida |
| `UnauthorizedAccessException` | 401 | Unauthorized — fallo de credenciales |
| `ForbiddenException` | 403 | Forbidden — acceso denegado (multi-tenancy) |
| `NotFoundException` | 404 | Not Found — recurso inexistente o borrado lógicamente |
| `ConflictException` / `InvalidOperationException` | 409 | Conflict — violación de reglas relacionales |
| Cualquier excepción no controlada | 500 | Internal Server Error — se registra el detalle en logs internos; el cliente recibe un mensaje genérico |

---

## Patrones de Diseño

| Patrón | Aplicación en el proyecto |
|---|---|
| **Vertical Slice Architecture** | Cada feature es autónoma: comando/query, handler y validador viven juntos |
| **CQRS** | Separación de Commands (mutaciones) y Queries (lecturas), mediada por MediatR |
| **Rich Domain Model** | Entidades con `private set`, constructores fuertemente tipados y métodos de negocio explícitos (`.SoftDelete()`, `.AssignToFaculty()`) |
| **Repository / Unit of Work (vía EF Core)** | `ApplicationDbContext` como unidad de trabajo, `DbSet<T>` como repositorios |
| **Fluent API Configuration** | Configuraciones EF Core separadas por entidad, sin Data Annotations en el dominio |
| **Interceptor Pattern** | `UpdateAuditableEntitiesInterceptor` para auditoría automática de `CreatedAt`/`UpdatedAt` |
| **Middleware Pattern** | `ExceptionHandlingMiddleware`, rate limiting, cabeceras de seguridad |
| **Options Pattern** | `JwtSettings`, `BCryptSettings` fuertemente tipados desde `appsettings.json` |
| **Mapper Pattern (manual)** | Extension methods estáticos por módulo, proyecciones eficientes sobre `IQueryable` |
| **Pipeline Behavior** | `ValidationBehavior` intercepta el bus de MediatR y ejecuta validadores en paralelo antes del handler |
| **Soft Delete Pattern** | Filtros globales de EF Core + métodos de dominio `SoftDelete()` / `Restore()` |
| **Background Worker** | `TokenCleanupWorker` como `Hosted Service` para mantenimiento periódico de sesiones |

---

## Testing y Calidad de Código

El backend cuenta con una suite de pruebas automatizadas completa construida sobre **xUnit**, que cubre:

- **Pruebas unitarias** de handlers (Commands y Queries), validadores de FluentValidation y reglas de negocio del dominio.
- **Pruebas de integración** sobre el pipeline HTTP completo (`WebApplicationFactory`), verificando autenticación, autorización jerárquica y contratos de respuesta (`ProblemDetails`, códigos de estado, paginación).
- **Casos de autorización multi-tenant**, validando que cada rol (`Admin`, `DistrictAdmin`, `SchoolAdmin`) solo pueda acceder y mutar los recursos dentro de su alcance jerárquico.

```bash
# Ejecutar toda la suite de pruebas
dotnet test

# Ejecutar con reporte de cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Principios SOLID aplicados

| Principio | Implementación |
|---|---|
| **S**ingle Responsibility | Cada handler resuelve exactamente un caso de uso |
| **O**pen/Closed | Nuevas features se agregan como slices independientes, sin modificar código existente |
| **L**iskov Substitution | Interfaces (`IAuthService`, `IResourceAuthorizationService`) intercambiables sin romper contratos |
| **I**nterface Segregation | Interfaces específicas y acotadas por servicio |
| **D**ependency Inversion | Inyección de dependencias en todos los servicios de infraestructura |

---

## Logging y Observabilidad

Se utiliza **Serilog** como motor de logging estructurado, configurado desde el arranque de la aplicación. Cada entrada de log se emite como un objeto JSON indexable con propiedades dinámicas (`Environment`, `UserId`, `Exception`, etc.), habilitando auditoría y búsqueda en tiempo real en consolas de la nube (Render, Railway, Datadog) ante cualquier incidente en producción.

---

## CI/CD y Despliegue

El proyecto sigue una estrategia **Database-as-Code**: no existen scripts SQL manuales ni intervención humana directa sobre la base de datos de producción.

1. El pipeline de CI/CD despliega el código a la nube (Render / Railway).
2. En el arranque, `Program.cs` ejecuta `context.Database.Migrate()` de forma resiliente.
3. El backend detecta nuevas tablas, columnas o índices filtrados y aplica las migraciones automáticamente.
4. Una vez sincronizado el esquema, el servicio abre el puerto y comienza a recibir tráfico.

Esto garantiza que el esquema físico se mantenga simétrico y consistente en todos los entornos sin errores manuales.

---

## Containerización (Docker)

El servicio está preparado para ejecutarse en contenedores mediante un **build multi-stage** que separa la compilación del runtime final, manteniendo la imagen de producción mínima y libre de herramientas de build.

### `Dockerfile`

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["FacilityOS.Domain/FacilityOS.Domain.csproj", "FacilityOS.Domain/"]
COPY ["FacilityOS.Application/FacilityOS.Application.csproj", "FacilityOS.Application/"]
COPY ["FacilityOS.API/FacilityOS.API.csproj", "FacilityOS.API/"]

RUN dotnet restore "FacilityOS.API/FacilityOS.API.csproj"

COPY . .

RUN dotnet publish "FacilityOS.API/FacilityOS.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

USER $APP_UID

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "FacilityOS.API.dll"]
```

**Puntos clave del build:**

- **Etapa `build`** — usa la imagen `sdk:10.0`, copia únicamente los `.csproj` primero para aprovechar el cacheo de capas de Docker en `dotnet restore`, y publica en modo `Release`.
- **Etapa `final`** — usa la imagen ligera `aspnet:10.0` (solo runtime, sin SDK), reduciendo drásticamente el tamaño de la imagen final.
- **Usuario no root** (`USER $APP_UID`) — el contenedor no se ejecuta como root, alineado con las buenas prácticas de seguridad de contenedores.
- **Puerto expuesto:** `8080` (interno del contenedor).

### `docker-compose.local.yml`

Orquesta el levantamiento local del servicio junto con su configuración de entorno, sin necesidad de instalar el SDK de .NET localmente:

```yaml
services:
  facilityos-api:
    container_name: facilityos-api-local
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=host.docker.internal;Database=FacilityOSDB;User Id=sa;Password=${DB_PASSWORD};TrustServerCertificate=True;
      - Jwt__Key=${JWT_KEY}
      - Jwt__Issuer=FacilityOSAPI
      - Jwt__Audience=FacilityClient
      - Jwt__AccessTokenExpirationMinutes=15
      - Jwt__RefreshTokenExpirationDays=7
      - Cors__AllowedOrigins__0=http://localhost:3000
      - Cors__AllowedOrigins__1=http://localhost:5000
      - RateLimiting__PermitLimit=100
      - RateLimiting__WindowMinutes=1
      - BCrypt__WorkFactor=12
    restart: always
```

> El servicio se conecta a una instancia de SQL Server corriendo en el host (fuera del contenedor) a través de `host.docker.internal`. Las credenciales sensibles (`DB_PASSWORD`, `JWT_KEY`) deben suministrarse mediante variables de entorno o un archivo `.env` ignorado por Git — nunca deben quedar hardcodeadas en el `docker-compose.local.yml` versionado.

### Comandos básicos

```bash
# Construir y levantar el contenedor en modo local
docker compose -f docker-compose.local.yml up --build

# Levantar en segundo plano
docker compose -f docker-compose.local.yml up -d

# Ver logs del contenedor
docker compose -f docker-compose.local.yml logs -f facilityos-api

# Detener y remover el contenedor
docker compose -f docker-compose.local.yml down
```

La API queda disponible en `http://localhost:5000`, con Swagger UI en `http://localhost:5000/swagger`.

---

## Configuración

### `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=FacilityOS;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Jwt": {
    "Key": "your-super-secret-key-at-least-32-characters",
    "Issuer": "FacilityOS.API",
    "Audience": "FacilityOS.Client",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000"]
  },
  "RateLimiting": {
    "PermitLimit": 100,
    "WindowMinutes": 1
  },
  "BCrypt": {
    "WorkFactor": 12
  }
}
```

### Variables de entorno requeridas

| Variable | Descripción | Requerida |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | Cadena de conexión a SQL Server | Sí |
| `Jwt__Key` | Secreto JWT (mínimo 32 caracteres) | Sí |
| `Jwt__Issuer` | Emisor del token JWT | Solo en producción |
| `Jwt__Audience` | Audiencia del token JWT | Solo en producción |
| `Cors__AllowedOrigins__0` | Origen del frontend | Solo en producción |

---

## Base de Datos

### Entidades

| Tabla | Descripción | Soft Delete |
|---|---|---|
| `Users` | Usuarios del sistema | ✅ |
| `RefreshTokens` | Tokens de refresco | Revocación explícita |
| `Districts` | Distritos escolares | Hard delete |
| `Schools` | Escuelas | ✅ |
| `Beacons` | Dispositivos de emergencia | ✅ |
| `Faculties` | Personal académico | ✅ |

### Migraciones

```bash
# Crear una nueva migración
dotnet ef migrations add MigrationName --project FacilityOS.API

# Aplicar migraciones pendientes
dotnet ef database update --project FacilityOS.API

# Revertir a una migración anterior
dotnet ef database update PreviousMigrationName --project FacilityOS.API
```

En producción, las migraciones se aplican automáticamente en el arranque del servicio (ver [CI/CD y Despliegue](#cicd-y-despliegue)).

### Interceptor de auditoría

`UpdateAuditableEntitiesInterceptor` establece automáticamente:

- `CreatedAt` al insertar una entidad
- `UpdatedAt` al modificarla
- Usa siempre `DateTime.UtcNow` para consistencia entre zonas horarias

---

## Convenciones de Código

### Nomenclatura

- `PascalCase` para clases, métodos y propiedades
- `camelCase` para variables locales
- `UPPER_CASE` para constantes
- Sufijos consistentes: `Command`, `Query`, `Handler`, `Validator`, `Response`, `Request`

### Estructura de un feature

```
FeatureName/
├── FeatureNameCommand.cs          # Request / Command
├── FeatureNameHandler.cs   # Lógica de negocio
```

Los validadores de cada módulo se agrupan en su propia carpeta `Validators/` dentro del feature raíz.

### Reglas del equipo

1. Un caso de uso = un Command o Query.
2. Un Command/Query = un Handler.
3. La lógica de negocio vive en los Handlers, nunca en los Controllers.
4. Controllers delgados: solo traducen HTTP a MediatR y viceversa.
5. Features autocontenidas: evitar dependencias cruzadas entre módulos.
6. Abstracciones solo cuando existe duplicación real, no de forma anticipada.
7. Mapeo manual con Extension Methods estáticos, sin AutoMapper.
8. Rich Domain Model: propiedades con `private set`, mutaciones vía métodos de negocio.

---

## Desarrollo Local

```bash
# Clonar el repositorio
git clone https://github.com/your-org/facilityos.git

# Navegar al backend
cd facilityos/server/FacilityOS.API

# Restaurar paquetes
dotnet restore

# Aplicar migraciones
dotnet ef database update

# Ejecutar el servicio
dotnet run

# Ejecutar la suite de pruebas
dotnet test

# Swagger UI
# http://localhost:5000/swagger
```

### Alternativa: ejecución vía Docker

No requiere instalar el SDK de .NET ni SQL Server localmente (ver [Containerización (Docker)](#containerización-docker)):

```bash
docker compose -f docker-compose.local.yml up --build
```

---

## Licencia

**Copyright © 2026 FacilityOS. Todos los derechos reservados.**

Este software, incluyendo su código fuente, arquitectura y documentación, es propiedad exclusiva de FacilityOS y se distribuye bajo licencia **propietaria**. Queda prohibida su reproducción, distribución, modificación, ingeniería inversa o uso total o parcial fuera del alcance autorizado, sin el consentimiento previo y por escrito del propietario.

Para consultas sobre licenciamiento, colaboración o uso autorizado, contactar al equipo responsable del proyecto.
