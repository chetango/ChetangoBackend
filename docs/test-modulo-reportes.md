# 🧪 Casos de Prueba - Módulo Reportes

## 📋 Información General

**Base URL:** `https://localhost:7194`  
**Autenticación:** Bearer Token (OAuth 2.0 - Microsoft Entra External ID)

### Usuarios de Prueba

| Usuario | Email | Rol | IdUsuario |
|---------|-------|-----|-----------|
| Admin | Chetango@chetangoprueba.onmicrosoft.com | admin | b91e51b9-4094-441e-a5b6-062a846b3868 |
| Profesor Jorge | Jorgepadilla@chetangoprueba.onmicrosoft.com | profesor | 8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB |
| Alumno Juan David | JuanDavid@chetangoprueba.onmicrosoft.com | alumno | 71462106-9863-4fd0-b13d-9878ed231aa6 |

---

## 🔐 Autenticación

### 1. Obtener Token
**Método:** Usar OAuth 2.0 Authorization Code + PKCE con Microsoft Entra External ID

**Configuración en Postman:**
- Authorization Type: OAuth 2.0
- Grant Type: Authorization Code (With PKCE)
- Auth URL: `https://chetangoprueba.ciamlogin.com/chetangoprueba.onmicrosoft.com/oauth2/v2.0/authorize`
- Access Token URL: `https://chetangoprueba.ciamlogin.com/chetangoprueba.onmicrosoft.com/oauth2/v2.0/token`
- Client ID: (de appsettings.json)
- Scope: `openid profile email api://{ClientId}/chetango.api`
- **Auto-refresh Token:** Activado ✅

---

## 📊 FASE 1: Reportes Administrativos

### 1. GET /api/reportes/asistencias - Reporte de Asistencias

**Endpoint:** `GET /api/reportes/asistencias`  
**Autorización:** AdminOrProfesor  
**Usuario:** Admin (ve todo), Profesor (solo sus clases)

**Request:**
```
GET https://localhost:7194/api/reportes/asistencias?fechaDesde=2026-01-01&fechaHasta=2026-01-31
```

**Headers:**
```
Authorization: Bearer {{token}}
```

**Query Parameters Opcionales:**
- `idClase` (Guid): Filtrar por clase específica
- `idAlumno` (Guid): Filtrar por alumno específico
- `idProfesor` (Guid): Filtrar por profesor
- `estadoAsistencia` (string): "Presente", "Ausente", "Justificada"

**Respuesta Esperada (200 OK):**
```json
{
  "totalAsistencias": 150,
  "presentes": 135,
  "ausentes": 10,
  "justificadas": 5,
  "porcentajeAsistencia": 90.00,
  "listaDetallada": [
    {
      "fecha": "2026-01-10T00:00:00",
      "nombreAlumno": "Juan Pérez",
      "nombreClase": "Tango Avanzado",
      "estado": "Presente",
      "observaciones": null,
      "nombreProfesor": "Jorge Padilla"
    }
  ],
  "graficaAsistenciasPorDia": {
    "type": "bar",
    "labels": ["Lunes", "Martes", "Miércoles", "Jueves", "Viernes"],
    "datasets": [
      {
        "label": "Asistencias",
        "data": [25, 30, 22, 28, 30],
        "backgroundColor": "#4F46E5",
        "borderColor": "#4338CA"
      }
    ]
  }
}
```

**Validaciones:**
- ✅ `fechaDesde` <= `fechaHasta`
- ✅ No fechas futuras
- ✅ Rango máximo: 1 año
- ✅ Profesor solo ve sus clases (ownership por email)
- ✅ Admin ve todo

**Casos de Error:**
```json
// Fecha inicial > fecha final
{
  "error": "La fecha inicial no puede ser mayor a la fecha final."
}

// Rango > 1 año
{
  "error": "El rango de fechas no puede ser mayor a 1 año."
}

// Fechas futuras
{
  "error": "No se pueden generar reportes de fechas futuras."
}
```

---

### 2. GET /api/reportes/ingresos - Reporte de Ingresos

**Endpoint:** `GET /api/reportes/ingresos`  
**Autorización:** AdminOnly  
**Usuario:** Solo Admin

**Request:**
```
GET https://localhost:7194/api/reportes/ingresos?fechaDesde=2026-01-01&fechaHasta=2026-01-31
```

**Query Parameters Opcionales:**
- `idMetodoPago` (Guid): Filtrar por método de pago
- `comparativa` (bool): true para comparativa con mes anterior

