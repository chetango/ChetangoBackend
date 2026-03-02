# Deployment Strategy - Chetango API

**Author:** Jorge Padilla  
**Last Updated:** March 2026

## Architecture Overview

### **LOCAL Environment (Development)**
- **Database:** `ChetangoDB_Dev` (SQL Server LocalDB)
- **API:** `localhost:5194` (HTTPS QA profile)
- **Frontend:** `localhost:5173`
- **Authentication:** Azure AD B2C (users `@chetangoprueba.onmicrosoft.com`)

### **PRODUCTION Environment**
- **Database:** `chetango-db-prod` (Azure SQL Database)
- **API:** `chetango-api-prod.azurewebsites.net`
- **Frontend:** Azure Static Web App
- **Authentication:** Azure AD B2C (users with custom emails)

---

## Authentication

Both environments use the **same Azure AD B2C tenant:**
```
Tenant ID: 8a57ec5a-e2e3-44ad-9494-77fbc7467251
Instance: https://8a57ec5a-e2e3-44ad-9494-77fbc7467251.ciamlogin.com/
```

**Test Users (Local):**
- `Chetango@chetangoprueba.onmicrosoft.com`
- `admin@chetangoprueba.onmicrosoft.com`
- `profesor@chetangoprueba.onmicrosoft.com`

**Production Users:**
- Custom emails validated by Azure AD B2C
- Role-based access: Admin, Professor, Student

---

## Deployment Process

### 1. Local Development
```bash
# Backend
cd Chetango.Api
dotnet run --launch-profile https-qa

# Frontend
cd chetangoFrontend
npm run dev
```

### 2. Database Migrations
```bash
# Create migration
dotnet ef migrations add MigrationName --project Chetango.Infrastructure --startup-project Chetango.Api

# Apply to local database
dotnet ef database update --project Chetango.Infrastructure --startup-project Chetango.Api

# Generate SQL script for production
dotnet ef migrations script --project Chetango.Infrastructure --startup-project Chetango.Api --output migration.sql
```

### 3. Production Deployment

#### Backend (Azure App Service)
```bash
# Build and publish
dotnet publish -c Release -o ./publish

# Deploy to Azure (automated via GitHub Actions)
# See: .github/workflows/azure-app-service.yml
```

#### Frontend (Azure Static Web Apps)
```bash
# Build
npm run build

# Deploy (automated via GitHub Actions)
# See: .github/workflows/azure-static-web-apps.yml
```

---

## CI/CD Pipeline (GitHub Actions)

### Backend Workflow
- **Trigger:** Push to `main` branch
- **Steps:**
  1. Build .NET 9 project
  2. Run unit tests
  3. Publish artifacts
  4. Deploy to Azure App Service
- **Environment Variables:** Managed in Azure App Service Configuration

### Frontend Workflow
- **Trigger:** Push to `main` branch
- **Steps:**
  1. Install dependencies
  2. Build with Vite
  3. Run E2E tests (Playwright)
  4. Deploy to Azure Static Web Apps
- **Environment Variables:** `.env.production` (not committed to Git)

---

## Environment Configuration

