# 🏢 Testing Multi-Sede en Desarrollo

## 📋 Resumen

Este documento describe cómo probar la funcionalidad multi-sede (Medellín y Manizales) en el ambiente de desarrollo.

## ✅ Estado Actual

### Backend Implementado
- ✅ Enum `Sede` creado (Medellin=1, Manizales=2)
- ✅ Campo `Sede` agregado a entidades: `Usuario`, `Pago`, `LiquidacionMensual`
- ✅ Migración ejecutada: `20260218150247_AgregarCampoSede`
- ✅ Herencia automática de sede desde usuario logueado
- ✅ Dashboard con métricas separadas por sede

### Base de Datos Actualizada
- ✅ Nuevo admin creado: `chetango.manizales@chetangoprueba.onmicrosoft.com`
- ✅ Script de seed actualizado: `scripts/seed_usuarios_prueba_ciam.sql`
- ✅ Script de limpieza actualizado: `scripts/limpieza-base-datos.sql`

## 👥 Usuarios de Prueba para Multi-Sede

| Usuario | Email | Contraseña | Sede | IdUsuario | Estado |
|---------|-------|------------|------|-----------|--------|
| Admin Medellín | Chetango@chetangoprueba.onmicrosoft.com | Chet4ngo20# | Medellín (1) | b91e51b9-4094-441e-a5b6-062a846b3868 | ✅ En Azure AD + BD |
| Admin Manizales | chetango.manizales@chetangoprueba.onmicrosoft.com | Maniz4les20# | Manizales (2) | c91e51b9-4094-441e-a5b6-062a846b3869 | ⚠️ Solo en BD |
| Profesor | Jorgepadilla@chetangoprueba.onmicrosoft.com | Jorge2026 | Medellín (1) | 8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB | ✅ En Azure AD + BD |
| Alumno | JuanDavid@chetangoprueba.onmicrosoft.com | Juaj0rge20# | Medellín (1) | 71462106-9863-4fd0-b13d-9878ed231aa6 | ✅ En Azure AD + BD |

## 🔧 Próximos Pasos

### 1. Crear Usuario en Azure AD B2C (Microsoft Entra External ID)

**⚠️ PENDIENTE:** El usuario `chetango.manizales@chetangoprueba.onmicrosoft.com` ya existe en la base de datos pero falta crearlo en Azure AD B2C.

**Pasos para crear:**

1. Ve al portal de Azure: https://portal.azure.com
2. Busca tu tenant: **chetangoprueba.onmicrosoft.com**
3. Ve a **Microsoft Entra ID** → **Users**
4. Click en **+ New user** → **Create new user**
5. Completa:
   - **User principal name:** `chetango.manizales@chetangoprueba.onmicrosoft.com`
   - **Display name:** `Administrador Chetango Manizales`
   - **Password:** `Maniz4les20#`
   - **Auto-generate password:** NO (desmarcado)
   - **Force password change on first login:** NO (desmarcado)
6. En la pestaña **Properties:**
   - **Job title:** Administrador
   - **Usage location:** Colombia
7. (Opcional) En la pestaña **Assignments**, agregar a grupos si es necesario
8. Click **Review + Create**

### 2. Asignar Claims/Roles (si tu app usa claims personalizados)

Si tu aplicación verifica roles en el token JWT, deberás configurar claims personalizados:

1. En Azure AD B2C, ve a **App registrations** → tu aplicación
2. Ve a **Token configuration**
3. Agrega el claim `admin` o el rol que uses en tu backend
4. Asegúrate de que el scope `openid profile email` esté configurado

### 3. Verificar Configuración

Después de crear el usuario en Azure AD, verifica:

```sql
-- Verificar que ambos admins existen en BD
SELECT IdUsuario, NombreUsuario, Correo, Sede 
FROM Usuarios 
WHERE Correo LIKE '%chetango%@chetangoprueba%'
ORDER BY Sede;

-- Resultado esperado:
-- IdUsuario: b91e51b9-4094-441e-a5b6-062a846b3868 | Admin Medellin | Chetango@... | 1
-- IdUsuario: c91e51b9-4094-441e-a5b6-062a846b3869 | Admin Manizales | chetango.manizales@... | 2
```

## 🧪 Plan de Pruebas

### Test 1: Herencia Automática de Sede en Pagos

**Objetivo:** Verificar que los pagos heredan la sede del admin que los crea.

**Pasos:**

1. **Login como Admin Medellín** (`Chetango@chetangoprueba.onmicrosoft.com`)
2. Crear un pago para cualquier alumno
3. Verificar en BD:
   ```sql
   SELECT TOP 1 NumeroPago, MontoTotal, Sede 
   FROM Pagos 
   ORDER BY FechaCreacion DESC;
   -- Esperado: Sede = 1 (Medellín)
   ```

4. **Login como Admin Manizales** (`chetango.manizales@chetangoprueba.onmicrosoft.com`)
5. Crear un pago para cualquier alumno
6. Verificar en BD:
   ```sql
   SELECT TOP 1 NumeroPago, MontoTotal, Sede 
   FROM Pagos 
   ORDER BY FechaCreacion DESC;
   -- Esperado: Sede = 2 (Manizales)
   ```