**Con Comparativa:**
```
GET https://localhost:7194/api/reportes/ingresos?fechaDesde=2026-01-01&fechaHasta=2026-01-31&comparativa=true
```

**Respuesta Esperada (200 OK):**
```json
{
  "totalRecaudado": 350000.00,
  "cantidad": 45,
  "promedio": 7777.78,
  "comparativaMesAnterior": 15.50,
  "tendenciaMensual": [
    {
      "año": 2026,
      "mes": 1,
      "mesNombre": "enero",
      "totalIngresos": 350000.00,
      "cantidadPagos": 45
    }
  ],
  "graficaIngresosMensuales": {
    "type": "line",
    "labels": ["Ene 2026", "Feb 2026"],
    "datasets": [
      {
        "label": "Ingresos ($)",
        "data": [350000, 420000],
        "backgroundColor": "#10B981",
        "borderColor": "#059669"
      }
    ]
  },
  "desgloseMetodosPago": [
    {
      "metodoPago": "Efectivo",
      "totalRecaudado": 200000.00,
      "cantidadPagos": 25,
      "porcentajeDelTotal": 57.14
    },
    {
      "metodoPago": "Transferencia Bancaria",
      "totalRecaudado": 150000.00,
      "cantidadPagos": 20,
      "porcentajeDelTotal": 42.86
    }
  ]
}
```

**Validaciones:**
- ✅ Solo Admin puede ejecutar
- ✅ Validaciones de fechas iguales a asistencias
- ✅ `comparativaMesAnterior` muestra % de cambio vs periodo anterior

---

### 3. GET /api/reportes/paquetes - Reporte de Paquetes

**Endpoint:** `GET /api/reportes/paquetes`  
**Autorización:** AdminOnly  
**Usuario:** Solo Admin

**Request:**
```
GET https://localhost:7194/api/reportes/paquetes?fechaDesde=2025-12-01&fechaHasta=2026-01-31
```

**Query Parameters Opcionales:**
- `estado` (string): "Activo", "Vencido", "Agotado"
- `idTipoPaquete` (Guid): Filtrar por tipo de paquete

**Respuesta Esperada (200 OK):**
```json
{
  "totalActivos": 50,
  "totalVencidos": 10,
  "totalPorVencer": 8,
  "totalAgotados": 5,
  "alertasPorVencer": [
    {
      "idPaquete": "123e4567-e89b-12d3-a456-426614174000",
      "nombreAlumno": "María González",
      "correoAlumno": "maria@example.com",
      "nombreTipoPaquete": "Paquete Mensual 8 Clases",
      "fechaVencimiento": "2026-01-18T00:00:00",
      "diasRestantes": 6,
      "clasesRestantes": 3
    }
  ],
  "desgloseEstados": [
    {
      "estado": "Activo",
      "cantidad": 50,
      "porcentajeDelTotal": 68.49
    },
    {
      "estado": "Vencido",
      "cantidad": 10,
      "porcentajeDelTotal": 13.70
    }
  ],
  "graficaPaquetesPorTipo": {
    "type": "pie",
    "labels": ["Mensual 8", "Mensual 12"],
    "datasets": [
      {
        "label": "Paquetes",
        "data": [30, 20],
        "backgroundColor": "#F59E0B"
      }
    ]
  }
}
```

**Validaciones:**
- ✅ Solo Admin puede ejecutar
- ✅ Alertas muestran paquetes que vencen en próximos 7 días
- ✅ `clasesRestantes` = `ClasesDisponibles - ClasesUsadas`

---

### 4. GET /api/reportes/clases - Reporte de Clases

**Endpoint:** `GET /api/reportes/clases`  
**Autorización:** AdminOrProfesor  
**Usuario:** Admin (ve todo), Profesor (solo sus clases)

**Request:**
```
GET https://localhost:7194/api/reportes/clases?fechaDesde=2026-01-01&fechaHasta=2026-01-31
```

**Query Parameters Opcionales:**
- `idTipoClase` (Guid): Filtrar por tipo de clase
- `idProfesor` (Guid): Filtrar por profesor

