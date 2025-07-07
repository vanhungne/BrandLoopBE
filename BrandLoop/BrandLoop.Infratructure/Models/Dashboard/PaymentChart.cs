using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.Dashboard
{
    public class PaymentChart
    {
        public int totalAmount { get; set; } = 0;
        public int totalTransactions { get; set; } = 0;
        public int totalSubscriptionsPayments { get; set; } = 0;
        public int totalSubscriptionMoney { get; set; } = 0;
        public int totalCampaignsPayments { get; set; } = 0;
        public int totalCampaignsMoney { get; set; } = 0;
        public List<PaymentPerMonth> paymentPerMonths { get; set; } = new List<PaymentPerMonth>();
    }

    public class PaymentPerMonth
    {
        public int month { get; set; } // Tháng
        public int totalSubscriptionsPayments { get; set; } = 0;
        public int totalSubscriptionMoney { get; set; } = 0;
        public int totalCampaignsPayments { get; set; } = 0;
        public int totalCampaignsMoney { get; set; } = 0;
    }
}
