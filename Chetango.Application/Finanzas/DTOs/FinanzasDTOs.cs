using Chetango.Domain.Enums;

namespace Chetango.Application.Finanzas.DTOs;

/// <summary>
/// DTO para representar un ingreso adicional (no de pagos de alumnos)
/// </summary>
public class OtroIngresoDTO
{
    public Guid IdOtroIngreso { get; set; }
    public string Concepto { get; set; } = null!;
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public Sede Sede { get; set; }
    public string SedeName => Sede.ToString();
    public Guid? IdCategoriaIngreso { get; set; }
    public string? NombreCategoria { get; set; }
    public string? Descripcion { get; set; }
    public string? UrlComprobante { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string UsuarioCreacion { get; set; } = null!;
}

/// <summary>
/// DTO para representar un gasto adicional (no nómina de profesores)
/// </summary>
public class OtroGastoDTO
{
    public Guid IdOtroGasto { get; set; }
    public string Concepto { get; set; } = null!;
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public Sede Sede { get; set; }
    public string SedeName => Sede.ToString();
    public Guid? IdCategoriaGasto { get; set; }
    public string? NombreCategoria { get; set; }
    public string? Proveedor { get; set; }
    public string? Descripcion { get; set; }
    public string? UrlFactura { get; set; }
    public string? NumeroFactura { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string UsuarioCreacion { get; set; } = null!;
}

/// <summary>
/// DTO para categorías de ingresos
/// </summary>
public class CategoriaIngresoDTO
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
}

/// <summary>
/// DTO para categorías de gastos
/// </summary>
public class CategoriaGastoDTO
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
}

/// <summary>
/// DTO para resumen financiero completo con desglose
/// </summary>
public class ResumenFinancieroDTO
{
    // Ingresos
    public decimal TotalIngresosAlumnos { get; set; }
    public decimal TotalOtrosIngresos { get; set; }
    public decimal TotalIngresos => TotalIngresosAlumnos + TotalOtrosIngresos;
    
    // Egresos
    public decimal TotalNominaProfesores { get; set; }
    public decimal TotalOtrosGastos { get; set; }
    public decimal TotalEgresos => TotalNominaProfesores + TotalOtrosGastos;
    
    // Ganancia
    public decimal GananciaNeta => TotalIngresos - TotalEgresos;
    
    // Desglose por Sede
    public ResumenPorSedeDTO? Medellin { get; set; }
    public ResumenPorSedeDTO? Manizales { get; set; }
}

/// <summary>
/// DTO para resumen por sede
/// </summary>
public class ResumenPorSedeDTO
{
    public Sede Sede { get; set; }
    public decimal IngresosAlumnos { get; set; }
    public decimal OtrosIngresos { get; set; }
    public decimal TotalIngresos => IngresosAlumnos + OtrosIngresos;
    
    public decimal NominaProfesores { get; set; }
    public decimal OtrosGastos { get; set; }
    public decimal TotalEgresos => NominaProfesores + OtrosGastos;
    
    public decimal GananciaNeta => TotalIngresos - TotalEgresos;
}

/// <summary>
/// DTO para crear un nuevo ingreso adicional
/// </summary>
public class CrearOtroIngresoDTO
{
    public string Concepto { get; set; } = null!;
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public Sede Sede { get; set; }
    public Guid? IdCategoriaIngreso { get; set; }
    public string? Descripcion { get; set; }
    public string? UrlComprobante { get; set; }
}

/// <summary>
/// DTO para crear un nuevo gasto adicional
/// </summary>
public class CrearOtroGastoDTO
{
    public string Concepto { get; set; } = null!;
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public Sede Sede { get; set; }
    public Guid? IdCategoriaGasto { get; set; }
    public string? Proveedor { get; set; }
    public string? Descripcion { get; set; }
    public string? UrlFactura { get; set; }
    public string? NumeroFactura { get; set; }
}
