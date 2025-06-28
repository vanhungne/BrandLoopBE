using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface ISubscriptionRegisterRepository
    {
        Task<IEnumerable<SubscriptionRegister>> GetAllAsync();
        Task<SubscriptionRegister> GetByIdAsync(int id);
        Task<IEnumerable<SubscriptionRegister>> GetByUserIdAsync(string uid);
        Task<SubscriptionRegister> GetActiveSubscriptionByUserIdAsync(string uid);
        Task<IEnumerable<SubscriptionRegister>> GetBySubscriptionIdAsync(int subscriptionId);
        Task<IEnumerable<SubscriptionRegister>> GetExpiredSubscriptionsAsync();
        Task<IEnumerable<SubscriptionRegister>> GetByStatusAsync(RegisterSubStatus status);
        Task<bool> HasActiveSubscriptionAsync(string uid);
        Task<int> GetTotalSubscriptionCountAsync();
        Task<int> GetActiveSubscriptionCountAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetMonthlyRevenueAsync(int year, int month);
        Task ApplySubscription(string uid, int subscriptionId);
    }
}
