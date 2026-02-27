# 🚀 GUÍA DE MIGRACIÓN A DOMINIO NUEVO - CHETANGO SAAS

**Versión:** 1.0  
**Fecha:** 25 de Febrero de 2026  
**Autor:** Equipo Técnico Chetango  
**Propósito:** Migrar de `app.corporacionchetango.com` a dominio neutral para SaaS multi-tenant

---

## 📋 TABLA DE CONTENIDOS

1. [Contexto y Motivación](#1-contexto-y-motivación)
2. [Dominio Elegido](#2-dominio-elegido)
3. [Plan de Migración](#3-plan-de-migración)
4. [Configuración Azure](#4-configuración-azure)
5. [Actualización de Código](#5-actualización-de-código)
6. [Testing y Validación](#6-testing-y-validación)
7. [Comunicación a Usuarios](#7-comunicación-a-usuarios)
8. [Checklist Completo](#8-checklist-completo)
9. [Rollback Plan](#9-rollback-plan)

---

## 1. CONTEXTO Y MOTIVACIÓN

### 1.1 Situación Actual

**Dominio actual:**
```
corporacionchetango.com (Wix - Sitio web comercial)
└── app.corporacionchetango.com (Azure - Aplicación SaaS)
```

**Problema:**
- Otras academias verán "chetango" en su URL
- Puede generar confusión de marca
- No es neutral para un SaaS multi-tenant

### 1.2 Solución Propuesta

**Estructura nueva:**
```
corporacionchetango.com (Wix)
└── Sitio web comercial de TU academia (sin cambios)

[NUEVO-DOMINIO].app (Azure - Nuevo)
├── corporacionchetango.[NUEVO-DOMINIO].app  → Tu academia
├── salsacali.[NUEVO-DOMINIO].app            → Cliente 1
├── bachata.[NUEVO-DOMINIO].app              → Cliente 2
└── danza.[NUEVO-DOMINIO].app                → Cliente 3
```

### 1.3 Ventajas

✅ **Marca neutral** para todas las academias  
✅ **Profesional** - suena a plataforma de software  
✅ **Escalable** - ilimitados subdominios  
✅ **Sin conflicto** de identidad entre academias  
✅ **Costo bajo** - $15 USD/año (~$63,000 COP)

---

## 2. DOMINIO ELEGIDO

### 2.1 Opciones Preseleccionadas

**Llena este espacio después de elegir:**

```
Dominio elegido: _________________.app

Ejemplos de cómo quedaría:
- corporacionchetango._________.app
- salsacali._________.app
- bachata._________.app
```

### 2.2 Opciones Consideradas

| Dominio | Concepto | Ventajas | Elegido |
|---------|----------|----------|---------|
| **och8.app** | Ocho (8 tiempos) | Corto, danza latina, creativo | ☐ |
| **eight.app** | Eight (8 en inglés) | Universal, profesional, claro | ☐ |
| **beat.app** | Tiempo musical | Directo, relacionado con música | ☐ |
| **flow.app** | Flujo | Universal, estado mental | ☐ |
| **surge.app** | Oleada de energía | Adrenalina, esfuerzo | ☐ |
| **tyme.app** | Tiempo | Timing perfecto | ☐ |
| **paso.app** | Paso de danza | Identidad latina | ☐ |
| **sync.app** | Sincronización | Tech + danza | ☐ |
| Otro: _______ | - | - | ☐ |

---

## 3. PLAN DE MIGRACIÓN

### 3.1 Timeline Completo (Sin Usuarios Activos)

**Ventaja:** Los 50 usuarios creados NO han accedido nunca → Migración limpia

```
📅 DÍA 1: Compra de dominio
📅 DÍA 2-3: Configuración Azure DNS + SSL
📅 DÍA 4: Actualización de código y configuraciones
📅 DÍA 5: Testing completo
📅 DÍA 6: Deploy a producción
📅 DÍA 7: ✅ Dar credenciales con URL NUEVA
```

### 3.2 Fases Detalladas

#### **FASE 0: Preparación (Día 1)**

**Tareas:**
- [ ] Decidir dominio final de la lista
- [ ] Verificar disponibilidad en Namecheap/GoDaddy
- [ ] Comprar dominio .app (~$15 USD)
- [ ] Obtener nameservers del proveedor

**Tiempo estimado:** 1 hora

---

#### **FASE 1: Configuración DNS en Azure (Días 2-3)**

**Objetivo:** Configurar Azure para reconocer el nuevo dominio

**Comandos Azure CLI:**

```powershell
# 1. Variables (REEMPLAZAR CON TUS VALORES)
$RESOURCE_GROUP = "chetango-rg"
$DOMAIN_NAME = "[NUEVO-DOMINIO].app"  # Ejemplo: och8.app
$APP_SERVICE_NAME = "chetango-api"
$STATIC_WEB_APP_NAME = "chetango-frontend"

# 2. Crear DNS Zone en Azure
az network dns zone create `
  --resource-group $RESOURCE_GROUP `
  --name $DOMAIN_NAME

# 3. Obtener nameservers de Azure
az network dns zone show `
  --resource-group $RESOURCE_GROUP `
  --name $DOMAIN_NAME `
  --query nameServers `
  --output table

# Copiar los 4 nameservers (ej: ns1-01.azure-dns.com)
# Ir al proveedor del dominio y configurarlos
```

**En el proveedor del dominio (Namecheap/GoDaddy):**
```
1. Ir a Domain Management
2. Cambiar Nameservers a "Custom DNS"
3. Agregar los 4 nameservers de Azure:
   - ns1-01.azure-dns.com
   - ns2-01.azure-dns.net
   - ns3-01.azure-dns.org
   - ns4-01.azure-dns.info
4. Guardar cambios
5. Esperar propagación (5 minutos - 2 horas)
```

**Configurar Wildcard DNS:**

```powershell
# 4. Obtener IP del App Service
$APP_SERVICE_IP = az webapp show `
  --resource-group $RESOURCE_GROUP `
  --name $APP_SERVICE_NAME `
  --query outboundIpAddresses `
  --output tsv

# Nota: Si usas App Service Plan Standard+, usar IP dedicada
# Para Basic/Free, usar CNAME

# 5. Crear wildcard record *.dominio.app
az network dns record-set a add-record `
  --resource-group $RESOURCE_GROUP `
  --zone-name $DOMAIN_NAME `
  --record-set-name '*' `
  --ipv4-address $APP_SERVICE_IP

# 6. Crear record para dominio raíz (opcional)
az network dns record-set a add-record `
  --resource-group $RESOURCE_GROUP `
  --zone-name $DOMAIN_NAME `
  --record-set-name '@' `
  --ipv4-address $APP_SERVICE_IP
```

**Agregar dominio al App Service:**

```powershell
# 7. Agregar wildcard custom domain
az webapp config hostname add `
  --webapp-name $APP_SERVICE_NAME `
  --resource-group $RESOURCE_GROUP `
  --hostname "*.$DOMAIN_NAME"

# 8. Agregar dominio específico para Corporación Chetango
az webapp config hostname add `
  --webapp-name $APP_SERVICE_NAME `
  --resource-group $RESOURCE_GROUP `
  --hostname "corporacionchetango.$DOMAIN_NAME"

# 9. Configurar SSL automático (Certificate Manager)
az webapp config ssl bind `
  --resource-group $RESOURCE_GROUP `
  --name $APP_SERVICE_NAME `
  --certificate-thumbprint auto `
  --ssl-type SNI
```

**Tiempo estimado:** 2-4 horas (incluyendo propagación DNS)

---

#### **FASE 2: Actualización de Configuraciones Backend (Día 4)**

**Archivos a Modificar:**

##### **1. appsettings.json**

**Ubicación:** `Chetango.Api/appsettings.json`

```json
{
  "ConnectionStrings": {
    "ChetangoConnection": "Server=localhost;Database=ChetangoDB_Dev;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "<tu-dominio>.onmicrosoft.com",
    "TenantId": "<tenant-guid>",
    "ClientId": "<api-app-client-id>",
    "Audience": "api://<api-app-client-id>",
    "CallbackPath": "/signin-oidc",
    "Scopes": "user.read"
  },
  "Cors": {
    "AllowedOrigins": [
      "https://*.[NUEVO-DOMINIO].app",
      "https://corporacionchetango.[NUEVO-DOMINIO].app",
      "http://localhost:5173",
      "https://localhost:5173"
    ]
  },
  "JwtSettings": {
    "Issuer": "https://api.[NUEVO-DOMINIO].app",
    "Audience": "https://[NUEVO-DOMINIO].app",
    "AllowedDomains": "*.[NUEVO-DOMINIO].app"
  },
  "TenantSettings": {
    "DomainBase": "[NUEVO-DOMINIO].app"
  },
  "Auth": {
    "RequiredScopes": [
      "api.read"
    ],
    "RequiredRoles": []
  }
}
```

##### **2. appsettings.QA.json**

**Ubicación:** `Chetango.Api/appsettings.QA.json`

```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://*.[NUEVO-DOMINIO].app",
      "https://corporacionchetango.[NUEVO-DOMINIO].app"
    ]
  },
  "JwtSettings": {
    "Issuer": "https://api.[NUEVO-DOMINIO].app",
    "Audience": "https://[NUEVO-DOMINIO].app"
  }
}
```

##### **3. Program.cs (Si tienes configuración de CORS adicional)**

**Ubicación:** `Chetango.Api/Program.cs`

**Buscar sección de CORS y actualizar:**

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "https://*.[NUEVO-DOMINIO].app",  // Nuevo
            "https://corporacionchetango.[NUEVO-DOMINIO].app",  // Nuevo
            "http://localhost:5173",
            "https://localhost:5173"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
```

##### **4. Middleware de Tenant (Si existe)**

**Ubicación:** `Chetango.Api/Infrastructure/Middleware/TenantMiddleware.cs`

**Actualizar lógica de extracción de subdomain:**

```csharp
public async Task InvokeAsync(HttpContext context, RequestDelegate next)
{
    var host = context.Request.Host.Host;
    
    // Extraer subdomain de: academia.[NUEVO-DOMINIO].app
    var domainBase = "[NUEVO-DOMINIO].app"; // Desde configuración
    
    if (host.EndsWith(domainBase) && host != domainBase)
    {
        var subdomain = host.Replace($".{domainBase}", "");
        
        // Buscar tenant por subdomain
        var tenant = await _tenantService.GetBySubdomainAsync(subdomain);
        
        if (tenant != null)
        {
            context.Items["TenantId"] = tenant.Id;
            context.Items["Tenant"] = tenant;
        }
    }
    
    await next(context);
}
```

---

#### **FASE 3: Actualización Microsoft Entra ID (Día 4)**

**Objetivo:** Agregar nuevas URLs de redirección para autenticación

**Pasos en Azure Portal:**

1. **Ir a Microsoft Entra ID (Azure AD)**
   ```
   Portal Azure → Microsoft Entra ID → App registrations
   → [Tu App Registration]
   ```

2. **Actualizar Redirect URIs**
   ```
   Authentication → Platform configurations → Web
   
   AGREGAR (sin eliminar las actuales):
   ✅ https://corporacionchetango.[NUEVO-DOMINIO].app/signin-oidc
   ✅ https://corporacionchetango.[NUEVO-DOMINIO].app
   ✅ https://*.[NUEVO-DOMINIO].app/signin-oidc
   ✅ https://*.[NUEVO-DOMINIO].app
   
   MANTENER:
   ⚠️ https://app.corporacionchetango.com/signin-oidc
   ⚠️ https://app.corporacionchetango.com
   (Por si necesitas rollback)
   ```

3. **Actualizar Logout URLs**
   ```
   Front-channel logout URL:
   https://corporacionchetango.[NUEVO-DOMINIO].app/signout-oidc
   ```

4. **Guardar cambios**

---

#### **FASE 4: Actualización Frontend (Día 4)**

##### **1. Variables de Entorno**

**Archivo:** `chetango-frontend/.env` o `.env.production`

```env
# API Base URL
VITE_API_URL=https://api.[NUEVO-DOMINIO].app

# Domain Base para detección de tenant
VITE_DOMAIN_BASE=[NUEVO-DOMINIO].app

# Microsoft Auth
VITE_AZURE_AD_CLIENT_ID=<client-id>
VITE_AZURE_AD_TENANT_ID=<tenant-id>
VITE_AZURE_AD_REDIRECT_URI=https://corporacionchetango.[NUEVO-DOMINIO].app
```

##### **2. TenantContext.tsx (Si tienes detección de tenant)**

**Ubicación:** `src/contexts/TenantContext.tsx`

```typescript
export function TenantProvider({ children }: { children: ReactNode }) {
  const [tenant, setTenant] = useState<TenantBranding | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function loadTenant() {
      try {
        const hostname = window.location.hostname;
        const domainBase = import.meta.env.VITE_DOMAIN_BASE || '[NUEVO-DOMINIO].app';
        
        // Detectar subdomain
        let subdomain = 'corporacionchetango'; // Default
        
        if (hostname === 'localhost' || hostname.startsWith('127.') || hostname.startsWith('192.')) {
          // Desarrollo: usar query param o default
          const urlParams = new URLSearchParams(window.location.search);
          subdomain = urlParams.get('tenant') || 'corporacionchetango';
        } else if (hostname.endsWith(domainBase)) {
          // Producción: extraer subdomain
          subdomain = hostname.replace(`.${domainBase}`, '');
        }

        // Cargar info del tenant desde API
        const response = await fetch(
          `${import.meta.env.VITE_API_URL}/api/tenants/by-subdomain/${subdomain}`
        );

        if (!response.ok) {
          throw new Error('Tenant no encontrado');
        }

        const data = await response.json();
        setTenant(data);
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
    // CSS Variables
    document.documentElement.style.setProperty('--color-primary', tenant.colorPrimario);
    document.documentElement.style.setProperty('--color-secondary', tenant.colorSecundario);
    document.documentElement.style.setProperty('--color-accent', tenant.colorAccent);

    // Favicon
    if (tenant.faviconUrl) {
      const favicon = document.querySelector("link[rel*='icon']") as HTMLLinkElement;
      if (favicon) {
        favicon.href = tenant.faviconUrl;
      }
    }

    // Título
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
```

##### **3. Actualizar Llamadas API**

**Verificar que todas las llamadas API usen la variable de entorno:**

```typescript
// ✅ CORRECTO
const response = await fetch(`${import.meta.env.VITE_API_URL}/api/usuarios`);

// ❌ INCORRECTO (hardcodeado)
const response = await fetch('https://app.corporacionchetango.com/api/usuarios');
```

---

## 4. CONFIGURACIÓN AZURE

### 4.1 Azure App Service Settings

**Ir a:** Azure Portal → App Services → chetango-api → Configuration

**Application Settings a Agregar/Actualizar:**

| Key | Value | Descripción |
|-----|-------|-------------|
| `JwtSettings__Issuer` | `https://api.[NUEVO-DOMINIO].app` | Emisor JWT |
| `JwtSettings__Audience` | `https://[NUEVO-DOMINIO].app` | Audiencia JWT |
| `JwtSettings__AllowedDomains` | `*.[NUEVO-DOMINIO].app` | Dominios permitidos |
| `TenantSettings__DomainBase` | `[NUEVO-DOMINIO].app` | Base del dominio |
| `WEBSITE_DNS_SERVER` | `168.63.129.16` | DNS Server Azure |

### 4.2 Azure Static Web App (Frontend)

**Ir a:** Azure Portal → Static Web Apps → chetango-frontend → Custom domains

**Agregar dominio:**
```
1. Click "Add"
2. Domain type: "Other"
3. Domain name: corporacionchetango.[NUEVO-DOMINIO].app
4. Validation type: TXT record (o CNAME)
5. Agregar registro en Azure DNS
6. Validar
```

### 4.3 SSL Certificates

**Azure App Service:**
```powershell
# SSL automático para wildcard
az webapp config ssl create `
  --resource-group chetango-rg `
  --name chetango-api `
  --hostname "*.[NUEVO-DOMINIO].app"

# SSL para dominio específico
az webapp config ssl create `
  --resource-group chetango-rg `
  --name chetango-api `
  --hostname "corporacionchetango.[NUEVO-DOMINIO].app"
```

**Azure Static Web App:**
- SSL se configura automáticamente al agregar custom domain

---

## 5. ACTUALIZACIÓN DE CÓDIGO

### 5.1 Cambios en Backend

**Resumen de archivos:**

```
Chetango.Api/
├── appsettings.json                        → Actualizar CORS, JWT
├── appsettings.QA.json                     → Actualizar CORS, JWT
├── Program.cs                              → Verificar CORS
└── Infrastructure/
    └── Middleware/
        └── TenantMiddleware.cs             → Actualizar extracción subdomain
```

### 5.2 Cambios en Frontend

**Resumen de archivos:**

```
chetango-frontend/
├── .env                                    → Actualizar API_URL, DOMAIN_BASE
├── .env.production                         → Actualizar variables producción
└── src/
    ├── contexts/
    │   └── TenantContext.tsx               → Actualizar detección subdomain
    └── config/
        └── api.ts                          → Verificar baseURL
```

### 5.3 Base de Datos

**NO requiere cambios** si ya tienes implementado multi-tenancy.

**Si aún NO tienes tabla Tenants:**
- Revisar documento `PLAN-ESCALAMIENTO-SAAS.md` sección 3.2
- Ejecutar migration para agregar tabla Tenants y columna TenantId

---

## 6. TESTING Y VALIDACIÓN

### 6.1 Testing Local (Día 5)

**Simular nuevo dominio en local:**

**Opción 1: Query Parameter**
```
http://localhost:5173?tenant=corporacionchetango
http://localhost:5173?tenant=otraacademia
```

**Opción 2: Modificar archivo hosts**
```
Windows: C:\Windows\System32\drivers\etc\hosts
Mac/Linux: /etc/hosts

Agregar línea:
127.0.0.1  corporacionchetango.[NUEVO-DOMINIO].local
127.0.0.1  test.[NUEVO-DOMINIO].local
```

Luego acceder a: `http://corporacionchetango.[NUEVO-DOMINIO].local:5173`

### 6.2 Testing en Azure (Día 5)

**Checklist de pruebas:**

#### **Prueba 1: DNS Resuelve Correctamente**
```powershell
# Verificar que DNS funciona
nslookup corporacionchetango.[NUEVO-DOMINIO].app

# Debe retornar la IP del App Service
```

#### **Prueba 2: SSL Funciona**
```
Abrir navegador en modo incógnito:
https://corporacionchetango.[NUEVO-DOMINIO].app

✅ Verificar candado verde (SSL válido)
✅ Sin warnings de certificado
```

#### **Prueba 3: Login con Microsoft**
```
1. Ir a https://corporacionchetango.[NUEVO-DOMINIO].app
2. Click "Iniciar sesión con Microsoft"
3. Ingresar credenciales
4. ✅ Debe redirigir correctamente
5. ✅ Dashboard debe cargar
```

#### **Prueba 4: Middleware de Tenant**
```
1. Acceder a https://corporacionchetango.[NUEVO-DOMINIO].app
2. Abrir DevTools → Network
3. Hacer request a API
4. ✅ Verificar que headers incluyen TenantId correcto
```

#### **Prueba 5: Branding Dinámico (Si aplica)**
```
1. Tenant con logo configurado
2. ✅ Logo debe aparecer
3. ✅ Colores deben aplicarse
4. ✅ Título de página debe ser correcto
```

#### **Prueba 6: CORS**
```
1. Desde frontend hacer llamada a API
2. ✅ No debe haber errores CORS en consola
3. ✅ Requests deben completarse exitosamente
```

#### **Prueba 7: Crear Tenant de Prueba**
```sql
-- Crear tenant de prueba
INSERT INTO Tenants (
    Id, Nombre, Subdomain, Plan, Estado,
    MaxSedes, MaxAlumnos, MaxProfesores, MaxStorageMB,
    EmailContacto, FechaRegistro
) VALUES (
    NEWID(),
    'Academia de Prueba',
    'prueba',
    'Basico',
    'Activo',
    1, 100, 5, 10240,
    'prueba@test.com',
    GETDATE()
);
```

Luego acceder a: `https://prueba.[NUEVO-DOMINIO].app`

### 6.3 Checklist de Validación Completo

**Antes de dar credenciales a usuarios:**

- [ ] DNS resuelve correctamente
- [ ] SSL válido (candado verde)
- [ ] Login con Microsoft funciona
- [ ] Dashboard carga correctamente
- [ ] API responde sin errores CORS
- [ ] Tenant middleware funciona
- [ ] Branding dinámico aplica (si configurado)
- [ ] Puedo crear usuarios
- [ ] Puedo registrar asistencias
- [ ] Puedo registrar pagos
- [ ] Reportes funcionan
- [ ] No hay errores en Application Insights
- [ ] Performance es aceptable (<2s load time)

---

## 7. COMUNICACIÓN A USUARIOS

### 7.1 Email de Bienvenida (Primera vez)

**Asunto:** Acceso a tu Sistema de Gestión - Chetango

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
            <h2>Hola {{NombreUsuario}},</h2>
            
            <p>Tu cuenta está lista. Ya puedes acceder al sistema de gestión.</p>
            
            <p><strong>URL de acceso:</strong></p>
            <p style="text-align: center; font-size: 18px; background: #f5f5f5; padding: 15px; border-radius: 5px;">
                <strong>https://corporacionchetango.[NUEVO-DOMINIO].app</strong>
            </p>
            
            <p><strong>Cómo ingresar:</strong></p>
            <ol>
                <li>Click en el botón de abajo</li>
                <li>Selecciona "Iniciar sesión con Microsoft"</li>
                <li>Usa tu email: <strong>{{Email}}</strong></li>
                <li>Ingresa tu contraseña de Microsoft</li>
            </ol>
            
            <p style="text-align: center; margin: 30px 0;">
                <a href="https://corporacionchetango.[NUEVO-DOMINIO].app" class="button">
                    Acceder al Sistema
                </a>
            </p>
            
            <p><strong>Tip:</strong> Guarda esta URL en tus favoritos para acceso rápido.</p>
            
            <p>¿Problemas para ingresar? Responde este email o escríbenos a 
               <a href="mailto:soporte@corporacionchetango.com">soporte@corporacionchetango.com</a></p>
            
            <p>¡Estamos para ayudarte!</p>
            
            <p>Saludos,<br>
            Equipo Chetango</p>
        </div>
        
        <div class="footer">
            <p>&copy; 2026 Corporación Chetango - Gestión Inteligente para Academias</p>
        </div>
    </div>
</body>
</html>
```

### 7.2 Variables para Personalizar Email

```json
{
  "NombreUsuario": "Carlos",
  "Email": "carlos@corporacionchetango.com",
  "NUEVO-DOMINIO": "och8"  // O el que elijas
}
```

### 7.3 Comunicación Interna (Profesores/Admins)

**WhatsApp/Mensaje Directo:**

```
🎉 ¡Nueva URL del Sistema!

A partir de hoy, ingresa desde:
https://corporacionchetango.[NUEVO-DOMINIO].app

📌 Guarda este link en tus favoritos

Usuario: tu-email@corporacionchetango.com
Contraseña: Tu contraseña de Microsoft

¿Dudas? Escríbeme por acá 💬
```

---

## 8. CHECKLIST COMPLETO

### 8.1 Pre-Migración

**Decisiones:**
- [ ] Dominio elegido: _________________
- [ ] Verificada disponibilidad
- [ ] Presupuesto aprobado ($15 USD/año)

**Compra:**
- [ ] Dominio comprado en Namecheap/GoDaddy
- [ ] Nameservers obtenidos del proveedor

### 8.2 Configuración Infraestructura

**Azure DNS:**
- [ ] DNS Zone creada en Azure
- [ ] Nameservers configurados en proveedor
- [ ] Propagación DNS completada (verificar con nslookup)
- [ ] Wildcard record `*.[DOMINIO].app` creado
- [ ] Record `@` para dominio raíz creado

**Azure App Service:**
- [ ] Custom domain `*.[DOMINIO].app` agregado
- [ ] Custom domain `corporacionchetango.[DOMINIO].app` agregado
- [ ] SSL certificate configurado para wildcard
- [ ] SSL certificate configurado para corporacionchetango
- [ ] Application Settings actualizados

**Azure Static Web App:**
- [ ] Custom domain agregado
- [ ] Validación DNS completada
- [ ] SSL automático activado

### 8.3 Actualización Código

**Backend:**
- [ ] `appsettings.json` actualizado
- [ ] `appsettings.QA.json` actualizado
- [ ] `Program.cs` CORS verificado
- [ ] Middleware de Tenant actualizado (si aplica)
- [ ] Variables de entorno en Azure actualizadas

**Frontend:**
- [ ] `.env` actualizado
- [ ] `.env.production` actualizado
- [ ] `TenantContext.tsx` actualizado (si aplica)
- [ ] Todas las llamadas API usan variables de entorno
- [ ] Build de producción generado

**Microsoft Entra ID:**
- [ ] Redirect URIs agregadas
- [ ] Logout URL actualizada
- [ ] Cambios guardados

### 8.4 Deploy

**Backend:**
- [ ] Código commiteado a Git
- [ ] Push a rama principal
- [ ] GitHub Actions/Azure DevOps ejecutado
- [ ] Deploy completado sin errores
- [ ] Health check endpoint responde

**Frontend:**
- [ ] Build generado (`npm run build`)
- [ ] Deploy a Azure Static Web App
- [ ] Deploy completado sin errores

### 8.5 Testing

- [ ] DNS resuelve: `nslookup corporacionchetango.[DOMINIO].app`
- [ ] HTTPS funciona sin warnings
- [ ] Login con Microsoft exitoso
- [ ] Dashboard carga correctamente
- [ ] API responde sin errores CORS
- [ ] Tenant detection funciona
- [ ] Branding dinámico funciona (si configurado)
- [ ] Operaciones CRUD funcionan (crear, leer, actualizar, eliminar)
- [ ] No hay errores en Application Insights
- [ ] Performance aceptable (<2s)

### 8.6 Comunicación

- [ ] Email de bienvenida preparado
- [ ] Variables personalizadas en email
- [ ] Lista de 50 usuarios preparada
- [ ] Emails enviados
- [ ] Soporte disponible para dudas

### 8.7 Post-Deploy

- [ ] Monitorear Application Insights por 24h
- [ ] Verificar que usuarios pueden acceder
- [ ] Resolver tickets de soporte (si los hay)
- [ ] Documentar cualquier issue encontrado

---

## 9. ROLLBACK PLAN

### 9.1 Cuándo Hacer Rollback

**Situaciones que requieren rollback:**
- Error rate >10% en Application Insights
- Usuarios no pueden hacer login
- API no responde o tiene errores críticos
- DNS no resuelve después de 4 horas

### 9.2 Procedimiento de Rollback

#### **Opción 1: Revertir Código (Si el problema es en código)**

```powershell
# 1. Identificar último commit funcional
git log --oneline

# 2. Revertir a commit anterior
git revert HEAD
git push origin main

# 3. GitHub Actions/Azure DevOps redesplegarán automáticamente
# Tiempo: 3-5 minutos
```

#### **Opción 2: Mantener Ambas URLs (Si DNS es el problema)**

**Temporalmente, ambas URLs funcionarán:**
- `app.corporacionchetango.com` (antigua - funciona)
- `corporacionchetango.[NUEVO-DOMINIO].app` (nueva - posible problema)

Los usuarios pueden seguir usando la antigua mientras se resuelve.

#### **Opción 3: Revertir Configuración Azure**

```powershell
# Revertir Application Settings
az webapp config appsettings set `
  --resource-group chetango-rg `
  --name chetango-api `
  --settings `
    JwtSettings__Issuer="https://api.corporacionchetango.com" `
    JwtSettings__Audience="https://corporacionchetango.com" `
    JwtSettings__AllowedDomains="*.corporacionchetango.com"
```

### 9.3 Comunicación Durante Rollback

**Email/WhatsApp a usuarios:**

```
⚠️ Actualización Temporal

Hola equipo,

Estamos experimentando problemas técnicos con la nueva URL.

👉 Mientras lo resolvemos, usa la URL anterior:
https://app.corporacionchetango.com

Volveremos a la normalidad pronto.
Disculpa las molestias.

Equipo Técnico
```

### 9.4 Post-Mortem

**Después de resolver el problema:**

```markdown
# Post-Mortem: Migración de Dominio

## Fecha del Incidente
[Fecha]

## Duración
[X horas]

## Impacto
- Usuarios afectados: [número]
- Funcionalidades afectadas: [lista]

## Causa Raíz
[Descripción del problema]

## Solución Aplicada
[Pasos para resolver]

## Prevención Futura
- [ ] Acción 1
- [ ] Acción 2

## Lecciones Aprendidas
- Lección 1
- Lección 2
```

---

## 10. RECURSOS ADICIONALES

### 10.1 Documentos Relacionados

- `PLAN-ESCALAMIENTO-SAAS.md` - Plan general de SaaS
- `DEPLOYMENT-STRATEGY.md` - Estrategia de deployment

### 10.2 Contactos de Soporte

| Proveedor | Contacto | Para |
|-----------|----------|------|
| **Namecheap** | support@namecheap.com | Problemas con dominio |
| **GoDaddy** | +1-480-505-8877 | Problemas con dominio |
| **Azure Support** | Portal Azure → Support | Problemas infraestructura |
| **Microsoft** | Azure AD Support | Problemas autenticación |

### 10.3 Herramientas Útiles

**Verificar DNS:**
```
https://dnschecker.org
https://www.whatsmydns.net
```

**Verificar SSL:**
```
https://www.ssllabs.com/ssltest/
```

**Testing de URLs:**
```
https://httpstatus.io
```

---

## 11. PREGUNTAS FRECUENTES (FAQ)

### ¿Perderé datos durante la migración?
**No.** La base de datos NO cambia. Solo cambia la URL de acceso.

### ¿Los usuarios existentes perderán acceso?
**No.** Sus cuentas siguen funcionando. Solo cambia la URL donde ingresan.

### ¿Cuánto tiempo tomará la migración?
**5-7 días** en total, pero sin downtime. La app seguirá funcionando.

### ¿Qué pasa con el dominio viejo?
**Se mantiene** por 30 días como backup. Después podemos configurar redirect automático.

### ¿Puedo usar ambas URLs al mismo tiempo?
**Sí**, durante el período de transición (30 días).

### ¿Qué pasa si un usuario entra por la URL vieja?
Seguirá funcionando. Opcionalmente, podemos agregar un banner avisando del cambio.

### ¿Cuánto cuesta?
**$15 USD/año** (~$63,000 COP/año) por el dominio nuevo.

### ¿Necesito comprar certificado SSL?
**No.** Azure lo proporciona gratis.

---

## 12. NOTAS FINALES

### 12.1 Ventajas de Esta Migración

✅ **Sin downtime** - La app sigue funcionando durante toda la migración  
✅ **Sin pérdida de datos** - Base de datos no se toca  
✅ **Reversible** - Puedes hacer rollback si algo falla  
✅ **Timing perfecto** - Usuarios no han accedido aún  
✅ **Preparado para escalar** - Infraestructura lista para multi-tenant  

### 12.2 Próximos Pasos Después de Migración

1. **Semana 1:** Monitorear uso y resolver dudas
2. **Semana 2-4:** Implementar tabla Tenants si no existe
3. **Mes 2:** Preparar onboarding para nuevos clientes
4. **Mes 3:** Lanzar beta con primeros 5 clientes

### 12.3 Actualización del Documento

**Este documento se actualizará con:**
- El dominio final elegido
- Screenshots de configuraciones
- Problemas encontrados y soluciones
- Métricas de éxito

---

## 📝 REGISTRO DE CAMBIOS

| Fecha | Versión | Cambios |
|-------|---------|---------|
| 2026-02-25 | 1.0 | Documento inicial creado |
| __________ | 1.1 | Dominio elegido: ________ |
| __________ | 1.2 | Migración completada |

---

## ✅ FIRMA DE APROBACIÓN

**Preparado por:** _________________________  
**Revisado por:** _________________________  
**Aprobado por:** _________________________  
**Fecha de ejecución:** _________________________

---

**FIN DEL DOCUMENTO**

*Para cualquier duda o aclaración sobre este proceso, consulta con el equipo técnico.*
