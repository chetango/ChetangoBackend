# Scripts de Base de Datos

Esta carpeta contiene scripts SQL y PowerShell útiles para la configuración y mantenimiento de la base de datos Chetango.

## 📝 Scripts Principales

### `seed_usuarios_prueba_ciam.sql` ⭐ **REQUERIDO PARA DESARROLLO**
Crea los usuarios de prueba necesarios para autenticación con Microsoft Entra CIAM.

**Uso:**
```bash
# Opción 1: sqlcmd
sqlcmd -S "(localdb)\MSSQLLocalDB" -d ChetangoDB_Dev -i seed_usuarios_prueba_ciam.sql

# Opción 2: PowerShell
Invoke-Sqlcmd -ServerInstance "(localdb)\MSSQLLocalDB" -Database "ChetangoDB_Dev" -InputFile "seed_usuarios_prueba_ciam.sql"
```

**Crea:**
- Usuario Admin: `Chetango@chetangoprueba.onmicrosoft.com`
- Usuario Profesor: `Jorgepadilla@chetangoprueba.onmicrosoft.com`
- Usuario Alumno: `JuanDavid@chetangoprueba.onmicrosoft.com`
- 1 clase de prueba
- 1 paquete activo para el alumno
- 1 asistencia de ejemplo

**Características:**
- ✅ Idempotente (puede ejecutarse múltiples veces)
- ✅ Transaccional (todo o nada)
- ✅ Validaciones de integridad

---

### `seed_admin_asistencias.sql`
Script heredado para crear datos adicionales de asistencias.

### `seed_personas_roles.sql`
Script heredado para crear personas y roles.

### `seed_metodos_pago.sql`
Crea los catálogos de métodos de pago (Efectivo, Transferencia, Nequi, etc.).

### `seed_paquetes_catalogos.sql`
Crea los catálogos de tipos de paquete (4 clases, 8 clases, 12 clases, Mensual).

### `seed_reportes_datos_prueba.sql` ⭐ **NUEVO - MÓDULO REPORTES**
Crea datos de prueba realistas para el módulo de Reportes.

**Uso:**
```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -d ChetangoDB_Dev -i seed_reportes_datos_prueba.sql
```

**Crea:**
- 5 alumnos adicionales con diferentes perfiles
- 45 pagos distribuidos en los últimos 6 meses
- ~12 paquetes con estados variados (Activos, Vencidos, Por vencer, Agotados)
- ~40 clases de los últimos 3 meses con asistencias
- ~5 clases futuras (próximos 7 días) con pocos inscritos para alertas
- ~160 asistencias con diferentes tasas de asistencia por alumno

**Prerequisitos:**
- ✅ `seed_usuarios_prueba_ciam.sql` ejecutado
- ✅ `seed_metodos_pago.sql` ejecutado
- ✅ `seed_paquetes_catalogos.sql` ejecutado

**Características:**
- ✅ Idempotente (puede ejecutarse múltiples veces)
- ✅ Transaccional (todo o nada)
- ✅ Datos realistas para pruebas de reportes
- ✅ Incluye datos para todas las alertas del dashboard
- ✅ Distribución temporal correcta para gráficas de tendencias

### `update_tiposclase_tango.sql`
Actualiza los tipos de clase con información de Tango.

---

## 🔧 Scripts PowerShell de Utilidad

### `apply_migrations_with_connection.ps1`
Aplica migraciones de EF Core con una cadena de conexión específica.

### `check_connectionstrings.ps1`
Verifica las cadenas de conexión configuradas en appsettings.

### `check_db_details.ps1`
Muestra detalles de la base de datos.

### `check_dbs.ps1`
Lista todas las bases de datos disponibles en el servidor.

### `create_dbs_sqlclient.ps1`
Crea las bases de datos necesarias usando SqlClient.

### `fase0_setup.ps1`
Script de configuración inicial del proyecto.

### `fix_qa_clear_history.ps1`
Limpia el historial en el ambiente QA.

---

## 📚 Orden Recomendado para Nuevo Ambiente

1. **Levantar API** (ejecuta migraciones automáticamente):
   ```bash
   dotnet run --project ../Chetango.Api/Chetango.Api.csproj --launch-profile https-qa
   ```

2. **Crear catálogos base**:
   ```bash
   # Métodos de pago
   sqlcmd -S "(localdb)\MSSQLLocalDB" -d ChetangoDB_Dev -i seed_metodos_pago.sql
   
   # Tipos de paquete
   sqlcmd -S "(localdb)\MSSQLLocalDB" -d ChetangoDB_Dev -i seed_paquetes_catalogos.sql
   ```

3. **Crear usuarios de prueba**:
   ```bash
   sqlcmd -S "(localdb)\MSSQLLocalDB" -d ChetangoDB_Dev -i seed_usuarios_prueba_ciam.sql
   ```

4. **Crear datos para módulo Reportes** (OPCIONAL pero recomendado):
   ```bash
   sqlcmd -S "(localdb)\MSSQLLocalDB" -d ChetangoDB_Dev -i seed_reportes_datos_prueba.sql
   ```

5. **Verificar**: Probar login con cualquiera de los 3 usuarios en Postman o tu aplicación frontend.

---

## 🧪 Datos de Prueba Disponibles

### Usuarios CIAM (seed_usuarios_prueba_ciam.sql):
- **Admin**: `Chetango@chetangoprueba.onmicrosoft.com`
- **Profesor**: `Jorgepadilla@chetangoprueba.onmicrosoft.com` 
- **Alumno**: `JuanDavid@chetangoprueba.onmicrosoft.com`

### Datos Reportes (seed_reportes_datos_prueba.sql):
- 5 alumnos adicionales
- 45 pagos (últimos 6 meses)
- 12 paquetes (varios estados)
- 40+ clases con asistencias
- Datos para dashboard, gráficas y alertas

---

## ⚠️ Notas Importantes

- Los scripts están diseñados para **LocalDB** (`(localdb)\MSSQLLocalDB`)
- Si usas otro servidor SQL, ajusta la cadena de conexión
- La base de datos `ChetangoDB_Dev` debe existir antes de ejecutar los scripts (las migraciones la crean)
- Todos los usuarios del script `seed_usuarios_prueba_ciam.sql` YA EXISTEN en Microsoft Entra External ID

---

## 🔗 Más Información

Para información completa sobre la API, autenticación y usuarios de prueba, consulta:
- `docs/API-CONTRACT-FRONTEND.md` - Contrato completo de la API para frontend
