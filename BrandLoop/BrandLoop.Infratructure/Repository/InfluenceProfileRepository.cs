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
    public class InfluenceProfileRepository : IInfluenceRepository
    {
        private readonly BLDBContext _context;
        public InfluenceProfileRepository(BLDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<InfluenceProfile> GetByUidAsync(string uid)
        {
            return await _context.InfluenceProfiles.FirstOrDefaultAsync(ip => ip.UID == uid);
        }

        public async Task UpdateAsync(InfluenceProfile influenceProfile)
        {
            _context.InfluenceProfiles.Update(influenceProfile);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
