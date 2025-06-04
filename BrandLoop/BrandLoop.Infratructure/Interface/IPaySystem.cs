using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Models.UserModel;
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

        Task<CreatePaymentResult> CreatePaymentAsync(BasicAccountProfileModel user, string description, long orderCode, List<ItemData> itemDatas);
        Task<PaymentLinkInformation> getPaymentLinkInformation(long orderID);
        Task<PaymentLinkInformation> cancelPaymentLink(long orderID, string reason);
        Task<string> confirmWebhook(string webhookUrl);
    }
}
