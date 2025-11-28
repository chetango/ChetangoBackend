# API Admin – Asistencias

Endpoints agrupados bajo `/api/admin/asistencias/*`. Todos requieren:

- Autenticación JWT válida (política `ApiScope`).
- Rol/política `ApiRole` cuando haya roles configurados para administradores.
- Encabezado `Authorization: Bearer {token}`.

> **Formato JSON**: Los `DateOnly` se serializan como `YYYY-MM-DD` y los `TimeOnly` como `HH:mm:ss`.

---

## 1. GET `/api/admin/asistencias/dias-con-clases`
Obtiene el rango de los últimos 7 días útiles para el calendario y marca qué fechas poseen clases.

### Ejemplo de respuesta `200 OK`
```json
{
  "hoy": "2025-01-15",
  "desde": "2025-01-09",
  "hasta": "2025-01-15",
  "diasConClases": [
    "2025-01-10",
    "2025-01-12",
    "2025-01-13",
    "2025-01-15"
  ]
}
```

Posibles `400 Bad Request`: errores inesperados desde la consulta.

---

## 2. GET `/api/admin/asistencias/clases-del-dia?fecha=2025-01-15`
Devuelve las clases disponibles para la fecha seleccionada. La query string `fecha` es obligatoria.

### Ejemplo de respuesta `200 OK`
```json
{
  "fecha": "2025-01-15",
  "clases": [
    {
      "idClase": "c9d5af98-2b1a-4cb4-a1a8-13f3381c93ef",
      "nombre": "Tango Intermedio",
      "horaInicio": "19:00:00",
      "horaFin": "20:30:00",
      "profesorPrincipal": "Prof. María Gómez"
    },
    {
      "idClase": "c6a8a2a0-1b5f-44e1-9bcd-9f1197f4a3ed",
      "nombre": "Técnica Individual",
      "horaInicio": "20:30:00",
      "horaFin": "21:30:00",
      "profesorPrincipal": "Prof. Martín Ruiz"
    }
  ]
}
```

Error `400`: formato de fecha inválido o fallo en la consulta.

---

## 3. GET `/api/admin/asistencias/clase/{idClase}/resumen`
Entrega la información completa para la tarjeta de la clase (alumnos, paquetes, asistencias y contadores).

### Ejemplo de respuesta `200 OK`
```json
{
  "idClase": "c9d5af98-2b1a-4cb4-a1a8-13f3381c93ef",
  "fecha": "2025-01-15",
  "nombreClase": "Tango Intermedio - 19:00",
  "profesorPrincipal": "Prof. María Gómez",
  "alumnos": [
    {
      "idAlumno": "3a7db4a6-3f25-4f33-8b93-0a68b6c1a90b",
      "nombreCompleto": "María Rodríguez",
      "documentoIdentidad": "42.567.123",
      "avatarIniciales": "MR",
      "paquete": {
        "estado": "Activo",
        "descripcion": "Paquete 8 Clases",
        "clasesTotales": 8,
        "clasesUsadas": 3,
        "clasesRestantes": 5
      },
      "asistencia": {
        "estado": "Ausente",
        "observacion": null
      }
    },
    {
      "idAlumno": "c2eb2364-700f-4df3-b035-534233bf11fc",
      "nombreCompleto": "Diego Sánchez",
      "documentoIdentidad": "39.567.890",
      "avatarIniciales": "DS",
      "paquete": {
        "estado": "Activo",
        "descripcion": "Paquete 12 Clases",
        "clasesTotales": 12,
        "clasesUsadas": 7,
        "clasesRestantes": 5
      },
      "asistencia": {
        "estado": "Presente",
        "observacion": "Llegó tarde"
      }
    },
    {
      "idAlumno": "4af7f7c8-6a02-47ba-8a87-2e4178dbdfc2",
      "nombreCompleto": "Carlos Martínez",
      "documentoIdentidad": "40.123.789",
      "avatarIniciales": "CM",
      "paquete": {
        "estado": "SinPaquete",
        "descripcion": null,
        "clasesTotales": null,
        "clasesUsadas": null,
        "clasesRestantes": null
      },
      "asistencia": {
        "estado": "Ausente",
        "observacion": null
      }
    }
  ],
  "presentes": 2,
  "ausentes": 4,
  "sinPaquete": 1
}
```

### Respuestas de error
- `404 Not Found`: la clase no existe (mensaje "Clase no encontrada").
- `400 Bad Request`: otro error no controlado en la consulta.

---

## Estados manejados
- **Paquetes**: `Activo`, `Agotado`, `Congelado`, `SinPaquete`.
- **Asistencias**: `Presente`, `Ausente`.

Estos valores llegan en texto plano para que el frontend pueda mapear estilos directamente.

---

## Resumen rápido (para UI)
| Endpoint | Uso en UI |
| --- | --- |
| `/dias-con-clases` | Pinta el calendario restringido a fechas con clases (últimos 7 días). |
| `/clases-del-dia` | Alimenta el combo "Clase del Día" según la fecha que eligió el usuario. |
| `/clase/{id}/resumen` | Llena la grilla de alumnos, paquetes, estados de asistencia y contadores del módulo principal. |
