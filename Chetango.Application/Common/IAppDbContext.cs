using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Chetango.Domain.Entities;
using Chetango.Domain.Entities.Estados;

namespace Chetango.Application.Common
{
    // Abstracción mínima para desacoplar Application de Infrastructure
    public interface IAppDbContext
    {
        DbSet<Asistencia> Asistencias { get; }
        
        // Método genérico para acceder a otros DbSets sin exponerlos explícitamente
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
