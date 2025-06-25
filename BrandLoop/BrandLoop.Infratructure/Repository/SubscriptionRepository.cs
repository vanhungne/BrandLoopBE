using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.SubcriptionModel;
using BrandLoop.Infratructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Repository
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly BLDBContext _context;
        public SubscriptionRepository(BLDBContext context)
        {
            _context = context;
        }
        public async Task<Subscription> AddSubscriptionAsync(Subscription subscription)
        {
            await _context.Subscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task DeleteSubscriptionAsync(int subscriptionId)
        {
            var subscription = await _context.Subscriptions.FindAsync(subscriptionId);
            if (subscription != null)
            {
                subscription.isDeleted = true; // Soft delete
                _context.Subscriptions.Update(subscription);
                _context.SaveChanges();
                await _context.SaveChangesAsync();
            }
            else
                throw new Exception($"Subscription with ID {subscriptionId} not found.");
        }

        public async Task<List<Subscription>> GetAllSubscriptionsAsync()
        {
            var subscriptions = await _context.Subscriptions.Where(s => s.isDeleted == false).ToListAsync();
            return subscriptions;
        }

        public async Task<Subscription> GetSubscriptionByIdAsync(int subscriptionId)
        {
            return await _context.Subscriptions.FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId && s.isDeleted == false);
        }

        public async Task<Subscription> UpdateSubscriptionAsync(SubscriptionDTO subscription)
        {
            var existingSubscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.SubscriptionId == subscription.SubscriptionId);
            if (existingSubscription != null)
            {
                existingSubscription.SubscriptionName = subscription.SubscriptionName;
                existingSubscription.Duration = subscription.Duration;
                existingSubscription.Price = subscription.Price;
                existingSubscription.Description = subscription.Description;
                existingSubscription.CreatedAt = subscription.CreatedAt;
                _context.Subscriptions.Update(existingSubscription);
                await _context.SaveChangesAsync();
                return existingSubscription;
            }
            else
                throw new Exception($"Subscription with ID {subscription.SubscriptionId} not found.");
        }

        // Register subscription methods
        public async Task<SubscriptionRegister> GetSubscriptionRegisterByIdAsync(int id)
        {
            var subscriptionRegister = await _context.SubscriptionRegisters
                .Include(sr => sr.User)
                .Include(sr => sr.Subscription)
                .FirstOrDefaultAsync(sr => sr.Id == id);
            return subscriptionRegister;
        }

        public async Task<List<SubscriptionRegister>> GetSubscriptionRegistersOfUser(string userId)
        {
            var subscriptionRegisters = await _context.SubscriptionRegisters
                .Include(sr => sr.User)
                .Include(sr => sr.Subscription)
                .Where(sr => sr.UID == userId && sr.Status == RegisterSubStatus.Active)
                .OrderByDescending(sr => sr.RegistrationDate)
                .ToListAsync();
            return subscriptionRegisters;
        }

        public async Task<SubscriptionRegister> RegisterSubscription(SubscriptionRegister subscriptionRegister)
        {
            await _context.SubscriptionRegisters.AddAsync(subscriptionRegister);
            await _context.SaveChangesAsync();
            return subscriptionRegister;
        }

        public async Task<SubscriptionRegister> UpdateRegisterStatus(int registrationId, RegisterSubStatus status)
        {
            var subscriptionRegister = await _context.SubscriptionRegisters.FirstOrDefaultAsync(sr => sr.Id == registrationId);
            if (subscriptionRegister != null)
            {
                subscriptionRegister.Status = status;
                _context.SubscriptionRegisters.Update(subscriptionRegister);
                await _context.SaveChangesAsync();
                return subscriptionRegister;
            }
            else
                throw new Exception($"Subscription register with ID {registrationId} not found.");
        }

    }
}
