using AutoMapper;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.Report;
using BrandLoop.Shared.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Service
{
    public class InfluReportService : IInfluReportService
    {
        private readonly IInfluencerReportRepository _influencerReportRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IEvidenceRepository _evidenceRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;
        public InfluReportService(IInfluencerReportRepository influencerReportRepository, IFeedbackRepository feedbackRepository, ICampaignRepository campaignRepository, IEvidenceRepository evidenceRepository, IMapper mapper, IPaymentRepository paymentRepository)
        {
            _influencerReportRepository = influencerReportRepository;
            _feedbackRepository = feedbackRepository;
            _campaignRepository = campaignRepository;
            _evidenceRepository = evidenceRepository;
            _mapper = mapper;
            _paymentRepository = paymentRepository;
        }
        public async Task FinishReport(string userId, InfluReport influReport)
        {
            var campaign = await _campaignRepository.GetCampaignDetailAsync(influReport.CampaignId);
            if (!(campaign.Status == CampaignStatus.InProgress || campaign.Status == CampaignStatus.Overdue))
                throw new Exception("Chiến dịch chưa được bắt đầu.");

                var kolJoinCampaign = campaign.KolsJoinCampaigns.FirstOrDefault(k => k.UID == userId);
            if (kolJoinCampaign == null)
            {
                throw new AuthenticationException("Bạn không có quyền để báo cáo cho chiến dịch này.");
            }

            // Check if user is already reported
            var report = await _influencerReportRepository.GetReportById(kolJoinCampaign.KolsJoinCampaignId);
            if (report != null)
                throw new Exception("Bạn đã báo cáo rồi.");

            // Create a new Influencer Report
            var newReport = new InfluencerReport
            {
                InfluencerReportId = kolJoinCampaign.KolsJoinCampaignId,
                TotalContent = influReport.TotalContent,
                TotalReach = influReport.TotalReach,
                TotalImpressions = influReport.TotalImpressions,
                TotalEngagement = influReport.TotalEngagement,
                AvgEngagementRate = influReport.TotalReach > 0 ? ((double)influReport.TotalEngagement / influReport.TotalReach) * 100 : 0,   //(TotalEngagement / TotalReach) * 100
                TotalClicks = influReport.TotalClicks
            };
            await _influencerReportRepository.AddInfluencerReport(newReport);

            // Create a feedbackfor campaign
            var feedback = new Feedback
            {
                CampaignId = newReport.KolsJoinCampaign.CampaignId,
                FromUserId = userId,
                ToUserId = campaign.CreatedBy,
                FeedbackFrom = Domain.Enums.FeedbackType.Influencer,
                Rating = influReport.Rating,
                Description = influReport.Description
            };
            await _feedbackRepository.AddFeedbackAsync(feedback);

            // Create evidences for the report
            if (influReport.Evidences != null && influReport.Evidences.Any())
            {
                foreach (var evidence in influReport.Evidences)
                {
                    var newEvidence = new Evidence
                    {
                        Description = evidence.Description,
                        Link = evidence.Link,
                        EvidenceOf = Domain.Enums.EvidenceType.Influecner,
                        InfluencerReportId = newReport.InfluencerReportId
                    };
                    await _evidenceRepository.AddEvidenceAsync(newEvidence);
                }
            }

            // Update kol join campaign status to Completed
            await _campaignRepository.UpdateKolJoinCampaignStatus(kolJoinCampaign.KolsJoinCampaignId, Domain.Enums.KolJoinCampaignStatus.Completed);

            // Update campaign report
            var campaignReport = await _campaignRepository.GetCampaignReportByCampaignIdAsync(campaign.CampaignId);

            if (campaignReport == null)
            {
                var payment = await _paymentRepository.GetPaymentByCamaignId(campaign.CampaignId);
                campaignReport = new CampaignReport
                {
                    CampaignId = campaign.CampaignId,
                    TotalSpend = payment != null ? payment.Amount : 0,
                    TotalReach = influReport.TotalReach,
                    TotalImpressions = influReport.TotalImpressions,
                    TotalEngagement = influReport.TotalEngagement,
                    TotalClicks = influReport.TotalClicks,
                    AvgEngagementRate = influReport.TotalReach > 0 ? ((double)influReport.TotalEngagement / influReport.TotalReach) * 100 : 0,
                    CostPerEngagement = influReport.TotalEngagement > 0 ? ((payment != null ? payment.Amount : 0) / influReport.TotalEngagement) : 0,
                    CreatedAt = DateTimeHelper.GetVietnamNow()
                };

                await _influencerReportRepository.AddCampaignReport(campaignReport);
            }
            else
            {
                campaignReport.TotalReach += influReport.TotalReach;
                campaignReport.TotalImpressions += influReport.TotalImpressions;
                campaignReport.TotalEngagement += influReport.TotalEngagement;
                campaignReport.TotalClicks += influReport.TotalClicks;
                campaignReport.AvgEngagementRate = campaignReport.TotalReach > 0 ? ((double)campaignReport.TotalEngagement / campaignReport.TotalReach) * 100 : 0;
                campaignReport.CostPerEngagement = campaignReport.TotalEngagement > 0 ? campaignReport.TotalSpend / campaignReport.TotalEngagement : 0;
                await _influencerReportRepository.UpdateCampaignReport(campaignReport);
            }

        }

        public async Task<BrandFeedbackDTO> GetFeedbackOfInfluencerByCampaignId(int campaignId, string brandUID, string influencerUID)
        {
            var kolJoinCampaign = (await _campaignRepository.GetKolsJoinCampaigns(campaignId)).FirstOrDefault(k => k.UID == influencerUID);
            if (kolJoinCampaign == null)
                throw new Exception("Influencer này chưa tham gia chiến dịch này hoặc uid của Influencer bị sai");
            var feedback = (await _feedbackRepository.GetFeedbacksOfBrandByCampaignId(campaignId, brandUID)).FirstOrDefault(fb => fb.ToUserId == influencerUID);
            var evidence = await _evidenceRepository.GetEvidencesOfBrand(kolJoinCampaign.KolsJoinCampaignId);
            
            if (feedback == null)
                throw new Exception("Bạn chưa cho feedback cho Influencer trong chiến dịch này.");
            if (evidence == null)
                throw new Exception("Bạn chưa cung cấp bằng chứng cho Influencer trong chiến dịch này.");

            var result = _mapper.Map<BrandFeedbackDTO>(feedback);
            result.EvidenceLink = evidence.Link;
            result.EvidenceDescription = evidence.Description;
            result.InfluencerMoney = kolJoinCampaign.InfluencerEarning;
            return result;
        }

        public async Task<List<FeedbackDTO>> GetFeedbacksOfBrandByCampaignId(int campaignId, string brandUID)
        {
            var feedbacks = await _feedbackRepository.GetFeedbacksOfBrandByCampaignId(campaignId, brandUID);
            return _mapper.Map<List<FeedbackDTO>>(feedbacks);
        }

        public async Task<InfluencerReportModel> GetReportByCampaignId(int campaignId, string influencerUID)
        {
            var report = (await _influencerReportRepository.GetReportsByCampaignId(campaignId)).FirstOrDefault(r => r.KolsJoinCampaign.UID == influencerUID);

            if (report == null)
                throw new ArgumentException("Bạn chưa báo cáo cho chiến dịch này.");


            var influReport = _mapper.Map<InfluReport>(report);
            var evidences = await _evidenceRepository.GetEvidences(report.InfluencerReportId);
            influReport.Evidences = _mapper.Map<List<EvidenceDTO>>(evidences);
            var model = _mapper.Map<InfluencerReportModel>(influReport);
            model.AvgEngagementRate = report.AvgEngagementRate;

            var feedback = await _feedbackRepository.GetFeedbackOfKolByCampaignIdAsync(campaignId, influencerUID);
            if (feedback != null)
                model.Feedback = _mapper.Map<FeedbackDTO>(feedback);

            return model;
        }
    }
}
