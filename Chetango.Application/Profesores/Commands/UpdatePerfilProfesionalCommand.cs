// ============================================
// UPDATE PERFIL PROFESIONAL COMMAND
// ============================================

using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Chetango.Application.Profesores.Commands;

public record UpdatePerfilProfesionalCommand(
    Guid IdProfesor,
    string? Biografia,
    List<string> Especialidades
) : IRequest<Result<Unit>>;

public class UpdatePerfilProfesionalCommandHandler : IRequestHandler<UpdatePerfilProfesionalCommand, Result<Unit>>
{
    private readonly IAppDbContext _db;

    public UpdatePerfilProfesionalCommandHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Unit>> Handle(UpdatePerfilProfesionalCommand request, CancellationToken cancellationToken)
    {
        var profesor = await _db.Set<Chetango.Domain.Entities.Estados.Profesor>()
            .FirstOrDefaultAsync(p => p.IdProfesor == request.IdProfesor, cancellationToken);

        if (profesor == null)
            return Result<Unit>.Failure("Profesor no encontrado");

        // Actualizar biografía
        profesor.Biografia = request.Biografia;

        // Serializar especialidades a JSON
        profesor.Especialidades = JsonSerializer.Serialize(request.Especialidades);

        await _db.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
