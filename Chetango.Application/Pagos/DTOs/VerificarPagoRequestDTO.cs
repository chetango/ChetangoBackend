namespace Chetango.Application.Pagos.DTOs;

public record VerificarPagoRequestDTO(
    string IdPago,
    string Accion, // "aprobar" | "rechazar"
    string? Nota,
    bool NotificarAlumno
);
