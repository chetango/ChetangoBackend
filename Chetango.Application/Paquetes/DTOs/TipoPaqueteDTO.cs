namespace Chetango.Application.Paquetes.DTOs;

// DTO para representar un tipo de paquete
public record TipoPaqueteDTO(
    Guid IdTipoPaquete,
    string Nombre,
    int NumeroClases,
    decimal Precio,
    int DiasVigencia,
    string? Descripcion,
    bool Activo
);
