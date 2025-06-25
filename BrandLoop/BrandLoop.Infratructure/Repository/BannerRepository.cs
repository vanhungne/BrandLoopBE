using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Persistence;
using BrandLoop.Shared.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Repository
{
    public class BannerRepository : IBannerRepository
    {
        private readonly BLDBContext _context;
        public BannerRepository(BLDBContext context) => _context = context;

        public async Task<List<Banner>> GetActiveBannersAsync()
        {
            var now = DateTimeHelper.GetVietnamNow();
            return await _context.Banners
                .Include(b => b.InfluenceProfile)
                .Where(b => b.StartDate <= now && b.EndDate >= now && b.InfluenceProfile.HasExclusiveBanner == true || b.InfluenceProfile.IsInSpotlight == true)
                .ToListAsync();
        }
    }
}
