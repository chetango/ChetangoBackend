namespace Chetango.Domain.Entities
{
    public class TarifaProfesor
    {
        public Guid IdTarifa { get; set; }
        public TipoProfesor TipoProfesor { get; set; }
        public RolEnClase RolEnClase { get; set; }
        public decimal ValorPorClase { get; set; }
    }
}