**Respuesta Esperada (200 OK):**
```json
{
  "totalClases": 120,
  "promedioAsistencia": 18.50,
  "ocupacionPromedio": 85.00,
  "clasesMasPopulares": [
    {
      "nombreTipoClase": "Tango Avanzado",
      "totalClases": 40,
      "promedioAsistencia": 22.00,
      "ocupacionPorcentaje": 95.00
    }
  ],
  "graficaAsistenciaPorDia": {
    "type": "bar",
    "labels": ["Lunes", "Martes", "Miércoles"],
    "datasets": [
      {
        "label": "Asistencias",
        "data": [180, 200, 165],
        "backgroundColor": "#8B5CF6",
        "borderColor": "#7C3AED"
      }
    ]
  },
  "desgloseporTipo": [
    {
      "nombreTipoClase": "Tango Avanzado",
      "cantidadClases": 40,
      "promedioAsistencia": 22.00
    }
  ]
}
```

**Validaciones:**
- ✅ Profesor solo ve sus clases (ownership por email)
- ✅ Admin ve todas las clases
- ✅ Top 5 clases más populares por asistencia

---

### 5. GET /api/reportes/alumnos - Reporte de Alumnos

**Endpoint:** `GET /api/reportes/alumnos`  
**Autorización:** AdminOnly  
**Usuario:** Solo Admin

**Request:**
```
GET https://localhost:7194/api/reportes/alumnos
```

**Query Parameters Opcionales:**
- `fechaInscripcionDesde` (DateTime): Filtrar por fecha de inscripción desde
- `fechaInscripcionHasta` (DateTime): Filtrar por fecha hasta
- `estado` (string): "Activo", "Inactivo"

**Con Filtros:**
```
GET https://localhost:7194/api/reportes/alumnos?fechaInscripcionDesde=2025-01-01&fechaInscripcionHasta=2026-01-31&estado=Activo
```

**Respuesta Esperada (200 OK):**
```json
{
  "totalActivos": 80,
  "totalInactivos": 10,
  "nuevosEsteMes": 5,
  "tasaRetencion": 85.00,
  "alumnosInactivos": [
    {
      "idAlumno": "295093d5-b36f-4737-b68a-ab40ca871b2e",
      "nombreAlumno": "Pedro López",
      "correo": "pedro@example.com",
      "ultimaAsistencia": "2025-11-15T00:00:00",
      "diasInactivo": 58
    }
  ],
  "alumnosPorVencer": [
    {
      "idAlumno": "395093d5-b36f-4737-b68a-ab40ca871b2f",
      "nombreAlumno": "Ana Martínez",
      "correo": "ana@example.com",
      "fechaVencimiento": "2026-01-18T00:00:00",
      "diasRestantes": 6
    }
  ],
  "graficaAlumnosPorMes": {
    "type": "line",
    "labels": ["Ene 2025", "Feb 2025", "Mar 2025"],
    "datasets": [
      {
        "label": "Nuevos Alumnos",
        "data": [5, 8, 3],
        "backgroundColor": "#3B82F6",
        "borderColor": "#2563EB"
      }
    ]
  }
}
```

**Validaciones:**
- ✅ Solo Admin puede ejecutar
- ✅ Alumnos inactivos: sin asistencias en más de 30 días
- ✅ Tasa de retención: alumnos con paquetes activos / total activos
- ✅ Top 20 alumnos inactivos ordenados por días sin asistir

---

### 6. GET /api/reportes/dashboard - Dashboard Ejecutivo

**Endpoint:** `GET /api/reportes/dashboard`  
**Autorización:** AdminOnly  
**Usuario:** Solo Admin

**Request:**
```
GET https://localhost:7194/api/reportes/dashboard
```

**Sin parámetros** (usa fecha actual)

