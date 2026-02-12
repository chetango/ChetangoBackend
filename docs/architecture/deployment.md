# Estrategia de Deployment - Chetango API

## 🏗️ Arquitectura de Ambientes

### **Ambiente LOCAL (Desarrollo)**
- **Base de Datos:** `ChetangoDB_Dev` (SQL Server LocalDB)
- **API:** `localhost:5194` (HTTPS QA profile)
- **Frontend:** `localhost:5173`
- **Autenticación:** Azure AD B2C (usuarios `@chetangoprueba.onmicrosoft.com`)

### **Ambiente PRODUCCIÓN**
- **Base de Datos:** `chetango-db-prod` (Azure SQL Database)
- **API:** `chetango-api-prod.azurewebsites.net`
- **Frontend:** Azure Static Web App
- **Autenticación:** Azure AD B2C (usuarios con correos personalizados)

---

## 🔐 Autenticación

Ambos ambientes usan el **mismo tenant de Azure AD B2C:**
```
Tenant ID: 8a57ec5a-e2e3-44ad-9494-77fbc7467251
Instance: https://8a57ec5a-e2e3-44ad-9494-77fbc7467251.ciamlogin.com/
```

**Usuarios de Prueba (Local):**
- `Chetango@chetangoprueba.onmicrosoft.com`
- `admin@chetangoprueba.onmicrosoft.com`
- `profesor@chetangoprueba.onmicrosoft.com`

**Usuarios Reales (Producción):**
- `usuario@dominio.com` (correos personalizados)

**Separación:** Los usuarios se registran en diferentes bases de datos:
- Local → `ChetangoDB_Dev`
- Producción → `chetango-db-prod`

---

## 🚀 Flujo de Deployment (GitFlow)

### **Estrategia de Ramas:**

```
feature/* → develop → main → PRODUCCIÓN (auto-deploy)
```

### **1. Desarrollo de Features**

```bash
# Crear rama feature
git checkout develop
git pull origin develop
git checkout -b feature/nombre-funcionalidad

# Desarrollar localmente
dotnet run --project Chetango.Api/Chetango.Api.csproj --launch-profile https-qa

# Probar contra BD local (ChetangoDB_Dev)
# Autenticar con usuarios @chetangoprueba.onmicrosoft.com
```

### **2. Pull Request a Develop**

```bash
# Commit y push
git add .
git commit -m "feat: descripción de la funcionalidad"
git push origin feature/nombre-funcionalidad

# En GitHub: crear Pull Request a develop
# Revisar código, aprobar, merge
```

**⚠️ Importante:** Hacer merge a `develop` **NO despliega automáticamente**.

### **3. Promoción a Producción**

```bash
# Cuando estés 100% seguro
git checkout main
git pull origin main
git merge develop
git push origin main
```

**✅ Resultado:** GitHub Actions automáticamente:
1. Compila el proyecto (.NET 9)
2. Ejecuta tests (si existen)
3. Publica a `chetango-api-prod`
4. Deployment completo en ~2-3 minutos

---

## 📝 CI/CD Configuración

### **GitHub Actions Workflows**

#### `azure-deploy-production.yml`
- **Trigger:** Push a `main`
- **Destino:** `chetango-api-prod` (Producción)
- **Autenticación:** `secrets.AZURE_WEBAPP_PUBLISH_PROFILE`

#### `cd.yml`
- **Triggers:** Push a `main` o manual dispatch
- **Estado:** Placeholders (TODOs, no despliega realmente)
- **Propósito:** Documentar futuros pipelines Dev/QA

---

## 🧪 Estrategia de Testing

### **Local (Desarrollo)**

1. **Modificar código** en Visual Studio Code
2. **Ejecutar API local:**
   ```bash
   dotnet run --project Chetango.Api/Chetango.Api.csproj --launch-profile https-qa
   ```
3. **Frontend local** conecta a API local
4. **Autenticarse** con usuarios de prueba
5. **BD local** contiene datos de testing

### **Pre-Deployment**

Antes de hacer merge a `main`:
- ✅ Verificar que API local funciona sin errores
- ✅ Probar endpoints críticos
- ✅ Revisar logs locales
- ✅ Confirmar que migraciones EF Core están sincronizadas

### **Post-Deployment**

Después de desplegar a producción:
- ✅ Verificar endpoints críticos: `/api/tipos-paquete`, `/api/reportes/dashboard`
- ✅ Revisar logs en Azure Portal
- ✅ Confirmar que usuarios reales pueden autenticarse
- ✅ Monitorear Application Insights (si está configurado)

---

## 🔄 Manejo de Migraciones EF Core

### **Proceso Recomendado:**

1. **Crear migración localmente:**
   ```bash
   dotnet ef migrations add NombreMigracion --project Chetango.Infrastructure
   ```

2. **Aplicar a BD local:**
   ```bash
   dotnet ef database update --project Chetango.Api
   ```

3. **Probar con BD local** hasta confirmar que funciona

4. **Commit migración:**
   ```bash
   git add Chetango.Infrastructure/Persistence/Migrations/
   git commit -m "feat: agregar migración NombreMigracion"
   ```

5. **Deployment automático:** Al hacer merge a `main`, EF Core aplica migraciones automáticamente en Azure

### **⚠️ Advertencia: Cambios Manuales en BD**

- ❌ Evitar ejecutar SQL manual en producción
- ✅ Si es necesario (emergencias), documentar cambios en `scripts/`
- ✅ Sincronizar cambios manuales con migraciones EF Core posteriormente

---

## 🛡️ Rollback en Caso de Errores

### **Opción 1: Git Revert (Recomendada)**

```bash
# Revertir último commit en main
git revert HEAD
git push origin main

# GitHub Actions desplegará versión anterior automáticamente
```

### **Opción 2: Azure Portal (Emergencia)**

1. Azure Portal → App Service → Deployment Center
2. Seleccionar commit anterior
3. Re-deploy manual

### **Opción 3: Rollback de Migración EF Core**

```bash
# Revertir última migración
dotnet ef migrations remove --project Chetango.Infrastructure

# O revertir BD a migración específica
dotnet ef database update NombreMigracionAnterior --project Chetango.Api
```

---

## 📊 Monitoreo y Logs

### **Logs de Deployment**

- GitHub Actions: Ver logs en pestaña "Actions" del repositorio
- Azure App Service: Portal → Log Stream

### **Logs de Aplicación**

- Azure Portal → App Service → Logs
- Application Insights (si configurado)

---

## 🎯 Checklist de Deployment

### **Antes de Merge a Main:**

- [ ] Código funciona localmente sin errores
- [ ] Migraciones EF Core probadas en local
- [ ] Tests pasan (si existen)
- [ ] Código revisado en Pull Request
- [ ] Documentación actualizada (si aplica)

### **Después de Merge a Main:**

- [ ] GitHub Actions workflow completó exitosamente
- [ ] API producción responde correctamente
- [ ] Endpoints críticos funcionan
- [ ] Usuarios reales pueden autenticarse
- [ ] No hay errores en logs de Azure

---

## 🆘 Contacto y Soporte

Si tienes problemas con deployment:
1. Revisar logs de GitHub Actions
2. Revisar logs de Azure App Service
3. Verificar configuración de `appsettings.json` en Azure
4. Confirmar que connection string de producción es correcta

---

**Última actualización:** 2026-02-12
**Autor:** Equipo Chetango
