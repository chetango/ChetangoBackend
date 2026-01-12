namespace Chetango.Application.Pagos.DTOs;

public record EstadisticasPagosDTO(
    decimal TotalRecaudado,
    int CantidadPagos,
    decimal PromedioMonto,
    List<DesglosePagoDTO> DesgloseMetodosPago
);
