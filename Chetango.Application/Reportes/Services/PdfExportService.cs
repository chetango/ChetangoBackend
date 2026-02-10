using Chetango.Application.Reportes.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Chetango.Application.Reportes.Services;

/// <summary>
/// Servicio para exportar reportes a PDF usando QuestPDF
/// </summary>
public class PdfExportService
{
    public byte[] ExportarAsistencias(ReporteAsistenciasDTO reporte, DateTime fechaDesde, DateTime fechaHasta)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Height(80)
                    .Background(Colors.Blue.Medium)
                    .Padding(20)
                    .Text(text =>
                    {
                        text.Span("REPORTE DE ASISTENCIAS\n").FontSize(24).Bold().FontColor(Colors.White);
                        text.Span($"Periodo: {fechaDesde:dd/MM/yyyy} - {fechaHasta:dd/MM/yyyy}").FontSize(12).FontColor(Colors.White);
                    });

                page.Content().Padding(20).Column(column =>
                {
                    // Métricas
                    column.Item().PaddingBottom(15).Row(row =>
                    {
                        row.RelativeItem().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Column(c =>
                        {
                            c.Item().Text($"Total Asistencias: {reporte.TotalAsistencias}").Bold().FontSize(14);
                            c.Item().Text($"Presentes: {reporte.Presentes} ({reporte.PorcentajeAsistencia:F1}%)").FontColor(Colors.Green.Medium);
                            c.Item().Text($"Ausentes: {reporte.Ausentes}").FontColor(Colors.Red.Medium);
                            c.Item().Text($"Justificadas: {reporte.Justificadas}").FontColor(Colors.Orange.Medium);
                        });
                    });

                    column.Item().PaddingTop(15).Text("Detalle de Asistencias").Bold().FontSize(16);

                    // Tabla
                    column.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(80);  // Fecha
                            columns.RelativeColumn(2);    // Alumno
                            columns.RelativeColumn(2);    // Clase
                            columns.ConstantColumn(70);   // Estado
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Fecha").Bold().FontColor(Colors.White);
                            header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Alumno").Bold().FontColor(Colors.White);
                            header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Clase").Bold().FontColor(Colors.White);
                            header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Estado").Bold().FontColor(Colors.White);
                        });

