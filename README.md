# Chetango API

RESTful API backend for Chetango dance studio management system, built with .NET 9 and Entity Framework Core.

## Tech Stack

- **.NET 9.0** - Framework
- **Entity Framework Core 9.0** - ORM
- **SQL Server** - Database (LocalDB for dev, Azure SQL for production)
- **Azure AD B2C** - Authentication
- **CQRS Pattern** - Architecture
- **MediatR** - Command/Query handling
- **Swagger/ReDoc** - API documentation

## Project Structure

```
ChetangoBackend/
├── Chetango.Api/              # API endpoints & startup configuration
├── Chetango.Application/      # Business logic (CQRS handlers)
├── Chetango.Domain/           # Entities & domain models
├── Chetango.Infrastructure/   # Data access & external services
├── docs/                      # Technical documentation
└── scripts/                   # Database & deployment scripts
```

## Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server 2019+ or LocalDB
- Azure AD B2C tenant (for authentication)

### Installation

1. Clone the repository
```bash
git clone https://github.com/chetango/ChetangoBackend.git
cd ChetangoBackend
```

2. Restore dependencies
```bash
dotnet restore
```

3. Configure connection strings in `appsettings.Development.json`
```json
{
  "ConnectionStrings": {
    "ChetangoConnection": "Server=localhost;Database=ChetangoDB_Dev;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  }
}
```

4. Apply migrations
```bash
dotnet ef database update --project Chetango.Api
```

5. Run the application
```bash
dotnet run --project Chetango.Api/Chetango.Api.csproj --launch-profile https-qa
```

API will be available at `https://localhost:7194`

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

- **Usuarios**: User management & profiles
- **Clases**: Class scheduling & management
- **Asistencias**: Attendance tracking with business rules
- **Paquetes**: Package management (class bundles)
- **Pagos**: Payment processing & verification
- **Nómina**: Payroll calculations for instructors
- **Reportes**: Dashboards & analytics
- **Eventos**: Event management
- **Referidos**: Referral program

### Architecture

- **CQRS Pattern**: Separate read/write operations
- **Transaction Management**: Atomic operations with rollback
- **Authorization**: Role-based access control (Admin, Profesor, Alumno)
- **Validation**: FluentValidation for business rules
- **Error Handling**: Centralized exception handling

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

### CI/CD Pipeline

Automated deployment using GitHub Actions:

- **`main` branch** → Production (chetango-api-prod)
- **`develop` branch** → Staging/integration

### Manual Deployment

See [DEPLOYMENT-STRATEGY.md](docs/DEPLOYMENT-STRATEGY.md) for detailed deployment procedures.

## Testing

### Run Tests
```bash
dotnet test
```

### Integration Tests

Playwright tests available in frontend repository for end-to-end testing.

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

## License

Proprietary - Chetango Dance Studio

## Contact

For questions or support, contact the development team.