**Respuesta Esperada (200 OK):**
```json
{
  "kpIs": {
    "totalAlumnosActivos": 80,
    "ingresosEsteMes": 420000.00,
    "clasesProximos7Dias": 15,
    "paquetesActivos": 50,
    "paquetesVencidos": 10,
    "paquetesPorVencer": 8,
    "asistenciasHoy": 12,
    "crecimientoIngresosMesAnterior": 15.50
  },
  "graficaIngresos": {
    "type": "line",
    "labels": ["Ago 2025", "Sep 2025", "Oct 2025", "Nov 2025", "Dic 2025", "Ene 2026"],
    "datasets": [
      {
        "label": "Ingresos ($)",
        "data": [280000, 320000, 300000, 350000, 380000, 420000],
        "backgroundColor": "#10B981",
        "borderColor": "#059669"
      }
    ]
  },
  "graficaAsistencias": {
    "type": "bar",
    "labels": ["Lunes", "Martes", "Miércoles", "Jueves", "Viernes"],
    "datasets": [
      {
        "label": "Asistencias",
        "data": [45, 52, 48, 50, 55],
        "backgroundColor": "#6366F1",
        "borderColor": "#4F46E5"
      }
    ]
  },
  "graficaPaquetes": {
    "type": "doughnut",
    "labels": ["Activo", "Vencido", "Agotado"],
    "datasets": [
      {
        "label": "Paquetes",
        "data": [50, 10, 5],
        "backgroundColor": "#F59E0B"
      }
    ]
  },
  "alertas": [
    {
      "tipo": "PaquetePorVencer",
      "titulo": "Paquetes por Vencer",
      "descripcion": "8 paquete(s) vencen en los próximos 7 días",
      "fechaGeneracion": "2026-01-12T10:30:00",
      "prioridad": "Alta",
      "datosAdicionales": {
        "cantidad": 8
      }
    },
    {
      "tipo": "AlumnoInactivo",
      "titulo": "Alumnos Inactivos",
      "descripcion": "15 alumno(s) sin asistencias en más de 30 días",
      "fechaGeneracion": "2026-01-12T10:30:00",
      "prioridad": "Media",
      "datosAdicionales": {
        "cantidad": 15
      }
    }
  ]
}
```

**Validaciones:**
- ✅ Solo Admin puede ejecutar
- ✅ Cache de 5 minutos (clave: `dashboard_{yyyyMMdd}`)
- ✅ Segunda llamada en 5 min debe ser más rápida
- ✅ KPIs basados en fecha actual
- ✅ Gráficas: últimos 6 meses (ingresos), últimos 30 días (asistencias)

---

## 👤 FASE 2: Reportes Personales

### 7. GET /api/reportes/mi-reporte - Mi Reporte (Alumno)

**Endpoint:** `GET /api/reportes/mi-reporte`  
**Autorización:** ApiScope (cualquier usuario autenticado)  
**Usuario:** Alumno Juan David

**Request:**
```
GET https://localhost:7194/api/reportes/mi-reporte
```

**Headers:**
```
Authorization: Bearer {{token}}
```

**Sin parámetros** (extrae email del token JWT)

**Respuesta Esperada (200 OK):**
```json
{
  "nombreAlumno": "Juan David",
  "correo": "JuanDavid@chetangoprueba.onmicrosoft.com",
  "fechaInscripcion": "2025-01-15T00:00:00",
  "totalClasesTomadas": 24,
  "porcentajeAsistencia": 92.31,
  "clasesRestantes": 4,
  "paqueteActual": {
    "idPaquete": "45e4567-e89b-12d3-a456-426614174000",
    "nombreTipoPaquete": "Paquete Mensual 8 Clases",
    "fechaActivacion": "2026-01-01T00:00:00",
    "fechaVencimiento": "2026-01-31T00:00:00",
    "clasesRestantes": 4,
    "clasesOriginales": 8,
    "estado": "Activo"
  },
  "historialPagos": [
    {
      "idPago": "67e4567-e89b-12d3-a456-426614174000",
      "fechaPago": "2026-01-01T00:00:00",
      "monto": 150000.00,
      "metodoPago": "Efectivo",
      "conceptoPago": "Paquete 8 clases",
      "idPaqueteGenerado": null
    }
  ],
  "graficaAsistenciasMensuales": {
    "type": "bar",
    "labels": ["Ago 2025", "Sep 2025", "Oct 2025"],
    "datasets": [
      {
        "label": "Asistencias",
        "data": [6, 8, 5],
        "backgroundColor": "#8B5CF6",
        "borderColor": "#7C3AED"
      }
    ]
  },
  "proximasClases": [
    {
      "idClase": "89e4567-e89b-12d3-a456-426614174000",
      "fecha": "2026-01-13T00:00:00",
      "horaInicio": "19:00:00",
      "tipoClase": "Tango Avanzado",
      "nombreProfesor": "Jorge Padilla",
      "cupoMaximo": 25,
      "inscritosActual": 18
    }
  ]
}
```

**Validaciones:**
- ✅ Extrae email del token JWT automáticamente
- ✅ Solo ve sus propios datos (ownership)
- ✅ Historial limitado a últimos 10 pagos
- ✅ Próximas clases: próximos 7 días, máximo 5
- ✅ Gráfica: últimos 6 meses de asistencias

