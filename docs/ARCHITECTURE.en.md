# System Architecture - Aphellion (Chetango)

**Author:** Jorge Padilla  
**Last Updated:** March 2026

## Overview

Aphellion (Chetango) is an enterprise SaaS platform for dance academy management built with **Clean Architecture + CQRS** pattern. The system manages 500+ active users across multiple locations with 99.8% uptime.

---

## Architectural Patterns

### Clean Architecture

The backend follows Uncle Bob's Clean Architecture with clear separation of concerns:

```
┌─────────────────────────────────────────┐
│         Chetango.Api (Presentation)     │  ← HTTP Endpoints, Controllers
├─────────────────────────────────────────┤
│      Chetango.Application (Use Cases)   │  ← CQRS, Commands, Queries, Handlers
├─────────────────────────────────────────┤
│        Chetango.Domain (Entities)       │  ← Business Logic, Domain Models
├─────────────────────────────────────────┤
│   Chetango.Infrastructure (Data/I/O)    │  ← EF Core, External APIs, Azure Services
└─────────────────────────────────────────┘
```

**Benefits:**
- **Testability:** Business logic isolated from infrastructure
- **Maintainability:** Changes to UI/DB don't affect core logic
- **Flexibility:** Easy to swap data sources or presentation layers

### CQRS (Command Query Responsibility Segregation)

Separate models for reading and writing data using **MediatR**:

**Commands** (Write Operations):
```csharp
// Example: CreateClaseCommand.cs
public record CreateClaseCommand(
    string Nombre,
    DateTime FechaHora,
    int Duracion,
    int CupoMaximo,
    int ProfesorId
) : IRequest<ClaseDto>;

// Handler: CreateClaseHandler.cs
public class CreateClaseHandler : IRequestHandler<CreateClaseCommand, ClaseDto>
{
    public async Task<ClaseDto> Handle(CreateClaseCommand request, CancellationToken ct)
    {
        var clase = new Clase { /* map properties */ };
        _context.Clases.Add(clase);
        await _context.SaveChangesAsync(ct);
        return _mapper.Map<ClaseDto>(clase);
    }
}
```

**Queries** (Read Operations):
```csharp
// Example: GetClasesQuery.cs
public record GetClasesQuery(DateTime? FechaInicio, DateTime? FechaFin) : IRequest<List<ClaseDto>>;

// Handler: GetClasesHandler.cs
public class GetClasesHandler : IRequestHandler<GetClasesQuery, List<ClaseDto>>
{
    public async Task<List<ClaseDto>> Handle(GetClasesQuery request, CancellationToken ct)
    {
        return await _context.Clases
            .AsNoTracking()
            .Where(c => c.FechaHora >= request.FechaInicio)
            .ProjectTo<ClaseDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
}
```

**Benefits:**
- **Performance:** Read models optimized with `.AsNoTracking()`
- **Scalability:** Separate read/write databases (future enhancement)
- **Clear Intent:** Command vs Query naming makes code self-documenting

---

## Technology Stack

### Backend
- **.NET 9:** Latest LTS version with performance improvements
- **Entity Framework Core 9:** ORM with migrations, LINQ queries
- **MediatR:** CQRS implementation, decouples handlers from controllers
- **AutoMapper:** Object-to-object mapping (Entity ↔ DTO)
- **FluentValidation:** Request validation before handler execution
- **Azure AD B2C:** OAuth 2.0 / OpenID Connect authentication

### Frontend
- **React 18:** UI library with concurrent rendering
- **TypeScript 5:** Type safety and IntelliSense
- **Vite:** Fast build tool and dev server
- **TailwindCSS:** Utility-first styling with custom glassmorphism theme
- **TanStack Query:** Data fetching, caching, synchronization
- **React Router:** Client-side routing
- **Axios:** HTTP client with interceptors for auth tokens

### Infrastructure
- **Azure App Service (B1):** Hosts .NET backend
- **Azure SQL Database (S0):** Relational database (10 DTUs)
- **Azure Static Web Apps:** Hosts React frontend with CDN
- **Azure AD B2C:** Identity provider
- **GitHub Actions:** CI/CD pipelines

---

## Database Design

### Multi-Tenant Strategy (Current)

**Shared Database + Tenant Identifier:**
```sql
CREATE TABLE Usuarios (
    IdUsuario INT PRIMARY KEY IDENTITY,
    NombreUsuario NVARCHAR(100),
    Correo NVARCHAR(255),
    TenantId NVARCHAR(50) NOT NULL,  -- Identifies tenant
    SedeId INT NOT NULL,              -- Enum: Medellin, Manizales
    FechaCreacion DATETIME2 DEFAULT GETDATE()
);

CREATE INDEX IX_Usuarios_TenantId ON Usuarios(TenantId);
CREATE INDEX IX_Usuarios_SedeId ON Usuarios(SedeId);
```

**Tenant Context Middleware:**
```csharp
public class TenantMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var tenantId = context.User.FindFirst("TenantId")?.Value;
        context.Items["TenantId"] = tenantId;
        await _next(context);
    }
}

// Applied globally to all queries
_context.Clases.Where(c => c.TenantId == CurrentTenantId);
```

### Entity Relationships

**Core Entities:**
- `Usuarios` (Users) - Base user information
- `Alumnos` (Students) - Extends Users with student-specific data
- `Profesores` (Professors) - Extends Users with professor-specific data
- `Clases` (Classes) - Dance classes scheduled
- `Asistencias` (Attendances) - Student attendance records
- `Pagos` (Payments) - Payment transactions
- `Paquetes` (Packages) - Class packages (e.g., 10 classes for $50)
- `Suscripciones` (Subscriptions) - Recurring subscriptions (future)

