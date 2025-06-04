using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.SubcriptionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface ISubscriptionRepository
    {
        Task<List<Subscription>> GetAllSubscriptionsAsync();
        Task<Subscription> GetSubscriptionByIdAsync(int subscriptionId);
        Task<Subscription> AddSubscriptionAsync(Subscription subscription);
        Task<Subscription> UpdateSubscriptionAsync(SubscriptionDTO subscription);
        Task DeleteSubscriptionAsync(int subscriptionId);

        // Register subscription methods
        Task<List<SubscriptionRegister>> GetSubscriptionRegistersOfUser(string userId);
        Task<SubscriptionRegister> GetSubscriptionRegisterByIdAsync(int id);
        Task<SubscriptionRegister> RegisterSubscription(SubscriptionRegister subscriptionRegister);
        Task<SubscriptionRegister> UpdateRegisterStatus(int registrationId, RegisterSubStatus status);
    }
}
