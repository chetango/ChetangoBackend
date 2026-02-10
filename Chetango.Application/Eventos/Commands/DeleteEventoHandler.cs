using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Eventos.Commands;

public class DeleteEventoHandler : IRequestHandler<DeleteEventoCommand, Result<bool>>
{
    private readonly IAppDbContext _db;

    public DeleteEventoHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<bool>> Handle(DeleteEventoCommand request, CancellationToken cancellationToken)
    {
        var evento = await _db.Eventos
            .FirstOrDefaultAsync(e => e.IdEvento == request.IdEvento, cancellationToken);

        if (evento == null)
            return Result<bool>.Failure("Evento no encontrado.");

        // Soft delete - marcar como inactivo
        evento.Activo = false;
        evento.FechaModificacion = DateTimeHelper.Now;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
