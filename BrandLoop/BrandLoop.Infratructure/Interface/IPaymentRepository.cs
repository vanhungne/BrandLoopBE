using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface IPaymentRepository
    {
        Task<List<Payment>> GetAllPaymentsAsync();
        Task<List<Payment>> GetPaymentsByUserIdAsync(string userId);
        Task<Payment> GetPaymentByIdAsync(long paymentId);
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment> GetPaymentByOrderCodeAsync(long orderCode);
        Task UpdatePaymentTransactionCode(long orderCode, string transactionCode);
        Task UpdatePaymentStatus(long orderCode, PaymentStatus status);
        Task UpdatePaymentLink(long orderCode, string paymentLink);
        Task<Payment> GetPaymentByCamaignId(int campaignId);
        Task<List<Payment>> GetPaymentOfBrandByYear(string uid, int year);
        Task<List<Payment>> GetPaymentOfInfluencerByYear(string uid, int year);
    }
}