**Key Relationships:**
```
Usuarios 1──*  Alumnos
Usuarios 1──* Profesores
Profesores 1──* Clases
Clases 1──* Asistencias
Alumnos 1──* Asistencias
Alumnos 1──* Pagos
Paquetes 1──* Pagos
```

---

## Authentication & Authorization Flow

### OAuth 2.0 with Azure AD B2C

1. **User Login:**
   - Frontend redirects to Azure AD B2C login page
   - User enters credentials
   - Azure returns JWT access token + ID token

2. **Token Validation:**
   ```csharp
   builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
   ```

3. **Authorization:**
   ```csharp
   [Authorize(Roles = "Admin")]
   public class PagosController : ControllerBase { }
   
   [Authorize(Roles = "Profesor,Admin")]
   public async Task<IActionResult> TomarAsistencia() { }
   ```

4. **Claims Extraction:**
   ```csharp
   var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
   var tenantId = User.FindFirst("TenantId")?.Value;
   var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);
   ```

---

## API Design

### RESTful Endpoints

**Conventions:**
- `GET /api/clases` - List all classes
- `GET /api/clases/{id}` - Get class by ID
- `POST /api/clases` - Create new class
- `PUT /api/clases/{id}` - Update class
- `DELETE /api/clases/{id}` - Delete class

**Response Format:**
```json
{
  "success": true,
  "data": {
    "id": 123,
    "nombre": "Salsa Intermedia",
    "profesor": "Juan Pérez"
  },
  "errors": []
}
```

### Validation & Error Handling

**FluentValidation:**
```csharp
public class CreateClaseValidator : AbstractValidator<CreateClaseCommand>
{
    public CreateClaseValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CupoMaximo).GreaterThan(0).LessThanOrEqualTo(50);
        RuleFor(x => x.FechaHora).GreaterThan(DateTime.Now);
    }
}
```

**Global Exception Handler:**
```csharp
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var error = context.Features.Get<IExceptionHandlerFeature>();
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { 
            success = false, 
            errors = new[] { error.Error.Message } 
        });
    });
});
```

---

## Performance Optimizations

### Backend

**1. Database Query Optimization:**
```csharp
// ❌ Bad: N+1 query problem
var clases = await _context.Clases.ToListAsync();
foreach (var clase in clases)
{
    var profesor = await _context.Profesores.FindAsync(clase.ProfesorId);
}

// ✅ Good: Eager loading with Include
var clases = await _context.Clases
    .Include(c => c.Profesor)
    .AsNoTracking()  // Read-only optimization
    .ToListAsync();
```

**2. Async/Await Everywhere:**
```csharp
// All I/O operations are async
public async Task<ClaseDto> Handle(GetClaseQuery request, CancellationToken ct)
{
    return await _context.Clases
        .AsNoTracking()
        .FirstOrDefaultAsync(c => c.Id == request.Id, ct);
}
```

**3. Caching (Planned):**
```csharp
// Redis caching for frequently accessed data
[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
public async Task<IActionResult> GetProfesores() { }
```

### Frontend

**1. Code Splitting:**
```typescript
// Lazy load routes
const AsistenciasPage = lazy(() => import('./pages/Asistencias'));
const PagosPage = lazy(() => import('./pages/Pagos'));
```

**2. TanStack Query Caching:**
```typescript
const { data: clases } = useQuery({
  queryKey: ['clases', fecha],
  queryFn: () => apiClient.getClases(fecha),
  staleTime: 5 * 60 * 1000,  // 5 minutes
  cacheTime: 10 * 60 * 1000  // 10 minutes
});
```

---

## Testing Strategy

### Backend Tests
- **Unit Tests:** Test handlers in isolation (mocked DbContext)
- **Integration Tests:** Test API endpoints with in-memory database
- **Tools:** xUnit, Moq, FluentAssertions

### Frontend Tests
- **Unit Tests:** Test React components with Vitest + Testing Library
- **E2E Tests:** Test user flows with Playwright (15+ tests)
  - Authentication flow
  - Attendance marking
  - Payment verification
  - Class scheduling

---

## Future Architecture Evolution

### Phase 1: Database-per-Tenant (Q2 2026)
- Implement connection string resolver based on tenant identifier
- Migrate existing tenants to isolated databases
- Benefits: Complete data isolation, independent scaling

### Phase 2: Microservices (Q4 2026)
- Split monolith into services:
  - **Auth Service:** User authentication and authorization
  - **Attendance Service:** Attendance tracking and QR codes
  - **Payment Service:** Payment processing and invoicing
  - **Notification Service:** WhatsApp, email, push notifications
- API Gateway (Azure API Management)
- Event-driven communication (Azure Service Bus)

### Phase 3: Real-Time Features (Q1 2027)
- SignalR for real-time dashboard updates
- Live class capacity updates
- Real-time notifications

---

## Security Best Practices

- **HTTPS Enforced:** All communication encrypted (TLS 1.2+)
- **SQL Injection Prevention:** EF Core parameterized queries
- **XSS Protection:** React automatically escapes JSX
- **CSRF Protection:** Anti-forgery tokens on state-changing operations
- **Secrets Management:** Azure Key Vault (production), User Secrets (local)
- **Rate Limiting:** Throttle API requests to prevent abuse (planned)

---

## Contact & Support

**Author:** Jorge Padilla  
**Email:** seguridad.padilla@hotmail.com  
**Location:** Medellín, Colombia

Built with ❤️ from Medellín, Colombia  
*Jorge Padilla © 2024-2026*
