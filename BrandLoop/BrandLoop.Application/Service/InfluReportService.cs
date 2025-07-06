using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.Report;
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
        public InfluReportService(IInfluencerReportRepository influencerReportRepository, IFeedbackRepository feedbackRepository, ICampaignRepository campaignRepository, IEvidenceRepository evidenceRepository)
        {
            _influencerReportRepository = influencerReportRepository;
            _feedbackRepository = feedbackRepository;
            _campaignRepository = campaignRepository;
            _evidenceRepository = evidenceRepository;
        }
        public async Task FinishReport(string userId, InfluReport influReport)
        {
            var campaign = await _campaignRepository.GetCampaignDetailAsync(influReport.CampaignId);
            var kolJoinCampaign = campaign.KolsJoinCampaigns.FirstOrDefault(k => k.UID == userId);
            if (kolJoinCampaign == null)
            {
                throw new AuthenticationException("You can not report this campaign.");
            }
            // Create a new Influencer Report
            var newReport = new InfluencerReport
            {
                InfluencerReportId = kolJoinCampaign.KolsJoinCampaignId,
                TotalContent = influReport.TotalContent,
                TotalReach = influReport.TotalReach,
                TotalImpressions = influReport.TotalImpressions,
                TotalEngagement = influReport.TotalEngagement,
                AvgEngagementRate = (influReport.TotalEngagement / influReport.TotalReach) * 100,   //(TotalEngagement / TotalReach) * 100
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
        }
    }
}
