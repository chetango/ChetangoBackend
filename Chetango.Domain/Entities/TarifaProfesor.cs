namespace Chetango.Domain.Entities.Estados
{
    public class TarifaProfesor
    {
        public Guid IdTarifa { get; set; }
        public Guid IdTipoProfesor { get; set; }
        public TipoProfesor TipoProfesor { get; set; } = null!;
        public Guid IdRolEnClase { get; set; }
        public RolEnClase RolEnClase { get; set; } = null!;
        public decimal ValorPorClase { get; set; }
    }
}
