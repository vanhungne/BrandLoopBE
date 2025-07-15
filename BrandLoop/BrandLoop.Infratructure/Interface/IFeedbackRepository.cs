using BrandLoop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface IFeedbackRepository
    {
        Task AddFeedbackAsync(Feedback feedback);
        Task<List<Feedback>> GetFeedbacksByCampaignIdAsync(int campaignId);
        Task<List<Feedback>> GetFeedbackOfUser(string userId);
        Task<Feedback> GetFeedbackByIdAsync(int feedbackId);
        Task<Feedback> GetFeedbackForKolOfCampaignAsync(int campaignId, string kolId);
        Task<Feedback> GetFeedbackOfKolByCampaignIdAsync(int campaignId, string kolId);
        Task<List<Feedback>> GetFeedbacksOfBrandByCampaignId(int campaignId, string brandUID);
    }
}
