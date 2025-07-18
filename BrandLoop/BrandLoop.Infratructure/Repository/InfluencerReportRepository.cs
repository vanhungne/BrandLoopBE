using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.Report;
using BrandLoop.Infratructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Repository
{
    public class InfluencerReportRepository : IInfluencerReportRepository
    {
        private readonly BLDBContext _context;
        public InfluencerReportRepository(BLDBContext context)
        {
            _context = context;
        }

        public async Task AddCampaignReport(CampaignReport campaignReport)
        {
            await _context.CampaignReports.AddAsync(campaignReport);
            await _context.SaveChangesAsync();
        }

        public async Task AddInfluencerReport(InfluencerReport influencerReport)
        {
            await _context.InfluencerReports.AddAsync(influencerReport);
            await _context.SaveChangesAsync();
        }

        public Task<InfluencerReport> GetReportById(int reportId)
        {
            var report = _context.InfluencerReports.FirstOrDefaultAsync(r => r.InfluencerReportId == reportId);
            return report;
        }

        public Task<List<InfluencerReport>> GetReportsByCampaignId(int campaignId)
        {
            var reports = _context.InfluencerReports
                .Include(r => r.KolsJoinCampaign)
                .Where(r => r.KolsJoinCampaign.CampaignId == campaignId) // Assuming InfluencerReportId is the foreign key to Campaign
                .ToListAsync();
            return reports;
        }

        public async Task UpdateCampaignReport(CampaignReport campaignReport)
        {
            _context.CampaignReports.Update(campaignReport);
            await _context.SaveChangesAsync();
        }
    }
}
