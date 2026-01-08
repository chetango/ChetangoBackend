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

2. **Crear usuarios de prueba**:
   ```bash
   sqlcmd -S "(localdb)\MSSQLLocalDB" -d ChetangoDB_Dev -i seed_usuarios_prueba_ciam.sql
   ```

3. **Verificar**: Probar login con cualquiera de los 3 usuarios en Postman o tu aplicación frontend.

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
