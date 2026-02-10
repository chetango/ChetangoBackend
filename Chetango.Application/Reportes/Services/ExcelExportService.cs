using Chetango.Application.Reportes.DTOs;
using ClosedXML.Excel;
using System.Globalization;

namespace Chetango.Application.Reportes.Services;

/// <summary>
/// Servicio para exportar reportes a Excel usando ClosedXML
/// </summary>
public class ExcelExportService
{
    public byte[] ExportarAsistencias(ReporteAsistenciasDTO reporte)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Asistencias");

        // Título
        worksheet.Cell(1, 1).Value = "REPORTE DE ASISTENCIAS";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        worksheet.Range(1, 1, 1, 6).Merge();

        // Métricas
        worksheet.Cell(3, 1).Value = "Total Asistencias:";
        worksheet.Cell(3, 2).Value = reporte.TotalAsistencias;
        worksheet.Cell(4, 1).Value = "Presentes:";
        worksheet.Cell(4, 2).Value = reporte.Presentes;
        worksheet.Cell(5, 1).Value = "Ausentes:";
        worksheet.Cell(5, 2).Value = reporte.Ausentes;
        worksheet.Cell(6, 1).Value = "% Asistencia:";
        worksheet.Cell(6, 2).Value = $"{reporte.PorcentajeAsistencia}%";

        // Headers detalle
        int startRow = 8;
        worksheet.Cell(startRow, 1).Value = "Fecha";
        worksheet.Cell(startRow, 2).Value = "Alumno";
        worksheet.Cell(startRow, 3).Value = "Clase";
        worksheet.Cell(startRow, 4).Value = "Estado";
        worksheet.Cell(startRow, 5).Value = "Profesor";
        worksheet.Cell(startRow, 6).Value = "Observaciones";

        var headerRange = worksheet.Range(startRow, 1, startRow, 6);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        // Datos
        int row = startRow + 1;
        foreach (var asistencia in reporte.ListaDetallada)
        {
            worksheet.Cell(row, 1).Value = asistencia.Fecha.ToString("dd/MM/yyyy");
            worksheet.Cell(row, 2).Value = asistencia.NombreAlumno;
            worksheet.Cell(row, 3).Value = asistencia.NombreClase;
            worksheet.Cell(row, 4).Value = asistencia.Estado;
            worksheet.Cell(row, 5).Value = asistencia.NombreProfesor ?? "";
            worksheet.Cell(row, 6).Value = asistencia.Observaciones ?? "";
            row++;
        }

        // Formato tabla
        if (reporte.ListaDetallada.Any())
        {
            var tableRange = worksheet.Range(startRow, 1, row - 1, 6);
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Hair;
        }

