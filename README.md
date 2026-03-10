# Aphellion (Chetango) - Backend API

> Enterprise SaaS platform for dance academy management | 500+ active users in production

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Azure](https://img.shields.io/badge/Azure-Deployed-0078D4?logo=microsoft-azure)](https://azure.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

RESTful API built with Clean Architecture + CQRS, powering a complete management system for dance academies with multi-tenant support, attendance tracking, payment processing, and automated payroll.

**Live Demo:** [https://app.corporacionchetango.com](https://app.corporacionchetango.com)

---

## 🏗️ Architecture Overview

### Clean Architecture + CQRS Pattern

```
┌─────────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                            │
│                   Chetango.Api (ASP.NET Core)                   │
│                                                                   │
│  Controllers → MediatR → Commands/Queries                       │
│  Middleware: Auth, Tenant Resolution, Exception Handling        │
└──────────────────────────┬──────────────────────────────────────┘
                           │ Depends on ↓
┌──────────────────────────▼──────────────────────────────────────┐
│                    APPLICATION LAYER                             │
│                  Chetango.Application                            │
│                                                                   │
│  ┌─────────────────┐              ┌─────────────────┐          │
│  │   COMMANDS      │              │    QUERIES      │          │
│  │  (Write Model)  │              │  (Read Model)   │          │
│  │                 │              │                 │          │
│  │ • Create        │              │ • Get           │          │
│  │ • Update        │              │ • List          │          │
│  │ • Delete        │              │ • Search        │          │
│  │                 │              │                 │          │
│  │ SaveChanges()   │              │ AsNoTracking()  │          │
│  └─────────────────┘              └─────────────────┘          │
│                                                                   │
│  DTOs, Interfaces (IAppDbContext), Validators                   │
└──────────────────────────┬──────────────────────────────────────┘
                           │ Depends on ↓
┌──────────────────────────▼──────────────────────────────────────┐
│                      DOMAIN LAYER                                │
│                    Chetango.Domain                               │
│                                                                   │
│  Entities: Alumno, Profesor, Clase, Asistencia, Pago, etc.     │
│  Enums: EstadoAsistencia, TipoPago, RolUsuario                  │
│  Business Rules (pure, no dependencies)                         │
└─────────────────────────────────────────────────────────────────┘
                           ↑ Implements
┌──────────────────────────┴──────────────────────────────────────┐
│                   INFRASTRUCTURE LAYER                           │
│                 Chetango.Infrastructure                          │
│                                                                   │
│  • AppDbContext (EF Core)                                       │
│  • Configurations (Fluent API)                                  │
│  • Interceptors (Multi-Tenancy)                                 │
│  • Services (Email, Storage, etc.)                              │
│  • Persistence (Repository implementations)                     │
└─────────────────────────────────────────────────────────────────┘
```

### Production Architecture (Azure Cloud)

```
┌─────────────────────────────────────────────────────────────────┐
│                         INTERNET                                 │
│                  DNS: *.aphellion.com                           │
│                                                                   │
│   academia1.aphellion.com  →  Client 1                          │
│   academia2.aphellion.com  →  Client 2                          │
│   api.aphellion.com        →  Backend API                       │
└────────────────────────┬────────────────────────────────────────┘
                         │
         ┌───────────────┴───────────────┐
         │                               │
         ▼                               ▼
┌─────────────────────┐         ┌─────────────────────┐
│   Frontend (SPA)    │         │    Backend API      │
│ Azure Static Web App│  HTTPS  │  Azure App Service  │
│                     │ ◄─────► │     (Linux B1)      │
│ • React 18 + TS     │  CORS   │ • .NET 9            │
│ • Vite Build        │         │ • Clean Arch        │
│ • CDN Global        │         │ • CQRS + MediatR    │
└─────────────────────┘         └──────────┬──────────┘
                                           │
                                           ▼
                              ┌────────────────────────┐
                              │   Azure SQL Database   │
                              │    (Standard S0)       │
                              │                        │
                              │ Row-Level Security     │
                              │ Multi-Tenant (RLS)     │
                              │ Auto-Backups (7d)      │
                              └────────────────────────┘
```

**Current Stats:**
-  Capacity: 1 academy, 500+ users
- 📈 Scalable to: 10-15 academies without upgrades
- 🌍 Region: East US (60-80ms latency to Colombia)

## Key Features

- **Multi-Tenant SaaS** - Subdomain-based isolation with Row-Level Security (RLS)
- **Role-Based Access Control** - Admin, Professor, Student roles with OAuth 2.0 + JWT
- **Real-Time Dashboards** - Business KPIs, attendance metrics, revenue analytics
- **Payment Processing** - Receipt verification, package management, automated invoicing
- **Class Scheduling** - Recurring classes, capacity management, QR attendance
- **Automated Payroll** - Calculate instructor compensation based on classes taught
- **Comprehensive Reporting** - Export-ready reports for accounting and management
- **Smart Notifications** - WhatsApp integration for reminders and alerts
- **Enterprise Security** - Azure Entra ID, HTTPS, CORS, data encryption

## Tech Stack

### Backend Core
- **.NET 9.0** - Latest LTS framework
- **Entity Framework Core 9.0** - ORM with advanced features
- **SQL Server** - RDBMS (LocalDB for dev, Azure SQL for production)
- **MediatR 12.0** - Mediator pattern implementation for CQRS

### Architecture & Patterns
- **Clean Architecture** - 4-layer separation (Domain, Application, Infrastructure, API)
- **CQRS Pattern** - Command Query Responsibility Segregation
- **Repository Pattern** - Data access abstraction
- **Unit of Work** - Transaction management via EF Core
- **Dependency Injection** - Built-in ASP.NET Core DI container

### Authentication & Security
- **Azure AD B2C (Entra External ID)** - OAuth 2.0 + OpenID Connect
- **JWT Bearer Tokens** - Stateless authentication
- **Role-Based Authorization** - Claims-based with policies
- **Row-Level Security (RLS)** - Database-level tenant isolation

### DevOps & Monitoring
- **GitHub Actions** - CI/CD pipeline
- **Azure App Service** - PaaS hosting
- **Azure SQL Database** - Managed database
- **Swagger/OpenAPI** - API documentation
- **Serilog** - Structured logging

## 🎯 Multi-Tenant Architecture

### How It Works

This is a **SaaS multi-tenant** application where each academy gets its own subdomain:
- `academia1.aphellion.com` → Academia 1 (isolated data)
- `academia2.aphellion.com` → Academia 2 (isolated data)
- `api.aphellion.com` → Shared API backend

**Data Isolation Strategy: Row-Level Security (RLS)**

All tenants share the same database, but data is automatically filtered by `TenantId`:

```
┌─────────────────────────────────────────────────────────────┐
│  Request: academia1.aphellion.com/api/alumnos               │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌────────────────────────────────────────────────────────────┐
│  1. TenantResolutionMiddleware                             │
│     - Extracts subdomain: "academia1"                      │
│     - Queries: SELECT Id FROM Tenants WHERE Subdomain=...  │
│     - Stores: TenantId in scoped provider                  │
└────────────────────────┬───────────────────────────────────┘
                         │
                         ▼
┌────────────────────────────────────────────────────────────┐
│  2. TenantDbConnectionInterceptor                          │
│     - Intercepts SQL connection open                       │
│     - Executes: sp_set_session_context 'TenantId', '...'  │
└────────────────────────┬───────────────────────────────────┘
                         │
                         ▼
┌────────────────────────────────────────────────────────────┐
│  3. Row-Level Security (SQL Server)                        │
│     - Automatically filters ALL queries:                   │
│       WHERE TenantId = SESSION_CONTEXT('TenantId')        │
│     - Works transparently (no code changes needed)         │
└────────────────────────────────────────────────────────────┘
```

**Advantages:**
- ✅ Cost-effective (shared infrastructure)
- ✅ Easy maintenance (single database, single schema)
- ✅ Automatic isolation (RLS prevents data leaks)
- ✅ Scales to 100+ tenants on same hardware

**Security:**
- 🔒 RLS policies enforce tenant filtering at database level
- 🔒 SESSION_CONTEXT is read-only per connection
- 🔒 Even SQL injection cannot bypass RLS
- 🔒 Application code is tenant-agnostic

**Implementation Files:**
- [TenantResolutionMiddleware.cs](Chetango.Api/Infrastructure/Middleware/TenantResolutionMiddleware.cs) - Subdomain extraction
- [TenantDbConnectionInterceptor.cs](Chetango.Infrastructure/Persistence/Interceptors/TenantDbConnectionInterceptor.cs) - Session context
- [apply-row-level-security.sql](scripts/apply-row-level-security.sql) - RLS policies

## 🎨 CQRS Implementation

### Command Example (Write Operation)

**File:** [ConfirmarAsistenciaCommand.cs](Chetango.Application/Asistencias/Commands/ConfirmarAsistencia/ConfirmarAsistenciaCommand.cs)

```csharp
// Command: Represents intent to modify data
public record ConfirmarAsistenciaCommand(Guid IdAsistencia) 
    : IRequest<Result<bool>>;

// Handler: Contains business logic
public class ConfirmarAsistenciaCommandHandler 
    : IRequestHandler<ConfirmarAsistenciaCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(...)
    {
        // 1. Load entity with tracking
        var asistencia = await _db.Set<Asistencia>()
            .Include(a => a.Estado)
            .FirstOrDefaultAsync(...);
        
        // 2. Apply business rules
        if (asistencia.Estado.Nombre != "Presente")
            return Result<bool>.Failure("...");
        
        // 3. Modify entity
        asistencia.Confirmado = true;
        asistencia.FechaConfirmacion = DateTime.UtcNow;
        
        // 4. Persist changes
        await _db.SaveChangesAsync();
        
        return Result<bool>.Success(true);
    }
}
```

### Query Example (Read Operation)

**File:** [GetAsistenciasPorClaseQuery.cs](Chetango.Application/Asistencias/Queries/GetAsistenciasPorClase/GetAsistenciasPorClaseQuery.cs)

```csharp
// Query: Represents intent to read data
public record GetAsistenciasPorClaseQuery(Guid IdClase) 
    : IRequest<Result<IReadOnlyList<AsistenciaDto>>>;

// Handler: Optimized for reading
public class GetAsistenciasPorClaseQueryHandler 
    : IRequestHandler<GetAsistenciasPorClaseQuery, ...>
{
    public async Task<Result<...>> Handle(...)
    {
        var asistencias = await _db.Asistencias
            .AsNoTracking()  // ← No change tracking (performance)
            .Where(a => a.IdClase == request.IdClase)
            .Include(a => a.Alumno)
            .Include(a => a.Estado)
            .Select(a => new AsistenciaDto(...))  // ← Project to DTO
            .ToListAsync();
        
        return Result<...>.Success(asistencias);
    }
}
```

**Key Differences:**
- **Commands**: Use tracking, modify entities, call `SaveChanges()`
- **Queries**: Use `AsNoTracking()`, project to DTOs, never modify data

### Request Flow

```
HTTP Request
    ↓
Controller.Post/Get(...)
    ↓
await _mediator.Send(new Command/Query(...))
    ↓
MediatR routes to correct Handler
    ↓
Handler executes business logic
    ↓
Returns Result<T> (success or failure)
    ↓
Controller returns HTTP response (200, 400, 404, etc.)
```

## Tech Stack

## Project Structure

```
ChetangoBackend/
├── Chetango.Api/                    # 🌐 Presentation Layer (ASP.NET Core)
│   ├── Controllers/                 # REST endpoints
│   ├── Infrastructure/
│   │   └── Middleware/              # Tenant resolution, exception handling
│   ├── Program.cs                   # Startup & DI configuration
│   └── appsettings.json             # Configuration
│
├── Chetango.Application/            # 💼 Application Layer (Use Cases)
│   ├── Alumnos/
│   │   ├── Commands/                # Write operations (Create, Update, Delete)
│   │   ├── Queries/                 # Read operations (Get, List, Search)
│   │   └── DTOs/                    # Data Transfer Objects
│   ├── Asistencias/
│   ├── Clases/
│   ├── Pagos/
│   ├── Nomina/
│   ├── Reportes/
│   └── Common/
│       ├── Interfaces/              # IAppDbContext, ITenantProvider
│       └── Result.cs                # Railway-Oriented Programming
│
├── Chetango.Domain/                 # 🏛️ Domain Layer (Business Rules)
│   ├── Entities/                    # Domain entities (Alumno, Clase, etc.)
│   │   ├── Alumno.cs
│   │   ├── Profesor.cs
│   │   ├── Clase.cs
│   │   ├── Asistencia.cs
│   │   ├── Pago.cs
│   │   └── ...
│   └── Enums/                       # Domain enumerations
│
├── Chetango.Infrastructure/         # 🔧 Infrastructure Layer (Technical Details)
│   ├── Persistence/
│   │   ├── AppDbContext.cs          # EF Core DbContext
│   │   ├── Configurations/          # Fluent API entity configurations
│   │   └── Interceptors/            # TenantDbConnectionInterceptor
│   └── Services/                    # External services (Email, Storage)
│
├── docs/                            # 📚 Documentation
│   ├── ARCHITECTURE.en.md           # Architecture overview
│   ├── DEPLOYMENT-STRATEGY.md       # CI/CD guide
│   ├── ARQUITECTURA-MULTI-TENANCY-RESUMEN.md
│   └── PLAN-ESCALAMIENTO-SAAS.md    # Scaling strategy
│
└── scripts/                         # 📜 Database & Deployment
    ├── apply-row-level-security.sql # RLS implementation
    ├── migration-compliance-PRODUCCION.sql
    └── seed_*.sql                   # Test data
```

**Layer Dependencies:**
```
Api → Application → Domain ← Infrastructure
                     ↑
                 (Pure, no deps)
```

## Getting Started

### Prerequisites

- **.NET 9 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **SQL Server 2019+** or **LocalDB** (included with Visual Studio)
- **Visual Studio 2022** or **VS Code** with C# extension
- **Azure AD B2C Tenant** (for authentication) - Optional for local dev
- **Git** - Version control

### Quick Start (5 minutes)

1. **Clone the repository**
```bash
git clone https://github.com/yourorg/aphellion-backend.git
cd aphellion-backend
```

2. **Restore dependencies**
```bash
dotnet restore
```

3. **Update database connection string**

Edit `Chetango.Api/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "ChetangoConnection": "Server=(localdb)\\mssqllocaldb;Database=ChetangoDB_Dev;Trusted_Connection=True;MultipleActiveResultSets=true;"
  }
}
```

4. **Apply database migrations**
```bash
cd Chetango.Api
dotnet ef database update
```

5. **Run the application**
```bash
dotnet run --launch-profile https-qa
```

6. **Access the API**
- Swagger UI: https://localhost:7194/swagger
- ReDoc: https://localhost:7194/redoc
- API Base: https://localhost:7194/api

### Development Workflow

```bash
# Create feature branch
git checkout -b feature/my-new-feature

# Make changes, test locally
dotnet run --project Chetango.Api

# Create migration (if needed)
dotnet ef migrations add MyMigration --project Chetango.Infrastructure --startup-project Chetango.Api

# Commit and push
git add .
git commit -m "feat: add new feature"
git push origin feature/my-new-feature

# Create PR to develop branch
```

### Installation

### Troubleshooting

**Database Connection Issues:**
```bash
# Verify SQL Server is running
sqlcmd -S localhost -E -Q "SELECT @@VERSION"

# Check LocalDB instances
sqllocaldb info

# Start LocalDB if needed
sqllocaldb start mssqllocaldb
```

**Migration Errors:**
```bash
# Reset database (CAUTION: deletes all data)
dotnet ef database drop --project Chetango.Api
dotnet ef database update --project Chetango.Api
```

**Port Already in Use:**
```bash
# Change port in launchSettings.json
# Or kill process using port
netstat -ano | findstr :7194
taskkill /PID <process_id> /F
```

## 🔐 Security & Authentication

### Azure AD B2C Configuration

**Tenant Information:**
```
Tenant ID: 8a57ec5a-e2e3-44ad-9494-77fbc7467251
Instance: https://8a57ec5a-e2e3-44ad-9494-77fbc7467251.ciamlogin.com/
Client ID: d35c1d4d-9ddc-4a8b-bb89-1964b37ff573
```

### Authentication Flow

```
┌──────────────┐         ┌──────────────┐         ┌──────────────┐
│   Frontend   │         │   Azure AD   │         │   Backend    │
│              │         │     B2C      │         │     API      │
└──────┬───────┘         └──────┬───────┘         └──────┬───────┘
       │                        │                        │
       │  1. Redirect to login  │                        │
       │───────────────────────>│                        │
       │                        │                        │
       │  2. User authenticates │                        │
       │<───────────────────────│                        │
       │   (returns JWT token)  │                        │
       │                        │                        │
       │  3. Request with token │                        │
       │────────────────────────┼───────────────────────>│
       │                        │                        │
       │                        │  4. Validate token     │
       │                        │<───────────────────────│
       │                        │                        │
       │                        │  5. Return user claims │
       │                        │───────────────────────>│
       │                        │                        │
       │  6. API response       │                        │
       │<───────────────────────┼────────────────────────│
```

### Authorization Roles

| Role | Permissions | Typical Users |
|------|------------|---------------|
| **Admin** | Full system access, tenant management, financial reports | Academy owners, managers |
| **Profesor** | Class management, attendance, view assigned students | Dance instructors |
| **Alumno** | View own data, confirm attendance, purchase packages | Students |

**Example Authorization:**
```csharp
[Authorize(Roles = "Admin")]
public async Task<IActionResult> GetFinancialReports(...)

[Authorize(Roles = "Admin,Profesor")]
public async Task<IActionResult> RegisterAttendance(...)

[Authorize] // All authenticated users
public async Task<IActionResult> GetProfile(...)
```

### Security Features

✅ **Data Protection:**
- Row-Level Security (RLS) for tenant isolation
- SQL injection prevention (parameterized queries)
- HTTPS enforced (TLS 1.2+)
- Secure cookie settings (HttpOnly, Secure, SameSite)

✅ **Authentication & Authorization:**
- OAuth 2.0 + OpenID Connect
- JWT token validation
- Claims-based authorization
- Automatic token refresh

✅ **API Security:**
- CORS restricted to known origins
- Rate limiting (planned)
- Input validation on all endpoints
- Exception details hidden in production

✅ **Database Security:**
- Encrypted connections (TLS)
- Firewall rules (Azure SQL)
- Minimal permissions principle
- Audit logging enabled

## Configuration

### Environments

- **Development**: Local development with LocalDB
- **QA**: Testing environment with local SQL Server
- **Production**: Azure App Service + Azure SQL Database

### Launch Profiles

- `https-qa`: HTTPS with QA configuration (port 7194)
- `http-qa`: HTTP with QA configuration (port 5194)

## API Documentation

Once running, access interactive API documentation at:

- **Swagger UI**: `https://localhost:7194/swagger`
- **ReDoc**: `https://localhost:7194/redoc`

## Features

### Core Modules

#### 👥 **Usuarios (User Management)**
- User registration & profile management
- Role assignment (Admin, Profesor, Alumno)
- Multi-sede support (Medellín, Manizales, etc.)
- Profile pictures & contact information
- Password reset via Azure AD

**Endpoints:** `/api/usuarios/*`

#### 📅 **Clases (Class Management)**
- Class scheduling with recurring patterns
- Capacity management (max students per class)
- Professor assignment
- Class types (Salsa, Bachata, Merengue, etc.)
- Real-time availability check

**Endpoints:** `/api/clases/*`

#### ✅ **Asistencias (Attendance Tracking)**
- QR code-based check-in
- Manual attendance registration
- Confirmation system (student confirms attendance)
- Late arrival tracking
- Business rules:
  - Cannot mark attendance for future classes
  - Students need active package
  - Auto-consume class from package

**Endpoints:** `/api/asistencias/*`

#### 📦 **Paquetes (Package Management)**
- Package catalog by sede
- Flexible pricing (per package type)
- Validity period management
- Class consumption tracking
- Package transfer between students
- Types: Grupal (group) / Privada (private)

**Endpoints:** `/api/paquetes/*`

#### 💳 **Pagos (Payment Processing)**
- Receipt upload & verification
- Admin approval workflow
- Payment methods (transfer, cash, card)
- Automatic package activation
- Payment history & reports
- Refund handling

**Endpoints:** `/api/pagos/*`

#### 💰 **Nómina (Payroll)**
- Automated instructor compensation
- Calculation based on:
  - Class attendance count
  - Instructor rate (per class)
  - Class type (private vs group)
- Monthly liquidation reports
- Historical payroll data

**Endpoints:** `/api/nomina/*`

#### 📊 **Reportes (Reports & Analytics)**
- Dashboard KPIs:
  - Monthly revenue
  - Active students
  - Class attendance rate
  - Package sales
- Financial reports
- Attendance reports
- Student activity reports
- Export to Excel/PDF

**Endpoints:** `/api/reportes/*`

#### 🎉 **Eventos (Events)**
- Event creation & management
- Image upload (posters)
- Target audience (All, Students, Public)
- Event carousel on homepage
- RSVP tracking

**Endpoints:** `/api/eventos/*`

#### 🔗 **Referidos (Referral Program)**
- Track student referrals
- Referral rewards
- Analytics on referral effectiveness

**Endpoints:** `/api/referidos/*`

### Architecture

#### Clean Architecture Principles

1. **Dependency Rule**: Dependencies point inward (Domain has no dependencies)
2. **Independence**: Business logic independent of frameworks, UI, database
3. **Testability**: Business rules can be tested without external dependencies
4. **Flexibility**: Easy to change databases, frameworks, or UI

#### Design Patterns

- **CQRS (Command Query Responsibility Segregation)**
  - Separate models for reading and writing data
  - Commands modify state, Queries read state
  - Enables independent scaling and optimization

- **Mediator Pattern** (via MediatR)
  - Decouples request senders from handlers
  - Simplifies adding cross-cutting concerns (logging, validation)
  - All requests flow through a single pipeline

- **Repository Pattern**
  - Abstraction over data access (IAppDbContext)
  - Enables testing without database
  - Centralized query logic

- **Unit of Work** (via EF Core)
  - Groups operations into transactions
  - SaveChanges() commits all or nothing
  - Maintains consistency

- **Railway-Oriented Programming**
  - Result<T> for explicit error handling
  - No exceptions for business rule violations
  - Chainable success/failure flows

#### Request Pipeline

```
┌─────────────────────────────────────────────────────────────┐
│  1. HTTP Request arrives at Controller                      │
│     POST /api/asistencias/confirmar                         │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│  2. Authentication Middleware                                │
│     - Validates JWT token                                   │
│     - Extracts user claims (Id, Role, Email)                │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│  3. TenantResolutionMiddleware                              │
│     - Extracts subdomain from Host header                   │
│     - Resolves TenantId from database                       │
│     - Stores in scoped ITenantProvider                      │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│  4. Controller creates Command/Query                         │
│     var command = new ConfirmarAsistenciaCommand(id);       │
│     var result = await _mediator.Send(command);             │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│  5. MediatR routes to Handler                               │
│     - Finds ConfirmarAsistenciaCommandHandler               │
│     - Executes Handle() method                              │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│  6. Handler opens database connection                       │
│     - TenantDbConnectionInterceptor sets SESSION_CONTEXT    │
│     - RLS automatically filters queries by TenantId         │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│  7. Business logic executes                                 │
│     - Load entity                                           │
│     - Validate business rules                               │
│     - Modify entity                                         │
│     - SaveChanges() (Unit of Work)                          │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│  8. Result<T> returned to Controller                        │
│     - Success: 200 OK with data                             │
│     - Failure: 400 Bad Request with error message           │
└─────────────────────────────────────────────────────────────┘
```

## Database

### Migrations

Create new migration:
```bash
dotnet ef migrations add MigrationName --project Chetango.Infrastructure
```

Apply migrations:
```bash
dotnet ef database update --project Chetango.Api
```

Remove last migration:
```bash
dotnet ef migrations remove --project Chetango.Infrastructure
```

### Connection Strings

Development (LocalDB):
```
Server=localhost;Database=ChetangoDB_Dev;Trusted_Connection=True;
```

Production (Azure SQL):
```
Server=tcp:chetango-sql-prod.database.windows.net;Database=chetango-db-prod;
```

## Deployment

### Production Environment (Azure Cloud)

**Infrastructure:**
- **Backend API**: Azure App Service (Basic B1, Linux)
  - .NET 9 runtime
  - Always On enabled
  - HTTPS only
  - Auto-restart on failure

- **Database**: Azure SQL Database (Standard S0)
  - 10 DTUs (sufficient for 1,500-3,000 users)
  - 7-day point-in-time recovery
  - Geo-redundant backups
  - Row-Level Security enabled

- **Frontend**: Azure Static Web App
  - Global CDN
  - Free SSL certificate
  - Automatic preview environments

**Current Configuration:**
- 🌍 Region: East US
-  Current Load: 1 academy, 500+ users
- 📈 Capacity: Can scale to 10-15 academies without upgrades

### CI/CD Pipeline (GitHub Actions)

**Backend Deployment:**
```yaml
Trigger: Push to main branch
├── 1. Checkout code
├── 2. Setup .NET 9 SDK
├── 3. Restore dependencies (dotnet restore)
├── 4. Build project (dotnet build -c Release)
├── 5. Run tests (if available)
├── 6. Publish artifacts (dotnet publish)
└── 7. Deploy to Azure App Service
    └── Deployment time: ~3-5 minutes
```

**Deployment Strategy:**
- **Branch Strategy**: GitFlow
  - `main` → Production (auto-deploy)
  - `develop` → Integration (manual deploy)
  - `feature/*` → Development (local only)

- **Rollback**: Azure keeps previous 20 deployments
  - Instant rollback via Azure Portal
  - Zero-downtime swap slots (if needed)

- **Database Migrations**: 
  - Auto-applied on application startup
  - Transactional (all-or-nothing)
  - Backward-compatible

### Connection Strings & Secrets

**Managed via Azure App Service Configuration:**
```json
{
  "ConnectionStrings__ChetangoConnection": "***",
  "AzureAd__ClientId": "***",
  "AzureAd__ClientSecret": "***",
  "Cors__AllowedOrigins__0": "https://academia1.aphellion.com",
  "Cors__AllowedOrigins__1": "https://academia2.aphellion.com"
}
```

**Local Development:**
```json
// appsettings.Development.json (not committed to git)
{
  "ConnectionStrings": {
    "ChetangoConnection": "Server=localhost;Database=ChetangoDB_Dev;..."
  }
}
```

### Manual Deployment

See [DEPLOYMENT-STRATEGY.md](docs/DEPLOYMENT-STRATEGY.md) for detailed deployment procedures.

## Testing

### Unit Tests (Planned)

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

**Test Strategy:**
- Unit tests for business logic (Handlers)
- Integration tests for database operations
- E2E tests via Playwright (frontend repository)

**Example Test Structure:**
```
Chetango.Tests/
├── Application/
│   ├── Asistencias/
│   │   └── Commands/
│   │       └── ConfirmarAsistenciaCommandHandlerTests.cs
│   └── Pagos/
└── Infrastructure/
    └── Persistence/
        └── AppDbContextTests.cs
```

### Integration Tests

End-to-end tests are managed in the frontend repository using Playwright.

**Test Coverage (Frontend E2E):**
- ✅ Authentication flows
- ✅ Attendance registration
- ✅ Payment verification
- ✅ Class scheduling
- ✅ Report generation

**Run E2E Tests:**
```bash
cd ../chetango-frontend
npm run test:e2e
```

### Manual Testing

**Swagger UI** provides interactive API testing:
1. Navigate to https://localhost:7194/swagger
2. Click "Authorize" and paste JWT token
3. Test endpoints directly from browser

**Get JWT Token:**
1. Login via frontend (http://localhost:5173)
2. Open browser DevTools → Network tab
3. Find request with Authorization header
4. Copy token (excluding "Bearer " prefix)

## Authentication

Uses Azure AD B2C (External ID) for authentication:

- **Tenant**: `8a57ec5a-e2e3-44ad-9494-77fbc7467251.ciamlogin.com`
- **Supported Roles**: Admin, Profesor, Alumno
- **Token Format**: JWT Bearer

Include token in requests:
```
Authorization: Bearer <token>
```

## CORS Configuration

Configured origins by environment in `appsettings.json`:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "http://localhost:3000"
    ]
  }
}
```

## Common Issues

### Migration Errors

If migrations fail, ensure connection string is correct and database server is running.

### Authentication Errors

Verify Azure AD configuration matches `appsettings.json` and tokens are not expired.

### CORS Errors

Add frontend URL to `AllowedOrigins` in appropriate `appsettings.{Environment}.json`.

## Contributing

1. Create feature branch from `develop`
2. Make changes with clear commit messages
3. Create Pull Request to `develop`
4. After approval, merge to `main` for production deployment

## Project Stats

- **500+ Active Users** across 2 locations (Medellín, Manizales)
- **10,000+ Classes** managed since launch
- **99.8% Uptime** on Azure infrastructure
- **80% Reduction** in administrative time vs manual processes
- **50+ Deployments** with zero rollbacks

## Roadmap

- [ ] AWS Lambda integration for receipt OCR with AI
- [ ] Redis distributed caching
- [ ] Elasticsearch for advanced search
- [ ] Stripe/Wompi payment gateway integration
- [ ] Mobile apps (iOS/Android)
- [ ] Microservices architecture (Event-Driven)

## Documentation

- [Architecture Overview](docs/ARCHITECTURE.en.md) - Clean Architecture + CQRS pattern
- [Deployment Strategy](docs/DEPLOYMENT-STRATEGY.en.md) - CI/CD, environments, rollback
- [Manual de Administrador](docs/MANUAL-ADMINISTRADOR.md) - Admin guide (Spanish)
- [Manual de Profesor](docs/MANUAL-PROFESOR.md) - Professor guide (Spanish)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

**Jorge Padilla** - Backend Software Engineer  
- LinkedIn: [Jorge Padilla](https://linkedin.com/in/yourprofile)
- Email: seguridad.padilla@hotmail.com
- Location: Medellín, Colombia

Developed with 10+ years of experience in enterprise software and passion for clean architecture.

## 🤝 Contributing

Contributions, issues, and feature requests are welcome! Feel free to check the [issues page](../../issues).

## ⭐ Show your support

Give a ⭐️ if this project helped you or you find it interesting!

---

**Built with ❤️ from Medellín, Colombia**  
*Jorge Padilla © 2024-2026*
