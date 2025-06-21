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
    public class InfluencerTypeRepository : IInfluencerTypeRepository
    {
        private readonly BLDBContext _context;
        public InfluencerTypeRepository(BLDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<InfluencerType> AddInfluencerTypeAsync(InfluencerType influencerType)
        {
            _context.InfluencerTypes.Add(influencerType);
            await _context.SaveChangesAsync();
            return influencerType;
        }

        public async Task<List<InfluencerType>> GetAllInfluencerTypesAsync()
        {
            var influencerTypes = await _context.InfluencerTypes.ToListAsync();
            return influencerTypes;
        }

        public async Task<InfluencerType> GetInfluencerTypeByIdAsync(int id)
        {
            var influencerType = await _context.InfluencerTypes.FirstOrDefaultAsync(x => x.Id == id);
            return influencerType;
        }

        public async Task<InfluencerType> UpdateInfluencerTypeAsync(InfluencerType influencerType)
        {
            _context.Update(influencerType);
            await _context.SaveChangesAsync();
            return influencerType;
        }
    }
}
