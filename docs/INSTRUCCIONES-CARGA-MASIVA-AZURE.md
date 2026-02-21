# Instrucciones para Carga Masiva de Usuarios en Azure AD

## 📋 Archivo Preparado

**Archivo:** `usuarios-produccion-azure.csv`

Este archivo contiene:
- ✅ Formato correcto para Azure AD (`version:v1.0`)
- ✅ 15 usuarios de ejemplo con correos @corporacionchetango.com
- ✅ Contraseña inicial: `Chetango2026!`
- ✅ Ubicación: Colombia (CO), Sede Medellín

---

## 🔧 Pasos para Importar en Azure Portal

### 1️⃣ Acceder al Portal de Azure

1. Ve a: https://portal.azure.com
2. Inicia sesión con tu cuenta de administrador
3. Busca **"Azure Active Directory"** o **"Microsoft Entra ID"**

### 2️⃣ Ir a Bulk Operations

1. En el menú lateral, selecciona **"Users"** (Usuarios)
2. Haz clic en **"Bulk operations"** (Operaciones masivas)
3. Selecciona **"Bulk create"** (Creación masiva)

### 3️⃣ Descargar y Modificar Plantilla (Opcional)

**Opción A - Usar archivo generado:**
- Salta este paso, ya tienes `usuarios-produccion-azure.csv`

**Opción B - Descargar plantilla de Azure:**
1. Haz clic en **"Download a CSV template"**
2. Abre la plantilla descargada
3. Copia los usuarios del archivo `usuarios-produccion-azure.csv`
4. Pega en la plantilla de Azure (respetando la línea `version:v1.0`)

### 4️⃣ Subir el Archivo CSV

1. Haz clic en **"Select a file"** (Seleccionar archivo)
2. Navega hasta: `chetango-backend/docs/usuarios-produccion-azure.csv`
3. Selecciona el archivo
4. Haz clic en **"Upload"**

### 5️⃣ Validar y Crear

1. Azure validará el formato del archivo (tarda 10-30 segundos)
2. Si hay errores:
   - Lee los mensajes de error
   - Corrige el archivo CSV
   - Vuelve a subirlo
3. Si todo está bien:
   - Haz clic en **"Submit"** (Enviar)
   - La creación masiva comenzará

### 6️⃣ Verificar Resultados

1. Azure mostrará un mensaje: **"Bulk operation submitted"**
2. Puedes descargar el reporte de resultados
3. Los usuarios aparecerán en la lista en 2-5 minutos

---

## 📝 Estructura del Archivo CSV

```csv
version:v1.0
Name [displayName] Required,User name [userPrincipalName] Required,...
María Barrera,maria.barrera@corporacionchetango.com,Chetango2026!,No,...
```

### Columnas Obligatorias

| Columna | Descripción | Ejemplo |
|---------|-------------|---------|
| **Name [displayName]** | Nombre completo | María Barrera |
| **User name [userPrincipalName]** | Email corporativo | maria.barrera@corporacionchetango.com |
| **Initial password [passwordProfile]** | Contraseña inicial | Chetango2026! |
| **Block sign in [accountEnabled]** | ¿Bloquear acceso? | No |

### Columnas Opcionales (Incluidas)

| Columna | Valor Configurado |
|---------|-------------------|
| **First name [givenName]** | Nombre |
| **Last name [surname]** | Apellido |
| **Job title [jobTitle]** | Alumno |
| **Department [department]** | Danza |
| **Usage location [usageLocation]** | CO (Colombia) |
| **Office [physicalDeliveryOfficeName]** | Sede Medellín |
| **City [city]** | Medellín |
| **State or province [state]** | Antioquia |
| **Country or region [country]** | Colombia |

---

## ⚙️ Personalización del Archivo

### Cambiar Contraseña Inicial

Edita la columna **"Initial password"** en el CSV:

```csv
María Barrera,maria.barrera@corporacionchetango.com,MiContraseña2026!,No,...
```

### Agregar Más Usuarios

1. Abre `usuarios-produccion-azure.csv` en Excel
2. Copia la última fila
3. Pega debajo
4. Modifica: Nombre, Email, etc.
5. Guarda como CSV (UTF-8)

⚠️ **IMPORTANTE:** No elimines la primera línea `version:v1.0`

### Cambiar Sede a Manizales

Modifica las columnas:
- **Office:** Sede Manizales
- **City:** Manizales
- **State or province:** Caldas

