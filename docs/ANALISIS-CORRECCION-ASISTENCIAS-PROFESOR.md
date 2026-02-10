# ANÁLISIS PARA CORRECCIÓN - Vista de Asistencias del Profesor

**Fecha:** 6 Febrero 2026  
**Objetivo:** Permitir que el profesor vea TODOS los alumnos activos, no solo los que tienen asistencia registrada

---

## 🔍 ESTADO ACTUAL

### Endpoint Profesor
```
GET /api/clases/{idClase}/asistencias
Handler: GetAsistenciasPorClaseQueryHandler
Línea: Program.cs línea 918-960
```

**Problema:**
```csharp
// Línea 17-20 de GetAsistenciasPorClaseQueryHandler.cs
var asistencias = await _db.Asistencias
    .Where(a => a.IdClase == request.IdClase)  // ❌ SOLO asistencias YA registradas
    .ToListAsync();
```

**Resultado:** Si no hay asistencias previas → lista vacía → profesor no ve ningún alumno

---

## ✅ ENDPOINT QUE FUNCIONA CORRECTAMENTE (Admin)

### Endpoint Admin
```
GET /api/admin/asistencias/clase/{idClase}/resumen
Handler: GetResumenAsistenciasClaseAdminQueryHandler
Línea: Program.cs línea 338-342
```

**Solución correcta:**
```csharp
// Línea 56-60 de GetResumenAsistenciasClaseAdminQueryHandler.cs
var alumnosActivos = await _db.Set<Alumno>()
    .Include(a => a.Usuario).ThenInclude(u => u.Estado)
    .Where(a => a.Usuario.Estado.Nombre == "Activo")
    .ToListAsync();

// Línea 76: Para cada alumno, busca si tiene asistencia
var asistencia = clase.Asistencias.FirstOrDefault(a => a.IdAlumno == alumno.IdAlumno);

// Línea 118-122: Si no tiene asistencia, crea DTO con valores por defecto
var estadoAsistenciaAdmin = asistencia is null
    ? EstadoAsistenciaAdmin.Ausente  // ✅ Por defecto "Ausente"
    : (asistencia.Estado?.Nombre == "Presente" 
        ? EstadoAsistenciaAdmin.Presente 
        : EstadoAsistenciaAdmin.Ausente);
```

---

## 📋 DTOs ACTUALES

### AsistenciaDto (Usado por profesor)
```csharp
public record AsistenciaDto(
    Guid IdAsistencia,        // ❌ No puede ser null → problema
    Guid IdClase,
    DateTime FechaClase,
    TimeSpan HoraInicio,
    TimeSpan HoraFin,
    string TipoClase,
    Guid IdAlumno,
    string NombreAlumno,
    string EstadoAsistencia,  // "Presente" | "Ausente"
    Guid? IdPaqueteUsado,
    int IdTipoAsistencia,
    string TipoAsistencia,
    string? Observacion
);
```

### AlumnoEnClaseAdminDto (Usado por admin)
```csharp
public sealed class AlumnoEnClaseAdminDto
{
    Guid IdAlumno
    string NombreCompleto
    string DocumentoIdentidad
    string AvatarIniciales
    PaqueteAlumnoAdminDto Paquete
    AsistenciaAlumnoAdminDto Asistencia  // ✅ Puede tener IdAsistencia null
}

public sealed class AsistenciaAlumnoAdminDto
{
    Guid? IdAsistencia  // ✅ Nullable
    EstadoAsistenciaAdmin Estado
    string? Observacion
}
```

---

## 🎯 OPCIONES DE SOLUCIÓN

### Opción A: Modificar AsistenciaDto (Breaking Change ❌)
```csharp
public record AsistenciaDto(
    Guid? IdAsistencia,  // ⚠️ Cambio de signature
    ...
);
```
**Problema:** Rompe contrato existente, requiere cambios en frontend

### Opción B: Crear nuevo Query para Profesor (RECOMENDADO ✅)
```csharp
// Nuevo handler: GetResumenAsistenciasClaseProfesorQueryHandler
// Nuevo DTO: ResumenAsistenciasClaseProfesorDto
// Nuevo endpoint: GET /api/clases/{idClase}/asistencias/resumen
```
**Ventajas:**
- No rompe nada existente
- Específico para necesidades del profesor
- Reutiliza lógica del admin

### Opción C: Usar directamente endpoint Admin (Rápido ✅)
```csharp
// Modificar Program.cs línea 918
// Cambiar de GetAsistenciasPorClaseQuery 
// a GetResumenAsistenciasClaseAdminQuery
```
**Ventajas:**
- Cero cambios en handlers
- Ya funciona correctamente
**Desventajas:**
- Frontend debe adaptarse a DTOs admin

---

## 📊 ENTIDADES DEL DOMINIO

