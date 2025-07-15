using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Repository
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly BLDBContext _context;
        public FeedbackRepository(BLDBContext context)
        {
            _context = context;
        }
        public async Task AddFeedbackAsync(Feedback feedback)
        {
            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Feedback>> GetFeedbackOfUser(string userId)
        {
            var feedbacks = await _context.Feedbacks
                .Where(f => f.ToUserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
            return feedbacks;
        }

        public async Task<List<Feedback>> GetFeedbacksByCampaignIdAsync(int campaignId)
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.FromUser)
                    .ThenInclude(fu => fu.BrandProfile)
                .Where(f => f.CampaignId == campaignId && f.FeedbackFrom == Domain.Enums.FeedbackType.Influencer)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
            return feedbacks;
        }

        public async Task<Feedback> GetFeedbackByIdAsync(int feedbackId)
        {
            var feedback = await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);
            return feedback;
        }
        public async Task<Feedback> GetFeedbackForKolOfCampaignAsync(int campaignId, string kolId)
        {
            var feedback = await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.CampaignId == campaignId && f.ToUserId == kolId && f.FeedbackFrom == Domain.Enums.FeedbackType.Brand);
            return feedback;
        }

        public async Task<List<Feedback>> GetFeedbacksOfBrandByCampaignId(int campaignId, string brandUID)
        {
            return await _context.Feedbacks
                .Include(f => f.FromUser)
                    .ThenInclude(fu => fu.BrandProfile) // Include the brand profile of the user who sent the feedback
                .Include(f => f.ToUser) // Include the user who received the feedback
                .Where(f => f.CampaignId == campaignId && f.FromUserId == brandUID && f.FeedbackFrom == Domain.Enums.FeedbackType.Brand)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public Task<Feedback> GetFeedbackOfKolByCampaignIdAsync(int campaignId, string kolId)
        {
            var feedback = _context.Feedbacks
                .Include(f => f.ToUser)
                    .ThenInclude(fu => fu.BrandProfile)
                .Include(f => f.FromUser)
                .FirstOrDefaultAsync(f => f.CampaignId == campaignId && f.FromUserId == kolId && f.FeedbackFrom == Domain.Enums.FeedbackType.Influencer);
            return feedback;
        }
    }
}
