# Plan de Implementación - Módulo Nómina Profesores

## 📋 Resumen
Implementar sistema de gestión de pagos a profesores siguiendo arquitectura existente.

## 🎯 Requisitos
- Mantener patrón CQRS (Commands/Queries/Handlers)
- Seguir estructura de carpetas actual
- Usar mismos estilos visuales (SCSS Modules, design tokens)
- Integrar con módulo de Clases existente
- No modificar código funcionando sin necesidad

---

## 📊 FASE 1: Modelo de Datos (Backend)

### 1.1 Entidades Nuevas

**RolEnClase.cs** (ya existe en TarifaProfesor, verificar)
```csharp
namespace Chetango.Domain.Entities.Estados
{
    public class RolEnClase
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!; // "Principal" | "Monitor"
    }
}
```

**ClaseProfesor.cs** (NUEVA - vincula clases con profesores y sus pagos)
```csharp
namespace Chetango.Domain.Entities
{
    public class ClaseProfesor
    {
        public Guid IdClaseProfesor { get; set; }
        public Guid IdClase { get; set; }
        public Clase Clase { get; set; } = null!;
        public Guid IdProfesor { get; set; }
        public Profesor Profesor { get; set; } = null!;
        public Guid IdRolEnClase { get; set; }
        public RolEnClase RolEnClase { get; set; } = null!;
        
        // Tarifas y Pagos
        public decimal TarifaProgramada { get; set; } // Calculada al crear clase
        public decimal ValorAdicional { get; set; } = 0; // Ajustes manuales
        public string? ConceptoAdicional { get; set; } // Razón del ajuste
        public decimal TotalPago { get; set; } // TarifaProgramada + ValorAdicional
        
        // Estado del Pago
        public string EstadoPago { get; set; } = "Pendiente"; // Pendiente/Aprobado/Liquidado/Pagado
        public DateTime? FechaAprobacion { get; set; }
        public DateTime? FechaPago { get; set; }
        public Guid? AprobadoPorIdUsuario { get; set; }
        public Usuario? AprobadoPor { get; set; }
        
        // Auditoría
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
```

**LiquidacionMensual.cs** (NUEVA - resumen mensual por profesor)
```csharp
namespace Chetango.Domain.Entities
{
    public class LiquidacionMensual
    {
        public Guid IdLiquidacion { get; set; }
        public Guid IdProfesor { get; set; }
        public Profesor Profesor { get; set; } = null!;
        
        public int Mes { get; set; } // 1-12
        public int Año { get; set; }
        
        public int TotalClases { get; set; }
        public decimal TotalHoras { get; set; }
        public decimal TotalBase { get; set; }
        public decimal TotalAdicionales { get; set; }
        public decimal TotalPagar { get; set; }
        
        public string Estado { get; set; } = "EnProceso"; // EnProceso/Cerrada/Pagada
        public DateTime? FechaCierre { get; set; }
        public DateTime? FechaPago { get; set; }
        public string? Observaciones { get; set; }
        
        public DateTime FechaCreacion { get; set; }
        public Guid CreadoPorIdUsuario { get; set; }
    }
}
```

### 1.2 Modificaciones a Entidades Existentes

**Clase.cs** - AGREGAR relación con ClaseProfesor
```csharp
// Agregar a la clase Clase:
public ICollection<ClaseProfesor> Profesores { get; set; } = new List<ClaseProfesor>();
```

**Profesor.cs** - Ya tiene `Tarifas`, agregar:
```csharp
public ICollection<ClaseProfesor> ClasesProfesores { get; set; } = new List<ClaseProfesor>();
public ICollection<LiquidacionMensual> Liquidaciones { get; set; } = new List<LiquidacionMensual>();
```

### 1.3 Migración
```bash
dotnet ef migrations add AgregarSistemaNomina \
  --project Chetango.Infrastructure/Chetango.Infrastructure.csproj \
  --startup-project Chetango.Api/Chetango.Api.csproj
```

### 1.4 Seed Data (TarifasProfesor)
Agregar a `DataSeeder.cs` o crear seed específico:
```csharp
// Jorge Padilla y Ana Zoraida: $40,000/hora (Grupal), $35,000 (Privada)
// Santiago, María Alejandra, Brandon, Susana, Laura, Daniel, Carolina: $30,000/hora
// Monitores: $10,000 por clase
```

