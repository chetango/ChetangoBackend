namespace Chetango.Application.Asistencias.DTOs;

public class TipoAsistenciaDto
{
    public int IdTipoAsistencia { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool RequierePaquete { get; set; }
    public bool DescontarClase { get; set; }
}
