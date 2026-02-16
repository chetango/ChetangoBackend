# Scripts de Población de Datos para Marketing

## 📋 Descripción

Scripts SQL para poblar la base de datos **ChetangoDB_Dev** con datos realistas para producción del video de marketing de Chetango.

**IMPORTANTE**: Estos scripts preservan los 3 usuarios de prueba existentes (admin, profesor, alumno).

## 📊 Volumetría de Datos

| Entidad | Cantidad | Descripción |
|---------|----------|-------------|
| **Profesores** | 5 | Distribuidos desde Ago 2024 |
| **Alumnos** | 50 | Crecimiento gradual (10→50) |
| **Pagos** | ~95 | Transacciones financieras |
| **Paquetes** | ~100 | Básicos y Premium |
| **Clases** | ~180 | Ago 2024 - Feb 2026 |
| **Asistencias** | ~2,300 | Ocupación 60-80% |
| **Liquidaciones** | ~48 | 6 meses de nómina |
| **Códigos Referido** | 15 | Alumnos con código activo |
| **Usos Referido** | 30 | Referidos exitosos |
| **Eventos** | 12 | Workshops, sociales, competencias |
| **Notificaciones** | 80 | Comunicación con alumnos |
| **Solicitudes** | 35 | Privadas (20) + Renovaciones (15) |

**Total:** ~3,570 registros distribuidos en 18 meses

## 🗂️ Estructura de Scripts

```
scripts/marketing/
├── 00_ejecutar_todo.sql         ← Script maestro (instrucciones)
├── 01_catalogos_base.sql        ← Tipos, estados, métodos pago
├── 02_usuarios_y_perfiles.sql   ← 5 profesores + 50 alumnos
├── 03_transacciones_financieras.sql ← Pagos y paquetes
├── 04_programacion_clases.sql   ← 180 clases programadas
├── 05_asistencias_masivas.sql   ← 2,300 asistencias realistas
├── 06_liquidaciones_mensuales.sql ← Nómina de profesores
├── 07_sistema_referidos.sql     ← Códigos y usos
├── 08_eventos_y_notificaciones.sql ← Engagement
├── 09_solicitudes.sql           ← Workflow de solicitudes
└── 99_validaciones.sql          ← Verificación de integridad
```

## 🚀 Instrucciones de Ejecución

### Opción 1: Ejecución Manual en SSMS (Recomendado)

1. **Abrir SQL Server Management Studio (SSMS)**
2. **Conectar a:** `localhost` (ChetangoDB_Dev)
3. **Ejecutar scripts en orden:**

```sql
-- 1. Catálogos base
:r "C:\Proyectos\AppChetango\AppChetango\chetango-backend\scripts\marketing\01_catalogos_base.sql"

-- 2. Usuarios y perfiles
:r "C:\Proyectos\AppChetango\AppChetango\chetango-backend\scripts\marketing\02_usuarios_y_perfiles.sql"

-- 3. Transacciones financieras
:r "C:\Proyectos\AppChetango\AppChetango\chetango-backend\scripts\marketing\03_transacciones_financieras.sql"

-- 4. Programación de clases
:r "C:\Proyectos\AppChetango\AppChetango\chetango-backend\scripts\marketing\04_programacion_clases.sql"

-- 5. Asistencias masivas (el más lento - ~30 seg)
:r "C:\Proyectos\AppChetango\AppChetango\chetango-backend\scripts\marketing\05_asistencias_masivas.sql"

-- 6. Liquidaciones mensuales
:r "C:\Proyectos\AppChetango\AppChetango\chetango-backend\scripts\marketing\06_liquidaciones_mensuales.sql"

-- 7. Sistema de referidos
:r "C:\Proyectos\AppChetango\AppChetango\chetango-backend\scripts\marketing\07_sistema_referidos.sql"

-- 8. Eventos y notificaciones
:r "C:\Proyectos\AppChetango\AppChetango\chetango-backend\scripts\marketing\08_eventos_y_notificaciones.sql"

-- 9. Solicitudes
:r "C:\Proyectos\AppChetango\AppChetango\chetango-backend\scripts\marketing\09_solicitudes.sql"

-- 10. VALIDAR (IMPORTANTE)
:r "C:\Proyectos\AppChetango\AppChetango\chetango-backend\scripts\marketing\99_validaciones.sql"
```

