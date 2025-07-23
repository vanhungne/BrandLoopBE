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
    public class EvidenceRepository : IEvidenceRepository
    {
        private readonly BLDBContext _context;
        public EvidenceRepository(BLDBContext context)
        {
            _context = context;
        }
        public async Task AddEvidenceAsync(Evidence evidence)
        {
            await _context.Evidences.AddAsync(evidence);
        }

        public async Task<List<Evidence>> GetEvidences(int influencerReportId)
        {
            var evidences = await _context.Evidences
                .Where(e => e.InfluencerReportId == influencerReportId)
                .ToListAsync();
            return evidences;
        }

        public async Task<Evidence> GetEvidencesOfBrand(int kolJoinCampaignId)
        {
            var evidences = await _context.Evidences
                .FirstOrDefaultAsync(e => e.KolsJoinCampaignId == kolJoinCampaignId);
            return evidences;
        }
    }
}