---

## 📁 FASE 2: Backend - Application Layer

### 2.1 Estructura de Carpetas
```
Chetango.Application/
└── Nomina/
    ├── DTOs/
    │   ├── ClaseProfesorDTO.cs
    │   ├── ClaseRealizadaDTO.cs
    │   ├── LiquidacionDetalleDTO.cs
    │   ├── LiquidacionMensualDTO.cs
    │   ├── AprobarPagoDTO.cs
    │   └── ResumenProfesorDTO.cs
    ├── Commands/
    │   ├── AprobarPagoClase/
    │   │   ├── AprobarPagoClaseCommand.cs
    │   │   └── AprobarPagoClaseCommandHandler.cs
    │   ├── LiquidarMes/
    │   │   ├── LiquidarMesCommand.cs
    │   │   └── LiquidarMesCommandHandler.cs
    │   └── RegistrarPagoProfesor/
    │       ├── RegistrarPagoProfesorCommand.cs
    │       └── RegistrarPagoProfesorCommandHandler.cs
    └── Queries/
        ├── GetClasesRealizadas/
        │   ├── GetClasesRealizadasQuery.cs
        │   └── GetClasesRealizadasQueryHandler.cs
        ├── GetClasesAprobadas/
        │   ├── GetClasesAprobadasQuery.cs
        │   └── GetClasesAprobadasQueryHandler.cs
        ├── GetLiquidacionMensual/
        │   ├── GetLiquidacionMensualQuery.cs
        │   └── GetLiquidacionMensualQueryHandler.cs
        └── GetResumenProfesor/
            ├── GetResumenProfesorQuery.cs
            └── GetResumenProfesorQueryHandler.cs
```

### 2.2 Endpoints (Program.cs)
```csharp
// GRUPO: /api/nomina (AdminOnly)
var nomina = app.MapGroup("/api/nomina").RequireAuthorization("AdminOnly");

// GET /api/nomina/clases-realizadas - Clases realizadas sin aprobar
nomina.MapGet("/clases-realizadas", async (IMediator mediator) => { ... });

// GET /api/nomina/clases-aprobadas - Clases aprobadas este mes
nomina.MapGet("/clases-aprobadas", async (IMediator mediator, int? mes, int? año) => { ... });

// POST /api/nomina/aprobar-pago - Aprobar pago de clase
nomina.MapPost("/aprobar-pago", async (IMediator mediator, AprobarPagoDTO dto) => { ... });

// GET /api/nomina/liquidacion/{mes}/{año} - Ver liquidación del mes
nomina.MapGet("/liquidacion/{mes:int}/{año:int}", async (IMediator mediator, int mes, int año) => { ... });

// POST /api/nomina/liquidar-mes - Liquidar y cerrar mes
nomina.MapPost("/liquidar-mes", async (IMediator mediator, LiquidarMesCommand cmd) => { ... });

// GET /api/nomina/resumen-profesor/{idProfesor} - Resumen de un profesor
nomina.MapGet("/resumen-profesor/{idProfesor:guid}", async (IMediator mediator, Guid idProfesor) => { ... });
```

---

## 🎨 FASE 3: Frontend

### 3.1 Estructura de Carpetas
```
src/features/
└── payroll/              ← NUEVO MÓDULO
    ├── api/
    │   └── payrollQueries.ts
    ├── components/
    │   ├── PayrollDashboard.tsx
    │   ├── ClassPaymentCard.tsx
    │   ├── ProfessorSummaryCard.tsx
    │   ├── ApprovePaymentModal.tsx
    │   ├── LiquidationDetailModal.tsx
    │   └── index.ts
    ├── hooks/
    │   └── usePayrollFilters.ts
    ├── store/
    │   └── payrollStore.ts (opcional)
    ├── types/
    │   └── payroll.types.ts
    └── index.ts
```

### 3.2 Routing (seguir patrón de routes/index.tsx)
```tsx
// src/app/router/routes/index.tsx
{
  path: 'payroll',
  element: <PayrollPage />,
  handle: { title: 'Nómina Profesores' }
}
```

