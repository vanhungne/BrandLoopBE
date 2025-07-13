using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.FeartureDTO;
using BrandLoop.Infratructure.Models.Influence;
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

        public async Task<List<InfluenceProfile>> SearchAsync(InfluenceSearchOptions opts)
        {
            var now = DateTimeHelper.GetVietnamNow();
            // Only consider profiles with an active "Priority Listed" subscription (SubscriptionId=1)
            var priorityUids = await _context.SubscriptionRegisters
                .Where(r => r.SubscriptionId == 1
                         && r.Status == RegisterSubStatus.Active
                         && r.ExpirationDate >= now)
                .Select(r => r.UID)
                .Distinct()
                .ToListAsync();

            var q = BuildBaseQuery(opts);
            // Prioritize: active priority sub OR spotlight flag
            q = q.OrderByDescending(p => (priorityUids.Contains(p.UID) && p.IsPriorityListed) || p.IsInSpotlight)
                 .ThenByDescending(p => p.EngagementRate ?? 0)
                 .ThenByDescending(p => p.FollowerCount ?? 0)
                 .ThenByDescending(p => p.UpdatedAt);

            return await q
                .Skip((opts.Page - 1) * opts.PageSize)
                .Take(opts.PageSize)
                .ToListAsync();
        }


        public async Task<List<InfluenceProfile>> SearchHomeFeaturedAsync(InfluenceSearchOptions opts)
        {
            var now = DateTimeHelper.GetVietnamNow();
            var activeUids = await _context.SubscriptionRegisters
                .Where(r => r.SubscriptionId == 2
                         && r.Status == RegisterSubStatus.Active
                         && r.ExpirationDate >= now)
                .Select(r => r.UID)
                .Distinct()
                .ToListAsync();

            var q = BuildBaseQuery(opts);

            // Thay đổi ở đây
            q = q
                 .OrderByDescending(p => p.IsFeaturedOnHome || p.IsInSpotlight)
                 .ThenByDescending(p => p.EngagementRate ?? 0)
                 .ThenByDescending(p => p.FollowerCount ?? 0)
                 .ThenByDescending(p => p.UpdatedAt);

            var skip = (opts.Page - 1) * opts.PageSize;
            return await q.Skip(skip)
                          .Take(opts.PageSize)
                          .ToListAsync();
        }

        private IQueryable<InfluenceProfile> BuildBaseQuery(InfluenceSearchOptions opts)
        {
            var q = _context.InfluenceProfiles.Include(u=>u.User).Include(u=>u.InfluencerType).AsQueryable();

            if (!string.IsNullOrWhiteSpace(opts.UID))
                q = q.Where(p => p.UID == opts.UID);
            if (!string.IsNullOrWhiteSpace(opts.Keyword))
            {
                var kw = $"%{opts.Keyword}%";
                q = q.Where(p => EF.Functions.Like(p.Nickname, kw)
                              || EF.Functions.Like(p.Bio, kw)
                              || EF.Functions.Like(p.ContentCategory, kw));
            }
            if (!string.IsNullOrWhiteSpace(opts.ContentCategory))
                q = q.Where(p => p.ContentCategory == opts.ContentCategory);
            if (!string.IsNullOrWhiteSpace(opts.Location))
                q = q.Where(p => p.Location == opts.Location);
            if (!string.IsNullOrWhiteSpace(opts.Languages))
                q = q.Where(p => p.Languages.Contains(opts.Languages));
            if (opts.Verified.HasValue)
                q = q.Where(p => p.Verified == opts.Verified.Value);
            if (!string.IsNullOrWhiteSpace(opts.Gender))
                q = q.Where(p => p.Gender == opts.Gender);
            if (opts.InfluencerTypeId.HasValue)
                q = q.Where(p => p.InfluencerTypeId == opts.InfluencerTypeId.Value);
            if (opts.MinFollowerCount.HasValue)
                q = q.Where(p => p.FollowerCount >= opts.MinFollowerCount.Value);
            if (opts.MaxFollowerCount.HasValue)
                q = q.Where(p => p.FollowerCount <= opts.MaxFollowerCount.Value);
            if (opts.MinEngagementRate.HasValue)
                q = q.Where(p => p.EngagementRate >= opts.MinEngagementRate.Value);
            if (opts.MaxEngagementRate.HasValue)
                q = q.Where(p => p.EngagementRate <= opts.MaxEngagementRate.Value);
            if (opts.CreatedAfter.HasValue)
                q = q.Where(p => p.CreatedAt >= opts.CreatedAfter.Value);
            if (opts.UpdatedAfter.HasValue)
                q = q.Where(p => p.UpdatedAt >= opts.UpdatedAfter.Value);

            return q;
        }

        public async Task<List<InfluenceProfile>> SearchInfluencer(string? name, string? contentCategory, int? influencerTypeId)
        {
            var query = _context.InfluenceProfiles
                .Include(ip => ip.User)
                .Include(ip => ip.InfluencerType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(ip => ip.Nickname.Contains(name) || ip.User.FullName.Contains(name));

            if (!string.IsNullOrEmpty(contentCategory))
                query = query.Where(ip => ip.ContentCategory.Contains(contentCategory));

            if (influencerTypeId.HasValue)
                query = query.Where(ip => ip.InfluencerTypeId == influencerTypeId.Value);

            var result = await query
                .OrderByDescending(ip => ip.IsPriorityListed)
                .ToListAsync();

            return result;
        }
    }
}