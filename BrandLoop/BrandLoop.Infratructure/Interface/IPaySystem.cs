using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface IPaySystem
    {
        
        Task<CreatePaymentResult> CreatePaymentAsync(long orderCode, List<ItemData> listItem);
        Task<PaymentLinkInformation> getPaymentLinkInformation(long orderID);
        Task<PaymentLinkInformation> cancelPaymentLink(long orderID, string reason);
        Task<string> confirmWebhook(string webhookUrl);
        Task<WebhookType> verifyPaymentWebhookData(WebhookData data);
    }
}