### Opción 2: Línea de Comandos con sqlcmd

```powershell
# Navegar a la carpeta
cd "C:\Proyectos\AppChetango\AppChetango\chetango-backend\scripts\marketing"

# Ejecutar cada script
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "01_catalogos_base.sql"
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "02_usuarios_y_perfiles.sql"
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "03_transacciones_financieras.sql"
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "04_programacion_clases.sql"
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "05_asistencias_masivas.sql"
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "06_liquidaciones_mensuales.sql"
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "07_sistema_referidos.sql"
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "08_eventos_y_notificaciones.sql"
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "09_solicitudes.sql"

# Validar
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "99_validaciones.sql"
```

### Opción 3: Script Batch Automatizado

Crear archivo `ejecutar_poblacion.bat`:

```batch
@echo off
echo ========================================
echo   Poblando Base de Datos Marketing
echo ========================================
echo.

cd /d "%~dp0"

sqlcmd -S localhost -d ChetangoDB_Dev -E -i "01_catalogos_base.sql" -o "log_01.txt"
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "02_usuarios_y_perfiles.sql" -o "log_02.txt"
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "03_transacciones_financieras.sql" -o "log_03.txt"
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "04_programacion_clases.sql" -o "log_04.txt"
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "05_asistencias_masivas.sql" -o "log_05.txt"
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "06_liquidaciones_mensuales.sql" -o "log_06.txt"
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "07_sistema_referidos.sql" -o "log_07.txt"
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "08_eventos_y_notificaciones.sql" -o "log_08.txt"
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "09_solicitudes.sql" -o "log_09.txt"

echo.
echo ========================================
echo   Ejecutando Validaciones
echo ========================================
sqlcmd -S localhost -d ChetangoDB_Dev -E -i "99_validaciones.sql" -o "log_validaciones.txt"

echo.
echo Proceso completado. Revisar archivos log_*.txt
pause
```

## ⏱️ Tiempo Estimado de Ejecución

| Script | Tiempo | Complejidad |
|--------|--------|-------------|
| 01 - Catálogos | < 1 seg | Baja |
| 02 - Usuarios | ~3 seg | Media |
| 03 - Transacciones | ~5 seg | Media |
| 04 - Clases | ~8 seg | Media |
| 05 - Asistencias | ~30 seg | **Alta** |
| 06 - Liquidaciones | ~10 seg | Media |
| 07 - Referidos | ~2 seg | Baja |
| 08 - Eventos | ~3 seg | Baja |
| 09 - Solicitudes | ~2 seg | Baja |
| 99 - Validaciones | ~5 seg | Media |

**Total:** ~70 segundos (1 minuto 10 segundos)

## ✅ Validación Post-Ejecución

El script `99_validaciones.sql` verifica:

1. ✓ Usuarios y perfiles correctos (5 profes + 50 alumnos)
2. ✓ Transacciones financieras íntegras
3. ✓ Clases con profesor asignado
4. ✓ Asistencias con ocupación 60-80%
5. ✓ Paquetes: ClasesUsadas ≤ ClasesDisponibles
6. ✓ Liquidaciones con montos correctos
7. ✓ Códigos de referido con contador correcto
8. ✓ Eventos pasados y futuros
9. ✓ Notificaciones distribuidas
10. ✓ Solicitudes en diferentes estados
11. ✓ **Usuarios de prueba preservados**
12. ✓ Volumetría total

## 🔍 Identificación de Datos

Todos los datos de marketing tienen marcadores para fácil identificación:

- **Correos:** `*@marketing.chetango.com`
- **Descripciones:** Incluyen `[MKT]`
- **Documentos alumnos:** `20000001` - `20000050`
- **Documentos profesores:** `1000000001` - `1000000005`

## 🗑️ Limpieza de Datos

