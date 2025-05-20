using Microsoft.EntityFrameworkCore.Storage;
using BrandLoop.Infratructure.Persistence;

namespace BrandLoop.Infratructure.Configurations
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly BLDBContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(BLDBContext context)
        {
            _context = context;
        }

        public async Task<IDbContextTransaction?> BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                return null;
            }

            _transaction = await _context.Database.BeginTransactionAsync();
            return _transaction;
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