### Test 2: Herencia de Sede al Crear Usuarios

**Objetivo:** Verificar que los nuevos usuarios heredan la sede del admin que los crea.

**Pasos:**

1. **Login como Admin Medellín**
2. Crear un nuevo alumno: `alumno.medellin.test@correo.com`
3. Verificar en BD:
   ```sql
   SELECT NombreUsuario, Correo, Sede 
   FROM Usuarios 
   WHERE Correo = 'alumno.medellin.test@correo.com';
   -- Esperado: Sede = 1
   ```

4. **Login como Admin Manizales**
5. Crear un nuevo alumno: `alumno.manizales.test@correo.com`
6. Verificar en BD:
   ```sql
   SELECT NombreUsuario, Correo, Sede 
   FROM Usuarios 
   WHERE Correo = 'alumno.manizales.test@correo.com';
   -- Esperado: Sede = 2
   ```

### Test 3: Dashboard con Métricas Separadas

**Objetivo:** Verificar que el dashboard muestra métricas separadas por sede.

**Pasos:**

1. Crear algunos pagos de prueba para cada sede usando los admins correspondientes
2. **Login como Admin Medellín**
3. Llamar al endpoint: `GET /api/reportes/dashboard`
4. Verificar respuesta JSON:
   ```json
   {
     "ingresosMedellinEsteMes": 1500000,
     "ingresosManizalesEsteMes": 0,
     "egresosMedellinEsteMes": 500000,
     "egresosManizalesEsteMes": 0,
     ...
   }
   ```

5. **Login como Admin Manizales**
6. Crear algunos pagos y liquidaciones
7. Llamar nuevamente al endpoint
8. Verificar que ahora `ingresosManizalesEsteMes` y `egresosManizalesEsteMes` tienen valores

### Test 4: Liquidación de Nómina

**Objetivo:** Verificar que las liquidaciones heredan la sede del admin.

**Pasos:**

1. **Login como Admin Medellín**
2. Liquidar mes actual: `POST /api/nomina/liquidar-mes`
3. Verificar en BD:
   ```sql
   SELECT TOP 1 Mes, Anio, MontoTotal, Sede 
   FROM LiquidacionesMensuales 
   ORDER BY FechaCreacion DESC;
   -- Esperado: Sede = 1
   ```

4. **Login como Admin Manizales**
5. Liquidar mes actual
6. Verificar en BD:
   ```sql
   SELECT TOP 1 Mes, Anio, MontoTotal, Sede 
   FROM LiquidacionesMensuales 
   ORDER BY FechaCreacion DESC;
   -- Esperado: Sede = 2
   ```

## 📝 Notas Importantes

### Comportamiento por Defecto

- **Valor por defecto:** Si no se puede determinar la sede del usuario, se usa `Sede.Medellin (1)`
- **Prioridad:** `request.Sede ?? usuarioCreador.Sede ?? Sede.Medellin`
- **Todos los usuarios existentes:** Tienen `Sede = 1` (Medellín) por la migración

### Consultas SQL Útiles

```sql
-- Ver distribución de usuarios por sede
SELECT Sede, COUNT(*) as Cantidad
FROM Usuarios
GROUP BY Sede;

-- Ver pagos por sede este mes
SELECT Sede, COUNT(*) as CantidadPagos, SUM(MontoTotal) as TotalIngresos
FROM Pagos
WHERE MONTH(FechaCreacion) = MONTH(GETDATE()) 
  AND YEAR(FechaCreacion) = YEAR(GETDATE())
GROUP BY Sede;

-- Ver liquidaciones por sede
SELECT Sede, Mes, Anio, MontoTotal
FROM LiquidacionesMensuales
ORDER BY Anio DESC, Mes DESC, Sede;

-- Actualizar sede de un usuario específico (si es necesario para pruebas)
UPDATE Usuarios 
SET Sede = 2 
WHERE Correo = 'ejemplo@correo.com';
```

## ⚠️ Recordatorios

1. **No olvides crear el usuario en Azure AD B2C** antes de intentar hacer login con `chetango.manizales@chetangoprueba.onmicrosoft.com`
2. Los scripts de seed están listos para re-ejecutarse de forma idempotente
3. El script de limpieza ahora preserva **4 usuarios** en lugar de 3
4. Para testing rápido, puedes cambiar manualmente la sede de usuarios existentes con UPDATE

## 🔗 Referencias

- **Documentación principal:** `docs/development/auth-setup.md`
- **Script de seed:** `scripts/seed_usuarios_prueba_ciam.sql`
- **Script de limpieza:** `scripts/limpieza-base-datos.sql`
- **Manual de administrador:** `MANUAL-ADMINISTRADOR.md` (sección de multi-sede)
- **Migración:** `Chetango.Infrastructure/Persistence/Migrations/20260218150247_AgregarCampoSede.cs`

---

**Última actualización:** 2025-02-18  
**Estado:** ✅ Backend implementado | ⚠️ Pendiente: Crear usuario en Azure AD