**Casos de Error:**
```json
// Alumno no encontrado con ese email
{
  "error": "No se encontró el alumno autenticado."
}
```

---

### 8. GET /api/reportes/mis-clases - Mis Clases (Profesor)

**Endpoint:** `GET /api/reportes/mis-clases`  
**Autorización:** ApiScope (cualquier usuario autenticado)  
**Usuario:** Profesor Jorge

**Request:**
```
GET https://localhost:7194/api/reportes/mis-clases?fechaDesde=2026-01-01&fechaHasta=2026-01-31
```

**Headers:**
```
Authorization: Bearer {{token}}
```

**Respuesta Esperada (200 OK):**
```json
{
  "nombreProfesor": "Jorge Padilla",
  "correo": "Jorgepadilla@chetangoprueba.onmicrosoft.com",
  "totalClasesImpartidas": 45,
  "promedioAsistencia": 18.50,
  "alumnosUnicos": 32,
  "clasesProximas": [
    {
      "idClase": "12e4567-e89b-12d3-a456-426614174000",
      "fecha": "2026-01-13T00:00:00",
      "horaInicio": "19:00:00",
      "tipoClase": "Tango Avanzado",
      "nombreProfesor": "Jorge Padilla",
      "cupoMaximo": 25,
      "inscritosActual": 18
    }
  ],
  "graficaAsistenciaPorTipo": {
    "type": "bar",
    "labels": ["Tango Avanzado", "Tango Inicial", "Bachata"],
    "datasets": [
      {
        "label": "Asistencias",
        "data": [450, 320, 180],
        "backgroundColor": "#EC4899",
        "borderColor": "#DB2777"
      }
    ]
  },
  "desgloseporTipo": [
    {
      "nombreTipoClase": "Tango Avanzado",
      "cantidadClases": 20,
      "promedioAsistencia": 22.50,
      "ocupacionPromedio": 90.00
    },
    {
      "nombreTipoClase": "Tango Inicial",
      "cantidadClases": 15,
      "promedioAsistencia": 18.00,
      "ocupacionPromedio": 75.00
    }
  ]
}
```

**Validaciones:**
- ✅ Extrae email del token JWT automáticamente
- ✅ Solo ve sus propias clases (ownership)
- ✅ Alumnos únicos: conteo DISTINCT de alumnos presentes
- ✅ Ocupación promedio: % de asistencia vs cupo máximo
- ✅ Próximas clases: próximos 7 días

**Casos de Error:**
```json
// Profesor no encontrado con ese email
{
  "error": "No se encontró el profesor autenticado."
}

// Rango > 1 año
{
  "error": "El rango de fechas no puede ser mayor a 1 año."
}
```

---

## 📄 FASE 3: Exportaciones

### 9. GET /api/reportes/exportar - Exportar a Excel

**Endpoint:** `GET /api/reportes/exportar`  
**Autorización:** AdminOrProfesor  
**Usuario:** Admin, Profesor

**Request:**
```
GET https://localhost:7194/api/reportes/exportar?tipoReporte=asistencias&formato=excel&fechaDesde=2026-01-01&fechaHasta=2026-01-31
```

**Query Parameters:**
- `tipoReporte` (string): "asistencias", "ingresos", "paquetes", "clases"
- `formato` (string): "excel", "pdf", "csv"
- `fechaDesde` (DateTime): Fecha inicial
- `fechaHasta` (DateTime): Fecha final
- Parámetros opcionales según tipo de reporte

**Respuesta Esperada (200 OK):**
- Content-Type: `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`
- Archivo descargable: `reporte-asistencias-20260112.xlsx`

**Estructura Excel:**
- Título del reporte
- Métricas principales (KPIs)
- Tabla de datos detallados con formato
- Colores y bordes profesionales
- Columnas auto-ajustadas

---

### 10. GET /api/reportes/exportar - Exportar a PDF

**Request:**
```
GET https://localhost:7194/api/reportes/exportar?tipoReporte=ingresos&formato=pdf&fechaDesde=2026-01-01&fechaHasta=2026-01-31
```

**Respuesta Esperada (200 OK):**
- Content-Type: `application/pdf`
- Archivo descargable: `reporte-ingresos-20260112.pdf`

**Estructura PDF:**
- Header con logo/título
- Período del reporte
- Métricas en sección destacada
- Tablas con datos
- Footer con fecha de generación

---

### 11. GET /api/reportes/exportar - Exportar a CSV

