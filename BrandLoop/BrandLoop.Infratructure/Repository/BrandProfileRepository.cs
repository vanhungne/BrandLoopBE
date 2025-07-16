using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.UserModel;
using BrandLoop.Infratructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Repository
{
    public class BrandProfileRepository : IBrandProfileRepository
    {
        private readonly BLDBContext _context;
        public BrandProfileRepository(BLDBContext context)
        {
            _context = context;
        }

        public async Task<BrandProfile> GetByUidAsync(string uid)
        {
            return await _context.BrandProfiles.Include(bp => bp.User).FirstOrDefaultAsync(bp => bp.UID == uid);
        }

        public async Task UpdateAsync(BrandProfile brandProfile)
        {
            _context.BrandProfiles.Update(brandProfile);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
