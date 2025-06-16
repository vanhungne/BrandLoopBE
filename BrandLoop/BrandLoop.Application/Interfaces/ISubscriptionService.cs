using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Models.SubcriptionModel;
using Net.payOS.Types;

namespace BrandLoop.Application.Interfaces
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionDTO>> GetAllSubscriptionsAsync();
        Task<SubscriptionDTO> GetSubscriptionByIdAsync(int subscriptionId);
        Task<SubscriptionDTO> AddSubscriptionAsync(AddSubcription subscription);
        Task<SubscriptionDTO> UpdateSubscriptionAsync(SubscriptionDTO subscription);
        Task DeleteSubscriptionAsync(int subscriptionId);
        // Register subscription methods
        Task<List<SubscriptionRegisterDTO>> GetSubscriptionRegistersOfUser(string userId);
        Task<SubscriptionRegisterDTO> GetSubscriptionRegisterByIdAsync(int id);
        Task<PaymentSubscription> RegisterSubscription(string userID, int subscriptionId);
        Task<CreatePaymentResult> CreatePaymentLink(string userID, long orderCode);
        Task ConfirmPayment(long orderCode);
        Task CancelPayment(long orderCode);
    }
}