**Request:**
```
GET https://localhost:7194/api/reportes/exportar?tipoReporte=paquetes&formato=csv&fechaDesde=2025-12-01&fechaHasta=2026-01-31
```

**Respuesta Esperada (200 OK):**
- Content-Type: `text/csv`
- Archivo descargable: `reporte-paquetes-20260112.csv`

**Formato CSV:**
```csv
Fecha,Alumno,Clase,Estado,Observaciones,Profesor
2026-01-10,Juan Pérez,Tango Avanzado,Presente,,Jorge Padilla
2026-01-10,María González,Tango Inicial,Ausente,Aviso previo,Jorge Padilla
```

---

### 12. Exportar Asistencias con Filtros

**Request:**
```
GET https://localhost:7194/api/reportes/exportar?tipoReporte=asistencias&formato=excel&fechaDesde=2026-01-01&fechaHasta=2026-01-31&idProfesor=8472BC4A-F83E-4A84-AB5B-ABD8C7D3E2AB
```

**Validaciones:**
- ✅ Todos los filtros de query params aplican
- ✅ Profesor solo puede exportar sus clases
- ✅ Admin puede exportar todo

---

## 🧪 FASE 4: Validaciones y Errores

### 13. Fecha Inicial Mayor a Final

**Request:**
```
GET https://localhost:7194/api/reportes/asistencias?fechaDesde=2026-01-31&fechaHasta=2026-01-01
```

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "La fecha inicial no puede ser mayor a la fecha final."
}
```

---

### 14. Fechas Futuras

**Request:**
```
GET https://localhost:7194/api/reportes/ingresos?fechaDesde=2027-01-01&fechaHasta=2027-01-31
```

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "No se pueden generar reportes de fechas futuras."
}
```

---

### 15. Rango Mayor a 1 Año

**Request:**
```
GET https://localhost:7194/api/reportes/clases?fechaDesde=2024-01-01&fechaHasta=2026-01-31
```

**Respuesta Esperada (400 Bad Request):**
```json
{
  "error": "El rango de fechas no puede ser mayor a 1 año."
}
```

---

### 16. Sin Autorización (401)

**Request:**
```
GET https://localhost:7194/api/reportes/dashboard
```

**Sin Header de Authorization**

**Respuesta Esperada (401 Unauthorized):**
```json
{
  "error": "No autorizado"
}
```

---

### 17. Sin Permisos Admin (403)

**Request:**
```
GET https://localhost:7194/api/reportes/ingresos?fechaDesde=2026-01-01&fechaHasta=2026-01-31
```

**Usuario:** Profesor (sin rol admin)

**Respuesta Esperada (403 Forbidden):**
```json
{
  "error": "Acceso denegado"
}
```

---

### 18. Profesor Accediendo a Clases de Otro (Ownership)

**Request:**
```
GET https://localhost:7194/api/reportes/asistencias?fechaDesde=2026-01-01&fechaHasta=2026-01-31&idProfesor={OTRO_PROFESOR_GUID}
```

**Usuario:** Profesor Jorge

**Respuesta Esperada (200 OK):**
```json
{
  "totalAsistencias": 0,
  "presentes": 0,
  "ausentes": 0,
  "justificadas": 0,
  "porcentajeAsistencia": 0,
  "listaDetallada": [],
  "graficaAsistenciasPorDia": {
    "type": "bar",
    "labels": [],
    "datasets": []
  }
}
```

**Validación:** No muestra error, pero lista vacía porque no tiene clases con ese profesor

---

## 📊 FASE 5: Verificación de Datos

### 19. Verificar Cache del Dashboard

**Test:**
1. Llamar `GET /api/reportes/dashboard` → Medir tiempo de respuesta
2. Llamar nuevamente dentro de 5 minutos → Debe ser más rápido
3. Esperar 5+ minutos → Llamar nuevamente → Tiempo normal

**Validación:**
- ✅ Primera llamada: ~500-1000ms
- ✅ Segunda llamada (cache): ~50-100ms
- ✅ Después de 5 min: cache expirado, tiempo normal

---

### 20. Verificar ClasesRestantes Calculado

**Test:**
1. Crear paquete con `ClasesDisponibles = 8`, `ClasesUsadas = 0`
2. Llamar `GET /api/reportes/mi-reporte` como alumno
3. Verificar `paqueteActual.clasesRestantes = 8`
4. Usar una clase (incrementar `ClasesUsadas`)
5. Llamar nuevamente
6. Verificar `paqueteActual.clasesRestantes = 7`

