using Chetango.Application.Common;
using Chetango.Application.Referidos.DTOs;
using Chetango.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Referidos.Commands;

public class GenerarCodigoReferidoHandler : IRequestHandler<GenerarCodigoReferidoCommand, Result<CodigoReferidoDTO>>
{
    private readonly IAppDbContext _db;

    public GenerarCodigoReferidoHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<CodigoReferidoDTO>> Handle(GenerarCodigoReferidoCommand request, CancellationToken cancellationToken)
    {
        // 1. Buscar alumno por email
        var alumno = await _db.Alumnos
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a => a.Usuario.Correo == request.EmailAlumno, cancellationToken);

        if (alumno == null)
            return Result<CodigoReferidoDTO>.Failure("No se encontró el alumno autenticado.");

        // 2. Verificar si ya tiene un código activo
        var codigoExistente = await _db.Set<CodigoReferido>()
            .FirstOrDefaultAsync(c => c.IdAlumno == alumno.IdAlumno && c.Activo, cancellationToken);

        if (codigoExistente != null)
        {
            // Retornar el código existente
            return Result<CodigoReferidoDTO>.Success(new CodigoReferidoDTO(
                codigoExistente.IdCodigo,
                codigoExistente.Codigo,
                codigoExistente.Activo,
                codigoExistente.VecesUsado,
                codigoExistente.BeneficioReferidor,
                codigoExistente.BeneficioNuevoAlumno,
                codigoExistente.FechaCreacion
            ));
        }

        // 3. Generar código único
        var codigo = await GenerarCodigoUnico(alumno.Usuario.NombreUsuario, cancellationToken);

        // 4. Crear nuevo código de referido
        var nuevoCodigo = new CodigoReferido
        {
            IdCodigo = Guid.NewGuid(),
            IdAlumno = alumno.IdAlumno,
            Codigo = codigo,
            Activo = true,
            VecesUsado = 0,
            BeneficioReferidor = "1 clase gratis",
            BeneficioNuevoAlumno = "10% descuento en primer paquete",
            FechaCreacion = DateTime.Now
        };

        _db.Set<CodigoReferido>().Add(nuevoCodigo);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<CodigoReferidoDTO>.Success(new CodigoReferidoDTO(
            nuevoCodigo.IdCodigo,
            nuevoCodigo.Codigo,
            nuevoCodigo.Activo,
            nuevoCodigo.VecesUsado,
            nuevoCodigo.BeneficioReferidor,
            nuevoCodigo.BeneficioNuevoAlumno,
            nuevoCodigo.FechaCreacion
        ));
    }

    private async Task<string> GenerarCodigoUnico(string nombreAlumno, CancellationToken cancellationToken)
    {
        // Generar código basado en el nombre + año + número aleatorio
        var nombreLimpio = new string(nombreAlumno.Where(char.IsLetterOrDigit).ToArray()).ToUpper();
        var prefijo = nombreLimpio.Length >= 4 ? nombreLimpio.Substring(0, 4) : nombreLimpio.PadRight(4, 'X');
        var año = DateTime.Now.Year.ToString().Substring(2); // Últimos 2 dígitos del año
        
        var random = new Random();
        string codigo;
        bool existe;

        do
        {
            var numero = random.Next(10, 99);
            codigo = $"{prefijo}{año}{numero}";
            
            existe = await _db.Set<CodigoReferido>()
                .AnyAsync(c => c.Codigo == codigo, cancellationToken);
                
        } while (existe);

        return codigo;
    }
}
