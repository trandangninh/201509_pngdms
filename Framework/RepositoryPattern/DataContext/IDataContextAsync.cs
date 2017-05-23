using System.Threading;
using System.Threading.Tasks;

namespace RepositoryPattern.DataContext
{
    public interface IDbContextAsync : IDataContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync();
    }
}