### Backend (`appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ChetangoDB_Dev;Trusted_Connection=True;"
  },
  "AzureAd": {
    "Instance": "https://8a57ec5a-e2e3-44ad-9494-77fbc7467251.ciamlogin.com/",
    "TenantId": "8a57ec5a-e2e3-44ad-9494-77fbc7467251",
    "ClientId": "YOUR_CLIENT_ID"
  }
}
```

### Frontend (`.env`)
```env
VITE_API_BASE_URL=http://localhost:5194/api
VITE_AZURE_AD_CLIENT_ID=YOUR_CLIENT_ID
VITE_AZURE_AD_TENANT_ID=8a57ec5a-e2e3-44ad-9494-77fbc7467251
```

---

## Multi-Tenant Architecture

### Current Implementation (Multi-Sede)
- **Tenant Isolation:** Enum-based `Sede` field (Medellin, Manizales)
- **Database:** Shared database with tenant filtering via `TenantId`
- **User Context:** `HttpContext.User.Claims` retrieves `TenantId`

### Future Evolution (Full Multi-Tenant)
- **Strategy:** Database-per-tenant
- **Benefits:**
  - Complete data isolation
  - Independent scaling per tenant
  - Simplified compliance (GDPR, data residency)
- **Implementation:** Connection string resolution via tenant identifier

---

## Security Considerations

### Authentication & Authorization
- **OAuth 2.0 / OpenID Connect** via Azure AD B2C
- **JWT Tokens:** Validated on every request
- **Role-Based Access Control (RBAC):** Admin, Professor, Student
- **Claims-Based Authorization:** `[Authorize(Roles = "Admin")]`

### Data Protection
- **HTTPS Enforced:** All communication encrypted (TLS 1.2+)
- **SQL Injection Prevention:** EF Core parameterized queries
- **Sensitive Data:** Encrypted in database (e.g., payment receipts)
- **CORS Policy:** Restricted to frontend domain

### Secrets Management
- **Local Development:** User Secrets (`dotnet user-secrets`)
- **Production:** Azure Key Vault integration
- **Never Commit:** `.env` files, `appsettings.Development.json`

---

## Monitoring & Logging

### Application Insights (Azure)
- **Metrics:** Request rate, response time, failure rate
- **Logs:** Structured logging with Serilog
- **Alerts:** Configured for critical errors and performance degradation

### Health Checks
- **Endpoint:** `/health`
- **Checks:**
  - Database connectivity
  - Azure AD B2C reachability
  - External API dependencies (WhatsApp, payment gateways)

---

## Rollback Strategy

### Backend
1. Stop Azure App Service
2. Swap deployment slot (staging ↔ production)
3. Verify health checks
4. If issues persist, redeploy previous version from GitHub Release

### Database
1. **DO NOT USE `dotnet ef database update --migration 0`** in production
2. Generate rollback script manually
3. Test in staging environment first
4. Apply with transaction:
   ```sql
   BEGIN TRANSACTION
   -- Rollback SQL here
   COMMIT
   ```

### Frontend
1. Revert commit in GitHub
2. Re-trigger CI/CD pipeline
3. Verify deployment via Azure Static Web Apps dashboard

---

## Performance Optimization

### Backend
- **Async/Await:** All I/O operations asynchronous
- **EF Core Optimization:**
  - `.AsNoTracking()` for read-only queries
  - `.Include()` for eager loading (avoid N+1)
- **Caching:** Redis (planned for v2.0)

### Frontend
- **Code Splitting:** Lazy loading routes with React Router
- **Image Optimization:** WebP format, lazy loading
- **API Caching:** TanStack Query with stale-while-revalidate

### Database
- **Indexes:** Added on foreign keys and frequently queried columns
- **Query Optimization:** Execution plans reviewed with SQL Server Profiler
- **Connection Pooling:** Enabled in connection string

---

## Disaster Recovery

### Backup Strategy
- **Database:** Automated daily backups (Azure SQL Database)
- **Retention:** 7 days point-in-time restore
- **Manual Backups:** Before major migrations

### Recovery Procedures
1. Identify issue (monitoring alerts)
2. Assess impact (user-facing vs internal)
3. Execute rollback (see Rollback Strategy)
4. Post-mortem analysis (document root cause)

---

## Compliance & Auditing

### GDPR Compliance
- **Data Minimization:** Only collect necessary user data
- **Right to Deletion:** Admin endpoint to anonymize user data
- **Data Portability:** Export user data in JSON format
- **Consent Management:** Terms acceptance tracked in database

### Audit Logging
- **User Actions:** All CRUD operations logged with user ID and timestamp
- **Admin Actions:** Financial operations (payments, refunds) fully audited
- **Retention:** 2 years minimum for compliance

---

## Contact & Support

**Author:** Jorge Padilla  
**Email:** seguridad.padilla@hotmail.com  
**Location:** Medellín, Colombia

Built with ❤️ from Medellín, Colombia  
*Jorge Padilla © 2024-2026*