### 3.3 Sidebar (MainLayout.tsx)
```tsx
<NavLink to="/admin/payroll" className={...}>
  <DollarSign size={20} />
  Nómina
</NavLink>
```

### 3.4 Componentes Reutilizables
- Usar **mismo patrón Kanban** que AdminPaymentsPage.tsx
- Usar **mismos estilos SCSS** que pagos (colores, cards, borders)
- Reutilizar componentes: Modal base, LoadingSpinner, EmptyState

---

## ✅ FASE 4: Integración con Módulo Clases

### 4.1 Modificar Creación de Clases
**CrearClaseCommandHandler.cs** - Agregar lógica:
```csharp
// Después de crear la clase, crear ClaseProfesor
var duracionHoras = (command.HoraFin - command.HoraInicio).TotalHours;
var tarifaProfesor = await ObtenerTarifaProfesor(command.IdProfesorPrincipal, tipoClase);

var claseProfesor = new ClaseProfesor {
    IdClase = nuevaClase.IdClase,
    IdProfesor = command.IdProfesorPrincipal,
    IdRolEnClase = rolPrincipalId,
    TarifaProgramada = tarifaProfesor.ValorPorClase * duracionHoras,
    TotalPago = tarifaProfesor.ValorPorClase * duracionHoras,
    EstadoPago = "Pendiente"
};

db.ClasesProfesores.Add(claseProfesor);

// Repetir para monitores con tarifa de monitor
```

### 4.2 Frontend - Modal de Crear Clase
**ClassScheduleModal.tsx** - Agregar sección:
```tsx
<div className="tariff-section">
  <h3>💰 Configuración de Pagos</h3>
  
  <div className="professor-tariff">
    <label>Profesor Principal:</label>
    <div>{selectedProfessor.nombre}</div>
    <div className="tariff-amount">
      Tarifa: ${tarifaCalculada.toLocaleString()} × {duracionHoras}h
      = ${totalProfesor.toLocaleString()}
    </div>
  </div>
  
  {monitors.map(monitor => (
    <div key={monitor.id}>
      {monitor.nombre}: $10,000
    </div>
  ))}
  
  <div className="total-cost">
    <strong>Costo Total Clase:</strong> ${costoTotal.toLocaleString()}
  </div>
</div>
```

---

## 🔄 FASE 5: Testing

### 5.1 Backend Tests
- `AprobarPagoClaseCommandHandlerTests.cs`
- `GetClasesRealizadasQueryHandlerTests.cs`
- `LiquidarMesCommandHandlerTests.cs`

### 5.2 Frontend Tests
- `PayrollDashboard.test.tsx`
- `ApprovePaymentModal.test.tsx`
- `payrollQueries.test.ts`

---

## 📝 FASE 6: Documentación

### 6.1 Crear Docs
- `docs/API-CONTRACT-NOMINA.md` - Endpoints y DTOs
- `docs/frontend-backend-alignment-nomina.md` - Mapeo de tipos
- `docs/test-modulo-nomina.md` - Casos de prueba

---

## 🚀 Plan de Ejecución

### Orden de Implementación:
1. ✅ **Revisar código existente** (COMPLETADO)
2. 🔨 Crear entidades y migración (Backend)
3. 🔨 Implementar Commands/Queries (Backend)
4. 🔨 Agregar endpoints en Program.cs (Backend)
5. 🔨 Seed de tarifas iniciales (Backend)
6. 🎨 Crear estructura frontend (Carpetas + routing)
7. 🎨 Implementar componentes visuales (Frontend)
8. 🎨 Integrar con módulo Clases (Frontend + Backend)
9. ✅ Testing end-to-end
10. 📝 Documentación

### Tiempo Estimado: 2-3 sesiones de trabajo

---

## ⚠️ Reglas Importantes

1. **NO modificar código existente** sin razón clara
2. **Seguir patrones** de módulos payments y classes
3. **Reutilizar componentes** cuando sea posible
4. **Validar cada paso** antes de continuar
5. **Hacer commits incrementales** por funcionalidad

---

¿Comenzamos con la Fase 2 (Entidades y Migración)?
