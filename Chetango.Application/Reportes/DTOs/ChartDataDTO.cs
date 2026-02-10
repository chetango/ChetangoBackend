namespace Chetango.Application.Reportes.DTOs;

/// <summary>
/// DTO para datos de gráficas compatible con Chart.js, Recharts, ApexCharts
/// </summary>
public class ChartDataDTO
{
    /// <summary>
    /// Tipo de gráfica: "line", "bar", "pie", "doughnut", "area"
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Etiquetas del eje X o categorías
    /// </summary>
    public List<string> Labels { get; set; } = new();

    /// <summary>
    /// Conjuntos de datos para la gráfica
    /// </summary>
    public List<ChartDatasetDTO> Datasets { get; set; } = new();
}

/// <summary>
/// Dataset individual para una gráfica
/// </summary>
public class ChartDatasetDTO
{
    /// <summary>
    /// Etiqueta del dataset
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Datos numéricos del dataset
    /// </summary>
    public List<decimal> Data { get; set; } = new();

    /// <summary>
    /// Color de fondo (hex color) - opcional
    /// </summary>
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// Color del borde (hex color) - opcional
    /// </summary>
    public string? BorderColor { get; set; }
}