**Validación:**
- ✅ `ClasesRestantes` = `ClasesDisponibles - ClasesUsadas`

---

### 21. Verificar Ownership en Múltiples Usuarios

**Test:**
1. Login como Profesor Jorge → `GET /api/reportes/mis-clases` → Debe ver sus clases
2. Login como otro profesor → `GET /api/reportes/mis-clases` → Debe ver SOLO sus clases
3. Login como Admin → `GET /api/reportes/clases` → Debe ver TODAS las clases

**Validación:**
- ✅ Cada profesor ve solo sus datos
- ✅ Admin ve todo sin restricciones

---

## 🎯 Checklist de Pruebas Completas

### Reportes Administrativos (Admin)
- [ ] GET /api/reportes/asistencias (sin filtros)
- [ ] GET /api/reportes/asistencias (con filtro idProfesor)
- [ ] GET /api/reportes/asistencias (con filtro estadoAsistencia)
- [ ] GET /api/reportes/ingresos (sin comparativa)
- [ ] GET /api/reportes/ingresos (con comparativa=true)
- [ ] GET /api/reportes/paquetes (sin filtros)
- [ ] GET /api/reportes/paquetes (con filtro estado=Activo)
- [ ] GET /api/reportes/clases (sin filtros)
- [ ] GET /api/reportes/alumnos (sin filtros)
- [ ] GET /api/reportes/alumnos (con filtro estado=Activo)
- [ ] GET /api/reportes/dashboard

### Reportes Personales
- [ ] GET /api/reportes/mi-reporte (como Alumno Juan David)
- [ ] GET /api/reportes/mis-clases (como Profesor Jorge)

### Reportes Profesor (Ownership)
- [ ] GET /api/reportes/asistencias (como Profesor Jorge)
- [ ] GET /api/reportes/clases (como Profesor Jorge)

### Exportaciones
- [ ] Exportar asistencias a Excel
- [ ] Exportar asistencias a PDF
- [ ] Exportar asistencias a CSV
- [ ] Exportar ingresos a Excel
- [ ] Exportar paquetes a PDF
- [ ] Exportar clases a CSV

### Validaciones de Errores
- [ ] Fecha inicial > fecha final (400)
- [ ] Fechas futuras (400)
- [ ] Rango > 1 año (400)
- [ ] Sin token (401)
- [ ] Usuario sin permisos (403)
- [ ] Profesor accediendo a datos de otro profesor (ownership)

### Verificaciones Técnicas
- [ ] Cache del dashboard (llamar 2 veces en <5 min)
- [ ] ClasesRestantes calculado correctamente
- [ ] ChartDataDTO con estructura correcta para Chart.js
- [ ] Estados usando .Nombre en lugar de comparar objetos
- [ ] MontoTotal en lugar de Monto

---

## 📝 Notas Finales

### Prerequisitos para Pruebas:
1. ✅ API corriendo en `https://localhost:7194`
2. ✅ Base de datos ChetangoDB_Dev con datos de prueba
3. ✅ Token OAuth 2.0 configurado en Postman con auto-refresh
4. ✅ Al menos 1 alumno, 1 profesor, 1 clase, 1 pago, 1 paquete en BD

### Comandos Útiles:
```powershell
# Levantar API
dotnet run --project Chetango.Api/Chetango.Api.csproj --launch-profile https-qa

# Verificar compilación
dotnet build

# Limpiar y rebuild
dotnet clean
dotnet build
```

### Usuarios Recomendados para Cada Prueba:
- **Dashboard, Ingresos, Paquetes, Alumnos:** Admin (Chetango@...)
- **Asistencias, Clases (todos):** Admin (Chetango@...)
- **Asistencias, Clases (ownership):** Profesor (Jorgepadilla@...)
- **Mi Reporte:** Alumno (JuanDavid@...)
- **Mis Clases:** Profesor (Jorgepadilla@...)
- **Exportaciones:** Admin o Profesor

### Tiempos de Respuesta Esperados:
- Reportes simples: 200-500ms
- Reportes complejos (Dashboard): 500-1000ms
- Dashboard con cache: <100ms
- Exportaciones: 1-3 segundos

---

**Fecha de Creación:** 2026-01-12  
**Módulo:** Reportes  
**Versión:** 1.0  
**Estado:** ✅ Compilación Exitosa
