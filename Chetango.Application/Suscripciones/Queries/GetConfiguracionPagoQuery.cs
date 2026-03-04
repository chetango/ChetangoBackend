using MediatR;
using Chetango.Application.Common;
using Chetango.Application.Suscripciones.DTOs;

namespace Chetango.Application.Suscripciones.Queries;

/// <summary>
/// Query para obtener la configuración de pago activa (datos bancarios).
/// </summary>
public record GetConfiguracionPagoQuery() : IRequest<Result<ConfiguracionPagoDto>>;
