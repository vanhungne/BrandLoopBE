using Microsoft.EntityFrameworkCore.Storage;

namespace BrandLoop.Infratructure.Configurations
{
    public interface IUnitOfWork : IDisposable
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task<int> SaveChangesAsync();
    }
}