                        // Rows
                        foreach (var asistencia in reporte.ListaDetallada.Take(50)) // Limitar a 50 para PDF
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(asistencia.Fecha.ToString("dd/MM/yy"));
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(asistencia.NombreAlumno);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(asistencia.NombreClase);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(asistencia.Estado);
                        }
                    });
                });

                page.Footer()
                    .Height(30)
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Generado: ").FontSize(9);
                        text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(9).Bold();
                        text.Span(" | Chetango App").FontSize(9);
                    });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] ExportarIngresos(ReporteIngresosDTO reporte, DateTime fechaDesde, DateTime fechaHasta)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Height(80)
                    .Background(Colors.Green.Medium)
                    .Padding(20)
                    .Text(text =>
                    {
                        text.Span("REPORTE DE INGRESOS\n").FontSize(24).Bold().FontColor(Colors.White);
                        text.Span($"Periodo: {fechaDesde:dd/MM/yyyy} - {fechaHasta:dd/MM/yyyy}").FontSize(12).FontColor(Colors.White);
                    });

                page.Content().Padding(20).Column(column =>
                {
                    // Métricas principales
                    column.Item().PaddingBottom(15).Row(row =>
                    {
                        row.RelativeItem().BorderBottom(2).BorderColor(Colors.Green.Medium).PaddingBottom(10).Column(c =>
                        {
                            c.Item().Text($"Total Recaudado: ${reporte.TotalRecaudado:N2}").Bold().FontSize(18).FontColor(Colors.Green.Darken2);
                            c.Item().Text($"Cantidad de Pagos: {reporte.Cantidad}").FontSize(14);
                            c.Item().Text($"Promedio por Pago: ${reporte.Promedio:N2}").FontSize(14);
                            
                            if (reporte.ComparativaMesAnterior.HasValue)
                            {
                                var color = reporte.ComparativaMesAnterior.Value >= 0 ? Colors.Green.Medium : Colors.Red.Medium;
                                var signo = reporte.ComparativaMesAnterior.Value >= 0 ? "↑" : "↓";
                                c.Item().Text($"{signo} Comparativa: {Math.Abs(reporte.ComparativaMesAnterior.Value):F2}%").FontSize(14).FontColor(color).Bold();
                            }
                        });
                    });

                    column.Item().PaddingTop(15).Text("Desglose por Método de Pago").Bold().FontSize(16);

                    // Tabla
                    column.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Green.Medium).Padding(5).Text("Método").Bold().FontColor(Colors.White);
                            header.Cell().Background(Colors.Green.Medium).Padding(5).Text("Total").Bold().FontColor(Colors.White);
                            header.Cell().Background(Colors.Green.Medium).Padding(5).Text("Cantidad").Bold().FontColor(Colors.White);
                            header.Cell().Background(Colors.Green.Medium).Padding(5).Text("% Total").Bold().FontColor(Colors.White);
                        });

                        // Rows
                        foreach (var desglose in reporte.DesgloseMetodosPago)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(desglose.MetodoPago);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text($"${desglose.TotalRecaudado:N2}");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(desglose.CantidadPagos.ToString());
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text($"{desglose.PorcentajeDelTotal:F1}%");
                        }
                    });
                });

                page.Footer()
                    .Height(30)
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Generado: ").FontSize(9);
                        text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(9).Bold();
                        text.Span(" | Chetango App").FontSize(9);
                    });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] ExportarPaquetes(ReportePaquetesDTO reporte, DateTime fechaDesde, DateTime fechaHasta)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Height(80)
                    .Background(Colors.Orange.Medium)
                    .Padding(20)
                    .Text(text =>
                    {
                        text.Span("REPORTE DE PAQUETES\n").FontSize(24).Bold().FontColor(Colors.White);
                        text.Span($"Periodo: {fechaDesde:dd/MM/yyyy} - {fechaHasta:dd/MM/yyyy}").FontSize(12).FontColor(Colors.White);
                    });

                page.Content().Padding(20).Column(column =>
                {
                    // Métricas
                    column.Item().PaddingBottom(15).Row(row =>
                    {
                        row.RelativeItem().BorderBottom(2).BorderColor(Colors.Orange.Medium).PaddingBottom(10).Column(c =>
                        {
                            c.Item().Text($"Paquetes Activos: {reporte.TotalActivos}").Bold().FontSize(16).FontColor(Colors.Green.Medium);
                            c.Item().Text($"Paquetes Vencidos: {reporte.TotalVencidos}").FontSize(14);
                            c.Item().Text($"Por Vencer (7 días): {reporte.TotalPorVencer}").FontSize(14).FontColor(Colors.Orange.Darken2).Bold();
                            c.Item().Text($"Agotados: {reporte.TotalAgotados}").FontSize(14);
                        });
                    });

                    // Alertas
                    if (reporte.AlertasPorVencer.Any())
                    {
                        column.Item().PaddingTop(15).Background(Colors.Orange.Lighten4).Padding(10).Column(alertColumn =>
                        {
                            alertColumn.Item().Text("⚠️ ALERTAS - PAQUETES POR VENCER").Bold().FontSize(14).FontColor(Colors.Orange.Darken3);
                            
                            alertColumn.Item().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.ConstantColumn(80);
                                    columns.ConstantColumn(60);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Orange.Medium).Padding(3).Text("Alumno").Bold().FontColor(Colors.White).FontSize(10);
                                    header.Cell().Background(Colors.Orange.Medium).Padding(3).Text("Tipo").Bold().FontColor(Colors.White).FontSize(10);
                                    header.Cell().Background(Colors.Orange.Medium).Padding(3).Text("Vencimiento").Bold().FontColor(Colors.White).FontSize(10);
                                    header.Cell().Background(Colors.Orange.Medium).Padding(3).Text("Días").Bold().FontColor(Colors.White).FontSize(10);
                                });

                                foreach (var alerta in reporte.AlertasPorVencer.Take(20))
                                {
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).Text(alerta.NombreAlumno).FontSize(9);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).Text(alerta.NombreTipoPaquete).FontSize(9);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).Text(alerta.FechaVencimiento.ToString("dd/MM/yy")).FontSize(9);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(3).Text(alerta.DiasRestantes.ToString()).FontSize(9).Bold();
                                }
                            });
                        });
                    }
                });

                page.Footer()
                    .Height(30)
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Generado: ").FontSize(9);
                        text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(9).Bold();
                        text.Span(" | Chetango App").FontSize(9);
                    });
            });
        });

        return document.GeneratePdf();
    }
}
