using MediatR;
using Microsoft.EntityFrameworkCore;
using Chetango.Application.Common;
using Chetango.Application.Suscripciones.DTOs;

namespace Chetango.Application.Suscripciones.Queries;

/// <summary>
/// Handler para obtener la configuración de pago activa.
/// </summary>
public class GetConfiguracionPagoQueryHandler : IRequestHandler<GetConfiguracionPagoQuery, Result<ConfiguracionPagoDto>>
{
    private readonly IAppDbContext _db;

    public GetConfiguracionPagoQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ConfiguracionPagoDto>> Handle(GetConfiguracionPagoQuery request, CancellationToken cancellationToken)
    {
        var configuracion = await _db.ConfiguracionPagos
            .AsNoTracking()
            .Where(c => c.Activo && c.MostrarEnPortal)
            .FirstOrDefaultAsync(cancellationToken);

        if (configuracion == null)
        {
            return Result<ConfiguracionPagoDto>.Failure("No se encontró configuración de pago activa.");
        }

        var resultado = new ConfiguracionPagoDto
        {
            Banco = configuracion.Banco,
            TipoCuenta = configuracion.TipoCuenta,
            NumeroCuenta = configuracion.NumeroCuenta,
            Titular = configuracion.Titular,
            NIT = configuracion.NIT
        };

        return Result<ConfiguracionPagoDto>.Success(resultado);
    }
}
