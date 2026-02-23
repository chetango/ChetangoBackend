# 📊 PLAN DE ESCALAMIENTO CHETANGO SAAS

**Versión:** 1.1  
**Fecha:** 21 de Febrero de 2026  
**Autor:** Equipo Técnico Chetango  
**Propósito:** Guía de infraestructura, costos y escalamiento para convertir Chetango en plataforma SaaS multi-tenant

---

## 📋 TABLA DE CONTENIDOS

1. [Situación Actual](#1-situación-actual)
2. [Visión SaaS y Objetivos](#2-visión-saas-y-objetivos)
3. [Arquitectura Multi-Tenant](#3-arquitectura-multi-tenant)
   - 3.1 Modelo Recomendado
   - 3.2 Componentes Clave
   - 3.3 Flujo de Autenticación
   - **3.4 Personalización Dinámica (Branding)** ⭐ NUEVO
4. [Plan de Escalamiento por Etapas](#4-plan-de-escalamiento-por-etapas)
5. [Especificaciones Técnicas por Fase](#5-especificaciones-técnicas-por-fase)
6. [Análisis de Costos y Rentabilidad](#6-análisis-de-costos-y-rentabilidad)
7. [Checklist de Implementación](#7-checklist-de-implementación)
8. [Métricas de Monitoreo](#8-métricas-de-monitoreo)
9. [Plan de Contingencia](#9-plan-de-contingencia)
10. [Anexos Técnicos](#10-anexos-técnicos)
11. **[Guía Rápida: Onboarding de Nuevo Cliente](#guía-rápida-onboarding-de-nuevo-cliente)** ⭐ NUEVO

---

## 1. SITUACIÓN ACTUAL

### 1.1 Estado de la Plataforma

**Implementación:**
- ✅ Sistema operativo en producción
- ✅ Multi-sede implementado (Medellín y Manizales)
- ✅ Autenticación con Microsoft Entra ID (Azure AD)
- ✅ Frontend en React + TypeScript
- ✅ Backend en .NET 9.0 con Clean Architecture
- ✅ Base de datos en Azure SQL Database

**Uso Actual:**
- **Cliente único:** Corporación Chetango (uso interno)
- **Usuarios totales:** ~300 (alumnos + profesores + administrativos)
- **Sedes:** 2 (Medellín y Manizales)
- **Clases activas:** ~40-50 clases/semana
- **Transacciones mensuales:** ~600-800 pagos/mes

### 1.2 Infraestructura Azure Actual

| Recurso | Tier Estimado | Capacidad | Costo Mensual |
|---------|---------------|-----------|---------------|
| **Azure SQL Database** | Basic o S0 | 2 GB - 250 GB | $21,000 - $63,000 COP |
| **Azure App Service (API)** | B1 Basic | 1 core, 1.75 GB RAM | $70,000 COP |
| **Azure Static Web App** | Free | 100 GB bandwidth | $0 COP |
| **Azure Storage Account** | Standard LRS | ~500 MB | $8,400 COP |
| **TOTAL** | - | - | **~$100,000 - $142,000 COP/mes** |

**Capacidad actual:**
- ✅ Suficiente para Corporación Chetango (1 cliente, 300 usuarios)
- ✅ Podría soportar 3-5 academias adicionales sin cambios
- ⚠️ Requiere ajustes para 10+ academias

### 1.3 Limitaciones Identificadas

**Técnicas:**
- ❌ No existe sistema de multi-tenancy (TenantId)
- ❌ No hay proceso de registro self-service
- ❌ No hay integración de pagos automáticos (Stripe/Wompi)
- ❌ No hay gestión de suscripciones
- ❌ Subdominios no configurados (*.chetango.com)
- ❌ Auto-scaling no disponible (tier Basic)

**Operacionales:**
- ❌ Onboarding manual (no escalable)
- ❌ Sin métricas de uso por cliente
- ❌ Sin monitoreo proactivo (Application Insights)
- ❌ Backups limitados (7 días)

---

## 2. VISIÓN SAAS Y OBJETIVOS

### 2.1 Modelo de Negocio

**Producto:** Plataforma de gestión administrativa para academias de danza en Colombia y LATAM

**Cliente Target:** 
- Academias de danza con 50-300 alumnos activos
- 1-3 sedes por academia
- Ubicadas en ciudades principales de Colombia
- Dispuestas a pagar entre $150,000 - $750,000 COP/mes

**Propuesta de Valor:**
- Control de asistencias con código QR
- Gestión automatizada de pagos y facturación
- Reportes financieros en tiempo real
- Gestión de profesores y nómina
- Dashboard con KPIs de negocio
- Multi-sede desde un solo lugar

### 2.2 Objetivos de Crecimiento

**Meta a 12 meses:**
- 🎯 50-60 academias suscritas
- 🎯 ~10,000 usuarios finales (alumnos + profesores)
- 🎯 MRR: $15-20M COP/mes
- 🎯 Margen operativo: >85%
- 🎯 Churn rate: <5%/mes
- 🎯 NPS: >70

**Meta a 24 meses:**
- 🎯 150-200 academias suscritas
- 🎯 ~30,000 usuarios finales
- 🎯 MRR: $50-70M COP/mes
- 🎯 Expansión a México y Argentina

### 2.3 Planes de Suscripción

#### **Plan Básico: $150,000 COP/mes**
- 1 sede
- Hasta 100 alumnos activos
- 5 profesores
- Control de asistencias con QR
- Gestión de pagos y facturación
- Reportes básicos
- Soporte por email (48h respuesta)
- 10 GB almacenamiento

#### **Plan Profesional: $350,000 COP/mes** ⭐ MÁS POPULAR
- Hasta 2 sedes
- Hasta 300 alumnos activos
- 15 profesores
- Todo lo del Plan Básico +
- Reportes financieros avanzados
- Dashboard con KPIs en tiempo real
- Análisis de rentabilidad por clase/paquete
- Soporte prioritario (24h respuesta)
- WhatsApp/Chat en horario laboral
- 50 GB almacenamiento
- Personalización de logo y colores

#### **Plan Enterprise: $750,000 COP/mes**
- Sedes ilimitadas
- Alumnos ilimitados
- Profesores ilimitados
- Todo lo del Plan Profesional +
- API REST completa
- Onboarding personalizado
- Capacitación en vivo
- Soporte 24/7 (2h respuesta)
- Almacenamiento ilimitado
- SLA 99.9% uptime garantizado
- Subdomain personalizado
- Whitelabel completo (opcional)

---

## 3. ARQUITECTURA MULTI-TENANT

### 3.1 Modelo Recomendado: Single Database Multi-Tenant

**¿Por qué este modelo?**

✅ **Es el estándar de la industria** (usado por 60-70% de SaaS)  
✅ **Más económico:** 1 base de datos vs múltiples  
✅ **Más simple de implementar:** 2-3 semanas vs 6-8 semanas  
✅ **Más fácil de mantener:** 1 migración vs múltiples  
✅ **Escalable:** Hasta 200-300 clientes  
✅ **Usado por grandes empresas:** Slack, HubSpot, Zendesk (en sus inicios)

**Alternativas descartadas:**
- ❌ **Database per Tenant:** Muy costoso ($5 USD/mes por cliente), complejo de mantener
- ❌ **App per Tenant:** No escalable, costos prohibitivos

### 3.2 Componentes Clave

#### **A. Tabla Tenants (Nueva)**

Almacena información de cada academia suscrita:

```sql
CREATE TABLE Tenants (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Nombre NVARCHAR(200) NOT NULL,          -- "Academia Salsa Caleña"
    Subdomain NVARCHAR(50) NOT NULL UNIQUE,  -- "salsa-cali"
    Plan NVARCHAR(20) NOT NULL,              -- "Basico", "Profesional", "Enterprise"
    Estado NVARCHAR(20) NOT NULL,            -- "Activo", "Suspendido", "Cancelado"
    FechaRegistro DATETIME NOT NULL,
    FechaVencimientoPlan DATETIME,
    
    -- Límites del plan
    MaxSedes INT NOT NULL,
    MaxAlumnos INT NOT NULL,
    MaxProfesores INT NOT NULL,
    MaxStorage INT NOT NULL,                 -- En MB
    
    -- Información de contacto
    EmailContacto NVARCHAR(100) NOT NULL,
    TelefonoContacto NVARCHAR(20),
    
    -- Personalización
    LogoUrl NVARCHAR(500),
    ColorPrimario NVARCHAR(7),               -- Hexadecimal #FF5733
    ColorSecundario NVARCHAR(7),
    
    -- Auditoría
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaActualizacion DATETIME,
    CreadoPor NVARCHAR(100),
    ActualizadoPor NVARCHAR(100)
);
```

#### **B. Columna TenantId en Todas las Tablas**

**Tablas que necesitan TenantId:**
- Usuarios (alumnos, profesores, admins)
- Pagos
- Clases
- Asistencias
- Paquetes
- Eventos
- LiquidacionesMensuales
- Solicitudes
- Referidos

**Ejemplo de migración:**

```sql
-- Agregar columna TenantId a tabla Usuarios
ALTER TABLE Usuarios
ADD TenantId UNIQUEIDENTIFIER NOT NULL 
    DEFAULT 'corp-chetango-001'  -- Valor por defecto para datos existentes
    FOREIGN KEY REFERENCES Tenants(Id);

-- Crear índice para mejorar performance
CREATE INDEX IX_Usuarios_TenantId ON Usuarios(TenantId);
```

#### **C. Filtros Globales en Entity Framework**

**Implementación en ApplicationDbContext:**

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // Filtro global para todas las entidades con TenantId
    modelBuilder.Entity<Usuario>()
        .HasQueryFilter(u => u.TenantId == _tenantProvider.GetCurrentTenantId());
    
    modelBuilder.Entity<Pago>()
        .HasQueryFilter(p => p.TenantId == _tenantProvider.GetCurrentTenantId());
    
    modelBuilder.Entity<Clase>()
        .HasQueryFilter(c => c.TenantId == _tenantProvider.GetCurrentTenantId());
    
    // ... repetir para todas las entidades
}
```

**Servicio TenantProvider:**

```csharp
public interface ITenantProvider
{
    Guid GetCurrentTenantId();
    Task<Tenant> GetCurrentTenant();
}

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;
    
    public Guid GetCurrentTenantId()
    {
        // Obtener TenantId del JWT claim
        var tenantIdClaim = _httpContextAccessor.HttpContext?
            .User.FindFirst("TenantId")?.Value;
        
        if (string.IsNullOrEmpty(tenantIdClaim))
            throw new UnauthorizedAccessException("TenantId no encontrado");
        
        return Guid.Parse(tenantIdClaim);
    }
    
    public async Task<Tenant> GetCurrentTenant()
    {
        var tenantId = GetCurrentTenantId();
        return await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == tenantId);
    }
}
```

#### **D. Resolución de Tenant por Subdomain**

**Middleware en Program.cs:**

```csharp
app.Use(async (context, next) =>
{
    var host = context.Request.Host.Host;
    
    // Extraer subdomain: salsa-cali.chetango.com → salsa-cali
    var parts = host.Split('.');
    if (parts.Length >= 3)
    {
        var subdomain = parts[0];
        
        // Buscar tenant por subdomain
        var tenantService = context.RequestServices
            .GetRequiredService<ITenantService>();
        var tenant = await tenantService.GetBySubdomain(subdomain);
        
        if (tenant == null)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("Academia no encontrada");
            return;
        }
        
        // Guardar TenantId en contexto
        context.Items["TenantId"] = tenant.Id;
    }
    
    await next();
});
```

### 3.3 Flujo de Autenticación Multi-Tenant

**1. Usuario accede:** `salsa-cali.chetango.com`

**2. Middleware detecta subdomain:** `salsa-cali`

**3. API consulta Tenant:**
```sql
SELECT * FROM Tenants WHERE Subdomain = 'salsa-cali'
```

**4. Usuario ingresa credenciales:**
- Email: `carlos@salsacali.com`
- Password: `******`

**5. API valida credenciales:**
```sql
SELECT * FROM Usuarios 
WHERE TenantId = 'salsa-cali-002' 
  AND Email = 'carlos@salsacali.com'
  AND PasswordHash = HASHBYTES('SHA2_256', '******')
```

**6. API genera JWT con claims:**
```json
{
  "sub": "user-id-123",
  "email": "carlos@salsacali.com",
  "tenantId": "salsa-cali-002",
  "tenantName": "Academia Salsa Caleña",
  "role": "Admin",
  "exp": 1708534800
}
```

**7. Usuario obtiene datos:**
Todas las queries automáticamente filtran por `TenantId = 'salsa-cali-002'`

### 3.4 Personalización Dinámica (Branding) por Tenant

**¿Cómo mostrar logos y colores diferentes según el cliente?**

Este es un patrón **estándar de la industria** usado por Shopify, Slack, Zendesk, Notion, y prácticamente todos los SaaS multi-tenant exitosos.

#### **A. Almacenamiento de Personalización en Tabla Tenants**

La tabla `Tenants` ya incluye columnas para personalización:

```sql
-- Columnas de personalización (ya incluidas en diseño de Tenants)
LogoUrl NVARCHAR(500),              -- URL del logo del cliente
ColorPrimario NVARCHAR(7),          -- Color principal en hex (#FF5733)
ColorSecundario NVARCHAR(7),        -- Color secundario en hex
ColorAccent NVARCHAR(7),            -- Color de acento
NombreComercial NVARCHAR(200),     -- Nombre para mostrar
FaviconUrl NVARCHAR(500),          -- Favicon personalizado
```

**Ejemplo de datos:**
```sql
INSERT INTO Tenants (...) VALUES
(
    'salsa-cali-002',
    'Academia Salsa Caleña',
    'salsa-cali',
    'Profesional',
    'Activo',
    ...
    'https://storage.chetango.com/logos/salsa-cali.png',  -- LogoUrl
    '#FF5733',                                             -- ColorPrimario (naranja)
    '#3498DB',                                             -- ColorSecundario (azul)
    '#FFD700',                                             -- ColorAccent (dorado)
    'Academia Salsa Caleña - La Mejor de Cali',          -- NombreComercial
    'https://storage.chetango.com/favicons/salsa-cali.ico' -- FaviconUrl
);
```

#### **B. Endpoint Público para Obtener Personalización**

**API: `GET /api/tenants/by-subdomain/{subdomain}`**

```csharp
[HttpGet("by-subdomain/{subdomain}")]
[AllowAnonymous] // ← Importante: público para que login pueda acceder
public async Task<ActionResult<TenantBrandingDto>> GetBySubdomain(string subdomain)
{
    var tenant = await _context.Tenants
        .Where(t => t.Subdomain == subdomain)
        .Where(t => t.Estado == "Activo")
        .Select(t => new TenantBrandingDto
        {
            Id = t.Id,
            Nombre = t.Nombre,
            NombreComercial = t.NombreComercial,
            LogoUrl = t.LogoUrl,
            ColorPrimario = t.ColorPrimario ?? "#FF6B6B",      // Default Chetango
            ColorSecundario = t.ColorSecundario ?? "#4ECDC4",
            ColorAccent = t.ColorAccent ?? "#FFE66D",
            FaviconUrl = t.FaviconUrl,
            Plan = t.Plan
        })
        .FirstOrDefaultAsync();
    
    if (tenant == null)
        return NotFound(new { message = "Academia no encontrada" });
    
    return Ok(tenant);
}
```

#### **C. Frontend: Detección de Subdomain y Carga de Personalización**

**1. Crear contexto de Tenant (TenantContext.tsx):**

```typescript
import { createContext, useContext, useState, useEffect, ReactNode } from 'react';

interface TenantBranding {
  id: string;
  nombre: string;
  nombreComercial?: string;
  logoUrl?: string;
  colorPrimario: string;
  colorSecundario: string;
  colorAccent: string;
  faviconUrl?: string;
  plan: string;
}

const TenantContext = createContext<TenantBranding | null>(null);

export function TenantProvider({ children }: { children: ReactNode }) {
  const [tenant, setTenant] = useState<TenantBranding | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function loadTenant() {
      try {
        // Detectar subdomain
        const hostname = window.location.hostname;
        const parts = hostname.split('.');
        
        // Si es localhost o IP, usar subdomain por defecto
        const subdomain = hostname === 'localhost' || hostname.startsWith('127.') || hostname.startsWith('192.')
          ? 'corporacionchetango'  // Default para desarrollo
          : parts[0];

        // Cargar info del tenant desde API
        const response = await fetch(
          `${import.meta.env.VITE_API_URL}/api/tenants/by-subdomain/${subdomain}`
        );

        if (!response.ok) {
          throw new Error('Tenant no encontrado');
        }

        const data = await response.json();
        setTenant(data);

        // Aplicar personalización global
        applyBranding(data);
      } catch (error) {
        console.error('Error cargando tenant:', error);
        // Usar valores por defecto de Chetango
        const defaultTenant: TenantBranding = {
          id: 'default',
          nombre: 'Chetango',
          colorPrimario: '#FF6B6B',
          colorSecundario: '#4ECDC4',
          colorAccent: '#FFE66D',
          plan: 'Básico'
        };
        setTenant(defaultTenant);
        applyBranding(defaultTenant);
      } finally {
        setLoading(false);
      }
    }

    loadTenant();
  }, []);

  function applyBranding(tenant: TenantBranding) {
    // Aplicar CSS variables globalmente
    document.documentElement.style.setProperty('--color-primary', tenant.colorPrimario);
    document.documentElement.style.setProperty('--color-secondary', tenant.colorSecundario);
    document.documentElement.style.setProperty('--color-accent', tenant.colorAccent);

    // Cambiar favicon
    if (tenant.faviconUrl) {
      const favicon = document.querySelector("link[rel*='icon']") as HTMLLinkElement;
      if (favicon) {
        favicon.href = tenant.faviconUrl;
      }
    }

    // Cambiar título de la pestaña
    document.title = `${tenant.nombreComercial || tenant.nombre} - Gestión Inteligente`;
  }

  if (loading) {
    return <div className="loading-spinner">Cargando...</div>;
  }

  return (
    <TenantContext.Provider value={tenant}>
      {children}
    </TenantContext.Provider>
  );
}

export function useTenant() {
  const context = useContext(TenantContext);
  if (!context) {
    throw new Error('useTenant debe usarse dentro de TenantProvider');
  }
  return context;
}
```

**2. Envolver App con TenantProvider (main.tsx):**

```typescript
import { TenantProvider } from './contexts/TenantContext';

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <TenantProvider>
      <App />
    </TenantProvider>
  </React.StrictMode>
);
```

**3. Usar personalización en Login (Login.tsx):**

```typescript
import { useTenant } from '@/contexts/TenantContext';

export function Login() {
  const tenant = useTenant();
  
  return (
    <div className="login-page">
      {/* Logo dinámico según tenant */}
      <div className="logo-container">
        {tenant.logoUrl ? (
          <img 
            src={tenant.logoUrl} 
            alt={tenant.nombre}
            className="h-20 w-auto"
          />
        ) : (
          // Fallback al logo de Chetango
          <img src="/chetango-logo.png" alt="Chetango" className="h-20 w-auto" />
        )}
      </div>
      
      {/* Título personalizado */}
      <h1 className="text-3xl font-bold mt-4">
        {tenant.nombreComercial || tenant.nombre}
      </h1>
      <p className="text-gray-600 mt-2">Sistema de Gestión Inteligente</p>
      
      {/* Botón con color personalizado (usa CSS variable) */}
      <button className="btn-primary mt-8">
        Iniciar sesión con Microsoft
      </button>
    </div>
  );
}
```

**4. CSS con variables personalizadas (globals.css):**

```css
:root {
  /* Valores por defecto (Chetango) - se sobrescriben dinámicamente */
  --color-primary: #FF6B6B;
  --color-secondary: #4ECDC4;
  --color-accent: #FFE66D;
}

/* Usar variables en todos los componentes */
.btn-primary {
  background-color: var(--color-primary);
  color: white;
}

.btn-primary:hover {
  background-color: color-mix(in srgb, var(--color-primary) 80%, black);
}

.navbar {
  background-color: var(--color-primary);
}

.link-active {
  color: var(--color-secondary);
}

.badge-accent {
  background-color: var(--color-accent);
}
```

#### **D. Desarrollo Local: Simular Diferentes Tenants**

**Opción 1: Query parameter (más fácil para testing)**

```typescript
// En TenantProvider, antes del fetch
const urlParams = new URLSearchParams(window.location.search);
const tenantParam = urlParams.get('tenant');

const subdomain = tenantParam 
  || (hostname === 'localhost' ? 'corporacionchetango' : parts[0]);
```

**Uso:**
```
http://localhost:5173?tenant=salsa-cali
http://localhost:5173?tenant=bachata-bogota
http://localhost:5173?tenant=corporacionchetango
```

**Opción 2: Dropdown selector en desarrollo**

```typescript
{import.meta.env.DEV && (
  <div className="fixed bottom-4 right-4 bg-white shadow-lg p-4 rounded">
    <label>Simular Tenant:</label>
    <select 
      onChange={(e) => window.location.href = `?tenant=${e.target.value}`}
      className="ml-2 border rounded px-2 py-1"
    >
      <option value="corporacionchetango">Corporación Chetango</option>
      <option value="salsa-cali">Salsa Cali</option>
      <option value="bachata-bogota">Bachata Bogotá</option>
    </select>
  </div>
)}
```

#### **E. Flujo Completo: Nuevo Cliente se Registra**

**Cuando una nueva academia se registra:**

1. **Usuario llena formulario de registro** en `/register` (sin subdomain aún)
2. **API crea tenant** con datos básicos + subdomain generado
3. **API sube logo a Azure Storage** (si lo proporcionó)
4. **API envía email de bienvenida** con link a `subdomain.chetango.com`
5. **Usuario accede a su subdomain** → ve su logo y colores
6. **Usuario completa onboarding** → configura más detalles

**Ejemplo de proceso de registro:**

```typescript
// POST /api/tenants/register
{
  "nombreAcademia": "Academia Salsa Caleña",
  "subdomain": "salsa-cali",  // Se valida que no exista
  "emailContacto": "admin@salsacali.com",
  "plan": "Profesional",
  "logo": <file>,  // Opcional
  "colorPrimario": "#FF5733"  // Opcional
}

// Backend crea tenant y retorna:
{
  "tenantId": "salsa-cali-002",
  "subdomain": "salsa-cali",
  "accessUrl": "https://salsa-cali.chetango.com",
  "setupComplete": false
}
```

#### **F. Panel de Configuración de Branding (Para el Cliente)**

Cada cliente puede personalizar su branding desde `/admin/configuracion/branding`:

```typescript
export function BrandingSettings() {
  const tenant = useTenant();
  const [logo, setLogo] = useState<File | null>(null);
  const [colors, setColors] = useState({
    primary: tenant.colorPrimario,
    secondary: tenant.colorSecundario,
    accent: tenant.colorAccent
  });

  async function handleSave() {
    const formData = new FormData();
    if (logo) formData.append('logo', logo);
    formData.append('colorPrimario', colors.primary);
    formData.append('colorSecundario', colors.secondary);
    formData.append('colorAccent', colors.accent);

    await fetch(`/api/tenants/${tenant.id}/branding`, {
      method: 'PUT',
      body: formData
    });

    // Recargar para aplicar cambios
    window.location.reload();
  }

  return (
    <div className="branding-settings">
      <h2>Personalización de Marca</h2>
      
      <div className="form-group">
        <label>Logo de la Academia</label>
        <input 
          type="file" 
          accept="image/*"
          onChange={(e) => setLogo(e.target.files?.[0] || null)}
        />
        <p className="help-text">
          Recomendado: PNG transparente, 400x100px
        </p>
      </div>

      <div className="form-group">
        <label>Color Principal</label>
        <input 
          type="color" 
          value={colors.primary}
          onChange={(e) => setColors({...colors, primary: e.target.value})}
        />
      </div>

      <div className="form-group">
        <label>Color Secundario</label>
        <input 
          type="color" 
          value={colors.secondary}
          onChange={(e) => setColors({...colors, secondary: e.target.value})}
        />
      </div>

      <button onClick={handleSave} className="btn-primary">
        Guardar Cambios
      </button>

      <div className="preview mt-4">
        <h3>Vista Previa</h3>
        <div 
          className="preview-card" 
          style={{ 
            backgroundColor: colors.primary,
            color: 'white',
            padding: '2rem',
            borderRadius: '8px'
          }}
        >
          <h4>Así se verá tu academia</h4>
          <button style={{ backgroundColor: colors.secondary }}>
            Botón Ejemplo
          </button>
        </div>
      </div>
    </div>
  );
}
```

#### **G. Niveles de Personalización por Plan**

| Plan | Logo | Colores | Favicon | Whitelabel* |
|------|------|---------|---------|-------------|
| **Básico** | ✅ | ❌ | ❌ | ❌ |
| **Profesional** | ✅ | ✅ (2 colores) | ✅ | ❌ |
| **Enterprise** | ✅ | ✅ (3+ colores) | ✅ | ✅ |

*Whitelabel = Ocultar completamente marca Chetango del footer/documentación

#### **H. Checklist: Agregar Nuevo Cliente con Personalización**

**Para el equipo cuando un cliente nuevo se una:**

1. ✅ **Validar subdomain disponible** (no duplicado)
2. ✅ **Crear registro en tabla Tenants** con plan correspondiente
3. ✅ **Subir logo a Azure Storage** (si lo proporciona)
   - Path: `/tenants/{tenantId}/logo.png`
   - Actualizar `LogoUrl` en DB
4. ✅ **Configurar colores** (o usar defaults)
5. ✅ **Verificar DNS** para `{subdomain}.chetango.com`
6. ✅ **Probar acceso** en navegador incógnito
7. ✅ **Enviar email de bienvenida** con credenciales
8. ✅ **Agendar onboarding call** (para plan Profesional/Enterprise)

**Comando de prueba rápida:**
```sql
-- Verificar que tenant se vea correctamente
SELECT 
    Subdomain,
    Nombre,
    LogoUrl,
    ColorPrimario,
    Estado
FROM Tenants
WHERE Subdomain = 'nuevo-cliente';
```

**Probar en navegador:**
```
https://nuevo-cliente.chetango.com
→ Debe cargar con logo y colores del cliente
→ Sin errores en consola
```

---

## 4. PLAN DE ESCALAMIENTO POR ETAPAS

### FASE 0: PREPARACIÓN (Semanas 1-4)

**Objetivo:** Adaptar código actual para soporte multi-tenant

**Tareas Técnicas:**
1. ✅ Crear tabla Tenants en base de datos
2. ✅ Agregar columna TenantId a todas las tablas
3. ✅ Migrar datos existentes de Corporación Chetango con TenantId único
4. ✅ Implementar ITenantProvider en backend
5. ✅ Configurar filtros globales en Entity Framework
6. ✅ Implementar middleware de resolución de subdomain
7. ✅ Modificar JWT para incluir TenantId
8. ✅ Crear página de registro self-service (frontend)
9. ✅ Integrar Wompi o Stripe para pagos
10. ✅ Configurar wildcard DNS en Azure (*.chetango.com)

**Infraestructura:** Mantener actual (sin cambios)

**Costo:** $100,000 - $142,000 COP/mes

**Clientes objetivo:** 0 (desarrollo)

**Testing:**
- Crear 2-3 tenants de prueba
- Verificar aislamiento de datos
- Probar registro y onboarding completo
- Validar que Corporación Chetango sigue funcionando

---

### FASE 1: LANZAMIENTO BETA (Semanas 5-12)

**Objetivo:** Conseguir primeros 5 clientes beta con descuento de fundadores

**Estrategia de Adquisición:**
- Contacto directo a academias conocidas
- Referidos de profesores de Corporación Chetango
- Oferta fundadores: 50% descuento de por vida + $0 setup fee

**Infraestructura Requerida:**

| Recurso | Tier | Capacidad | Costo COP/mes |
|---------|------|-----------|---------------|
| Azure SQL Database | **S0 Standard** | 250 GB, 10 DTUs | $63,000 |
| Azure App Service | **B2 Basic** | 2 cores, 3.5 GB RAM | $294,000 |
| Static Web App | Free | 100 GB bandwidth | $0 |
| Storage Account | Standard | ~2 GB | $8,400 |
| Application Insights | Basic (gratis) | 5 GB/mes | $0 |
| **TOTAL** | - | - | **$365,400** |

**Incremento de costos:** +$223,400 COP/mes

**Capacidad:**
- ✅ 5-10 academias
- ✅ ~1,500-2,500 usuarios
- ✅ ~5,000-10,000 transacciones/mes
- ✅ Backup 7 días incluido

**Clientes objetivo:** 5 academias

**Ingresos proyectados:**
- 5 academias x $175,000 promedio (50% descuento) = **$875,000 COP/mes**

**Rentabilidad:** $509,600 COP/mes (margen 58%)

**Métricas clave:**
- Time to value: <7 días
- Completion rate onboarding: >90%
- Churn rate: <10%/mes
- NPS: >60

**Ajustes en Azure:**

```powershell
# Escalar SQL Database a S0
az sql db update `
  --resource-group chetango-rg `
  --server chetango-db-prod `
  --name chetango-db `
  --service-objective S0

# Escalar App Service a B2
az appservice plan update `
  --name chetango-plan `
  --resource-group chetango-rg `
  --sku B2
```

---

### FASE 2: EARLY ADOPTERS (Meses 4-6)

**Objetivo:** Escalar a 15 academias con precios normales

**Infraestructura Requerida:**

| Recurso | Tier | Capacidad | Costo COP/mes |
|---------|------|-----------|---------------|
| Azure SQL Database | **S1 Standard** ⬆️ | 250 GB, 20 DTUs | $126,000 |
| Azure App Service | B2 Basic | 2 cores, 3.5 GB RAM | $294,000 |
| Static Web App | Free | 100 GB bandwidth | $0 |
| Storage Account | Standard | ~5 GB | $10,500 |
| Application Insights | Basic | 5 GB/mes gratis | $0 |
| **TOTAL** | - | - | **$430,500** |

**Incremento:** +$65,100 COP/mes vs Fase 1

**Capacidad:**
- ✅ 10-20 academias
- ✅ ~3,000-5,000 usuarios
- ✅ ~20,000-30,000 transacciones/mes
- ✅ Backup 7 días + retención extendida disponible

**Clientes objetivo:** 15 academias total (10 nuevas)

**Distribución:**
- 5 academias Plan Básico ($150K) = $750,000
- 8 academias Plan Profesional ($350K) = $2,800,000
- 2 academias Plan Enterprise ($750K) = $1,500,000

**Ingresos:** **$5,050,000 COP/mes**

**Rentabilidad:** $4,619,500 COP/mes (margen 91%)

**Cuándo escalar:**
- ⚠️ Cuando tengas 10 clientes activos
- ⚠️ O cuando CPU > 70% durante 1 hora
- ⚠️ O cuando DB queries > 1,000/minuto

**Ajuste en Azure:**

```powershell
# Escalar SQL Database a S1
az sql db update `
  --resource-group chetango-rg `
  --server chetango-db-prod `
  --name chetango-db `
  --service-objective S1

# Configurar backup extendido (35 días)
az sql db ltr-policy set `
  --resource-group chetango-rg `
  --server chetango-db-prod `
  --database chetango-db `
  --weekly-retention P4W `
  --monthly-retention P12M
```

---

### FASE 3: CRECIMIENTO (Meses 7-12)

**Objetivo:** Llegar a 50-60 academias

**Infraestructura Requerida:**

| Recurso | Tier | Capacidad | Costo COP/mes |
|---------|------|-----------|---------------|
| Azure SQL Database | S1 Standard | 250 GB, 20 DTUs | $126,000 |
| Azure App Service | **S1 Standard** ⬆️ | 1 core, 1.75 GB + Autoscale | $319,000 |
| Static Web App | Free | 100 GB bandwidth | $0 |
| Storage Account | Standard | ~10 GB | $16,800 |
| Application Insights | Basic | ~8 GB/mes | $21,000 |
| Azure CDN (opcional) | Standard | 100 GB | $21,000 |
| **TOTAL** | - | - | **$503,800** |

**Incremento:** +$73,300 COP/mes vs Fase 2

**Capacidad:**
- ✅ 30-60 academias
- ✅ ~8,000-12,000 usuarios
- ✅ ~60,000-100,000 transacciones/mes
- ✅ Auto-scaling 1-3 instancias
- ✅ Deployment slots (staging/production)

**Clientes objetivo:** 50 academias total (35 nuevas)

**Distribución:**
- 20 academias Plan Básico ($150K) = $3,000,000
- 25 academias Plan Profesional ($350K) = $8,750,000
- 5 academias Plan Enterprise ($750K) = $3,750,000

**Ingresos:** **$15,500,000 COP/mes**

**Rentabilidad:** $14,996,200 COP/mes (margen 97%)

**Cuándo escalar:**
- ⚠️ Cuando tengas 20-25 clientes activos
- ⚠️ O cuando necesites deployment slots
- ⚠️ O cuando quieras auto-scaling automático

**Ajuste en Azure:**

```powershell
# Escalar App Service a S1 (con autoscale)
az appservice plan update `
  --name chetango-plan `
  --resource-group chetango-rg `
  --sku S1

# Configurar auto-scaling
az monitor autoscale create `
  --resource-group chetango-rg `
  --resource chetango-plan `
  --resource-type Microsoft.Web/serverfarms `
  --name chetango-autoscale `
  --min-count 1 `
  --max-count 3 `
  --count 1

# Regla: Scale out cuando CPU > 70%
az monitor autoscale rule create `
  --resource-group chetango-rg `
  --autoscale-name chetango-autoscale `
  --condition "Percentage CPU > 70 avg 5m" `
  --scale out 1

# Regla: Scale in cuando CPU < 30%
az monitor autoscale rule create `
  --resource-group chetango-rg `
  --autoscale-name chetango-autoscale `
  --condition "Percentage CPU < 30 avg 10m" `
  --scale in 1
```

---

### FASE 4: ESCALA (Meses 13-24)

**Objetivo:** 100-200 academias con expansión regional

**Infraestructura Requerida:**

| Recurso | Tier | Capacidad | Costo COP/mes |
|---------|------|-----------|---------------|
| Azure SQL Database | **S3 Standard** ⬆️ | 250 GB, 100 DTUs | $630,000 |
| Azure App Service | S1 Standard | 1 core + Autoscale 1-5 | $319,000 + $160,000* |
| Static Web App | Free | 100 GB bandwidth | $0 |
| Storage Account | Standard | ~30 GB | $42,000 |
| Application Insights | Standard | ~20 GB/mes | $84,000 |
| Azure CDN | Standard | 500 GB | $84,000 |
| Azure Redis Cache | Basic C1 | 1 GB (opcional) | $63,000 |
| **TOTAL** | - | - | **~$1,382,000** |

_*Costo promedio autoscale: 2 instancias activas 50% del tiempo_

**Capacidad:**
- ✅ 100-200 academias
- ✅ ~20,000-40,000 usuarios
- ✅ ~200,000-400,000 transacciones/mes
- ✅ Alta disponibilidad
- ✅ Performance optimizado con Redis cache

**Clientes objetivo:** 150 academias total

**Distribución:**
- 60 academias Plan Básico = $9,000,000
- 70 academias Plan Profesional = $24,500,000
- 20 academias Plan Enterprise = $15,000,000

**Ingresos:** **$48,500,000 COP/mes**

**Rentabilidad:** $47,118,000 COP/mes (margen 97%)

**Consideraciones:**
- Evaluar migración a modelo híbrido (clientes enterprise con DB dedicada)
- Implementar sharding por región (Colombia, México, Argentina)
- Contratar equipo DevOps dedicado

---

## 5. ESPECIFICACIONES TÉCNICAS POR FASE

### 5.1 Base de Datos - Azure SQL Database

#### **Tier Comparison**

| Tier | DTUs | Storage | Queries/min | Backup | Costo COP/mes | Fase Recomendada |
|------|------|---------|-------------|--------|---------------|------------------|
| Basic | 5 | 2 GB | ~200 | 7 días | $21,000 | ❌ No recomendado |
| **S0 Standard** | 10 | 250 GB | ~500 | 7 días | $63,000 | ✅ Fase 1: 5-10 clientes |
| **S1 Standard** | 20 | 250 GB | ~1,000 | 7-35 días | $126,000 | ✅ Fase 2-3: 10-60 clientes |
| **S3 Standard** | 100 | 250 GB | ~5,000 | 7-35 días | $630,000 | ✅ Fase 4: 100-200 clientes |
| P1 Premium | 125 | 500 GB | ~10,000 | 7-35 días | $1,953,000 | ⏳ Fase 5: 200+ clientes |

**DTU (Database Transaction Unit):** Unidad que combina CPU, memoria y I/O

**Cuándo escalar:**
- S0 → S1: Cuando queries/minuto > 400 sostenido por 10+ minutos
- S1 → S3: Cuando queries/minuto > 800 sostenido, o >50 clientes
- S3 → P1: Cuando queries/minuto > 4,000, o necesitas read replicas

#### **Configuración de Backups**

**Básico (incluido en todos los tiers):**
- Backup completo: Semanal
- Backup diferencial: Cada 12 horas
- Backup de logs: Cada 5-10 minutos
- Retención: 7 días
- Restauración point-in-time: Cualquier momento en los últimos 7 días

**Extendido (configurar manualmente):**

```powershell
# Configurar Long-Term Retention (LTR)
az sql db ltr-policy set `
  --resource-group chetango-rg `
  --server chetango-db-prod `
  --database chetango-db `
  --weekly-retention P4W `      # 4 semanas
  --monthly-retention P12M `    # 12 meses
  --yearly-retention P5Y `      # 5 años (opcional)
  --week-of-year 1              # Para yearly backup
```

**Costo adicional LTR:**
- $0.05 USD/GB/mes (~$210 COP/GB/mes)
- Ejemplo: DB de 5 GB con retención 12 meses = ~$1,050 COP/mes

#### **Índices Recomendados**

```sql
-- Índice compuesto en TenantId + campos comunes
CREATE INDEX IX_Usuarios_TenantId_Email 
ON Usuarios(TenantId, Email) 
INCLUDE (Nombre, Rol);

CREATE INDEX IX_Pagos_TenantId_Fecha 
ON Pagos(TenantId, FechaPago) 
INCLUDE (Monto, EstadoPago);

CREATE INDEX IX_Asistencias_TenantId_Fecha 
ON Asistencias(TenantId, FechaAsistencia) 
INCLUDE (ClaseId, AlumnoId, Presente);

-- Índice para reportes
CREATE INDEX IX_Pagos_TenantId_FechaPago_EstadoPago 
ON Pagos(TenantId, FechaPago, EstadoPago) 
INCLUDE (Monto, MetodoPago);
```

### 5.2 Azure App Service (API Backend)

#### **Tier Comparison**

| Tier | CPU | RAM | Features | Costo COP/mes | Fase |
|------|-----|-----|----------|---------------|------|
| B1 Basic | 1 core | 1.75 GB | SSL, Custom domain | $70,000 | ❌ Solo desarrollo |
| **B2 Basic** | 2 cores | 3.5 GB | SSL, Custom domain | $294,000 | ✅ Fase 1: 5-10 clientes |
| **S1 Standard** | 1 core | 1.75 GB | + Autoscale, Slots | $319,000 | ✅ Fase 2-4: 10-200 clientes |
| S2 Standard | 2 cores | 3.5 GB | + Autoscale, Slots | $638,000 | ⏳ Si S1 es insuficiente |
| P1v2 Premium | 1 core | 3.5 GB | + Alta disponibilidad | $453,000 | ⏳ Fase 5: 200+ clientes |

**Deployment Slots (solo Standard+):**
- Slot "staging": Para probar cambios antes de producción
- Slot "production": Ambiente live
- Swap sin downtime

**Configuración Autoscale (S1):**

```powershell
# Reglas recomendadas
Mínimo: 1 instancia (siempre activa)
Máximo: 3 instancias (para 50 clientes)
Máximo: 5 instancias (para 100+ clientes)

Trigger Scale Out:
- CPU > 70% por 5 minutos → +1 instancia
- Requests > 1,000/min por 5 min → +1 instancia

Trigger Scale In:
- CPU < 30% por 10 minutos → -1 instancia
- Requests < 300/min por 10 min → -1 instancia
```

**Application Settings recomendados:**

```json
{
  "ASPNETCORE_ENVIRONMENT": "Production",
  "APPLICATIONINSIGHTS_CONNECTION_STRING": "InstrumentationKey=...",
  "ConnectionStrings__DefaultConnection": "Server=...",
  "JwtSettings__Secret": "...",
  "JwtSettings__Issuer": "https://api.chetango.com",
  "JwtSettings__Audience": "https://chetango.com",
  "JwtSettings__ExpirationMinutes": "60",
  "TenantSettings__AllowedDomains": "*.chetango.com",
  "Stripe__PublicKey": "pk_live_...",
  "Stripe__SecretKey": "sk_live_...",
  "Wompi__PublicKey": "pub_...",
  "Wompi__PrivateKey": "prv_...",
  "Storage__ConnectionString": "DefaultEndpointsProtocol=https;..."
}
```

### 5.3 Azure Static Web App (Frontend)

**Plan Free es suficiente para todas las fases (1-200 clientes)**

**Especificaciones Free:**
- Bandwidth: 100 GB/mes (suficiente)
- Build minutes: 10 GB/mes
- Custom domains: 2 incluidos (+ wildcard *.chetango.com)
- SSL automático
- CDN incluido
- Authentication providers: Azure AD, GitHub, Twitter

**Cuándo considerar Standard ($38,000/mes):**
- Si necesitas >100 GB bandwidth/mes (raro con <500 clientes)
- Si necesitas >2 custom domains sin wildcard
- Si necesitas más control sobre staging environments

**Configuración Custom Domain:**

```json
// staticwebapp.config.json
{
  "routes": [
    {
      "route": "/*",
      "allowedRoles": ["authenticated"]
    },
    {
      "route": "/login",
      "allowedRoles": ["anonymous"]
    }
  ],
  "navigationFallback": {
    "rewrite": "/index.html"
  },
  "globalHeaders": {
    "content-security-policy": "default-src 'self' https://api.chetango.com"
  }
}
```

### 5.4 Azure Storage Account

**Uso proyectado por fase:**

| Fase | Clientes | Alumnos | Archivos Estimados | Storage | Costo/mes |
|------|----------|---------|-------------------|---------|-----------|
| Fase 1 | 5 | 750 | ~400 fotos, docs | 500 MB | $8,400 |
| Fase 2 | 15 | 2,250 | ~1,200 archivos | 1.5 GB | $10,500 |
| Fase 3 | 50 | 7,500 | ~4,000 archivos | 5 GB | $16,800 |
| Fase 4 | 150 | 22,500 | ~12,000 archivos | 15 GB | $42,000 |

**Configuración recomendada:**

```powershell
# Crear Storage Account
az storage account create `
  --name chetangostorage `
  --resource-group chetango-rg `
  --location eastus `
  --sku Standard_LRS `
  --kind StorageV2

# Crear containers
az storage container create --name profile-photos --account-name chetangostorage
az storage container create --name documents --account-name chetangostorage
az storage container create --name invoices --account-name chetangostorage

# Configurar lifecycle policy (borrar archivos temporales después de 90 días)
az storage account management-policy create `
  --account-name chetangostorage `
  --policy @lifecycle-policy.json
```

**lifecycle-policy.json:**
```json
{
  "rules": [
    {
      "enabled": true,
      "name": "delete-old-temp-files",
      "type": "Lifecycle",
      "definition": {
        "actions": {
          "baseBlob": {
            "delete": {
              "daysAfterModificationGreaterThan": 90
            }
          }
        },
        "filters": {
          "blobTypes": ["blockBlob"],
          "prefixMatch": ["temp/"]
        }
      }
    }
  ]
}
```

### 5.5 Application Insights

**Telemetría estimada:**

| Fase | Clientes | GB/mes | Costo/mes |
|------|----------|--------|-----------|
| Fase 1 | 5 | 2-3 GB | $0 (gratis) |
| Fase 2 | 15 | 4-6 GB | $0-21,000 |
| Fase 3 | 50 | 10-15 GB | $42,000-84,000 |
| Fase 4 | 150 | 20-30 GB | $84,000-168,000 |

**Primeros 5 GB/mes son GRATIS**

**Métricas críticas a monitorear:**

```csharp
// Implementar en Program.cs
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    options.EnableAdaptiveSampling = true;  // Reducir telemetría en producción
    options.EnableQuickPulseMetricStream = true;  // Métricas en tiempo real
});

// Custom events en código
_telemetryClient.TrackEvent("TenantRegistered", new Dictionary<string, string>
{
    { "TenantId", tenant.Id.ToString() },
    { "Plan", tenant.Plan },
    { "Subdomain", tenant.Subdomain }
});

_telemetryClient.TrackMetric("ActiveUsers", activeUserCount, new Dictionary<string, string>
{
    { "TenantId", tenantId.ToString() }
});
```

**Alertas recomendadas:**

```powershell
# Alerta cuando API response time > 2 segundos
az monitor metrics alert create `
  --name high-response-time `
  --resource-group chetango-rg `
  --scopes /subscriptions/.../chetango-api `
  --condition "avg requests/duration > 2000" `
  --window-size 5m `
  --evaluation-frequency 1m `
  --action-group /subscriptions/.../alertas-equipo

# Alerta cuando error rate > 5%
az monitor metrics alert create `
  --name high-error-rate `
  --resource-group chetango-rg `
  --scopes /subscriptions/.../chetango-api `
  --condition "avg requests/failed > 5" `
  --window-size 5m
```

---

## 6. ANÁLISIS DE COSTOS Y RENTABILIDAD

### 6.1 Resumen de Costos por Fase

| Fase | Clientes | Infra COP/mes | Ingresos COP/mes | Margen Bruto | ROI |
|------|----------|---------------|------------------|--------------|-----|
| **Actual** | 1 | $142,000 | $0 | -$142,000 | - |
| **Fase 0 (Dev)** | 0 | $142,000 | $0 | -$142,000 | - |
| **Fase 1 (Beta)** | 5 | $365,400 | $875,000 | $509,600 (58%) | 2.4x |
| **Fase 2 (Early)** | 15 | $430,500 | $5,050,000 | $4,619,500 (91%) | 11.7x |
| **Fase 3 (Growth)** | 50 | $503,800 | $15,500,000 | $14,996,200 (97%) | 30.8x |
| **Fase 4 (Scale)** | 150 | $1,382,000 | $48,500,000 | $47,118,000 (97%) | 35.1x |

### 6.2 Punto de Equilibrio (Break-even)

**Con costos Fase 1 ($365,400/mes):**
- Necesitas: **2-3 academias** en Plan Profesional ($350K)
- O: **5-6 academias** en Plan Básico ($150K)
- **Tiempo estimado:** Mes 2-3 de lanzamiento

**Con costos Fase 2 ($430,500/mes):**
- Necesitas: **3 academias** en Plan Profesional
- O: **2 academias** en Plan Enterprise ($750K)
- Ya alcanzado con 5 clientes de Fase 1

### 6.3 Proyección Financiera 24 Meses

**Escenario Conservador:**

| Mes | Nuevos Clientes | Total Clientes | MRR | Costos Infra | Margen Bruto | Acumulado |
|-----|-----------------|----------------|-----|--------------|--------------|-----------|
| M1-3 | 5 | 5 | $875K | $365K | $510K | $1.5M |
| M4-6 | 10 | 15 | $5.0M | $431K | $4.6M | $15.3M |
| M7-9 | 15 | 30 | $10.5M | $504K | $10.0M | $45.3M |
| M10-12 | 20 | 50 | $15.5M | $504K | $15.0M | $90.3M |
| M13-18 | 50 | 100 | $32.0M | $1.0M | $31.0M | $276M |
| M19-24 | 50 | 150 | $48.5M | $1.4M | $47.1M | $559M |

**Total acumulado 24 meses: ~$559,000,000 COP** (~$140,000 USD)

**Escenario Optimista:**

| Mes | Nuevos Clientes | Total Clientes | MRR | Acumulado 24m |
|-----|-----------------|----------------|-----|---------------|
| M1-3 | 10 | 10 | $3.5M | - |
| M4-6 | 20 | 30 | $10.5M | - |
| M7-12 | 70 | 100 | $32.0M | - |
| M13-24 | 200 | 300 | $100M+ | **~$1,200M COP** |

### 6.4 Costos Adicionales a Considerar

**Equipo (no incluido en proyecciones):**

| Rol | Salario COP/mes | Cuándo Contratar |
|-----|-----------------|------------------|
| Growth Marketer | $4,000,000 | Mes 4 (10 clientes) |
| Customer Success | $3,500,000 | Mes 7 (30 clientes) |
| DevOps Engineer | $6,000,000 | Mes 12 (50 clientes) |
| Sales Development Rep | $3,000,000 | Mes 12 (50 clientes) |

**Marketing y Adquisición:**

| Canal | Budget COP/mes | CAC Esperado | Cuándo Empezar |
|-------|----------------|--------------|----------------|
| Google Ads | $2,000,000 | $800K | Mes 7 |
| Facebook/Instagram Ads | $1,500,000 | $600K | Mes 7 |
| Content Marketing | $500,000 | $400K | Mes 4 |
| Partnership Program | 20% comisión | $0 upfront | Mes 10 |

**LTV/CAC Target: 5:1 o superior**

**Ejemplo:**
- CAC: $1,000,000 (costo de adquirir 1 cliente)
- LTV: $5,000,000 (ingreso lifetime de 1 cliente)
- Ratio: 5:1 ✅ Saludable

**Cálculo LTV:**
```
LTV = (Precio promedio mensual) x (Tiempo de vida promedio en meses)
LTV = $350,000 x 24 meses = $8,400,000 COP
CAC objetivo = $8,400,000 / 5 = $1,680,000 COP máximo
```

### 6.5 Sensibilidad de Precios

**¿Qué pasa si reducimos precios 20%?**

| Plan | Precio Actual | Precio -20% | Impacto en 50 clientes |
|------|---------------|-------------|------------------------|
| Básico | $150,000 | $120,000 | -$600,000/mes |
| Profesional | $350,000 | $280,000 | -$1,750,000/mes |
| Enterprise | $750,000 | $600,000 | -$750,000/mes |

**Impacto total:** -$3,100,000/mes (-20% MRR)  
**Margen sigue siendo:** 94% (excelente)

**¿Qué pasa si aumentamos precios 20%?**

| Plan | Precio Actual | Precio +20% | Impacto en 50 clientes |
|------|---------------|-------------|------------------------|
| Básico | $150,000 | $180,000 | +$600,000/mes |
| Profesional | $350,000 | $420,000 | +$1,750,000/mes |
| Enterprise | $750,000 | $900,000 | +$750,000/mes |

**Impacto total:** +$3,100,000/mes (+20% MRR)  
**Riesgo:** Churn puede aumentar 10-15%

**Recomendación:** Mantener precios actuales hasta validar product-market fit con 30+ clientes

---

## 7. CHECKLIST DE IMPLEMENTACIÓN

### 7.1 Preparación Técnica (Semanas 1-4)

**Backend - Base de Datos:**
- [ ] Crear tabla `Tenants` con todos los campos requeridos
- [ ] Agregar columna `TenantId UNIQUEIDENTIFIER` a todas las tablas
- [ ] Crear índices en `TenantId` para todas las tablas
- [ ] Migrar datos de Corporación Chetango con TenantId único
- [ ] Verificar que todas las queries incluyen filtro por TenantId
- [ ] Ejecutar migration en ambiente QA y validar

**Backend - Código:**
- [ ] Crear `ITenantProvider` y `TenantProvider`
- [ ] Implementar middleware de resolución de tenant por subdomain
- [ ] Configurar filtros globales en `ApplicationDbContext`
- [ ] Modificar `JwtService` para incluir `TenantId` en claims
- [ ] Crear `TenantService` con métodos CRUD
- [ ] Implementar `TenantValidator` para límites de plan
- [ ] Agregar `TenantId` a todos los comandos Create/Update
- [ ] Modificar handlers para usar `ITenantProvider.GetCurrentTenantId()`
- [ ] Crear endpoints `/api/tenants` (registro, get, update)
- [ ] Implementar health checks con tenant validation

**Backend - Testing:**
- [ ] Crear unit tests para `TenantProvider`
- [ ] Crear integration tests con múltiples tenants
- [ ] Verificar aislamiento de datos entre tenants
- [ ] Test de performance con 100 tenants simulados
- [ ] Test de seguridad (intentar acceder a datos de otro tenant)

**Frontend:**
- [ ] Crear página de registro (`/register`)
- [ ] Implementar flujo de onboarding (steps 1-5)
- [ ] Configurar detección de subdomain en `main.tsx`
- [ ] Modificar `AuthContext` para manejar `tenantId`
- [ ] Crear página de selección de plan (`/plans`)
- [ ] Implementar integración de pagos (Wompi/Stripe)
- [ ] Crear dashboard de tenant admin (`/admin/settings`)

**Frontend - Personalización Dinámica (Branding):**
- [ ] Crear `TenantContext.tsx` con provider y hook `useTenant()`
- [ ] Implementar detección de subdomain (con fallback para localhost)
- [ ] Crear endpoint público `GET /api/tenants/by-subdomain/{subdomain}`
- [ ] Implementar carga de branding al iniciar app
- [ ] Configurar CSS variables para colores dinámicos (`:root`)
- [ ] Modificar Login.tsx para mostrar logo dinámico por tenant
- [ ] Implementar cambio de favicon dinámico
- [ ] Implementar cambio de título de página dinámico
- [ ] Crear página `/admin/configuracion/branding` para clientes
- [ ] Agregar query parameter `?tenant=xxx` para testing local
- [ ] Probar con 3 tenants diferentes (logos y colores distintos)

**Infraestructura:**
- [ ] Configurar wildcard DNS `*.chetango.com` en Azure
- [ ] Configurar SSL certificate para wildcard domain
- [ ] Configurar Application Insights con sampling
- [ ] Configurar alertas en Azure Monitor
- [ ] Crear backup manual de base de datos actual
- [ ] Documentar proceso de rollback

### 7.2 Lanzamiento Beta (Semanas 5-8)

**Pre-Launch:**
- [ ] Escalar SQL Database a S0 Standard
- [ ] Escalar App Service a B2 Basic
- [ ] Configurar Application Insights
- [ ] Crear 3 tenants de prueba y validar todo el flujo
- [ ] Preparar materiales de marketing (landing page, videos)
- [ ] Crear documentación de onboarding
- [ ] Configurar email transaccional (SendGrid/Mailgun)
- [ ] Preparar template de emails (bienvenida, pago, soporte)

**Launch:**
- [ ] Contactar 10 academias target para beta
- [ ] Ofrecer descuento fundadores (50% lifetime)
- [ ] Onboarding personalizado con cada cliente
- [ ] **Configurar branding:** Logo + colores para cada cliente beta
- [ ] **Verificar personalización:** Probar acceso con subdomain de cada cliente
- [ ] Recopilar feedback en primeras 48 horas
- [ ] Iterar sobre problemas reportados
- [ ] Grabar sesiones de uso para análisis

**Checklist por Cliente Nuevo (Usar en cada onboarding):**
- [ ] 1. Validar subdomain disponible (no duplicado)
- [ ] 2. Crear registro en tabla Tenants con plan correcto
- [ ] 3. Solicitar logo del cliente (PNG transparente, 400x100px recomendado)
- [ ] 4. Subir logo a Azure Storage: `/tenants/{tenantId}/logo.png`
- [ ] 5. Actualizar columna `LogoUrl` en base de datos
- [ ] 6. Configurar colores (primario, secundario) o usar defaults
- [ ] 7. Probar acceso en incógnito: `https://{subdomain}.chetango.com`
- [ ] 8. Verificar que logo y colores se muestren correctamente
- [ ] 9. Enviar email de bienvenida con credenciales y link
- [ ] 10. Agendar call de onboarding (si es plan Profesional/Enterprise)

**Post-Launch:**
- [ ] Enviar encuesta de satisfacción a clientes beta
- [ ] Monitorear métricas clave diariamente
- [ ] Ajustar onboarding basado en feedback
- [ ] Crear casos de éxito con clientes beta
- [ ] Preparar testimonios y referidos

### 7.3 Crecimiento (Mes 4-12)

**Escalar Infraestructura:**
- [ ] Escalar SQL Database a S1 cuando tengas 10 clientes
- [ ] Escalar App Service a S1 cuando tengas 20 clientes
- [ ] Configurar auto-scaling (1-3 instancias)
- [ ] Configurar deployment slots (staging/production)
- [ ] Implementar Redis Cache para performance
- [ ] Configurar CDN para assets estáticos

**Marketing y Ventas:**
- [ ] Lanzar campaña Google Ads ($2M/mes)
- [ ] Lanzar campaña Facebook/Instagram Ads ($1.5M/mes)
- [ ] Crear blog con 2 posts/semana
- [ ] Webinar mensual sobre gestión de academias
- [ ] Participar en eventos del sector
- [ ] Crear partner program (20% comisión recurrente)

**Producto:**
- [ ] Implementar API REST pública
- [ ] Crear marketplace de integraciones
- [ ] Implementar webhooks
- [ ] Crear mobile app (React Native)
- [ ] Implementar whitelabel completo
- [ ] Agregar analytics avanzados

**Equipo:**
- [ ] Contratar Growth Marketer (Mes 4)
- [ ] Contratar Customer Success (Mes 7)
- [ ] Contratar DevOps Engineer (Mes 12)
- [ ] Contratar Sales Development Rep (Mes 12)

---

## 8. MÉTRICAS DE MONITOREO

### 8.1 KPIs Técnicos

**Performance:**
- ⏱️ **API Response Time:** <500ms (p95), <200ms (p50)
- ⏱️ **Page Load Time:** <2s (frontend)
- 📊 **Database Query Time:** <100ms (p95)
- 🔄 **Uptime:** >99.5% (SLA)
- 🚨 **Error Rate:** <1% de requests

**Capacidad:**
- 💾 **Database Size:** Monitorear crecimiento semanal
- 💾 **Storage Usage:** Alertar cuando >70% del plan
- 🔌 **Concurrent Connections:** Monitorear vs límite del tier
- 🔋 **DTU Usage:** Alertar cuando >80% sostenido por 10 min

**Queries a ejecutar semanalmente:**

```sql
-- Tamaño de base de datos
SELECT 
    DB_NAME() as DatabaseName,
    SUM(size) * 8 / 1024 AS SizeInMB,
    SUM(size) * 8 / 1024 / 1024 AS SizeInGB
FROM sys.master_files
WHERE DB_NAME(database_id) = 'chetango-db';

-- Top 10 tablas más grandes
SELECT 
    t.NAME AS TableName,
    SUM(a.total_pages) * 8 / 1024 AS TotalSpaceMB,
    SUM(a.used_pages) * 8 / 1024 AS UsedSpaceMB,
    (SUM(a.total_pages) - SUM(a.used_pages)) * 8 / 1024 AS UnusedSpaceMB
FROM sys.tables t
INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
GROUP BY t.Name
ORDER BY TotalSpaceMB DESC;

-- Queries lentas (>1 segundo)
SELECT TOP 10
    qt.text AS Query,
    qs.execution_count AS ExecutionCount,
    qs.total_elapsed_time / 1000 AS TotalElapsedTimeMS,
    qs.total_elapsed_time / qs.execution_count / 1000 AS AvgElapsedTimeMS,
    qs.creation_time AS CreationTime
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
WHERE qs.total_elapsed_time / qs.execution_count > 1000000 -- >1 segundo
ORDER BY qs.total_elapsed_time DESC;
```

### 8.2 KPIs de Negocio

**Adquisición:**
- 📈 **New Signups:** Academias registradas/semana
- 💰 **Conversion Rate:** Registro → Pago (target >60%)
- ⏱️ **Time to First Value:** Días hasta primera asistencia registrada (target <3 días)
- 💵 **CAC (Customer Acquisition Cost):** Costo de adquirir 1 cliente (target <$1.5M)

**Retención:**
- 📉 **Churn Rate:** % clientes cancelados/mes (target <5%)
- ⏳ **Customer Lifetime:** Promedio meses de suscripción (target >24 meses)
- 💰 **LTV (Lifetime Value):** Ingreso total por cliente (target >$8M)
- ❤️ **NPS (Net Promoter Score):** Satisfacción cliente (target >70)

**Engagement:**
- 👥 **DAU/MAU Ratio:** Usuarios activos diarios vs mensuales (target >40%)
- 🔄 **Weekly Active Tenants:** % tenants con actividad semanal (target >90%)
- 📊 **Feature Adoption:** % uso de features clave (QR, pagos, reportes)
- 📧 **Support Tickets:** Tickets/cliente/mes (target <2)

**Revenue:**
- 💰 **MRR (Monthly Recurring Revenue):** Ingresos recurrentes mensuales
- 📈 **MRR Growth Rate:** % crecimiento mes a mes (target >15%)
- 💵 **ARPU (Average Revenue Per User):** Ingreso promedio por academia
- 📊 **Revenue by Plan:** Distribución Básico/Profesional/Enterprise

**Dashboard ejecutivo (actualización diaria):**

```sql
-- Reporte diario de métricas clave
DECLARE @Today DATE = CAST(GETDATE() AS DATE);
DECLARE @WeekAgo DATE = DATEADD(DAY, -7, @Today);
DECLARE @MonthAgo DATE = DATEADD(MONTH, -1, @Today);

-- Total tenants activos
SELECT 
    COUNT(*) as TotalTenants,
    SUM(CASE WHEN Plan = 'Basico' THEN 1 ELSE 0 END) as TenantBasico,
    SUM(CASE WHEN Plan = 'Profesional' THEN 1 ELSE 0 END) as TenantProfesional,
    SUM(CASE WHEN Plan = 'Enterprise' THEN 1 ELSE 0 END) as TenantEnterprise,
    SUM(CASE WHEN Estado = 'Activo' THEN 1 ELSE 0 END) as TenantActivos,
    SUM(CASE WHEN Estado = 'Suspendido' THEN 1 ELSE 0 END) as TenantSuspendidos
FROM Tenants;

-- MRR actual
SELECT 
    SUM(CASE 
        WHEN Plan = 'Basico' THEN 150000
        WHEN Plan = 'Profesional' THEN 350000
        WHEN Plan = 'Enterprise' THEN 750000
    END) as MRR_Total
FROM Tenants
WHERE Estado = 'Activo';

-- Nuevos registros última semana
SELECT COUNT(*) as NuevosTenantsUltimaSemana
FROM Tenants
WHERE FechaRegistro >= @WeekAgo;

-- Usuarios activos última semana (por tenant)
SELECT 
    t.Nombre as Academia,
    COUNT(DISTINCT u.Id) as UsuariosActivos,
    COUNT(DISTINCT CASE WHEN u.UltimoAcceso >= @WeekAgo THEN u.Id END) as UsuariosActivosUltimaSemana,
    CAST(COUNT(DISTINCT CASE WHEN u.UltimoAcceso >= @WeekAgo THEN u.Id END) * 100.0 / COUNT(DISTINCT u.Id) AS DECIMAL(5,2)) as PorcentajeActivo
FROM Tenants t
LEFT JOIN Usuarios u ON t.Id = u.TenantId
WHERE t.Estado = 'Activo'
GROUP BY t.Id, t.Nombre
ORDER BY UsuariosActivosUltimaSemana DESC;

-- Churn rate último mes
SELECT 
    COUNT(*) as TenantsCancelados,
    CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Tenants WHERE Estado = 'Activo') AS DECIMAL(5,2)) as ChurnRate
FROM Tenants
WHERE Estado = 'Cancelado' 
  AND FechaActualizacion >= @MonthAgo;
```

### 8.3 Alertas Automáticas

**Configurar en Azure Monitor:**

```powershell
# Alerta: API Response Time > 2 segundos
az monitor metrics alert create `
  --name high-api-response-time `
  --resource-group chetango-rg `
  --scopes /subscriptions/.../chetango-api `
  --condition "avg requests/duration > 2000" `
  --description "API response time is too high" `
  --window-size 5m `
  --evaluation-frequency 1m `
  --severity 2

# Alerta: Error Rate > 5%
az monitor metrics alert create `
  --name high-error-rate `
  --resource-group chetango-rg `
  --scopes /subscriptions/.../chetango-api `
  --condition "avg requests/failed > 5" `
  --description "API error rate is too high" `
  --window-size 5m `
  --severity 1

# Alerta: Database DTU > 80%
az monitor metrics alert create `
  --name high-database-dtu `
  --resource-group chetango-rg `
  --scopes /subscriptions/.../chetango-db `
  --condition "avg dtu_consumption_percent > 80" `
  --description "Database is under high load" `
  --window-size 10m `
  --severity 2

# Alerta: Storage > 80%
az monitor metrics alert create `
  --name high-storage-usage `
  --resource-group chetango-rg `
  --scopes /subscriptions/.../chetango-db `
  --condition "avg storage_percent > 80" `
  --description "Database storage is running out" `
  --window-size 1h `
  --severity 1
```

**Notificaciones:**
- Email a equipo técnico
- SMS para severity 1 (crítico)
- Integración con Slack/Teams
- Crear incident en sistema de tickets

---

## 9. PLAN DE CONTINGENCIA

### 9.1 Escenarios de Riesgo

#### **Riesgo 1: Crecimiento más rápido de lo esperado**

**Síntoma:** 20 clientes en mes 3 (vs 5 esperados)

**Impacto:**
- ⚠️ Performance degradada (response time >2s)
- ⚠️ Database DTU >90% sostenido
- ⚠️ Quejas de clientes por lentitud

**Solución inmediata:**
```powershell
# Escalar SQL Database de S0 a S1
az sql db update --resource-group chetango-rg --server chetango-db-prod --name chetango-db --service-objective S1

# Escalar App Service de B2 a S1 con autoscale
az appservice plan update --name chetango-plan --resource-group chetango-rg --sku S1
```

**Costo adicional:** +$155,000 COP/mes  
**Tiempo de ejecución:** 5-10 minutos (sin downtime)

---

#### **Riesgo 2: Churn alto (>15% mensual)**

**Síntomas:**
- 3+ cancelaciones en el mismo mes
- NPS <50
- Múltiples tickets de soporte sin resolver

**Causas posibles:**
- Onboarding deficiente
- Bugs críticos
- Expectativas no cumplidas
- Competencia

**Solución:**
1. **Llamar a cada cliente que cancela** (exit interview)
2. **Ofrecer mes gratis** si problema es resoluble
3. **Priorizar features solicitados** por clientes en riesgo
4. **Mejorar onboarding:** Agregar video tutorials, sesión 1-on-1
5. **Implementar health score:** Identificar clientes en riesgo antes de cancelar

**Indicadores de riesgo:**
- Sin login en 7+ días
- <5 asistencias registradas en último mes
- Ticket de soporte sin responder en 48h
- No ha registrado pagos en 2+ meses

---

#### **Riesgo 3: Crecimiento más lento de lo esperado**

**Síntoma:** 2 clientes en mes 6 (vs 15 esperados)

**Causas posibles:**
- Product-market fit débil
- Precio muy alto
- Marketing insuficiente
- Propuesta de valor poco clara

**Solución:**
1. **Encuestar a 20+ academias target** (¿por qué no se suscriben?)
2. **Ofrecer trial gratuito 30 días** (eliminar fricción)
3. **Reducir precios 30% temporalmente** (validar sensibilidad)
4. **Pivotar plan Básico:** Reducir a $99K/mes con límites más estrictos
5. **Intensificar marketing:** Duplicar budget Google Ads
6. **Partnership agresivo:** Ofrecer 30% comisión a referidores

**Mantener costos bajos:**
- Mantener S0/B2 (no escalar prematuramente)
- No contratar equipo adicional hasta 10 clientes
- Enfocarse en retención sobre adquisición

---

#### **Riesgo 4: Brecha de seguridad (data leak)**

**Escenario:** Cliente accede a datos de otro tenant

**Impacto:**
- 🔴 **Crítico:** Pérdida de confianza completa
- 🔴 Posible demanda legal
- 🔴 Churn masivo (50-100% clientes)

**Prevención:**
1. **Auditoría de código mensual:** Verificar filtros por TenantId
2. **Penetration testing:** Contratar ethical hacker trimestral
3. **Query monitoring:** Alertar queries sin filtro TenantId
4. **Integration tests exhaustivos:** Cross-tenant access attempts

**Respuesta si ocurre:**
1. **Inmediato (hora 0):** Desactivar plataforma completamente
2. **Hora 1:** Identificar scope del leak (¿qué datos? ¿cuántos tenants?)
3. **Hora 2:** Patchear vulnerabilidad
4. **Hora 3:** Notificar a clientes afectados (email + llamada)
5. **Hora 6:** Publicar post-mortem transparente
6. **Día 1:** Ofrecer compensación (3 meses gratis)
7. **Semana 1:** Contratar auditoría externa completa

---

#### **Riesgo 5: Competidor lanza producto similar**

**Escenario:** Empresa grande (Mindbody, Glofox) lanza features específicas para danza

**Mitigación:**
1. **Diferenciación vertical:** Especializarse 100% en danza (no gimnasios, no yoga)
2. **Precio competitivo:** 40-50% más barato que competidores generales
3. **Soporte local:** Soporte en español, horario Colombia
4. **Features específicas:** Coreografías, vestuario, eventos de danza
5. **Comunidad:** Crear red de academias, compartir best practices

**Ventaja competitiva defendible:**
- Conocimiento profundo del sector danza en Colombia
- Network effects (más academias = más valor)
- Costo de cambio alto (datos históricos, onboarding)

---

### 9.2 Procedimiento de Rollback

**Situación:** Deploy de nueva versión causa errores críticos

**Síntomas:**
- Error rate >20%
- API response time >5 segundos
- Múltiples reportes de usuarios

**Rollback con Deployment Slots (S1 Standard):**

```powershell
# 1. Verificar estado del slot staging
az webapp deployment slot list --name chetango-api --resource-group chetango-rg

# 2. Swap back de production a staging (revertir)
az webapp deployment slot swap --name chetango-api --resource-group chetango-rg --slot staging --action swap

# Tiempo: ~30 segundos sin downtime
```

**Rollback sin Deployment Slots (B2 Basic):**

```powershell
# 1. Revertir a commit anterior en GitHub
git revert HEAD
git push origin main

# 2. GitHub Actions redesplegará versión anterior automáticamente
# Tiempo: 3-5 minutos con ~1 minuto de downtime
```

**Rollback de base de datos:**

```powershell
# 1. Restaurar a point-in-time (antes del deploy problemático)
az sql db restore `
  --dest-name chetango-db-restore `
  --resource-group chetango-rg `
  --server chetango-db-prod `
  --source-database chetango-db `
  --time "2026-02-19T10:00:00Z"

# 2. Validar base de datos restaurada
# 3. Swap connection strings en App Service
# 4. Eliminar base de datos con problemas

# Tiempo: 15-30 minutos con downtime completo
```

**Comunicación durante incidente:**
1. **Minuto 0:** Status page: "Investigando problema"
2. **Minuto 5:** Email a clientes: "Estamos al tanto del problema"
3. **Minuto 15:** Status update cada 15 minutos
4. **Resolución:** Email: "Problema resuelto, post-mortem en 24h"

---

## 10. ANEXOS TÉCNICOS

### 10.1 Script de Migración Multi-Tenant

**Archivo:** `scripts/migration-multitenant-complete.sql`

```sql
-- =====================================================
-- MIGRACIÓN COMPLETA MULTI-TENANT
-- Fecha: 2026-02-20
-- Descripción: Convierte Chetango single-tenant a multi-tenant SaaS
-- =====================================================

-- PASO 1: Crear tabla Tenants
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tenants')
BEGIN
    CREATE TABLE Tenants (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Nombre NVARCHAR(200) NOT NULL,
        Subdomain NVARCHAR(50) NOT NULL UNIQUE,
        Plan NVARCHAR(20) NOT NULL CHECK (Plan IN ('Basico', 'Profesional', 'Enterprise')),
        Estado NVARCHAR(20) NOT NULL DEFAULT 'Activo' CHECK (Estado IN ('Activo', 'Suspendido', 'Cancelado')),
        FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
        FechaVencimientoPlan DATETIME,
        
        -- Límites del plan
        MaxSedes INT NOT NULL,
        MaxAlumnos INT NOT NULL,
        MaxProfesores INT NOT NULL,
        MaxStorageMB INT NOT NULL,
        
        -- Información de contacto
        EmailContacto NVARCHAR(100) NOT NULL,
        TelefonoContacto NVARCHAR(20),
        
        -- Personalización
        LogoUrl NVARCHAR(500),
        ColorPrimario NVARCHAR(7),
        ColorSecundario NVARCHAR(7),
        
        -- Auditoría
        FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
        FechaActualizacion DATETIME,
        CreadoPor NVARCHAR(100),
        ActualizadoPor NVARCHAR(100)
    );
    
    PRINT 'Tabla Tenants creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Tabla Tenants ya existe';
END
GO

-- PASO 2: Insertar tenant de Corporación Chetango (datos existentes)
DECLARE @CorporacionChetangoId UNIQUEIDENTIFIER = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890';

IF NOT EXISTS (SELECT * FROM Tenants WHERE Id = @CorporacionChetangoId)
BEGIN
    INSERT INTO Tenants (
        Id, Nombre, Subdomain, Plan, Estado, 
        FechaRegistro, MaxSedes, MaxAlumnos, MaxProfesores, MaxStorageMB,
        EmailContacto, CreadoPor
    )
    VALUES (
        @CorporacionChetangoId,
        'Corporación Chetango',
        'corporacionchetango',
        'Enterprise',
        'Activo',
        '2024-01-01',
        99999, -- Ilimitado
        99999, -- Ilimitado
        99999, -- Ilimitado
        999999, -- Ilimitado
        'admin@corporacionchetango.com',
        'MIGRATION_SCRIPT'
    );
    
    PRINT 'Tenant Corporación Chetango creado';
END
GO

-- PASO 3: Agregar columna TenantId a todas las tablas principales
DECLARE @TenantId UNIQUEIDENTIFIER = 'A1B2C3D4-E5F6-7890-ABCD-EF1234567890';
DECLARE @SQL NVARCHAR(MAX);
DECLARE @TableName NVARCHAR(128);

-- Lista de tablas que necesitan TenantId
DECLARE @TablesToModify TABLE (TableName NVARCHAR(128));
INSERT INTO @TablesToModify VALUES 
    ('Usuarios'), ('Pagos'), ('Clases'), ('Asistencias'),
    ('Paquetes'), ('Eventos'), ('LiquidacionesMensuales'),
    ('Solicitudes'), ('Referidos'), ('Alumnos'), ('Profesores');

DECLARE table_cursor CURSOR FOR
    SELECT TableName FROM @TablesToModify;

OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @TableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Verificar si columna TenantId ya existe
    IF NOT EXISTS (
        SELECT * FROM sys.columns 
        WHERE object_id = OBJECT_ID(@TableName) 
        AND name = 'TenantId'
    )
    BEGIN
        -- Agregar columna TenantId
        SET @SQL = 'ALTER TABLE [dbo].[' + @TableName + '] 
                    ADD TenantId UNIQUEIDENTIFIER NOT NULL 
                    DEFAULT ''' + CAST(@TenantId AS NVARCHAR(36)) + '''
                    CONSTRAINT FK_' + @TableName + '_Tenant 
                    FOREIGN KEY REFERENCES Tenants(Id)';
        EXEC sp_executesql @SQL;
        
        -- Crear índice para mejorar performance
        SET @SQL = 'CREATE NONCLUSTERED INDEX IX_' + @TableName + '_TenantId 
                    ON [dbo].[' + @TableName + '](TenantId)';
        EXEC sp_executesql @SQL;
        
        PRINT 'Columna TenantId agregada a ' + @TableName + ' con índice';
    END
    ELSE
    BEGIN
        PRINT 'Columna TenantId ya existe en ' + @TableName;
    END
    
    FETCH NEXT FROM table_cursor INTO @TableName;
END

CLOSE table_cursor;
DEALLOCATE table_cursor;
GO

-- PASO 4: Registrar migración en historial
IF NOT EXISTS (SELECT * FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20260220000001_ConvertirMultiTenant')
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260220000001_ConvertirMultiTenant', N'9.0.0');
    
    PRINT 'Migración registrada en __EFMigrationsHistory';
END
GO

-- PASO 5: Verificación final
PRINT '==========================================';
PRINT 'VERIFICACIÓN DE MIGRACIÓN MULTI-TENANT';
PRINT '==========================================';

-- Verificar tenants
SELECT 
    'Tenants' as Tabla,
    COUNT(*) as TotalRegistros,
    SUM(CASE WHEN Estado = 'Activo' THEN 1 ELSE 0 END) as Activos
FROM Tenants;

-- Verificar usuarios con TenantId
SELECT 
    'Usuarios' as Tabla,
    COUNT(*) as TotalRegistros,
    COUNT(DISTINCT TenantId) as TenantsDiferentes
FROM Usuarios;

-- Verificar pagos con TenantId
SELECT 
    'Pagos' as Tabla,
    COUNT(*) as TotalRegistros,
    COUNT(DISTINCT TenantId) as TenantsDiferentes
FROM Pagos;

PRINT 'Migración Multi-Tenant completada exitosamente';
PRINT '==========================================';
GO
```

### 10.2 Configuración de Wildcard DNS

**En Azure DNS Zone:**

```powershell
# 1. Crear DNS Zone (si no existe)
az network dns zone create `
  --resource-group chetango-rg `
  --name chetango.com

# 2. Crear wildcard record *.chetango.com → API
az network dns record-set a add-record `
  --resource-group chetango-rg `
  --zone-name chetango.com `
  --record-set-name '*' `
  --ipv4-address <IP_AZURE_APP_SERVICE>

# 3. Crear CNAME para www
az network dns record-set cname set-record `
  --resource-group chetango-rg `
  --zone-name chetango.com `
  --record-set-name www `
  --cname chetango-frontend.azurestaticapps.net

# 4. Obtener nameservers para configurar en proveedor de dominio
az network dns zone show `
  --resource-group chetango-rg `
  --name chetango.com `
  --query nameServers
```

**En App Service (API):**

```powershell
# Agregar custom domain wildcard
az webapp config hostname add `
  --webapp-name chetango-api `
  --resource-group chetango-rg `
  --hostname '*.chetango.com'

# Configurar SSL certificate
az webapp config ssl create `
  --resource-group chetango-rg `
  --name chetango-api `
  --hostname '*.chetango.com'
```

### 10.3 Template de Email Transaccional

**Bienvenida - Nuevo Tenant:**

```html
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background: #FF6B6B; color: white; padding: 20px; text-align: center; }
        .content { padding: 30px 20px; }
        .button { background: #FF6B6B; color: white; padding: 12px 30px; 
                  text-decoration: none; border-radius: 5px; display: inline-block; }
        .footer { background: #f4f4f4; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>¡Bienvenido a Chetango! 💃</h1>
        </div>
        
        <div class="content">
            <h2>Hola {{NombreContacto}},</h2>
            
            <p>Tu academia <strong>{{NombreAcademia}}</strong> está lista para despegar. 🚀</p>
            
            <p>Puedes acceder a tu plataforma en:</p>
            <p style="text-align: center; font-size: 18px;">
                <strong>https://{{Subdomain}}.chetango.com</strong>
            </p>
            
            <p>Credenciales de acceso:</p>
            <ul>
                <li><strong>Email:</strong> {{EmailContacto}}</li>
                <li><strong>Contraseña:</strong> {{ContraseñaTemporal}}</li>
            </ul>
            
            <p style="text-align: center; margin: 30px 0;">
                <a href="https://{{Subdomain}}.chetango.com" class="button">
                    Acceder a Mi Academia
                </a>
            </p>
            
            <h3>Próximos Pasos:</h3>
            <ol>
                <li><strong>Configura tu academia:</strong> Logo, colores, información básica</li>
                <li><strong>Agrega profesores:</strong> Invita a tu equipo de instructores</li>
                <li><strong>Crea tus clases:</strong> Define horarios y cupos</li>
                <li><strong>Importa alumnos:</strong> Sube tu lista de estudiantes</li>
                <li><strong>¡Empieza a usar QR!</strong> Control de asistencias en segundos</li>
            </ol>
            
            <p>¿Necesitas ayuda? Responde este email o escríbenos a 
               <a href="mailto:soporte@chetango.com">soporte@chetango.com</a></p>
            
            <p>¡Estamos emocionados de tenerte con nosotros!</p>
            
            <p>Saludos,<br>
            Equipo Chetango</p>
        </div>
        
        <div class="footer">
            <p>Plan contratado: <strong>{{NombrePlan}}</strong></p>
            <p>Vigencia hasta: <strong>{{FechaVencimiento}}</strong></p>
            <p>&copy; 2026 Chetango - Gestión Inteligente para Academias de Danza</p>
        </div>
    </div>
</body>
</html>
```

---

## 📋 GUÍA RÁPIDA: ONBOARDING DE NUEVO CLIENTE

### Paso a Paso para Agregar una Nueva Academia

#### 1️⃣ **Pre-Registro: Información Requerida**

Solicitar al cliente:
- ✅ Nombre oficial de la academia
- ✅ Subdomain deseado (ej: `salsa-cali`, `bachata-bogota`)
- ✅ Email de contacto principal
- ✅ Plan elegido (Básico / Profesional / Enterprise)
- ✅ Logo en PNG transparente (400x100px recomendado) - opcional
- ✅ Colores corporativos en formato hex - opcional

#### 2️⃣ **Validación y Creación en Base de Datos**

```sql
-- Verificar que subdomain NO exista
SELECT COUNT(*) FROM Tenants WHERE Subdomain = 'nuevo-cliente';
-- Debe retornar 0

-- Crear nuevo tenant
INSERT INTO Tenants (
    Id,
    Nombre,
    Subdomain,
    Plan,
    Estado,
    FechaRegistro,
    MaxSedes,
    MaxAlumnos,
    MaxProfesores,
    MaxStorageMB,
    EmailContacto,
    ColorPrimario,
    ColorSecundario,
    CreadoPor
) VALUES (
    NEWID(),
    'Academia Salsa Caleña',
    'salsa-cali',
    'Profesional',
    'Activo',
    GETDATE(),
    2,      -- Plan Profesional: hasta 2 sedes
    300,    -- Plan Profesional: hasta 300 alumnos
    15,     -- Plan Profesional: hasta 15 profesores
    51200,  -- 50 GB en MB
    'admin@salsacali.com',
    '#FF5733',  -- Color naranja (o NULL para usar default)
    '#3498DB',  -- Color azul (o NULL para usar default)
    'ADMIN'
);
```

#### 3️⃣ **Subir Logo a Azure Storage** (si el cliente lo proporciona)

```powershell
# Obtener TenantId recién creado
$tenantId = "..." # Del INSERT anterior

# Subir logo a Azure Storage
az storage blob upload `
  --account-name chetangostorage `
  --container-name tenant-logos `
  --name "$tenantId/logo.png" `
  --file "C:\path\to\logo-cliente.png" `
  --content-type "image/png"

# Obtener URL pública
$logoUrl = az storage blob url `
  --account-name chetangostorage `
  --container-name tenant-logos `
  --name "$tenantId/logo.png" `
  --output tsv
```

```sql
-- Actualizar LogoUrl en base de datos
UPDATE Tenants 
SET LogoUrl = 'https://chetangostorage.blob.core.windows.net/tenant-logos/{tenantId}/logo.png'
WHERE Id = '{tenantId}';
```

#### 4️⃣ **Crear Usuario Administrador Inicial**

```sql
-- Crear usuario admin del cliente
INSERT INTO Usuarios (
    Id,
    TenantId,
    Email,
    Nombre,
    Rol,
    Estado,
    FechaCreacion
) VALUES (
    NEWID(),
    '{tenantId}',  -- Del paso anterior
    'admin@salsacali.com',
    'Administrador',
    'Admin',
    'Activo',
    GETDATE()
);
```

#### 5️⃣ **Verificación de Acceso**

**Probar en navegador (modo incógnito):**
```
https://salsa-cali.chetango.com
```

**Verificar que se vea:**
- ✅ Logo del cliente (si lo proporcionó)
- ✅ Colores personalizados (si los configuró)
- ✅ Nombre de la academia en el título
- ✅ Sin errores en consola del navegador

**Query de verificación:**
```sql
SELECT 
    Subdomain,
    Nombre,
    LogoUrl,
    ColorPrimario,
    ColorSecundario,
    Estado,
    Plan
FROM Tenants
WHERE Subdomain = 'salsa-cali';
```

#### 6️⃣ **Email de Bienvenida**

Enviar email al cliente con:
- ✅ URL de acceso: `https://{subdomain}.chetango.com`
- ✅ Credenciales iniciales (si aplica)
- ✅ Link a documentación de onboarding
- ✅ Información sobre cómo personalizar branding desde `/admin/configuracion/branding`
- ✅ Contacto de soporte

#### 7️⃣ **Onboarding Call** (Para Profesional/Enterprise)

Agendar sesión de 30-45 minutos para:
- ✅ Configurar sedes y horarios
- ✅ Importar alumnos existentes
- ✅ Configurar profesores
- ✅ Crear paquetes de clases
- ✅ Mostrar sistema de QR para asistencias
- ✅ Explicar reportes financieros

---

### 🔍 Troubleshooting Común

**Problema:** Logo no se muestra
- Verificar que `LogoUrl` esté en HTTPS
- Verificar permisos del blob en Azure Storage (público)
- Verificar tamaño del archivo (<2MB recomendado)
- Limpiar caché del navegador

**Problema:** Colores no aplican
- Verificar formato hex válido (#RRGGBB)
- Verificar que CSS variables estén definidas
- Recargar página con Ctrl+F5

**Problema:** Subdomain no resuelve
- Verificar wildcard DNS `*.chetango.com`
- Verificar SSL certificate para wildcard
- Puede tomar 5-10 minutos propagar DNS

**Problema:** Cliente no puede acceder
- Verificar Estado = 'Activo' en Tenants
- Verificar que usuario exista con ese TenantId
- Verificar fecha de vencimiento del plan

---

## 🎯 RESUMEN EJECUTIVO FINAL

### Situación Actual
- ✅ Sistema operativo con 1 cliente (Corporación Chetango)
- ✅ Infraestructura: ~$142,000 COP/mes
- ✅ Capacidad: Soporta hasta 5 academias sin cambios

### Recomendación Principal
**Implementar modelo Single Database Multi-Tenant** (usado por 60-70% de SaaS del mundo)

### Plan de Escalamiento

| Fase | Clientes | Infraestructura | Costo/mes | Ingresos/mes | Margen |
|------|----------|-----------------|-----------|--------------|--------|
| **Fase 1** | 5 | S0 + B2 | $365K | $875K | 58% |
| **Fase 2** | 15 | S1 + B2 | $431K | $5.0M | 91% |
| **Fase 3** | 50 | S1 + S1 | $504K | $15.5M | 97% |
| **Fase 4** | 150 | S3 + S1 | $1.4M | $48.5M | 97% |

### Próximos Pasos Inmediatos

**Semanas 1-2:**
1. Implementar tabla Tenants y columna TenantId
2. Configurar filtros globales en Entity Framework
3. Crear middleware de resolución de tenant
4. **Implementar sistema de branding dinámico** (TenantContext + CSS variables)

**Semanas 3-4:**
5. Implementar página de registro y onboarding
6. Integrar Wompi/Stripe para pagos
7. **Crear panel de configuración de branding para clientes**
8. Testing exhaustivo con 3 tenants de prueba (diferentes logos/colores)

**Mes 2:**
9. Escalar a S0 + B2 ($365K/mes)
10. Lanzar beta con 5 academias (50% descuento)
11. **Configurar branding personalizado para cada cliente beta**
12. Recopilar feedback e iterar

### Punto de Equilibrio
**2-3 academias** en Plan Profesional cubren costos de infraestructura

### ROI Proyectado
- **Mes 6:** 11.7x (ingresos 11.7 veces los costos)
- **Mes 12:** 30.8x
- **Mes 24:** 35.1x

### 📌 Recordatorio: Personalización por Cliente (Branding)

**Cada vez que se incorpore un nuevo cliente:**

1. ✅ Validar subdomain disponible
2. ✅ Crear tenant en base de datos
3. ✅ **Solicitar y subir logo del cliente**
4. ✅ **Configurar colores corporativos (primario, secundario)**
5. ✅ Verificar en navegador: `https://{subdomain}.chetango.com`
6. ✅ Confirmar que logo y colores se vean correctamente
7. ✅ Enviar credenciales de acceso
8. ✅ Mostrar al cliente cómo personalizar desde `/admin/configuracion/branding`

**Patrón estándar de la industria usado por:** Shopify, Slack, Zendesk, Notion, HubSpot, Salesforce.

---

**Documento preparado por:** Equipo Técnico Chetango  
**Última actualización:** 21 de Febrero de 2026 - **Agregada Sección 3.4: Personalización Dinámica (Branding)**  
**Próxima revisión:** Cada 3 meses o al llegar a 10, 30, 50, 100 clientes

````
3. Crear middleware de resolución de tenant

**Semanas 3-4:**
4. Implementar página de registro y onboarding
5. Integrar Wompi/Stripe para pagos
6. Testing exhaustivo con 3 tenants de prueba

**Mes 2:**
7. Escalar a S0 + B2 ($365K/mes)
8. Lanzar beta con 5 academias (50% descuento)
9. Recopilar feedback e iterar

### Punto de Equilibrio
**2-3 academias** en Plan Profesional cubren costos de infraestructura

### ROI Proyectado
- **Mes 6:** 11.7x (ingresos 11.7 veces los costos)
- **Mes 12:** 30.8x
- **Mes 24:** 35.1x

---

**Documento preparado por:** Equipo Técnico Chetango  
**Última actualización:** 20 de Febrero de 2026  
**Próxima revisión:** Cada 3 meses o al llegar a 10, 30, 50, 100 clientes
