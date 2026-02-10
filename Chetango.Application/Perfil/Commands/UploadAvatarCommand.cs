// ============================================
// UPLOAD AVATAR COMMAND
// ============================================

using Chetango.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chetango.Application.Perfil.Commands;

public record UploadAvatarCommand(
    Guid IdAlumno,
    string FileName,
    byte[] FileContent
) : IRequest<Result<string>>;

public class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, Result<string>>
{
    private readonly IAppDbContext _db;

    public UploadAvatarCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<string>> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        var alumno = await _db.Alumnos
            .FirstOrDefaultAsync(a => a.IdAlumno == request.IdAlumno, cancellationToken);

        if (alumno == null)
            return Result<string>.Failure("Alumno no encontrado");

        // Validar extensión
        var extension = Path.GetExtension(request.FileName).ToLower();
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        
        if (!allowedExtensions.Contains(extension))
            return Result<string>.Failure("Formato de imagen no válido. Use: jpg, jpeg, png o webp");

        // Validar tamaño (5MB máximo)
        if (request.FileContent.Length > 5 * 1024 * 1024)
            return Result<string>.Failure("La imagen no debe superar 5MB");

        // Generar nombre único
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
        
        // Crear carpeta si no existe
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        
        // Guardar archivo
        await File.WriteAllBytesAsync(filePath, request.FileContent, cancellationToken);

        // Actualizar URL en BD
        var avatarUrl = $"/uploads/avatars/{uniqueFileName}";
        alumno.AvatarUrl = avatarUrl;

        await _db.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(avatarUrl);
    }
}
