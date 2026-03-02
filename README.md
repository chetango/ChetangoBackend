# Aphellion (Chetango) - Backend API

> Enterprise SaaS platform for dance academy management | 500+ active users in production

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Azure](https://img.shields.io/badge/Azure-Deployed-0078D4?logo=microsoft-azure)](https://azure.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

RESTful API built with Clean Architecture + CQRS, powering a complete management system for dance academies with multi-tenant support, attendance tracking, payment processing, and automated payroll.

**Live Demo:** [https://app.corporacionchetango.com](https://app.corporacionchetango.com)

---

## Key Features

- **Multi-Tenant Architecture** - Isolated data per academy with database-per-tenant strategy
- **Role-Based Access Control** - Admin, Professor, Student roles with OAuth 2.0 + JWT
- **Real-Time Dashboards** - Business KPIs, attendance metrics, revenue analytics
- **Payment Processing** - Receipt verification, package management, automated invoicing
- **Class Scheduling** - Recurring classes, capacity management, QR attendance
- **Automated Payroll** - Calculate instructor compensation based on classes taught
- **Comprehensive Reporting** - Export-ready reports for accounting and management
- **Smart Notifications** - WhatsApp integration for reminders and alerts
- **Enterprise Security** - Azure Entra ID, HTTPS, CORS, data encryption

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
git clone https://github.com/yourusername/aphellion-backend.git
cd aphellion-backend
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
