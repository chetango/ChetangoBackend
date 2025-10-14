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
        // ...puedes exponer más DbSet si otros handlers lo requieren

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
