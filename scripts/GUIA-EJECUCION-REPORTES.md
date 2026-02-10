# 🧪 Guía de Ejecución - Scripts de Datos para Reportes

## 📋 Prerequisitos

Antes de ejecutar los scripts de datos de prueba, asegúrate de tener:

1. ✅ SQL Server o LocalDB instalado
2. ✅ Base de datos `ChetangoDB_Dev` creada (las migraciones de EF Core la crean automáticamente)
3. ✅ API levantada al menos una vez para ejecutar migraciones

---

## 🚀 Orden de Ejecución (IMPORTANTE)

### **PASO 1: Levantar la API para crear la BD**
```powershell
cd Chetango.Api
dotnet run --launch-profile https-qa
```
Espera a que diga "Application started" y luego presiona `Ctrl+C`.

---

### **PASO 2: Ejecutar scripts de catálogos**

#### 2.1 Métodos de Pago
```powershell
cd ..\scripts
sqlcmd -S "(localdb)\MSSQLLocalDB" -d ChetangoDB_Dev -i seed_metodos_pago.sql
```

#### 2.2 Tipos de Paquete
```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -d ChetangoDB_Dev -i seed_paquetes_catalogos.sql
```

---

### **PASO 3: Crear usuarios de prueba CIAM**
```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -d ChetangoDB_Dev -i seed_usuarios_prueba_ciam.sql
```

**Crea:**
- Admin: Chetango@chetangoprueba.onmicrosoft.com
- Profesor: Jorgepadilla@chetangoprueba.onmicrosoft.com
- Alumno: JuanDavid@chetangoprueba.onmicrosoft.com

---

### **PASO 4: Crear datos para módulo Reportes** ⭐
```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -d ChetangoDB_Dev -i seed_reportes_datos_prueba.sql
```

**Crea:**
- ✅ 5 alumnos adicionales
- ✅ 45 pagos distribuidos (últimos 6 meses)
- ✅ 12+ paquetes con estados variados
- ✅ 40+ clases con asistencias
- ✅ Datos para alertas del dashboard

---

## 🎯 Verificar Datos Creados

### Contar registros:
```sql
USE ChetangoDB_Dev;
GO

-- Ver usuarios
SELECT COUNT(*) AS TotalUsuarios FROM Usuarios;

-- Ver alumnos
SELECT COUNT(*) AS TotalAlumnos FROM Alumnos;

-- Ver pagos
SELECT COUNT(*) AS TotalPagos FROM Pagos;

-- Ver paquetes por estado
SELECT e.Nombre AS Estado, COUNT(*) AS Cantidad
FROM Paquetes p
JOIN EstadosPaquete e ON p.IdEstado = e.Id
GROUP BY e.Nombre;

-- Ver clases
SELECT COUNT(*) AS TotalClases FROM Clases;

-- Ver asistencias
SELECT COUNT(*) AS TotalAsistencias FROM Asistencias;

-- Ver distribución de pagos por mes
SELECT 
    YEAR(FechaPago) AS Año,
    MONTH(FechaPago) AS Mes,
    COUNT(*) AS CantidadPagos,
    SUM(MontoTotal) AS TotalIngresos
FROM Pagos
GROUP BY YEAR(FechaPago), MONTH(FechaPago)
ORDER BY Año DESC, Mes DESC;
```

---

## 🔄 Limpiar y Recrear Datos

Si necesitas limpiar y recrear los datos:

```powershell
# Solo ejecuta PASO 4 de nuevo (es idempotente)
sqlcmd -S "(localdb)\MSSQLLocalDB" -d ChetangoDB_Dev -i seed_reportes_datos_prueba.sql
```

El script `seed_reportes_datos_prueba.sql` es **idempotente**: limpia los datos previos antes de insertar, por lo que puedes ejecutarlo múltiples veces sin problemas.

---

## ⚠️ Solución de Problemas

### Error: "No se puede abrir la base de datos"
**Solución:** Levanta la API primero para que EF Core cree la base de datos.

### Error: "Falta referencia a tabla X"
**Solución:** Ejecuta los scripts en el orden correcto (PASO 2 antes de PASO 3).

### Error: "Violación de clave foránea"
**Solución:** Ejecuta `seed_usuarios_prueba_ciam.sql` antes de `seed_reportes_datos_prueba.sql`.

### Error con sqlcmd: "No se reconoce como comando"
**Solución:** Instala SQL Server Command Line Tools o usa SQL Server Management Studio.

### Alternativa con SSMS:
1. Abre SQL Server Management Studio
2. Conecta a `(localdb)\MSSQLLocalDB`
3. Abre el archivo .sql
4. Selecciona la base de datos `ChetangoDB_Dev`
5. Presiona F5 para ejecutar

---

## 📊 Datos Esperados para Reportes

Después de ejecutar todos los scripts, deberías tener:

| Entidad | Cantidad Aproximada | Notas |
|---------|---------------------|-------|
| Usuarios | 8 | 3 CIAM + 5 adicionales |
| Alumnos | 6 | 1 CIAM + 5 adicionales |
| Profesores | 1 | Jorge Padilla |
| Pagos | 45 | Distribuidos en 6 meses |
| Paquetes | 12+ | Activos, Vencidos, Por vencer, Agotados |
| Clases | 45+ | 3 meses pasados + 7 días futuros |
| Asistencias | 160+ | Con diferentes tasas por alumno |
| Métodos Pago | 4 | Efectivo, Transferencia, Nequi, Tarjeta |

---

## 🎮 Listo para Probar

Con estos datos ya puedes probar:

✅ **Dashboard**
- KPIs con comparativas
- Gráficas de tendencias
- Alertas inteligentes

✅ **Reportes**
- Asistencias (con filtros)
- Ingresos (con comparativas)
- Paquetes (con alertas)
- Clases (con ocupación)
- Alumnos (con inactividad)

✅ **Exportaciones**
- Excel, PDF, CSV

✅ **Reportes Personales**
- Mi Reporte (alumno)
- Mis Clases (profesor)

---

## 📝 Notas Finales

- Los usuarios CIAM YA EXISTEN en Microsoft Entra External ID
- Las contraseñas están documentadas en `docs/API-CONTRACT-FRONTEND.md`
- Los GUIDs son fijos para facilitar las pruebas
- Los datos simulan 6 meses de operación real

---

**¿Necesitas más datos?** 
Modifica las variables en `seed_reportes_datos_prueba.sql`:
- `@FechaBase`: Cambiar rango temporal
- Bucles WHILE: Cambiar cantidad de registros
- Porcentajes de asistencia: Ajustar tasas
