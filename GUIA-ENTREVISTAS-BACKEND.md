# 📋 GUÍA DE PREPARACIÓN PARA ENTREVISTAS - BASADA EN PROYECTO CHETANGO

> **Desarrollador Backend Senior | 10+ años de experiencia**  
> **Proyecto:** Chetango - Sistema de Gestión para Academias de Danza  
> **Fecha:** Febrero 2026

---

## 🎯 CATEGORÍAS DE PREGUNTAS

1. [ARQUITECTURA Y DISEÑO DE SOFTWARE](#1-arquitectura-y-diseño-de-software)
2. [TECNOLOGÍAS Y STACK TÉCNICO](#2-tecnologías-y-stack-técnico)
3. [PATRONES DE DISEÑO Y MEJORES PRÁCTICAS](#3-patrones-de-diseño-y-mejores-prácticas)
4. [BASE DE DATOS Y PERSISTENCIA](#4-base-de-datos-y-persistencia)
5. [SEGURIDAD Y AUTENTICACIÓN](#5-seguridad-y-autenticación)
6. [CLOUD Y DEVOPS (AZURE)](#6-cloud-y-devops-azure)
7. [ESCALABILIDAD Y PERFORMANCE](#7-escalabilidad-y-performance)
8. [TESTING Y CALIDAD](#8-testing-y-calidad)
9. [API DESIGN Y REST](#9-api-design-y-rest)
10. [PROBLEMAS COMPLEJOS Y SOLUCIONES](#10-problemas-complejos-y-soluciones)

---

## 1. ARQUITECTURA Y DISEÑO DE SOFTWARE

### ❓ **"¿Qué arquitectura utilizas en tus proyectos y por qué?"**

**TU RESPUESTA:**
```
"Implemento Clean Architecture con CQRS en mi proyecto más reciente, Chetango, 
un sistema de gestión para academias de danza. Elegí esta arquitectura porque:

1. SEPARACIÓN DE RESPONSABILIDADES:
   - Domain: Entidades puras sin lógica de infraestructura
   - Application: Lógica de negocio con Commands/Queries (MediatR)
   - Infrastructure: Acceso a datos, servicios externos
   - API: Endpoints y configuración

2. BENEFICIOS TANGIBLES:
   - Testing: Puedo testear lógica de negocio sin base de datos
   - Mantenibilidad: Cambié de Azure AD B2C a Entra ID sin tocar el dominio
   - Escalabilidad: Preparado para multi-tenant SaaS (está en el roadmap)

3. CQRS con MediatR:
   - Commands para escrituras: CrearPaqueteCommand, RegistrarAsistenciaCommand
   - Queries para lecturas: GetPaquetesDeAlumnoQuery, GetClasesPorFechaQuery
   - Validaciones centralizadas con FluentValidation
```

**CÓDIGO DE EJEMPLO A MENCIONAR:**
```csharp
// Estructura de carpetas en Application:
Application/
├── Paquetes/
│   ├── Commands/
│   │   └── CrearPaquete/
│   │       ├── CrearPaqueteCommand.cs
│   │       ├── CrearPaqueteHandler.cs
│   │       └── CrearPaqueteValidator.cs
│   └── Queries/
│       └── GetPaquetesDeAlumno/
│           ├── GetPaquetesDeAlumnoQuery.cs
│           └── GetPaquetesDeAlumnoHandler.cs
```

---

### ❓ **"¿Cómo manejas la separación de capas en tu aplicación?"**

**TU RESPUESTA:**
```
"Sigo el principio de Dependency Inversion estrictamente:

1. DOMAIN (núcleo):
   - Solo entidades y enums
   - Cero dependencias externas
   - Ejemplo: Usuario, Alumno, Paquete, Asistencia

2. APPLICATION:
   - Depende SOLO de Domain
   - Define interfaces (IAppDbContext, IWhatsAppService)
   - No conoce Entity Framework ni SQL Server

3. INFRASTRUCTURE:
   - Implementa interfaces de Application
   - ChetangoDbContext implementa IAppDbContext
   - TwilioWhatsAppService implementa IWhatsAppService

4. API:
   - Dependency Injection en Program.cs
   - Registra implementaciones concretas
   - Endpoints delgados, delegan a MediatR
```

**DIAGRAMA MENTAL PARA EXPLICAR:**
```
API → Application → Domain
 ↓         ↑
Infrastructure ----┘
```

---

### ❓ **"¿Has trabajado con Domain-Driven Design (DDD)?"**

**TU RESPUESTA:**
```
"Sí, aplico conceptos de DDD en Chetango, especialmente:

1. AGREGADOS:
   - Paquete como raíz de agregado, controla su lógica interna
   - ClasesPendientes se decrementa solo a través de métodos del Paquete
   - No permito modificación directa de ClasesPendientes desde fuera

2. VALUE OBJECTS (conceptualmente):
   - Enums fuertemente tipados: Sede (Medellin/Manizales)
   - TipoAsistencia (Presente, Ausente, Justificada)
   - EstadoPago (Pendiente, Verificado, Rechazado)

3. LENGUAJE UBICUO:
   - Términos del dominio: Paquete, Asistencia, Liquidación
   - No uso términos técnicos como 'Transaction' o 'Record'
   - El código habla el lenguaje del negocio

4. BOUNDED CONTEXTS (preparado para multi-tenant):
   - Cada sede opera como contexto aislado
   - Usuario.Sede filtra automáticamente datos
```

**EJEMPLO DE CÓDIGO DDD:**
```csharp
// Agregado Paquete con lógica de negocio encapsulada
public class Paquete
{
    public int ClasesPendientes { get; private set; }
    
    // Solo se puede descontar a través de este método
    public void DescontarClase()
    {
        if (ClasesPendientes <= 0)
            throw new InvalidOperationException("No quedan clases disponibles");
        
        ClasesPendientes--;
    }
}
```

---

## 2. TECNOLOGÍAS Y STACK TÉCNICO

### ❓ **"¿Por qué elegiste .NET Core/9 en lugar de otras tecnologías?"**

**TU RESPUESTA:**
```
"Elegí .NET 9 por varias razones estratégicas:

1. PERFORMANCE Y MODERNIDAD:
   - .NET 9 es una de las plataformas más rápidas (benchmarks vs Node/Java)
   - Async/await nativo, mejor que callbacks de Node
   - Minimal APIs más ligeras que controllers tradicionales

2. ECOSISTEMA MADURO:
   - Entity Framework Core 9: Migrations, LINQ, tracking
   - MediatR para CQRS: Patrón probado
   - Azure integración nativa (App Service, SQL, AD)

3. EXPERIENCIA PROFESIONAL:
   - 10+ años trabajando con .NET (Framework → Core → 9)
   - Conozco optimizaciones y patrones avanzados
   - Quería demostrar dominio en versiones más recientes

4. MULTI-PLATAFORMA:
   - Desarrollo en Windows, deploy en Linux containers
   - Compatible con Docker/Kubernetes si escalo
   - Menor costo de hosting vs Windows Server

5. PREPARACIÓN PARA EL MERCADO:
   - .NET está en alta demanda (especialmente en enterprise)
   - Azure es líder en cloud para .NET
   - Quería mostrar que manejo tecnologías actuales, no legacy
```

---

### ❓ **"¿Qué versiones de C# usas y qué features aprovechas?"**

**TU RESPUESTA:**
```
"Uso C# 12 con .NET 9, aprovechando features modernas:

1. NULLABLE REFERENCE TYPES:
   - <Nullable>enable</Nullable> en todos los proyectos
   - Elimino NullReferenceExceptions en compile-time
   - Ejemplo: public string Correo { get; set; } = null!;

2. PATTERN MATCHING:
   - Switch expressions para mapeo de estados
   - Property patterns en validaciones

3. RECORDS (para DTOs):
   - Inmutables por defecto
   - Equality por valor, no referencia
   - Ejemplo: public record PaqueteDto(Guid Id, string Nombre, int Clases);

4. ASYNC/AWAIT:
   - Todos los handlers son async
   - Task<T> en queries, Task en commands
   - ConfigureAwait(false) en librerías

5. LINQ AVANZADO:
   - Consultas complejas con Include, ThenInclude
   - Proyecciones con Select para DTOs
   - GroupBy para reportes agregados
```

**CÓDIGO DE EJEMPLO:**
```csharp
// Pattern matching + Nullable types
public async Task<Result<PaqueteDto?>> Handle(GetPaqueteQuery request, CancellationToken ct)
{
    var paquete = await _context.Paquetes
        .Include(p => p.Alumno)
        .Include(p => p.TipoPaquete)
        .FirstOrDefaultAsync(p => p.IdPaquete == request.IdPaquete, ct);

    return paquete switch
    {
        null => Result<PaqueteDto?>.Failure("Paquete no encontrado"),
        var p when p.Alumno.Usuario.Sede != _currentUserSede 
            => Result<PaqueteDto?>.Failure("No autorizado"),
        var p => Result<PaqueteDto?>.Success(MapToDto(p))
    };
}
```

---

### ❓ **"¿Has trabajado con Azure? ¿Qué servicios conoces?"**

**TU RESPUESTA:**
```
"Sí, Chetango está deployado 100% en Azure. Servicios que uso:

1. AZURE APP SERVICE:
   - Backend API en App Service Plan B1 (~$70k COP/mes)
   - Auto-deploy con GitHub Actions
   - Launch profiles para QA y Producción
   - HTTPS automático con certificados Azure

2. AZURE SQL DATABASE:
   - Base de datos en tier S0/S1
   - Backups automáticos (7 días)
   - Connection pooling configurado
   - Preparado para escalar a Premium si crece

3. AZURE ENTRA ID (CIAM - antes AD B2C):
   - Autenticación OAuth 2.0 + PKCE
   - JWT tokens con roles (admin, profesor, alumno)
   - User flows personalizados
   - Integración con Microsoft Identity Web

4. AZURE STATIC WEB APPS:
   - Frontend React deployado gratis
   - CDN global incluido
   - Custom domain configurado
   - Auto-deploy desde GitHub

5. AZURE STORAGE (próximo):
   - Subiré comprobantes de pago a Blob Storage
   - CDN para avatares de usuarios

6. EXPERIENCIA ADICIONAL:
   - He configurado Key Vault para secrets
   - Application Insights para logs (pendiente activar)
   - Conozco Azure DevOps pipelines
```

**COSTOS ACTUALES (para mencionar):**
```
Azure App Service:    $70,000 COP/mes
Azure SQL Database:   $60,000 COP/mes
Static Web App:       $0 (free tier)
Storage Account:      $8,000 COP/mes
------------------------------------------
TOTAL:               ~$140,000 COP/mes ($35 USD)
```

---

## 3. PATRONES DE DISEÑO Y MEJORES PRÁCTICAS

### ❓ **"¿Qué patrones de diseño utilizas frecuentemente?"**

**TU RESPUESTA:**
```
"En Chetango implemento varios patrones:

1. CQRS (Command Query Responsibility Segregation):
   - Separo escrituras (Commands) de lecturas (Queries)
   - Commands: RegistrarAsistenciaCommand, CrearPaqueteCommand
   - Queries: GetAsistenciasPorClaseQuery, GetPaquetesActivosQuery
   - Beneficio: Optimizo queries sin afectar escrituras

2. MEDIATOR (vía MediatR):
   - Desacoplo controllers de handlers
   - Un pipeline centralizado: Request → Behavior → Handler → Response
   - Puedo agregar logging, validación, transacciones sin tocar handlers

3. REPOSITORY (implícito con EF Core):
   - DbContext actúa como Unit of Work
   - DbSet<T> como Repository genérico
   - IAppDbContext como abstracción para testing

4. DEPENDENCY INJECTION:
   - Constructor injection en todos los handlers
   - Configurado en Program.cs
   - Scoped lifetime para DbContext

5. SPECIFICATION (parcial):
   - Expresiones reusables para filtros
   - Ejemplo: FiltroPorSede, FiltroPorEstado

6. FACTORY (enums):
   - EstadoPago, TipoAsistencia como factories de estados
   - Evito crear objetos inválidos
```

**EJEMPLO MEDIATOR:**
```csharp
// Controller delgado, delega a MediatR
[HttpPost]
public async Task<IActionResult> CrearPaquete(CrearPaqueteCommand command)
{
    var resultado = await _mediator.Send(command);
    return resultado.IsSuccess ? Ok(resultado) : BadRequest(resultado);
}

// Handler con toda la lógica
public class CrearPaqueteHandler : IRequestHandler<CrearPaqueteCommand, Result>
{
    private readonly IAppDbContext _context;
    
    public async Task<Result> Handle(CrearPaqueteCommand request, CancellationToken ct)
    {
        // Lógica de negocio aquí
    }
}
```

---

### ❓ **"¿Cómo manejas las validaciones?"**

**TU RESPUESTA:**
```
"Tengo un enfoque de validación en múltiples capas:

1. VALIDACIÓN DE DATOS (FluentValidation - pendiente):
   - Planeo agregar validators para Commands
   - Ejemplo: CrearPaqueteValidator valida que Clases > 0

2. VALIDACIÓN DE NEGOCIO (en Handlers):
   - ¿El usuario tiene permisos sobre este recurso?
   - ¿El paquete tiene clases disponibles?
   - ¿La fecha de clase es válida?

3. VALIDACIÓN DE SEGURIDAD (en API):
   - JWT válido y no expirado (middleware)
   - Roles correctos con [Authorize(Policy = "AdminOnly")]
   - Sede del usuario coincide con recurso solicitado

4. VALIDACIÓN DE ESTADO:
   - No puedo registrar asistencia en clase futura
   - No puedo pagar paquete ya pagado
   - No puedo congelar paquete agotado
```

**EJEMPLO DE VALIDACIÓN DE NEGOCIO:**
```csharp
public async Task<Result> Handle(RegistrarAsistenciaCommand request, CancellationToken ct)
{
    var clase = await _context.Clases.FindAsync(request.IdClase);
    
    // Validaciones de negocio
    if (clase.FechaHora > DateTime.UtcNow.AddHours(-5))
        return Result.Failure("No se puede registrar asistencia de clase futura");
    
    var paquete = await ObtenerPaqueteActivo(request.IdAlumno);
    if (paquete == null)
        return Result.Failure("Alumno no tiene paquete activo");
    
    if (paquete.ClasesPendientes <= 0)
        return Result.Failure("Paquete agotado");
    
    // Procesar asistencia...
}
```

---

## 4. BASE DE DATOS Y PERSISTENCIA

### ❓ **"¿Cómo diseñas tu esquema de base de datos?"**

**TU RESPUESTA:**
```
"Sigo un proceso estructurado:

1. MODELADO CONCEPTUAL:
   - Identifico entidades del dominio: Usuario, Alumno, Clase, Paquete
   - Relaciones: Alumno compra Paquete, Alumno asiste a Clase
   - Cardinalidades: 1:N (Usuario → Alumno), N:N (Clase ↔ Alumno via Asistencia)

2. NORMALIZACIÓN (3FN):
   - Sin redundancia: TipoPaquete define precio, Paquete referencia TipoPaquete
   - Tablas de catálogo: TipoAsistencia, EstadoPago, TipoDocumento
   - FK explícitas con navegación bidireccional

3. CONVENCIONES:
   - PK: IdUsuario, IdAlumno (Guid para distribuido)
   - FK: IdUsuario, IdTipoPaquete
   - Timestamps: FechaCreacion, FechaModificacion
   - Soft deletes con EstadoActivo (cuando aplica)

4. ÍNDICES:
   - Por defecto en PKs y FKs
   - Índices adicionales en columnas de búsqueda frecuente:
     * Usuario.Correo (unique)
     * Paquete.IdAlumno + Estado (compuesto)

5. AUDITORÍA:
   - Tabla Auditoria con triggers (pendiente)
   - Registro de cambios críticos (pagos, asistencias)
```

**EJEMPLO RELACIONES:**
```csharp
// Usuario (1) → (N) Alumno
public class Usuario
{
    public Guid IdUsuario { get; set; }
    public ICollection<Alumno> Alumnos { get; set; }
}

// Clase (N) ↔ (N) Alumno via Asistencia (tabla intermedia)
public class Asistencia
{
    public Guid IdAsistencia { get; set; }
    public Guid IdClase { get; set; }
    public Clase Clase { get; set; }
    public Guid IdAlumno { get; set; }
    public Alumno Alumno { get; set; }
    public int IdTipoAsistencia { get; set; }
    public TipoAsistencia TipoAsistencia { get; set; }
}
```

---

### ❓ **"¿Cómo manejas las migraciones de base de datos?"**

**TU RESPUESTA:**
```
"Uso Entity Framework Core Migrations con este flujo:

1. DESARROLLO LOCAL:
   - Cambio entidades en Domain
   - Ejecuto: dotnet ef migrations add NombreMigration
   - Reviso el código generado (Up/Down methods)
   - Aplico: dotnet ef database update
   - Pruebo cambios en ChetangoDB_Dev

2. VERSIONAMIENTO:
   - Migrations se commitean a Git
   - Son parte del código fuente
   - Nombradas con timestamp: 20260223_AgregarSedeAUsuario

3. PRODUCCIÓN:
   - NO uso automatic migrations
   - Script SQL: dotnet ef migrations script > migration.sql
   - Reviso script manualmente
   - Aplico en ventana de mantenimiento
   - Backup antes de aplicar

4. BUENAS PRÁCTICAS:
   - Nunca elimino migrations aplicadas
   - Para rollback: creo migration reversa
   - Seed data en archivos separados (scripts/seed_*.sql)
   - Documento breaking changes

5. MIGRATIONS COMPLEJAS:
   - Migración multi-sede: Agregué columna Sede con default Medellin
   - Migración datos corruptos: Fix encoding UTF-8
   - Script: fix_encoding_simple.sql
```

**EJEMPLO MIGRATION:**
```csharp
public partial class AgregarSedeAUsuario : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "Sede",
            table: "Usuarios",
            type: "int",
            nullable: false,
            defaultValue: 1); // Medellin por defecto
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Sede", table: "Usuarios");
    }
}
```

---

### ❓ **"¿Optimizas queries? ¿Cómo evitas N+1 problems?"**

**TU RESPUESTA:**
```
"Sí, optimizo queries constantemente:

1. EAGER LOADING con Include:
   - Cargo relaciones en una sola query
   - Ejemplo: .Include(p => p.Alumno).Include(p => p.TipoPaquete)
   - Evito N+1: 1 query para paquetes + N queries para alumnos

2. PROYECCIONES con Select:
   - Cargo solo columnas necesarias
   - Mapeo a DTOs directamente en query
   - Ejemplo: Select(u => new UsuarioDto { Nombre = u.NombreUsuario })

3. ASNOTRACKING para queries de solo lectura:
   - .AsNoTracking() en queries que no modifican datos
   - EF no trackea cambios, más rápido
   - Ejemplo: GetReportesQuery siempre usa AsNoTracking

4. PAGINACIÓN:
   - .Skip() y .Take() para listas grandes
   - Nunca cargo 1000+ registros sin paginación
   - Ejemplo: GetClasesQuery(page, pageSize)

5. ÍNDICES:
   - Analizo queries lentas con SQL Profiler
   - Creo índices en columnas de filtro frecuente
   - Índice en Paquete.IdAlumno + Estado

6. MONITOREO:
   - En desarrollo: EF logging de queries generadas
   - Producción (próximo): Application Insights
```

**EJEMPLO N+1 RESUELTO:**
```csharp
// ❌ MAL (N+1 problem)
var paquetes = await _context.Paquetes.ToListAsync();
foreach (var p in paquetes)
{
    // Lazy loading: 1 query por iteración
    var alumno = p.Alumno.Usuario.NombreUsuario; 
}

// ✅ BIEN (1 query total)
var paquetes = await _context.Paquetes
    .Include(p => p.Alumno)
        .ThenInclude(a => a.Usuario)
    .Include(p => p.TipoPaquete)
    .AsNoTracking()
    .ToListAsync();
```

---

## 5. SEGURIDAD Y AUTENTICACIÓN

### ❓ **"¿Cómo implementas la seguridad en tu API?"**

**TU RESPUESTA:**
```
"Seguridad en múltiples capas:

1. AUTENTICACIÓN (OAuth 2.0 + Azure Entra ID):
   - Delegada a proveedor externo (no manejo passwords)
   - JWT tokens con expiración (1 hora)
   - Refresh tokens para renovación
   - Authorization Code Flow + PKCE (más seguro que implicit flow)

2. AUTORIZACIÓN (basada en Roles y Policies):
   - Roles: admin, profesor, alumno (claims en JWT)
   - Policies en Program.cs:
     * AdminOnly: Solo admins
     * AdminOrProfesor: Admins y profesores
     * ApiScope: Valida audience del token
   - [Authorize(Policy = "AdminOnly")] en endpoints

3. OWNERSHIP VALIDATION:
   - Verifico que usuario tenga permiso sobre recurso
   - Ejemplo: Alumno solo ve sus propios pagos
   - Filtro por Sede automáticamente

4. HTTPS OBLIGATORIO:
   - Todos los endpoints en HTTPS
   - HTTP → HTTPS redirect automático
   - HSTS header habilitado

5. CORS RESTRICTIVO:
   - Solo orígenes permitidos en appsettings
   - Producción: solo https://app.corporacionchetango.com
   - No permito * (any origin) en producción

6. SQL INJECTION: Prevención automática
   - EF Core usa parámetros parametrizados
   - Nunca concateno SQL strings
   - Stored procedures con parámetros si los uso

7. SECRETOS:
   - No commiteo connection strings a Git
   - appsettings.Development.json en .gitignore
   - Producción: Azure App Configuration / Key Vault
```

**EJEMPLO OWNERSHIP:**
```csharp
public async Task<IActionResult> GetMisPagos()
{
    var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
    var usuario = await _context.Usuarios
        .FirstOrDefaultAsync(u => u.Correo == userEmail);
    
    // Filtro automático por usuario autenticado
    var pagos = await _context.Pagos
        .Where(p => p.Paquete.Alumno.IdUsuario == usuario.IdUsuario)
        .ToListAsync();
    
    return Ok(pagos);
}
```

---

### ❓ **"¿Has manejado autenticación multi-tenant?"**

**TU RESPUESTA:**
```
"Actualmente manejo multi-sede (preludio a multi-tenant):

1. IMPLEMENTACIÓN ACTUAL:
   - Enum Sede en Usuario (Medellin, Manizales)
   - Filtrado automático por sede del usuario
   - Administradores ven datos de su propia sede
   - Separación lógica, misma base de datos

2. PRÓXIMA EVOLUCIÓN (SaaS):
   - Plan documentado en PLAN-ESCALAMIENTO-SAAS.md
   - Estrategia: Database-per-tenant (más seguridad)
   - TenantId en JWT token
   - Connection string dinámico por tenant
   - Aislamiento total de datos

3. CONOCIMIENTOS APLICABLES:
   - He trabajado con múltiples esquemas en SQL
   - Conozco Row-Level Security (RLS) en PostgreSQL
   - Experiencia con discriminators en EF Core

4. CONSIDERACIONES:
   - Performance: Cache por tenant
   - Seguridad: Validación estricta de TenantId
   - Migraciones: Aplicar a todos los tenants
   - Backups: Por tenant individual
```

---

## 6. CLOUD Y DEVOPS (AZURE)

### ❓ **"¿Cómo manejas CI/CD?"**

**TU RESPUESTA:**
```
"GitHub Actions con Azure integration:

1. PIPELINE BACKEND:
   - Trigger: Push a branch 'main'
   - Steps:
     * Checkout código
     * dotnet restore
     * dotnet build -c Release
     * dotnet test (cuando tenga tests)
     * dotnet publish
     * Azure Web App Deploy
   - Deploy automático a App Service

2. PIPELINE FRONTEND:
   - Trigger: Push a branch 'develop' (directo a prod)
   - Steps:
     * npm install
     * npm run build
     * Deploy a Azure Static Web App
   - CDN invalidation automático

3. ENVIRONMENTS:
   - Develop: Rama de integración
   - Main: Producción
   - Feature branches: Review manual antes de merge

4. SECRETOS:
   - GitHub Secrets para tokens de Azure
   - No expongo credenciales en código
   - Azure Service Principal con permisos mínimos

5. ROLLBACK:
   - Azure App Service: Slots de staging
   - Puedo hacer swap si algo falla
   - 50+ deployments exitosos hasta ahora

6. PRÓXIMOS PASOS:
   - Agregar tests unitarios al pipeline
   - Stage de smoke tests post-deploy
   - Notificaciones a Slack/Teams
```

---

### ❓ **"¿Has trabajado con contenedores (Docker/Kubernetes)?"**

**TU RESPUESTA:**
```
"Experiencia con Docker, Kubernetes en estudio:

1. DOCKER:
   - He dockerizado aplicaciones .NET
   - Conozco multi-stage builds para reducir tamaño
   - Ejemplo Dockerfile para Chetango (listo para crear):
     * Stage 1: Build con SDK
     * Stage 2: Runtime con aspnet
     * Imagen final < 200MB

2. DOCKER COMPOSE:
   - Para desarrollo local con SQL Server
   - docker-compose.yml con API + DB + Frontend
   - Útil para onboarding de desarrolladores

3. KUBERNETES (teórico/en aprendizaje):
   - Entiendo conceptos: Pods, Services, Deployments
   - He usado kubectl básico
   - Listo para profundizar si el proyecto lo requiere

4. AZURE CONTAINER APPS:
   - Sé que es alternativa managed a K8s
   - En mi roadmap para escalamiento
   - Más económico que AKS para 50-100 academias

5. RAZÓN DE NO USAR AHORA:
   - App Service es suficiente para 300 usuarios
   - Menor complejidad operacional
   - Cuando llegue a 10k+ usuarios, migro a containers
```

**DOCKERFILE EJEMPLO (para mencionar):**
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "Chetango.Api.dll"]
```

---

## 7. ESCALABILIDAD Y PERFORMANCE

### ❓ **"¿Cómo preparas tu aplicación para escalar?"**

**TU RESPUESTA:**
```
"Tengo un plan de escalamiento documentado (PLAN-ESCALAMIENTO-SAAS.md):

1. ESTADO ACTUAL (300 usuarios, 2 sedes):
   - App Service B1: 1 core, 1.75GB RAM
   - SQL Database S0: 10 DTUs
   - Suficiente, costo $140k COP/mes

2. FASE 2 (50-60 academias, ~10k usuarios):
   - App Service S1: 1 core, 1.75GB RAM, auto-scaling
   - SQL Database S3: 100 DTUs
   - Azure Redis Cache para sesiones
   - Costo estimado: $1.8M COP/mes

3. FASE 3 (150-200 academias, ~30k usuarios):
   - App Service P1v3: 2 cores, 8GB, múltiples instancias
   - SQL Database P1: 125 DTUs, replicación geo
   - CDN para estáticos
   - Costo: $5M COP/mes

4. OPTIMIZACIONES APLICADAS:
   - Async/await en todos los endpoints
   - DTOs para reducir payload
   - AsNoTracking en queries de lectura
   - Connection pooling configurado

5. OPTIMIZACIONES PENDIENTES:
   - Implementar Redis Cache
   - Output caching para reportes
   - CQRS con lectura de replica (read/write split)
   - Message queue (Azure Service Bus) para async operations
```

---

### ❓ **"¿Cómo manejas caching?"**

**TU RESPUESTA:**
```
"Estrategia de caching por capas (próximo):

1. NIVEL 1: MEMORY CACHE (próximo a implementar):
   - IMemoryCache en .NET
   - Catálogos poco cambiantes: TiposPaquete, TiposAsistencia
   - Expiración: 1 hora
   - Ejemplo: GetTiposAsistenciaQuery cacheado

2. NIVEL 2: DISTRIBUTED CACHE (cuando escale):
   - Azure Redis Cache
   - Sesiones de usuario
   - Resultados de queries costosas
   - Cache invalidation cuando hay cambios

3. NIVEL 3: CDN:
   - Azure CDN (ya habilitado en Static Web App)
   - Assets estáticos: imágenes, CSS, JS
   - Avatares de usuario cuando implemente uploads

4. NIVEL 4: HTTP CACHING:
   - Cache-Control headers
   - ETags para recursos
   - 304 Not Modified responses

5. ESTRATEGIAS DE INVALIDACIÓN:
   - Time-based: Expira después de X minutos
   - Event-based: Invalido cuando cambia dato
   - Ejemplo: Creo nuevo TipoPaquete → invalido cache
```

---

## 8. TESTING Y CALIDAD

### ❓ **"¿Qué tipos de testing implementas?"**

**TU RESPUESTA:**
```
"Estrategia de testing (en implementación):

1. UNIT TESTS (próximo):
   - xUnit para .NET
   - Handlers de Application layer
   - Mocking con Moq (IAppDbContext)
   - Ejemplo: CrearPaqueteHandlerTests

2. INTEGRATION TESTS (próximo):
   - WebApplicationFactory para API
   - TestContainers para SQL Server
   - Probar endpoints end-to-end
   - Ejemplo: POST /api/paquetes → Verify DB

3. E2E TESTS (Frontend implementado):
   - Playwright para flujos completos
   - Tests críticos: Login, Registrar Asistencia, Crear Pago
   - ~15+ tests implementados
   - Ejecuto antes de cada deploy

4. TESTING MANUAL:
   - Swagger UI para explorar API
   - Postman collections (pendiente)
   - Testing de usuarios reales en QA

5. LOAD TESTING (futuro):
   - k6 o Apache JMeter
   - Simular 1000+ usuarios concurrentes
   - Identificar bottlenecks antes de escalar

6. RAZÓN DE PRIORIZACIÓN:
   - E2E primero: Detecta problemas críticos de UX
   - Unit tests próximos: Refactorización segura
   - Load tests cuando tenga tráfico real
```

---

## 9. API DESIGN Y REST

### ❓ **"¿Qué principios REST sigues?"**

**TU RESPUESTA:**
```
"Sigo RESTful best practices:

1. RECURSOS Y VERBOS HTTP:
   - GET /api/paquetes → Listar paquetes
   - GET /api/paquetes/{id} → Obtener uno
   - POST /api/paquetes → Crear
   - PUT /api/paquetes/{id} → Actualizar completo
   - PATCH /api/paquetes/{id}/congelar → Acción específica
   - DELETE /api/paquetes/{id} → Eliminar (soft delete)

2. CÓDIGOS DE ESTADO HTTP:
   - 200 OK: Operación exitosa
   - 201 Created: Recurso creado (con Location header)
   - 400 Bad Request: Validación falló
   - 401 Unauthorized: Token inválido
   - 403 Forbidden: Sin permisos
   - 404 Not Found: Recurso no existe
   - 500 Internal Server Error: Error servidor

3. ESTRUCTURA DE RESPUESTAS:
   - Consistente: { success, data, error }
   - Data siempre en formato camelCase (JSON)
   - Errores descriptivos: { message, code, details }

4. VERSIONAMIENTO (preparado):
   - /api/v1/paquetes
   - Headers: Accept: application/vnd.chetango.v1+json

5. PAGINACIÓN:
   - Query params: ?page=1&pageSize=20
   - Headers: X-Total-Count, Link (next/prev)

6. FILTROS Y BÚSQUEDA:
   - Query params: ?sede=1&estado=activo
   - Búsqueda: ?search=Juan

7. IDEMPOTENCIA:
   - GET, PUT, DELETE son idempotentes
   - POST no (crea recurso cada vez)
```

**EJEMPLO ENDPOINT:**
```csharp
[HttpGet("{id}")]
[ProducesResponseType(typeof(PaqueteDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public async Task<IActionResult> GetPaquete(Guid id)
{
    var query = new GetPaqueteByIdQuery(id);
    var result = await _mediator.Send(query);
    
    if (!result.IsSuccess)
        return NotFound(new { message = result.Error });
    
    return Ok(result.Data);
}
```

---

### ❓ **"¿Cómo documentas tu API?"**

**TU RESPUESTA:**
```
"Múltiples estrategias de documentación:

1. SWAGGER/OpenAPI:
   - Generación automática con Swashbuckle
   - Interfaz interactiva en /swagger
   - Documentación de schemas, parámetros, respuestas
   - Try it out para probar endpoints

2. REDOC:
   - Vista alternativa más elegante en /redoc
   - Mejor para leer que para probar
   - Exportable a PDF

3. MARKDOWN DOCS (implementado):
   - docs/api/asistencias.md
   - docs/api/paquetes.md
   - docs/api/pagos.md
   - Ejemplos de requests/responses
   - Casos de uso explicados

4. COMENTARIOS XML:
   - /// <summary> en controllers y DTOs
   - Aparecen en Swagger UI
   - Ejemplo: /// <summary>Obtiene paquetes de un alumno</summary>

5. POSTMAN COLLECTIONS (próximo):
   - Exportar desde Swagger
   - Compartir con frontend team
   - Environments para Dev/QA/Prod

6. MANUALES DE USUARIO:
   - MANUAL-ADMINISTRADOR.md
   - MANUAL-PROFESOR.md
   - MANUAL-ALUMNO.md
   - Flujos de negocio documentados
```

---

## 10. PROBLEMAS COMPLEJOS Y SOLUCIONES

### ❓ **"Cuéntame sobre un problema técnico complejo que resolviste"**

**PROBLEMA 1: Sistema Multi-Sede con Aislamiento de Datos**
```
CONTEXTO:
- Academia con 2 sedes independientes (Medellín y Manizales)
- Necesitaban ver solo sus datos, no compartir alumnos/profesores
- Misma base de datos, separación lógica

DESAFÍO:
- Evitar que admin de Medellín vea alumnos de Manizales
- Filtrado automático en TODAS las queries
- No duplicar código de filtrado

SOLUCIÓN:
1. Agregué enum Sede en entidad Usuario
2. Middleware que extrae sede del token JWT
3. Filtro global en DbContext:
   - QueryFilter en OnModelCreating
   - Filtra automáticamente por sede del usuario autenticado
4. Administrador General puede ver todas las sedes (bypass filter)

RESULTADO:
- Cero queries manuales con .Where(x => x.Sede == userSede)
- Imposible ver datos de otra sede (nivel EF Core)
- Preparado para multi-tenant real (cambiar Sede por TenantId)
```

**CÓDIGO:**
```csharp
// Filtro global en DbContext
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    var userSede = _httpContextAccessor.HttpContext?.User
        .FindFirst("sede")?.Value;
    
    if (!string.IsNullOrEmpty(userSede))
    {
        modelBuilder.Entity<Usuario>()
            .HasQueryFilter(u => u.Sede == (Sede)int.Parse(userSede));
    }
}
```

---

**PROBLEMA 2: Codificación UTF-8 Corrupta en SQL Server**
```
CONTEXTO:
- Nombres con tildes se guardaban corruptos: "María" → "MarÃa"
- Migración de datos causó problema
- 100+ usuarios afectados

DIAGNÓSTICO:
- SQL Server usa NVARCHAR para Unicode
- Necesita prefijo N'' en strings literales
- Scripts sin N'' causaron corrupción

SOLUCIÓN:
1. Script fix_encoding_simple.sql:
   - REPLACE anidado para todos los caracteres corruptos
   - UPDATE Usuarios SET Nombre = REPLACE(REPLACE(...))
2. Actualicé todos los seed scripts con N''
3. Documenté en módules.md para equipo

RESULTADO:
- 100% datos corregidos
- Prevención documentada
- Aprendizaje: Revisar encoding en migraciones
```

---

**PROBLEMA 3: Descontar Clases de Paquete con Concurrencia**
```
CONTEXTO:
- Registrar asistencia debe descontar 1 clase del paquete
- ¿Qué pasa si 2 profesores marcan presente al mismo alumno simultáneamente?

DESAFÍO:
- Race condition: Ambos leen ClasesPendientes = 5
- Ambos escriben ClasesPendientes = 4
- Se pierde 1 descuento

SOLUCIÓN:
1. Transaction con isolation level Serializable
2. Locking optimista con RowVersion (próximo)
3. Validación en Handler:
   - Verificar ClasesPendientes > 0 antes de descontar
   - Si falla, rollback automático

ALTERNATIVA (próxima):
- SQL update atómico: 
  UPDATE Paquetes 
  SET ClasesPendientes = ClasesPendientes - 1
  WHERE ClasesPendientes > 0
```

---

### ❓ **"¿Cómo priorizas features vs. deuda técnica?"**

**TU RESPUESTA:**
```
"Equilibrio pragmático:

1. MVP PRIMERO:
   - Prioricé features críticas: Asistencias, Pagos, Clases
   - Dejé para después: Testing exhaustivo, Caching

2. REGLA 80/20:
   - 80% del valor con 20% del esfuerzo
   - Ejemplo: Implementé Swagger antes que tests unitarios
   - Swagger da valor inmediato (documentación), tests son inversión

3. DEUDA TÉCNICA CONTROLADA:
   - Documento en TODOs y issues de GitHub
   - Ejemplo: // TODO: Implementar caching cuando tenga >1000 usuarios
   - No bloquea MVP pero está planificado

4. REFACTORIZACIÓN CONTINUA:
   - No espero "sprint de refactoring"
   - Boy Scout Rule: Dejo código mejor que lo encontré
   - Ejemplo: Migré a CQRS progresivamente

5. CRITERIO DE PRIORIZACIÓN:
   - ¿Bloquea producción? → Urgente
   - ¿Afecta seguridad? → Alta prioridad
   - ¿Mejora UX? → Media prioridad
   - ¿Optimiza código interno? → Baja prioridad (pero importante)

6. BALANCE ACTUAL EN CHETANGO:
   - Features: 70% completado
   - Testing: 30% completado (E2E sí, Unit no)
   - Docs: 90% completado (muy importante para SaaS)
```

---

## 📊 PREGUNTAS SOBRE TU EXPERIENCIA GENERAL

### ❓ **"¿Por qué creaste este proyecto?"**

**TU RESPUESTA:**
```
"Tres objetivos principales:

1. RESOLVER PROBLEMA REAL:
   - Mi academia usaba Excel y WhatsApp
   - Procesos manuales, propensos a errores
   - Necesitaban automatización

2. DEMOSTRAR HABILIDADES MODERNAS:
   - 10+ años en .NET, pero en versiones antiguas (Framework 4.x)
   - Quería probar dominio de .NET 9, C# 12, EF Core 9
   - Arquitecturas modernas: CQRS, Clean Architecture
   - Cloud: Azure end-to-end

3. PREPARACIÓN PARA EL MERCADO:
   - Entrevistas técnicas requieren proyectos demostrables
   - Mejor que ejercicios académicos: Es un producto real en producción
   - Roadmap a SaaS demuestra pensamiento de escalabilidad
```

---

### ❓ **"¿Qué aprendiste de este proyecto?"**

**TU RESPUESTA:**
```
1. ARQUITECTURA:
   - CQRS no es overkill si se planea crecer
   - Clean Architecture facilita testing (aunque aún no lo hice completo)
   - Separación de capas vale la pena

2. CLOUD:
   - Azure es poderoso pero costoso si no optimizas
   - Static Web Apps es excelente para React (gratis + CDN)
   - App Service B1 suficiente para arrancar

3. AUTENTICACIÓN:
   - OAuth 2.0 es complejo pero delegar a Azure AD vale la pena
   - No manejar passwords es gran ventaja de seguridad

4. MULTI-TENANCY:
   - Enum Sede fue buena primera iteración
   - Próximo paso: TenantId + database per tenant
   - Query filters son herramienta poderosa

5. DOCUMENTACIÓN:
   - Escribir manuales te obliga a pensar en UX
   - Documentación técnica acelera onboarding
   - Swagger + Markdown + comentarios = completeness
```

---

### ❓ **"¿Qué harías diferente si empezaras de nuevo?"**

**TU RESPUESTA:**
```
1. TDD DESDE EL INICIO:
   - Implementaría tests unitarios desde día 1
   - Ahora es más difícil agregar tests a código existente
   - xUnit + Moq configurado en setup inicial

2. FEATURE FLAGS:
   - Para activar/desactivar features en producción
   - Útil para rollouts graduales
   - Azure App Configuration o LaunchDarkly

3. STRUCTURED LOGGING:
   - Serilog desde el inicio
   - Application Insights integrado temprano
   - Rastrear errores es crítico en producción

4. API VERSIONAMIENTO:
   - /api/v1/ desde el primer endpoint
   - Evita breaking changes cuando escale

5. DOCKER DESDE EL INICIO:
   - Desarrollo local con Docker Compose
   - Reproducibilidad entre desarrolladores
   - Facilita onboarding

PERO:
- Estas son optimizaciones de lujo
- Priorizar MVP fue correcto
- Ahora puedo agregar estas mejoras incrementalmente
```

---

## 🎯 PRÓXIMOS PASOS QUE PUEDES MENCIONAR

```
"Mi roadmap inmediato para Chetango:

1. AWS INTEGRATION (próximo):
   - Lambda para procesar comprobantes de pago con IA
   - S3 para almacenamiento de imágenes
   - API Gateway como alternativa a Azure

2. TESTING:
   - xUnit tests para todos los Handlers
   - Integration tests con WebApplicationFactory
   - Coverage >80%

3. OBSERVABILITY:
   - Application Insights en producción
   - Custom metrics: Asistencias/día, Pagos/mes
   - Alertas proactivas

4. MULTI-TENANT REAL:
   - TenantId en lugar de Sede
   - Database per tenant
   - Self-service signup

5. FEATURES AVANZADAS:
   - Notificaciones push (Firebase)
   - Reportes PDF con QuestPDF
   - Integración Stripe/Wompi para pagos online

Esto demuestra que entiendo que el desarrollo es iterativo
y que siempre hay mejoras por hacer."
```

---

## 🎤 TIPS PARA LA ENTREVISTA

### **Cómo Presentar el Proyecto:**
```
"Chetango es un sistema de gestión SaaS para academias de danza 
que desarrollé end-to-end. 

Stack: .NET 9, React, Azure, SQL Server.

Implementa Clean Architecture con CQRS, autenticación OAuth 2.0 
con Azure AD, y está en producción con 300 usuarios reales.

Actualmente maneja multi-sede y tengo un plan documentado 
para escalarlo a 200+ academias como SaaS multi-tenant.

Puedo mostrar código en GitHub o demo en vivo si gustan."
```

### **Cómo Responder "Muéstrame tu código":**
```
1. Abre Program.cs: Muestra configuración de DI, middleware
2. Abre un Handler: Ejemplo de CQRS con validación
3. Abre una entidad: Domain model con relaciones
4. Abre Swagger UI: Demo de API funcionando
5. Abre docs/: Demuestra documentación profesional
```

### **Qué NO Decir:**
```
❌ "Es un proyecto simple"
❌ "Todavía tiene bugs"
❌ "No está terminado"

✅ "Es un MVP en producción que estoy evolucionando"
✅ "Implementa patrones enterprise que escalan"
✅ "Tengo roadmap claro de mejoras"
```

---

## 📚 RECURSOS ADICIONALES

### **Enlaces del Proyecto:**
- GitHub Backend: [Tu repo]
- GitHub Frontend: [Tu repo]
- Demo en vivo: https://app.corporacionchetango.com
- Documentación: Ver carpeta docs/

### **Preparación Pre-Entrevista:**
1. Tener el proyecto corriendo localmente
2. Swagger UI abierto en navegador
3. GitHub repo abierto en tabs
4. Arquitectura dibujada en whiteboard/papel
5. Este documento impreso o en segunda pantalla

### **Preguntas para Hacer al Entrevistador:**
```
"¿Qué arquitectura manejan actualmente?"
"¿Cómo manejan el escalamiento?"
"¿Usan microservicios o monolito modular?"
"¿Qué desafíos técnicos enfrenta el equipo?"
"¿Cómo es el proceso de deploy?"
```

---

**¡ÉXITO EN TUS ENTREVISTAS!** 🚀

*Documento generado para preparación de entrevistas técnicas*  
*Proyecto: Chetango - Sistema de Gestión para Academias*  
*Febrero 2026*