```csv
Juan Pérez,juan.perez@corporacionchetango.com,Chetango2026!,No,Juan,Pérez,Alumno,Danza,CO,,Caldas,Colombia,Sede Manizales,Manizales,,,
```

---

## 🔐 Políticas de Contraseña

### Requisitos de Azure AD

La contraseña debe cumplir:
- ✅ Mínimo 8 caracteres
- ✅ Al menos 1 mayúscula
- ✅ Al menos 1 minúscula
- ✅ Al menos 1 número
- ✅ Al menos 1 carácter especial (!@#$%^&*)

**Contraseña configurada:** `Chetango2026!`
- 13 caracteres ✅
- Mayúsculas (C) ✅
- Minúsculas (hetango) ✅
- Números (2026) ✅
- Especiales (!) ✅

### Cambio de Contraseña en Primer Inicio

Si quieres forzar cambio de contraseña:

1. Después de crear los usuarios
2. Ve a cada usuario en Azure Portal
3. Haz clic en **"Reset password"**
4. Marca: **"Require this user to change their password when they first sign in"**

---

## ⚠️ Errores Comunes

### Error: "Invalid file format"

**Causa:** Falta la línea `version:v1.0` al inicio

**Solución:**
```csv
version:v1.0
Name [displayName] Required,...
```

### Error: "User principal name already exists"

**Causa:** El correo ya existe en Azure AD

**Solución:**
- Cambia el correo: `maria.barrera2@corporacionchetango.com`
- O elimina esa fila si el usuario ya existe

### Error: "Password does not meet requirements"

**Causa:** Contraseña muy simple

**Solución:** Usa contraseñas complejas como `Chetango2026!`

### Error: "Usage location is required"

**Causa:** Falta código de país

**Solución:** Agrega `CO` en la columna **Usage location**

---

## 📊 Después de Crear Usuarios

### Asignar Licencias (Si es necesario)

1. Ve a **Azure Active Directory** > **Users**
2. Selecciona los usuarios creados
3. Haz clic en **"Assign licenses"**
4. Selecciona la licencia (ej: Microsoft 365 E3)
5. Confirma

### Asignar Roles en la Aplicación Chetango

Los usuarios creados en Azure AD **NO** están automáticamente en la base de datos de Chetango.

**Opciones:**

**Opción A - Registro automático en primer login:**
- Configura la app para auto-registro en primer login
- El usuario se crea automáticamente en SQL con rol "Alumno"

**Opción B - Creación manual en el sistema:**
1. Entra a https://chetango-app.azurewebsites.net/admin/usuarios
2. Haz clic en **"Nuevo Usuario"**
3. Usa el mismo email: `maria.barrera@corporacionchetango.com`
4. Selecciona Rol: **Alumno**
5. Selecciona Sede: **Medellín**

---

## 📧 Notificar Credenciales a los Usuarios

### Enviar Email Automático (Recomendado)

Azure puede enviar correos automáticamente:

1. En **Bulk create**, antes de Submit
2. Marca: **"Send new password by email"**
3. Azure enviará credenciales a cada correo

### Plantilla de Email Manual

```
Asunto: Bienvenido a Corporación Chetango - Credenciales de Acceso

Hola [Nombre],

Te damos la bienvenida a la plataforma digital de Corporación Chetango.

🔐 Tus credenciales de acceso:
- Usuario: maria.barrera@corporacionchetango.com
- Contraseña temporal: Chetango2026!

🌐 Ingresa aquí: https://chetango-app.azurewebsites.net

⚠️ IMPORTANTE:
- Cambia tu contraseña en el primer inicio de sesión
- Guarda tus credenciales en un lugar seguro
- Si tienes problemas, contacta a: soporte@corporacionchetango.com

¡Nos vemos en clase!

Corporación Chetango
www.corporacionchetango.com
```

---

## 🎯 Resumen

✅ **Archivo listo:** `usuarios-produccion-azure.csv`  
✅ **15 usuarios** con formato correcto  
✅ **Contraseña:** Chetango2026!  
✅ **Sede:** Medellín, Antioquia  
✅ **Formato validado** para Azure AD  

**Siguiente paso:** Sube el archivo en Azure Portal > Azure AD > Users > Bulk operations > Bulk create

---

## 📞 Soporte

Si encuentras problemas:
1. Revisa la sección **"Errores Comunes"** arriba
2. Descarga el reporte de errores de Azure
3. Contacta al equipo técnico con el reporte
