using BrandLoop.Infratructure.Interface;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Repository
{
    public class PaySystem : IPaySystem
    {
        private readonly PayOS _payOS;
        private readonly string cancelUrl;
        private readonly string returnUrl;
        public PaySystem(IConfiguration configuration)
        {
            _payOS = new PayOS(configuration["PayOS:ClientID"], configuration["PayOS:ApiKey"], configuration["PayOS:ChecksumKey"]);
            cancelUrl = configuration["Host:https"];
            returnUrl = configuration["Host:https"];
        }
        public Task<PaymentLinkInformation> cancelPaymentLink(long orderID, string reason)
        {
            throw new NotImplementedException();
        }

        public Task<string> confirmWebhook(string webhookUrl)
        {
            throw new NotImplementedException();
        }

        public async Task<CreatePaymentResult> CreatePaymentAsync(long orderCode, List<ItemData> itemDatas)
        {
            PaymentData paymentData = new PaymentData(orderCode, 1000, "Thanh toan don hang",
                itemDatas,cancelUrl, returnUrl);

            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);
            return createPayment;
        }

        public Task<PaymentLinkInformation> getPaymentLinkInformation(long orderID)
        {
            throw new NotImplementedException();
        }

        public Task<WebhookType> verifyPaymentWebhookData(WebhookData data)
        {
            throw new NotImplementedException();
        }
    }
}
