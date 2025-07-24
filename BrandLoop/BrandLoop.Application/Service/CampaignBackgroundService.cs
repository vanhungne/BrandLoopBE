using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Shared.Helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Service
{
    public class CampaignBackgroundService : BackgroundService
    {
        private readonly ILogger<CampaignBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(10); // chạy mỗi 10 phút
        public CampaignBackgroundService(ILogger<CampaignBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CampaignStatusUpdaterService đang chạy...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        // Cập nhật trạng thái campaign
                        var campaignRepository = scope.ServiceProvider.GetRequiredService<ICampaignRepository>();

                        var overdueCampaigns = await campaignRepository.GetAllOverdueCampaignToUpdateStatus(DateTimeHelper.GetVietnamNow());

                        foreach (var campaign in overdueCampaigns)
                        {
                            await campaignRepository.UpdateCampaignStatusAsync(campaign.CampaignId, CampaignStatus.Overdue);
                            _logger.LogInformation($"Updated campaign with id {campaign.CampaignId} into status Overdue.");
                        }

                        // Cap nhat cac paymnt qua han

                        var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();

                        var overduePayments = await paymentRepository.GetAllOverduePayment();
                        foreach (var payment in overduePayments)
                        {
                            await paymentRepository.UpdatePaymentStatus(payment.PaymentId, PaymentStatus.Failed);
                            _logger.LogInformation($"Updated payment with id {payment.PaymentId} into status Failed.");
                        }

                        // Cập nhật các subscription đã hết hạn
                        
                        var subscriptionRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();
                        var subscriptionRegisterRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionRegisterRepository>();

                        var expiredSubscriptions = await subscriptionRegisterRepository.GetExpiredSubscriptionsAsync();
                        foreach (var subscriptionRegister in expiredSubscriptions)
                        {
                            // Cập nhật trạng thái đăng ký
                             await subscriptionRepository.UpdateRegisterStatus(subscriptionRegister.Id, RegisterSubStatus.Expired);
                            _logger.LogInformation($"Updated subscription register with id {subscriptionRegister.Id} into status Expired.");
                        }

                        // Câp nhật các đăng ký đang chờ xử lý quá hạn
                        var pendingSubscriptions = await subscriptionRepository.GetExpiredSubscriptionsAsync();
                        foreach (var subscriptionRegister in pendingSubscriptions)
                        {
                            // Cập nhật trạng thái đăng ký
                            await subscriptionRepository.UpdateRegisterStatus(subscriptionRegister.Id, RegisterSubStatus.Failed);
                            _logger.LogInformation($"Updated subscription register with id {subscriptionRegister.Id} into status Failed.");
                        }

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi cập nhật trạng thái trong backgroud service.");
                }

                await Task.Delay(_interval, stoppingToken); // chờ 10 phút rồi chạy tiếp
            }
        }
    }
}
