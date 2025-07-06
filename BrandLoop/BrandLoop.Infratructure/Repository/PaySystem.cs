using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.UserModel;
using BrandLoop.Infratructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Enums;

namespace BrandLoop.Infratructure.Repository
{
    public class PaySystem : IPaySystem
    {
        private readonly PayOS _payOS;
        private string cancelUrl;
        private string returnUrl;
        private readonly BLDBContext _context;
        public PaySystem(IConfiguration configuration, BLDBContext context)
        {
            _payOS = new PayOS(configuration["PayOS:ClientID"], configuration["PayOS:ApiKey"], configuration["PayOS:ChecksumKey"]);
            cancelUrl = configuration["Host:https"];
            returnUrl = configuration["Host:https"];
            _context = context;
        }
        public async Task<PaymentLinkInformation> cancelPaymentLink(long orderID, string reason)
        {
            return await _payOS.cancelPaymentLink(orderID, reason);
        }

        public Task<string> confirmWebhook(string webhookUrl)
        {
            return _payOS.confirmWebhook(webhookUrl);
        }

        public async Task<CreatePaymentResult> CreatePaymentAsync(BasicAccountProfileModel user, string description,long orderCode, List<ItemData> itemDatas)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == orderCode);
            if (payment == null)
                throw new Exception("Payment not found");

            if(payment.Type == PaymentType.subscription)
            {
                returnUrl = $"https://www.brandloop.io.vn/payment/success?type=subscription";
                cancelUrl = $"https://www.brandloop.io.vn/payment/cancel?type=subscription";
            } else if(payment.Type == PaymentType.campaign)
            {
                returnUrl = $"https://www.brandloop.io.vn/payment/success?type=campaign";
                cancelUrl = $"https://www.brandloop.io.vn/payment/cancel?type=campaign";
            }

            PaymentData paymentData = new PaymentData(
                orderCode: orderCode,
                amount: payment.Amount,
                description: description,
                items: itemDatas,
                cancelUrl: cancelUrl,
                returnUrl: returnUrl,
                buyerName: user.FullName,
                buyerEmail: user.Email,
                buyerPhone: user.Phone
            );

            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);
            return createPayment;
        }

        public async Task<PaymentLinkInformation> getPaymentLinkInformation(long orderID)
        {
            return await _payOS.getPaymentLinkInformation(orderID);
        }
    }
}
