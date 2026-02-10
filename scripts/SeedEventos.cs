// Script temporal para insertar eventos de ejemplo
// Ejecutar con: dotnet script SeedEventos.cs

using Microsoft.EntityFrameworkCore;

// Este script debe ejecutarse desde Program.cs temporalmente
// o usando EF migrations

var eventos = new[]
{
    new
    {
        Titulo = "Taller de Musicalidad",
        Descripcion = "Aprende a bailar con la música. Taller intensivo de técnicas de musicalidad en tango.",
        Fecha = new DateTime(2026, 2, 1),
        Hora = new TimeSpan(15, 0, 0),
        Precio = 25000m,
        Destacado = true,
        ImagenUrl = "https://images.unsplash.com/photo-1504609813442-a8924e83f76e?w=800",
        ImagenNombre = "taller-musicalidad.jpg"
    },
    new
    {
        Titulo = "Milonga de Práctica",
        Descripcion = "Practica lo aprendido en un ambiente relajado y divertido. Entrada libre para alumnos.",
        Fecha = new DateTime(2026, 1, 31),
        Hora = new TimeSpan(21, 0, 0),
        Precio = 0m,
        Destacado = false,
        ImagenUrl = "https://images.unsplash.com/photo-1571156230214-50e0b9d14d45?w=800",
        ImagenNombre = "milonga-practica.jpg"
    },
    new
    {
        Titulo = "Workshop Avanzado",
        Descripcion = "Técnicas avanzadas de tango con maestros internacionales. Cupos limitados.",
        Fecha = new DateTime(2026, 2, 9),
        Hora = new TimeSpan(16, 0, 0),
        Precio = 40000m,
        Destacado = true,
        ImagenUrl = "https://images.unsplash.com/photo-1508700929628-666bc8bd84ea?w=800",
        ImagenNombre = "workshop-avanzado.jpg"
    }
};

/*
INSERT INTO Eventos (IdEvento, Titulo, Descripcion, Fecha, Hora, Precio, Destacado, ImagenUrl, ImagenNombre, Activo, FechaCreacion, FechaModificacion, IdUsuarioCreador)
SELECT 
    NEWID(),
    Titulo,
    Descripcion,
    Fecha,
    Hora,
    Precio,
    Destacado,
    ImagenUrl,
    ImagenNombre,
    1,
    GETDATE(),
    GETDATE(),
    (SELECT TOP 1 IdUsuario FROM Usuarios WHERE Email LIKE '%admin%')
FROM @eventos
*/
