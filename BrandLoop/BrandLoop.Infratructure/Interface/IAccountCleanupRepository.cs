using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Entities;

namespace BrandLoop.Infratructure.Interfaces
{
    public interface IAccountCleanupRepository
    {
        Task<List<User>> GetExpiredUnverifiedAccountsAsync(DateTime cutoffTime, int batchSize);
        Task<int> GetExpiredUnverifiedAccountsCountAsync(DateTime cutoffTime);
        Task DeleteUserAndRelatedDataAsync(User user);
        Task SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
