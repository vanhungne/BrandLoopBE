using AutoMapper;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.CampainModel;
using BrandLoop.Infratructure.Models.Report;
using BrandLoop.Infratructure.Models.SubcriptionModel;
using BrandLoop.Infratructure.Repository;
using BrandLoop.Shared.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Service
{
    public class InfluencerDashboardService : IInfluencerDashboardService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IMapper _mapper;
        public InfluencerDashboardService(ISubscriptionRepository subscriptionRepository, ICampaignRepository campaignRepository, IMapper mapper)
        {
            _subscriptionRepository = subscriptionRepository;
            _campaignRepository = campaignRepository;
            _mapper = mapper;
        }
        public async Task<List<SubscriptionRegisterDTO>> GetActiveSubscriptionRegisterAsync(string uid)
        {
            var subscriptionRegisters = await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(uid);
            if (subscriptionRegisters == null || !subscriptionRegisters.Any())
            {
                return new List<SubscriptionRegisterDTO>();
            }
            return _mapper.Map<List<SubscriptionRegisterDTO>>(subscriptionRegisters);
        }

        public async Task<CampaignCard> GetCampaignCardAsync(string uid)
        {
            var campaignCard = new CampaignCard();
            var now = DateTimeHelper.GetVietnamNow();
            var campaigns = await _campaignRepository.GetAllCampaignsInfluJoined(uid);
            if (campaigns == null || !campaigns.Any())
                throw new Exception("No campaigns found.");

            campaignCard.totalCampaigns = campaigns.Count();

            foreach (var campaign in campaigns)
            {
                switch (campaign.Status)
                {
                    case CampaignStatus.Approved:
                        campaignCard.totalUnstartedCampaigns++;
                        break;

                    case CampaignStatus.Pending:
                        campaignCard.totalUnpaidCampaigns++;
                        break;

                    case CampaignStatus.InProgress:
                        if (campaign.Deadline >= now)
                            campaignCard.totalInprogressCampaigns++;
                        else
                            campaignCard.totalOverdueCampaigns++;
                        break;

                    case CampaignStatus.Completed:
                        campaignCard.totalCompletedCampaigns++;
                        break;
                }
            }
            return campaignCard;
        }

        public async Task<List<CampaignChart>> GetCampaignChart(string uid, int year)
        {
            var result = new List<CampaignChart>();

            // Khởi tạo 12 tháng
            for (int month = 1; month <= 12; month++)
            {
                result.Add(new CampaignChart
                {
                    month = month,
                    approvedCampaigns = 0,
                    inprogressCampaigns = 0,
                    completedCampaigns = 0
                });
            }

            // Lấy tất cả campaigns theo năm
            var campaigns = await _campaignRepository.GetAllCampaignsInfluJoined(uid, year);
            if (campaigns == null || !campaigns.Any())
                throw new Exception($"No campaigns found in year {year}");

            foreach (var campaign in campaigns)
            {
                var campaignDate = campaign.StartTime ?? campaign.UploadedDate;

                int month = campaignDate.Month;

                var chart = result.FirstOrDefault(c => c.month == month);
                if (chart == null) continue;

                switch (campaign.Status)
                {
                    case CampaignStatus.Approved:
                        chart.approvedCampaigns++;
                        break;

                    case CampaignStatus.InProgress:
                        chart.inprogressCampaigns++;
                        break;

                    case CampaignStatus.Completed:
                        chart.completedCampaigns++;
                        break;
                }
            }

            return result;
        }

        public async Task<CampaignReportOfInfluencer> GetCampaignReportOfInfluencerAsync(string uid, int campaignId)
        {
            var campaign = await _campaignRepository.GetCampaignDetailAsync(campaignId);
            if (campaign == null)
                throw new Exception($"Campaign with ID {campaignId} not found.");

            var result = new CampaignReportOfInfluencer
            {
                StartTime = campaign.StartTime,
                Deadline = campaign.Deadline,
                Budget = campaign.Budget,
                Status = campaign.Status.ToString(),

                totalKolsJoin = campaign.KolsJoinCampaigns.Count(),
                totalInvitations = campaign.CampaignInvitations.Count(),
                totalAcceptedInvitations = campaign.CampaignInvitations.Where(ci => ci.Status == CampaignInvitationStatus.accepted).Count(),
                totalRejectedInvitations = campaign.CampaignInvitations.Where(ci => ci.Status == CampaignInvitationStatus.rejected).Count(),
                othersInvitations = campaign.CampaignInvitations.Where(ci => (ci.Status != CampaignInvitationStatus.accepted) && (ci.Status != CampaignInvitationStatus.rejected)).Count(),
                TotalSpend = campaign.CampaignReport?.TotalSpend ?? 0,
                TotalRevenue = campaign.CampaignReport?.TotalRevenue ?? 0,
                TotalReach = campaign.CampaignReport?.TotalReach ?? 0,
                TotalImpressions = campaign.CampaignReport?.TotalImpressions ?? 0,
                TotalEngagement = campaign.CampaignReport?.TotalEngagement ?? 0,
                TotalClicks = campaign.CampaignReport?.TotalClicks ?? 0,

                AvgEngagementRate = campaign.CampaignReport?.AvgEngagementRate ?? 0,
                CostPerEngagement = campaign.CampaignReport?.CostPerEngagement ?? 0,
                ROAS = campaign.CampaignReport?.ROAS ?? 0,
                report = _mapper.Map<KolInCampaignTracking>(campaign.KolsJoinCampaigns.FirstOrDefault(f => f.UID == uid)),
                FeedbackOfInfluencer = _mapper.Map<ShowFeedback>(campaign.Feedbacks.FirstOrDefault(f => f.ToUserId == uid))
            };

            return result;
        }

        public async Task<List<CampaignSelectOption>> GetCampaignSelectOption(string uid, CampaignStatus status)
        {
            var campaigns = await _campaignRepository.GetAllCampaignsInfluJoinedWithStatus(uid, status);
            return _mapper.Map<List<CampaignSelectOption>>(campaigns);
        }
    }
}
