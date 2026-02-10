# 🎯 Guía de Creación de Usuarios para Pruebas QA - Chetango

## 📋 Usuarios a crear

### 👨‍🏫 Profesores (7)
1. Santiago Salazar
2. Maria Alejandra Rodriguez
3. Ana Zoraida Gomez
4. Laura Machado
5. Susana Alzate
6. Jhonathan Pachon
7. Suly Pachon

### 👨‍🎓 Alumnos (8)
1. Juan Pablo Gomez
2. Diana Diaz
3. Humberto Giraldo
4. Manuela Gonzales
5. Andrea Solorzano
6. Pablo Murillo
7. Catalina Sanchez
8. Camilo Tobon

### 🔑 Administrador (1)
1. Yeny Padilla

**Total: 16 usuarios nuevos**

---

## 🚀 Pasos de Ejecución

### 1. Crear usuarios en Entra ID

```powershell
cd C:\Proyectos\AppChetango\AppChetango\chetango-backend\scripts
.\crear-usuarios-entra-id.ps1
```

**Este script:**
- ✅ Se conecta a Microsoft Graph
- ✅ Busca la aplicación "Chetango Backend"
- ✅ Crea 16 usuarios en Entra ID
- ✅ Asigna el rol correspondiente a cada usuario (admin/profesor/alumno)
- ✅ Genera contraseña temporal: `Chetango2026!`
- ✅ Guarda las credenciales en:
  - `credenciales-usuarios-qa.csv` (formato CSV)
  - `credenciales-usuarios-qa.txt` (formato legible)

**Tiempo estimado:** 2-3 minutos

### 2. Sincronizar con la base de datos local

```powershell
.\sincronizar-usuarios-bd.ps1
```

**Este script:**
- ✅ Lee las credenciales del archivo CSV
- ✅ Inserta usuarios en tabla `Usuarios`
- ✅ Crea registros en tabla `Profesores` (para los 7 profes)
- ✅ Crea registros en tabla `Alumnos` (para los 8 alumnos)
- ✅ Asigna estados y tipos correctos
- ✅ Genera números de documento y teléfonos ficticios

**Tiempo estimado:** 1 minuto

---

## 📊 Resultado Final

### Entra ID
- ✅ 16 usuarios creados
- ✅ Roles asignados automáticamente
- ✅ Todos con contraseña temporal

### Base de Datos Local
- ✅ 19 usuarios totales (16 nuevos + 3 existentes)
- ✅ 8 profesores (Jorge Padilla + 7 nuevos)
- ✅ 9 alumnos (Juan David + 8 nuevos)
- ✅ 2 administradores (Admin original + Yeny Padilla)

---

## 🔐 Credenciales

Todos los usuarios tendrán:
- **Contraseña temporal:** `Chetango2026!`
- **Formato de email:** `NombreApellido@chetangoprueba.onmicrosoft.com`
- **Cambio obligatorio** de contraseña en primer login

Ejemplos:
- `santiagosalazar@chetangoprueba.onmicrosoft.com`
- `juanpablogomez@chetangoprueba.onmicrosoft.com`
- `yenypadilla@chetangoprueba.onmicrosoft.com`

---

## ✅ Verificación

### En Entra ID (Azure Portal)
1. Ir a: https://portal.azure.com
2. Azure Active Directory → Usuarios
3. Verificar que aparezcan los 16 usuarios nuevos
4. Verificar roles en: Aplicaciones empresariales → Chetango Backend → Usuarios y grupos

### En Base de Datos
```sql
-- Ver todos los usuarios
SELECT COUNT(*) AS TotalUsuarios FROM Usuarios;

-- Ver profesores
SELECT u.NombreUsuario, u.Correo, tp.Nombre AS TipoProfesor
FROM Profesores p
JOIN Usuarios u ON p.IdUsuario = u.IdUsuario
LEFT JOIN TiposProfesor tp ON p.IdTipoProfesor = tp.Id;

-- Ver alumnos
SELECT u.NombreUsuario, u.Correo, ea.Nombre AS Estado
FROM Alumnos a
JOIN Usuarios u ON a.IdUsuario = u.IdUsuario
LEFT JOIN EstadosAlumno ea ON a.IdEstado = ea.Id;
```

---

## 🎯 Siguiente Paso

Una vez creados y sincronizados los usuarios:

1. **Probar login** con cada rol en la aplicación
2. **Crear tipos de clase** (si no existen)
3. **Crear tipos de paquete** (si no existen)
4. **Iniciar pruebas funcionales** siguiendo el flujo:
   - Vender paquetes a alumnos
   - Crear clases
   - Registrar asistencias
   - Procesar liquidaciones
   - Generar reportes

---

## ⚠️ Notas Importantes

- Los scripts son **idempotentes**: pueden ejecutarse varias veces sin duplicar datos
- Si un usuario ya existe, se omite la creación
- Los números de documento y teléfonos son **ficticios** para pruebas
- Todos los profesores se crean como **tipo "Principal"** por defecto
- Las contraseñas **deben cambiarse** en el primer login por seguridad

---

## 🔧 Requisitos

- PowerShell 5.1 o superior
- Módulo `Microsoft.Graph` instalado (se instala automáticamente)
- Permisos de administrador en Entra ID
- SQL Server con base de datos `ChetangoDB_Dev`
- Conexión a internet para Microsoft Graph API

---

## 📝 Archivos Generados

- `credenciales-usuarios-qa.csv` - Credenciales en formato CSV
- `credenciales-usuarios-qa.txt` - Credenciales en formato legible
- Este README con instrucciones completas

**⚠️ IMPORTANTE: No subir estos archivos a repositorios públicos**

---

## 🆘 Solución de Problemas

### Error: "Module Microsoft.Graph not found"
```powershell
Install-Module Microsoft.Graph -Scope CurrentUser -Force
```

### Error: "Insufficient privileges"
- Verificar que tienes permisos de administrador en Entra ID
- Solicitar los scopes necesarios: `User.ReadWrite.All`, `AppRoleAssignment.ReadWrite.All`

### Error: "Cannot connect to SQL Server"
- Verificar que SQL Server esté corriendo
- Verificar el nombre del servidor y base de datos
- Verificar autenticación Windows

---

## 📞 Soporte

Si encuentras problemas durante la ejecución:
1. Revisar los mensajes de error en consola
2. Verificar archivos de log generados
3. Consultar documentación de Microsoft Graph