        // Ajustar columnas
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] ExportarIngresos(ReporteIngresosDTO reporte)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Ingresos");

        // Título
        worksheet.Cell(1, 1).Value = "REPORTE DE INGRESOS";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        worksheet.Range(1, 1, 1, 4).Merge();

        // Métricas
        worksheet.Cell(3, 1).Value = "Total Recaudado:";
        worksheet.Cell(3, 2).Value = reporte.TotalRecaudado;
        worksheet.Cell(3, 2).Style.NumberFormat.Format = "$#,##0.00";
        
        worksheet.Cell(4, 1).Value = "Cantidad de Pagos:";
        worksheet.Cell(4, 2).Value = reporte.Cantidad;
        
        worksheet.Cell(5, 1).Value = "Promedio por Pago:";
        worksheet.Cell(5, 2).Value = reporte.Promedio;
        worksheet.Cell(5, 2).Style.NumberFormat.Format = "$#,##0.00";

        if (reporte.ComparativaMesAnterior.HasValue)
        {
            worksheet.Cell(6, 1).Value = "Comparativa Mes Anterior:";
            worksheet.Cell(6, 2).Value = $"{reporte.ComparativaMesAnterior.Value:F2}%";
            worksheet.Cell(6, 2).Style.Font.Bold = true;
            worksheet.Cell(6, 2).Style.Font.FontColor = reporte.ComparativaMesAnterior.Value >= 0 
                ? XLColor.Green 
                : XLColor.Red;
        }

        // Desglose por método de pago
        int startRow = 9;
        worksheet.Cell(startRow, 1).Value = "Método de Pago";
        worksheet.Cell(startRow, 2).Value = "Total Recaudado";
        worksheet.Cell(startRow, 3).Value = "Cantidad";
        worksheet.Cell(startRow, 4).Value = "% del Total";

        var headerRange = worksheet.Range(startRow, 1, startRow, 4);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        int row = startRow + 1;
        foreach (var desglose in reporte.DesgloseMetodosPago)
        {
            worksheet.Cell(row, 1).Value = desglose.MetodoPago;
            worksheet.Cell(row, 2).Value = desglose.TotalRecaudado;
            worksheet.Cell(row, 2).Style.NumberFormat.Format = "$#,##0.00";
            worksheet.Cell(row, 3).Value = desglose.CantidadPagos;
            worksheet.Cell(row, 4).Value = $"{desglose.PorcentajeDelTotal:F2}%";
            row++;
        }

        // Ajustar columnas
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] ExportarPaquetes(ReportePaquetesDTO reporte)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Paquetes");

        // Título
        worksheet.Cell(1, 1).Value = "REPORTE DE PAQUETES";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        worksheet.Range(1, 1, 1, 4).Merge();

        // Métricas
        worksheet.Cell(3, 1).Value = "Total Activos:";
        worksheet.Cell(3, 2).Value = reporte.TotalActivos;
        
        worksheet.Cell(4, 1).Value = "Total Vencidos:";
        worksheet.Cell(4, 2).Value = reporte.TotalVencidos;
        
        worksheet.Cell(5, 1).Value = "Por Vencer (7 días):";
        worksheet.Cell(5, 2).Value = reporte.TotalPorVencer;
        worksheet.Cell(5, 2).Style.Font.Bold = true;
        worksheet.Cell(5, 2).Style.Font.FontColor = XLColor.Orange;
        
        worksheet.Cell(6, 1).Value = "Total Agotados:";
        worksheet.Cell(6, 2).Value = reporte.TotalAgotados;

        // Alertas
        if (reporte.AlertasPorVencer.Any())
        {
            int startRow = 9;
            worksheet.Cell(startRow, 1).Value = "ALERTAS - PAQUETES POR VENCER";
            worksheet.Cell(startRow, 1).Style.Font.Bold = true;
            worksheet.Cell(startRow, 1).Style.Fill.BackgroundColor = XLColor.Orange;
            worksheet.Range(startRow, 1, startRow, 6).Merge();

            startRow++;
            worksheet.Cell(startRow, 1).Value = "Alumno";
            worksheet.Cell(startRow, 2).Value = "Correo";
            worksheet.Cell(startRow, 3).Value = "Tipo Paquete";
            worksheet.Cell(startRow, 4).Value = "Fecha Vencimiento";
            worksheet.Cell(startRow, 5).Value = "Días Restantes";
            worksheet.Cell(startRow, 6).Value = "Clases Restantes";

            var headerRange = worksheet.Range(startRow, 1, startRow, 6);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightYellow;

            int row = startRow + 1;
            foreach (var alerta in reporte.AlertasPorVencer)
            {
                worksheet.Cell(row, 1).Value = alerta.NombreAlumno;
                worksheet.Cell(row, 2).Value = alerta.CorreoAlumno;
                worksheet.Cell(row, 3).Value = alerta.NombreTipoPaquete;
                worksheet.Cell(row, 4).Value = alerta.FechaVencimiento.ToString("dd/MM/yyyy");
                worksheet.Cell(row, 5).Value = alerta.DiasRestantes;
                worksheet.Cell(row, 6).Value = alerta.ClasesRestantes;
                row++;
            }
        }

        // Ajustar columnas
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] ExportarClases(ReporteClasesDTO reporte)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Clases");

        // Título
        worksheet.Cell(1, 1).Value = "REPORTE DE CLASES";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        worksheet.Range(1, 1, 1, 4).Merge();

        // Métricas
        worksheet.Cell(3, 1).Value = "Total Clases:";
        worksheet.Cell(3, 2).Value = reporte.TotalClases;
        
        worksheet.Cell(4, 1).Value = "Promedio Asistencia:";
        worksheet.Cell(4, 2).Value = reporte.PromedioAsistencia;
        
        worksheet.Cell(5, 1).Value = "Ocupación Promedio:";
        worksheet.Cell(5, 2).Value = $"{reporte.OcupacionPromedio}%";

        // Clases más populares
        int startRow = 8;
        worksheet.Cell(startRow, 1).Value = "CLASES MÁS POPULARES";
        worksheet.Cell(startRow, 1).Style.Font.Bold = true;
        worksheet.Range(startRow, 1, startRow, 4).Merge();

        startRow++;
        worksheet.Cell(startRow, 1).Value = "Tipo Clase";
        worksheet.Cell(startRow, 2).Value = "Total Clases";
        worksheet.Cell(startRow, 3).Value = "Promedio Asistencia";
        worksheet.Cell(startRow, 4).Value = "% Ocupación";

        var headerRange = worksheet.Range(startRow, 1, startRow, 4);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(200, 150, 255); // Púrpura claro

        int row = startRow + 1;
        foreach (var clase in reporte.ClasesMasPopulares)
        {
            worksheet.Cell(row, 1).Value = clase.NombreTipoClase;
            worksheet.Cell(row, 2).Value = clase.TotalClases;
            worksheet.Cell(row, 3).Value = clase.PromedioAsistencia;
            worksheet.Cell(row, 4).Value = $"{clase.OcupacionPorcentaje:F2}%";
            row++;
        }

        // Ajustar columnas
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] ExportarAlumnos(ReporteAlumnosDTO reporte)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Alumnos");

        // Título
        worksheet.Cell(1, 1).Value = "REPORTE DE ALUMNOS";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        worksheet.Range(1, 1, 1, 4).Merge();

        // Métricas
        worksheet.Cell(3, 1).Value = "Total Activos:";
        worksheet.Cell(3, 2).Value = reporte.TotalActivos;
        
        worksheet.Cell(4, 1).Value = "Total Inactivos:";
        worksheet.Cell(4, 2).Value = reporte.TotalInactivos;
        
        worksheet.Cell(5, 1).Value = "Nuevos Este Mes:";
        worksheet.Cell(5, 2).Value = reporte.NuevosEsteMes;
        
        worksheet.Cell(6, 1).Value = "Tasa Retención:";
        worksheet.Cell(6, 2).Value = $"{reporte.TasaRetencion}%";

        // Alumnos inactivos
        if (reporte.AlumnosInactivos.Any())
        {
            int startRow = 9;
            worksheet.Cell(startRow, 1).Value = "ALUMNOS INACTIVOS (>30 días sin clases)";
            worksheet.Cell(startRow, 1).Style.Font.Bold = true;
            worksheet.Cell(startRow, 1).Style.Fill.BackgroundColor = XLColor.Orange;
            worksheet.Range(startRow, 1, startRow, 4).Merge();

            startRow++;
            worksheet.Cell(startRow, 1).Value = "Alumno";
            worksheet.Cell(startRow, 2).Value = "Correo";
            worksheet.Cell(startRow, 3).Value = "Última Asistencia";
            worksheet.Cell(startRow, 4).Value = "Días Inactivo";

            var headerRange = worksheet.Range(startRow, 1, startRow, 4);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightYellow;

            int row = startRow + 1;
            foreach (var alumno in reporte.AlumnosInactivos)
            {
                worksheet.Cell(row, 1).Value = alumno.NombreAlumno;
                worksheet.Cell(row, 2).Value = alumno.Correo;
                worksheet.Cell(row, 3).Value = alumno.UltimaAsistencia.HasValue 
                    ? alumno.UltimaAsistencia.Value.ToString("dd/MM/yyyy") 
                    : "Nunca";
                worksheet.Cell(row, 4).Value = alumno.DiasInactivo;
                row++;
            }
        }

        // Ajustar columnas
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
