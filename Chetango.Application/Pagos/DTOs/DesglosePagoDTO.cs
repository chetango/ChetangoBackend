namespace Chetango.Application.Pagos.DTOs;

public record DesglosePagoDTO(
    string NombreMetodo,
    decimal TotalRecaudado,
    int CantidadPagos
);
