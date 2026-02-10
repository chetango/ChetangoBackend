using Chetango.Application.Reportes.DTOs;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Chetango.Application.Reportes.Services;

/// <summary>
/// Servicio para exportar reportes a CSV usando CsvHelper
/// </summary>
public class CsvExportService
{
    public byte[] ExportarAsistencias(ReporteAsistenciasDTO reporte)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ","
        };

        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream);
        using var csvWriter = new CsvWriter(streamWriter, config);

        // Headers
        csvWriter.WriteField("Fecha");
        csvWriter.WriteField("Alumno");
        csvWriter.WriteField("Clase");
        csvWriter.WriteField("Estado");
        csvWriter.WriteField("Profesor");
        csvWriter.WriteField("Observaciones");
        csvWriter.NextRecord();

        // Datos
        foreach (var asistencia in reporte.ListaDetallada)
        {
            csvWriter.WriteField(asistencia.Fecha.ToString("dd/MM/yyyy"));
            csvWriter.WriteField(asistencia.NombreAlumno);
            csvWriter.WriteField(asistencia.NombreClase);
            csvWriter.WriteField(asistencia.Estado);
            csvWriter.WriteField(asistencia.NombreProfesor ?? "");
            csvWriter.WriteField(asistencia.Observaciones ?? "");
            csvWriter.NextRecord();
        }

        csvWriter.Flush();
        streamWriter.Flush();

        return memoryStream.ToArray();
    }

    public byte[] ExportarIngresos(ReporteIngresosDTO reporte)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ","
        };

        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream);
        using var csvWriter = new CsvWriter(streamWriter, config);

        // Headers
        csvWriter.WriteField("MetodoPago");
        csvWriter.WriteField("TotalRecaudado");
        csvWriter.WriteField("CantidadPagos");
        csvWriter.WriteField("PorcentajeDelTotal");
        csvWriter.NextRecord();

        // Datos
        foreach (var desglose in reporte.DesgloseMetodosPago)
        {
            csvWriter.WriteField(desglose.MetodoPago);
            csvWriter.WriteField(desglose.TotalRecaudado);
            csvWriter.WriteField(desglose.CantidadPagos);
            csvWriter.WriteField(desglose.PorcentajeDelTotal);
            csvWriter.NextRecord();
        }

        csvWriter.Flush();
        streamWriter.Flush();

        return memoryStream.ToArray();
    }

    public byte[] ExportarPaquetes(ReportePaquetesDTO reporte)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ","
        };

        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream);
        using var csvWriter = new CsvWriter(streamWriter, config);

        // Headers
        csvWriter.WriteField("NombreAlumno");
        csvWriter.WriteField("CorreoAlumno");
        csvWriter.WriteField("TipoPaquete");
        csvWriter.WriteField("FechaVencimiento");
        csvWriter.WriteField("DiasRestantes");
        csvWriter.WriteField("ClasesRestantes");
        csvWriter.NextRecord();

        // Datos
        foreach (var alerta in reporte.AlertasPorVencer)
        {
            csvWriter.WriteField(alerta.NombreAlumno);
            csvWriter.WriteField(alerta.CorreoAlumno);
            csvWriter.WriteField(alerta.NombreTipoPaquete);
            csvWriter.WriteField(alerta.FechaVencimiento.ToString("dd/MM/yyyy"));
            csvWriter.WriteField(alerta.DiasRestantes);
            csvWriter.WriteField(alerta.ClasesRestantes);
            csvWriter.NextRecord();
        }

        csvWriter.Flush();
        streamWriter.Flush();

        return memoryStream.ToArray();
    }

    public byte[] ExportarClases(ReporteClasesDTO reporte)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ","
        };

        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream);
        using var csvWriter = new CsvWriter(streamWriter, config);

        // Headers
        csvWriter.WriteField("TipoClase");
        csvWriter.WriteField("TotalClases");
        csvWriter.WriteField("PromedioAsistencia");
        csvWriter.WriteField("OcupacionPorcentaje");
        csvWriter.NextRecord();

        // Datos
        foreach (var clase in reporte.ClasesMasPopulares)
        {
            csvWriter.WriteField(clase.NombreTipoClase);
            csvWriter.WriteField(clase.TotalClases);
            csvWriter.WriteField(clase.PromedioAsistencia);
            csvWriter.WriteField(clase.OcupacionPorcentaje);
            csvWriter.NextRecord();
        }

        csvWriter.Flush();
        streamWriter.Flush();

        return memoryStream.ToArray();
    }

    public byte[] ExportarAlumnos(ReporteAlumnosDTO reporte)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ","
        };

        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream);
        using var csvWriter = new CsvWriter(streamWriter, config);

        // Headers
        csvWriter.WriteField("NombreAlumno");
        csvWriter.WriteField("Correo");
        csvWriter.WriteField("UltimaAsistencia");
        csvWriter.WriteField("DiasInactivo");
        csvWriter.NextRecord();

        // Datos
        foreach (var alumno in reporte.AlumnosInactivos)
        {
            csvWriter.WriteField(alumno.NombreAlumno);
            csvWriter.WriteField(alumno.Correo);
            csvWriter.WriteField(alumno.UltimaAsistencia.HasValue ? alumno.UltimaAsistencia.Value.ToString("dd/MM/yyyy") : "Nunca");
            csvWriter.WriteField(alumno.DiasInactivo);
            csvWriter.NextRecord();
        }

        csvWriter.Flush();
        streamWriter.Flush();

        return memoryStream.ToArray();
    }
}
