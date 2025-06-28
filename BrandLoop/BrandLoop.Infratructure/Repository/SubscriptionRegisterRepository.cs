using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Repository
{
    public class SubscriptionRegisterRepository : ISubscriptionRegisterRepository
    {
        private readonly BLDBContext _context;

        public SubscriptionRegisterRepository(BLDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubscriptionRegister>> GetAllAsync()
        {
            return await _context.SubscriptionRegisters
                .Include(sr => sr.User)
                .Include(sr => sr.Subscription)
                    .ThenInclude(s => s.SubscriptionFeatures)
                .OrderByDescending(sr => sr.RegistrationDate)
                .ToListAsync();
        }

        public async Task<SubscriptionRegister> GetByIdAsync(int id)
        {
            return await _context.SubscriptionRegisters
                .Include(sr => sr.User)
                .Include(sr => sr.Subscription)
                    .ThenInclude(s => s.SubscriptionFeatures)
                .FirstOrDefaultAsync(sr => sr.Id == id);
        }

        public async Task<SubscriptionRegister> CreateAsync(SubscriptionRegister subscriptionRegister)
        {
            _context.SubscriptionRegisters.Add(subscriptionRegister);
            await _context.SaveChangesAsync();
            return subscriptionRegister;
        }

        public async Task<SubscriptionRegister> UpdateAsync(SubscriptionRegister subscriptionRegister)
        {
            _context.SubscriptionRegisters.Update(subscriptionRegister);
            await _context.SaveChangesAsync();
            return subscriptionRegister;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var subscriptionRegister = await GetByIdAsync(id);
            if (subscriptionRegister == null) return false;

            _context.SubscriptionRegisters.Remove(subscriptionRegister);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SubscriptionRegister>> GetByUserIdAsync(string uid)
        {
            return await _context.SubscriptionRegisters
                .Include(sr => sr.Subscription)
                    .ThenInclude(s => s.SubscriptionFeatures)
                .Where(sr => sr.UID == uid)
                .OrderByDescending(sr => sr.RegistrationDate)
                .ToListAsync();
        }

        public async Task<SubscriptionRegister> GetActiveSubscriptionByUserIdAsync(string uid)
        {
            return await _context.SubscriptionRegisters
                .Include(sr => sr.Subscription)
                    .ThenInclude(s => s.SubscriptionFeatures)
                .FirstOrDefaultAsync(sr => sr.UID == uid &&
                                          sr.Status == RegisterSubStatus.Active &&
                                          (sr.ExpirationDate == null || sr.ExpirationDate > DateTime.Now));
        }

        public async Task<IEnumerable<SubscriptionRegister>> GetBySubscriptionIdAsync(int subscriptionId)
        {
            return await _context.SubscriptionRegisters
                .Include(sr => sr.User)
                .Where(sr => sr.SubscriptionId == subscriptionId)
                .OrderByDescending(sr => sr.RegistrationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<SubscriptionRegister>> GetExpiredSubscriptionsAsync()
        {
            return await _context.SubscriptionRegisters
                .Include(sr => sr.User)
                .Include(sr => sr.Subscription)
                .Where(sr => sr.ExpirationDate != null && sr.ExpirationDate <= DateTime.Now)
                .ToListAsync();
        }

        public async Task<IEnumerable<SubscriptionRegister>> GetByStatusAsync(RegisterSubStatus status)
        {
            return await _context.SubscriptionRegisters
                .Include(sr => sr.User)
                .Include(sr => sr.Subscription)
                .Where(sr => sr.Status == status)
                .OrderByDescending(sr => sr.RegistrationDate)
                .ToListAsync();
        }

        public async Task<bool> HasActiveSubscriptionAsync(string uid)
        {
            return await _context.SubscriptionRegisters
                .AnyAsync(sr => sr.UID == uid &&
                               sr.Status == RegisterSubStatus.Active &&
                               (sr.ExpirationDate == null || sr.ExpirationDate > DateTime.Now));
        }

        public async Task<int> GetTotalSubscriptionCountAsync()
        {
            return await _context.SubscriptionRegisters.CountAsync();
        }

        public async Task<int> GetActiveSubscriptionCountAsync()
        {
            return await _context.SubscriptionRegisters
                .CountAsync(sr => sr.Status == RegisterSubStatus.Active &&
                                 (sr.ExpirationDate == null || sr.ExpirationDate > DateTime.Now));
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.SubscriptionRegisters
                .Include(sr => sr.Subscription)
                .Where(sr => sr.Status == RegisterSubStatus.Active)
                .SumAsync(sr => sr.Subscription.Price ?? 0);
        }

        public async Task<decimal> GetMonthlyRevenueAsync(int year, int month)
        {
            return await _context.SubscriptionRegisters
                .Include(sr => sr.Subscription)
                .Where(sr => sr.Status == RegisterSubStatus.Active &&
                            sr.RegistrationDate.Year == year &&
                            sr.RegistrationDate.Month == month)
                .SumAsync(sr => sr.Subscription.Price ?? 0);
        }

        public async Task ApplySubscription(string uid, int subscriptionId)
        {
            var influenceProfile = await _context.InfluenceProfiles.FirstOrDefaultAsync(ip => ip.UID == uid);
            if (influenceProfile == null)
            {
                throw new Exception($"User with UID {uid} not found.");
            }

            switch(subscriptionId)
            {
                case 1:
                    influenceProfile.IsPriorityListed = true;
                    break;
                case 2:
                    influenceProfile.IsFeaturedOnHome = true;
                    break;
                case 3:
                    influenceProfile.HasExclusiveBanner = true;
                    break;
                case 4:
                    influenceProfile.IsInSpotlight = true;
                    break;
                default:
                    throw new Exception($"Invalid subscription ID: {subscriptionId}");
            }
        }
    }
}
