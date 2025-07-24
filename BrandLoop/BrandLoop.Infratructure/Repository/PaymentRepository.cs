using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Persistence;
using BrandLoop.Shared.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly BLDBContext _context;
        public PaymentRepository(BLDBContext context)
        {
            _context = context;
        }
        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            var payments = await _context.Payments.OrderByDescending(p => p.CreatedAt).ToListAsync();
            return payments;
        }

        public async Task<Payment> GetPaymentByIdAsync(long paymentId)
        {
            return await _context.Payments
                .Include(p => p.SubscriptionRegister)
                .Include(p => p.campaign)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<List<Payment>> GetPaymentsByUserIdAsync(string userId)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UID == userId);
            if (user == null)
                throw new Exception("User not found");
            if (user.Role.RoleName == "Influencer")
            {
                return await _context.Payments
                    .Include(p => p.SubscriptionRegister)
                    .Where(p => p.SubscriptionRegister.UID == userId)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }
            else if (user.Role.RoleName == "Brand")
            {
                return await _context.Payments
                    .Include(p => p.campaign)
                    .Where(p => p.campaign.CreatedBy == userId)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }
            else
            {
                throw new Exception("User role not supported for payment retrieval");
            }
        }
        public async Task<Payment> GetPaymentByOrderCodeAsync(long orderCode)
        {
            var payment = await _context.Payments
                .Include(p => p.SubscriptionRegister)
                .FirstOrDefaultAsync(p => p.PaymentId == orderCode);

            if (payment == null)
                return null;

            switch (payment.Type)
            {
                case PaymentType.subscription:
                    await _context.Entry(payment).Reference(p => p.SubscriptionRegister).LoadAsync();
                    break;

                case PaymentType.campaign:
                    await _context.Entry(payment).Reference(p => p.campaign).LoadAsync();
                    break;
            }

            return payment;
        }

        public async Task UpdatePaymentTransactionCode(long orderCode, string transactionCode)
        {
            var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == orderCode);
            if (existingPayment == null)
                throw new Exception("Payment not found");

            existingPayment.TransactionCode = transactionCode;
        }

        public async Task UpdatePaymentStatus(long orderCode, PaymentStatus status)
        {
            var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == orderCode);
            if (existingPayment == null)
                throw new Exception("Payment not found");

            existingPayment.Status = status;
            _context.Payments.Update(existingPayment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePaymentLink(long orderCode, string paymentLink)
        {
            var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == orderCode);
            if (existingPayment == null)
                throw new Exception("Payment not found");
            existingPayment.PaymentLink = paymentLink;
            _context.Payments.Update(existingPayment);
            await _context.SaveChangesAsync();
        }

        public async Task<Payment> GetPaymentByCamaignId(int campaignId)
        {
            return await _context.Payments
                .Include(p => p.campaign)
                .FirstOrDefaultAsync(p => p.CampaignId == campaignId && p.Status != PaymentStatus.Canceled);
        }

        public Task<List<Payment>> GetPaymentOfBrandByYear(string uid, int year)
        {
            var payments = _context.Payments
                .Include(p => p.campaign)
                .Where(p => p.campaign.CreatedBy == uid && p.campaign.UploadedDate.Year == year && p.Type == PaymentType.campaign && p.Status == PaymentStatus.Succeeded)
                .ToListAsync();
            return payments;
        }

        public Task<List<Payment>> GetPaymentOfInfluencerByYear(string uid, int year)
        {
            var payments = _context.Payments
                .Include(p => p.SubscriptionRegister)
                .Where(p => p.SubscriptionRegister.UID == uid && p.CreatedAt.Year == year && p.Type == PaymentType.subscription && p.Status == PaymentStatus.Succeeded)
                .ToListAsync();
            return payments;
        }

        public Task<List<Payment>> GetAllPaymentsByYear(int? year)
        {
            var payments = _context.Payments
                .AsNoTracking()
                .Include(p => p.SubscriptionRegister)
                .Include(p => p.campaign)
                .Where(p => (year == null || p.CreatedAt.Year == year) && p.Status == PaymentStatus.Succeeded)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return payments;
        }

        public Task<List<Payment>> GetAllOverduePayment()
        {
            var thresholdTime = DateTimeHelper.GetVietnamNow().AddMinutes(-10);
            var overduePayments = _context.Payments
                .AsNoTracking()
                .Where(p => p.Status == PaymentStatus.pending && p.CreatedAt < thresholdTime)
                .ToListAsync();

            return overduePayments;
        }

        public async Task<Payment> GetPaymentDetail(long paymentId)
        {
            var payment = await _context.Payments
                .AsNoTracking()
                .Include(p => p.SubscriptionRegister)
                    .ThenInclude(sr => sr.User)
                .Include(p => p.SubscriptionRegister)
                    .ThenInclude(sr => sr.Subscription)
                .Include(p => p.campaign)
                    .ThenInclude(c => c.Brand)
                .Include(p => p.campaign)
                    .ThenInclude(c => c.Creator)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
            if (payment == null)
                throw new Exception("Payment not found");
            return payment;
        }

        public async Task<List<Payment>> GetAllPaymentByYearTypeAndStatus(int? year, PaymentType? type, PaymentStatus? status)
        {
            var query = _context.Payments
                .AsNoTracking()
                .Include(p => p.SubscriptionRegister)
                .Include(p => p.campaign)
                .Where(p => (year == null || p.CreatedAt.Year == year));

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            if (type.HasValue)
                query = query.Where(p => p.Type == type.Value);

            return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        }
    }
}
