namespace Chetango.Application.Reportes.DTOs;

/// <summary>
/// Enum para tipos de reporte exportable
/// </summary>
public enum TipoReporte
{
    Asistencias,
    Ingresos,
    Paquetes,
    Clases,
    Alumnos
}

/// <summary>
/// Enum para formatos de exportación
/// </summary>
public enum FormatoExportacion
{
    PDF,
    Excel,
    CSV
}