Si necesitas reejecutar los scripts, cada uno limpia automáticamente sus datos previos **SIN AFECTAR usuarios de prueba**.

Para limpiar manualmente todos los datos de marketing:

```sql
-- USAR CON PRECAUCIÓN
DELETE FROM UsoCodigoReferido WHERE IdCodigoReferido IN (
    SELECT cr.IdCodigoReferido FROM CodigoReferido cr
    INNER JOIN Alumnos a ON cr.IdAlumno = a.IdAlumno
    INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
    WHERE u.Correo LIKE '%@marketing.chetango.com'
);

DELETE FROM CodigoReferido WHERE IdAlumno IN (
    SELECT a.IdAlumno FROM Alumnos a
    INNER JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
    WHERE u.Correo LIKE '%@marketing.chetango.com'
);

-- ... continuar con otras entidades ...
```

## 📈 Datos Generados para Video

### Dashboard Administrativo
- ✅ KPI Asistencias Hoy: 10-15 alumnos
- ✅ KPI Ingresos Este Mes: $6M - $11M
- ✅ KPI Egresos Este Mes: $1.5M - $3M
- ✅ KPI Ganancia Neta: $4.5M - $8M
- ✅ Gráfico ingresos: Curva crecimiento
- ✅ Gráfico asistencias: Tendencia estable

### Nómina Profesores
- ✅ 5 profesores activos
- ✅ 48 liquidaciones (6 meses)
- ✅ Historial pagos con filtros
- ✅ Estados: Pagada/Pendiente

### Gestión Alumnos
- ✅ 50 alumnos con datos completos
- ✅ Paquetes en diferentes estados
- ✅ Historial de asistencias
- ✅ Sistema de referidos activo

### Programación
- ✅ 180 clases distribuidas
- ✅ Horarios variados (8am - 8pm)
- ✅ Múltiples profesores rotando
- ✅ Ocupación realista 60-80%

### Eventos y Comunicación
- ✅ 12 eventos (pasados y futuros)
- ✅ 80 notificaciones enviadas
- ✅ Carrusel funcional
- ✅ Diferentes categorías

### Solicitudes
- ✅ 20 solicitudes clases privadas
- ✅ 15 solicitudes renovación
- ✅ Estados: Pendiente/Aprobada/Rechazada
- ✅ Workflow completo

## 🎬 Casos de Uso para Video

1. **Login y Dashboard**: Mostrar KPIs con valores reales
2. **Reportes**: Gráficos con datos de 6 meses
3. **Asistencias**: Registrar asistencias con ocupación visible
4. **Nómina**: Ver historial pagos con filtros funcionando
5. **Eventos**: Carrusel con eventos pasados y próximos
6. **Referidos**: Sistema activo con códigos usados
7. **Solicitudes**: Workflow de aprobación/rechazo
8. **Alumnos**: Perfiles completos con historial

## ⚠️ Advertencias

1. **Solo ejecutar en ChetangoDB_Dev** (desarrollo local)
2. **NO ejecutar en producción**
3. **Los scripts son idempotentes** (se pueden reejecutar)
4. **Usuarios de prueba siempre preservados**
5. **Fecha actual:** Scripts consideran Feb 13, 2025 como "hoy"

## 📞 Soporte

Si encuentras errores durante la ejecución:

1. Revisar mensajes de error en output
2. Ejecutar `99_validaciones.sql` para diagnóstico
3. Verificar conexión a ChetangoDB_Dev
4. Confirmar que catálogos base existen

## 📝 Notas Técnicas

- **Timezone:** Colombia UTC-5 (SA Pacific Standard Time)
- **Fechas:** Ago 2024 - Feb 2026 (18 meses)
- **Crecimiento:** 10 alumnos (Ago) → 50 alumnos (Feb)
- **Algoritmos:** Distribución realista con aleatoriedad controlada
- **Integridad:** Foreign keys respetadas, índices únicos validados

---

**Generado:** Febrero 2025  
**Versión:** 1.0  
**Para:** Video de Marketing Chetango
