using BrandLoop.Infratructure.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<PaymentChart> GetPaymentChart(int? year);
        Task<UserChart> GetUserChart(int? year);
    }
}
