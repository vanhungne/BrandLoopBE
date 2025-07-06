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
using static System.Net.Mime.MediaTypeNames;

namespace BrandLoop.Application.Service
{
    public class InfluencerDashboardService : IInfluencerDashboardService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IKolsJoinCampaignRepository _kolsJoinCampaignRepository;
        private readonly IMapper _mapper;
        public InfluencerDashboardService(ISubscriptionRepository subscriptionRepository, ICampaignRepository campaignRepository, IMapper mapper, IKolsJoinCampaignRepository kolsJoinCampaignRepository, IPaymentRepository paymentRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _campaignRepository = campaignRepository;
            _mapper = mapper;
            _kolsJoinCampaignRepository = kolsJoinCampaignRepository;
            _paymentRepository = paymentRepository;
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
                return campaignCard; // Trả về đối tượng rỗng nếu không có campaigns

            campaignCard.totalCampaigns = campaigns.Count();

            foreach (var campaign in campaigns)
            {
                switch (campaign.Status)
                {
                    case CampaignStatus.Approved:
                        campaignCard.totalUnstartedCampaigns++;
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

        public async Task<List<CampaignChart>> GetRevenueChart(string uid, int year)
        {
            var result = new List<CampaignChart>();

            // Khởi tạo 12 tháng
            for (int month = 1; month <= 12; month++)
            {
                result.Add(new CampaignChart
                {
                    month = month,
                    moneyIn= 0,
                    moneyOut = 0,
                });
            }

            var payments = await _paymentRepository.GetPaymentOfInfluencerByYear(uid, year);
            var kolJoinCampaigns = await _kolsJoinCampaignRepository.GetKolJoinCampaignOfInfluencer(uid, year);

            // Cập nhật số tiền vào và ra cho từng tháng
            foreach (var payment in payments)
            {
                var month = payment.CreatedAt.Month;
                result[month - 1].moneyOut += payment.Amount;
            }
            foreach (var kolJoinCampaign in kolJoinCampaigns)
            {
                var month = kolJoinCampaign.AppliedAt.Month;
                result[month - 1].moneyIn += kolJoinCampaign.InfluencerEarning;
            }

            return result;
        }

        public async Task<CampaignReportOfInfluencer> GetCampaignReportOfInfluencerAsync(string uid, int campaignId)
        {
            var campaign = await _campaignRepository.GetCampaignDetailAsync(campaignId);
            if (campaign == null)
                return null; // Trả về null nếu không tìm thấy chiến dịch

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