### Alumno
```csharp
public class Alumno
{
    Guid IdAlumno
    Guid IdUsuario
    Usuario Usuario  // ✅ Tiene NombreUsuario, NumeroDocumento
    
    [NotMapped]
    string NombreCompleto => Usuario?.NombreUsuario
    
    [NotMapped]
    string DocumentoIdentidad => Usuario?.NumeroDocumento
    
    DateTime FechaInscripcion
    int IdEstado
    EstadoAlumno Estado  // ✅ Para filtrar por "Activo"
}
```

### Usuario.Estado
```
IdEstado: 1 = "Activo"
Necesitamos filtrar: a.Usuario.Estado.Nombre == "Activo"
```

---

## 🔄 CONSUMO EN FRONTEND

### Frontend Profesor
```typescript
// profesorQueries.ts línea 130-149
export function useAsistenciasClaseQuery(idClase: string | null) {
  return useQuery({
    queryFn: async (): Promise<AsistenciasClaseResponse[]> => {
      const response = await httpClient.get<any[]>(
        `/api/clases/${idClase}/asistencias`
      )
      
      // ✅ Ya mapea correctamente
      const asistencias: AsistenciasClaseResponse[] = response.data.map(...)
      return asistencias
    }
  })
}
```

### Tipo esperado por frontend
```typescript
export interface AsistenciasClaseResponse {
  idAsistencia: string | null  // ✅ Ya soporta null
  idAlumno: string
  nombreAlumno: string
  presente: boolean
  observacion: string | null
  estadoPaquete?: string | number
  clasesRestantes?: number | null
}
```

---

## ✅ SOLUCIÓN ELEGIDA: Opción B Modificada

**Crear nuevo handler específico para profesor que:**

1. Lista TODOS los alumnos activos
2. Para cada alumno, busca si tiene asistencia registrada
3. Devuelve AsistenciaDto con `IdAsistencia?` nullable
4. Incluye info de paquete activo

**Ventajas:**
- No rompe nada existente
- Frontend ya soporta `idAsistencia: null`
- Reutiliza lógica probada del admin
- Específico para necesidades del profesor

---

## 📝 PASOS DE IMPLEMENTACIÓN

### 1. Crear nuevo DTO compatible
```csharp
// AsistenciaProfesorDto.cs
public record AsistenciaProfesorDto(
    Guid? IdAsistencia,  // Nullable
    Guid IdClase,
    Guid IdAlumno,
    string NombreAlumno,
    string DocumentoIdentidad,
    bool Presente,
    string? Observacion,
    string? EstadoPaquete,
    int? ClasesRestantes,
    Guid? IdPaquete
);
```

### 2. Crear nuevo Query
```csharp
// GetAsistenciasClaseConAlumnosQuery.cs
public record GetAsistenciasClaseConAlumnosQuery(Guid IdClase) 
    : IRequest<Result<IReadOnlyList<AsistenciaProfesorDto>>>;
```

### 3. Crear Handler
```csharp
// GetAsistenciasClaseConAlumnosQueryHandler.cs
// Lógica similar a GetResumenAsistenciasClaseAdminQueryHandler
// Pero devuelve lista plana de AsistenciaProfesorDto
```

### 4. Actualizar endpoint
```csharp
// Program.cs línea 934 y 957
// Cambiar:
var query = new GetAsistenciasPorClaseQuery(idClase);
// Por:
var query = new GetAsistenciasClaseConAlumnosQuery(idClase);
```

### 5. Frontend
```typescript
// ✅ No requiere cambios (ya soporta idAsistencia: null)
```

---

## ⚠️ VALIDACIONES NECESARIAS

1. ✅ Filtrar solo alumnos con Usuario.Estado.Nombre == "Activo"
2. ✅ Incluir datos de paquete activo (si existe)
3. ✅ IdAsistencia null si no hay registro previo
4. ✅ Presente = false por defecto
5. ✅ Ordenar por NombreUsuario
6. ✅ Ownership validation ya existe en Program.cs

---

## 🧪 CASOS DE PRUEBA

### Caso 1: Clase nueva sin asistencias
- Input: Clase sin registros en tabla Asistencias
- Expected: Lista de TODOS los alumnos activos con presente=false

### Caso 2: Clase con algunas asistencias
- Input: Clase con 3 asistencias registradas de 10 alumnos
- Expected: Lista de 10 alumnos (3 con presente=true, 7 con presente=false)

### Caso 3: Alumno sin paquete
- Input: Alumno activo sin paquetes activos
- Expected: estadoPaquete="SinPaquete", clasesRestantes=null

### Caso 4: Alumno con paquete
- Input: Alumno con paquete activo de 8 clases, 3 usadas
- Expected: estadoPaquete="Activo", clasesRestantes=5

---

**Autor:** GitHub Copilot  
**Revisión:** Pendiente aprobación del usuario
