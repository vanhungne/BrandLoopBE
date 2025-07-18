﻿using BrandLoop.Domain.Enums;
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
                        var campaignRepository = scope.ServiceProvider.GetRequiredService<ICampaignRepository>();

                        var overdueCampaigns = await campaignRepository.GetAllOverdueCampaignToUpdateStatus(DateTimeHelper.GetVietnamNow());

                        foreach (var campaign in overdueCampaigns)
                        {
                            await campaignRepository.UpdateCampaignStatusAsync(campaign.CampaignId, CampaignStatus.Overdue);
                            _logger.LogInformation($"Đã cập nhật campaign {campaign.CampaignId} sang trạng thái Overdue.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi cập nhật trạng thái campaign.");
                }

                await Task.Delay(_interval, stoppingToken); // chờ 10 phút rồi chạy tiếp
            }
        }
    }
}
