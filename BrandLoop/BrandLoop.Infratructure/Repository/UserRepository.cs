﻿using AutoMapper;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.UserModel;
using BrandLoop.Infratructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BrandLoop.Infratructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly BLDBContext _context;
        private readonly IMapper _mapper;
        public UserRepository(BLDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BasicAccountProfileModel> GetBasicAccountProfileAsync(string uid)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UID == uid);

            if (user == null) return null;

            var skills = await GetUserSkillsAsync(uid);
            var contentAndStyles = await GetUserContentAndStylesAsync(uid);

            return new BasicAccountProfileModel
            {
                UID = user.UID,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                ProfileImage = user.ProfileImage,
                LastLogin = user.LastLogin,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                RoleName = user.Role?.RoleName,
                Status = user.Status.ToString(),
                Skills = skills,
                ContentAndStyles = contentAndStyles
            };
        }

        public async Task<BrandProfileModel> GetBrandProfileAsync(string uid)
        {
            var brandProfile = await _context.BrandProfiles
                 .Include(bp => bp.User)
                 .ThenInclude(u => u.Role)
                 .Include(bp => bp.Campaigns)
                 .FirstOrDefaultAsync(bp => bp.UID == uid);

            if (brandProfile == null) return null;

            var skills = await GetUserSkillsAsync(uid);
            var contentAndStyles = await GetUserContentAndStylesAsync(uid);

            // Campaign statistics
            var activeCampaigns = brandProfile.Campaigns?.Count(c => c.Status == CampaignStatus.Approved) ?? 0;
            var completedCampaigns = brandProfile.Campaigns?.Count(c => c.Status == CampaignStatus.Completed) ?? 0;
            var totalCampaigns = brandProfile.Campaigns?.Count ?? 0;

            return new BrandProfileModel
            {
                BrandId = brandProfile.BrandId,
                UID = brandProfile.UID,
                CompanyName = brandProfile.CompanyName,
                Industry = brandProfile.Industry,
                Website = brandProfile.Website,
                Logo = brandProfile.Logo,
                CompanySize = brandProfile.CompanySize,
                Description = brandProfile.Description,
                Address = brandProfile.Address,
                TaxCode = brandProfile.TaxCode,
                EstablishedYear = brandProfile.EstablishedYear,
                Facebook = brandProfile.Facebook,
                Instagram = brandProfile.Instagram,
                Tiktok = brandProfile.Tiktok,
                CreatedAt = brandProfile.CreatedAt,
                UpdatedAt = brandProfile.UpdatedAt,

                // User basic info
                Email = brandProfile.User?.Email,
                FullName = brandProfile.User?.FullName,
                Phone = brandProfile.User?.Phone,
                ProfileImage = brandProfile.User?.ProfileImage,
                Status = brandProfile.User?.Status.ToString(),
                LastLogin = brandProfile.User?.LastLogin,

                // Statistics
                TotalCampaigns = totalCampaigns,
                ActiveCampaigns = activeCampaigns,
                CompletedCampaigns = completedCampaigns,

                // Skills and Content
                Skills = skills,
                ContentAndStyles = contentAndStyles
            };
        }

        public async Task<InfluenceProfileModel> GetInfluenceProfileAsync(string uid)
        {
            var influenceProfile = await _context.InfluenceProfiles
                  .Include(ip => ip.User)
                  .ThenInclude(u => u.Role)
                  .Include(ip => ip.User.KolsJoinCampaigns)
                  .ThenInclude(kjc => kjc.Campaign)
                  .Include(ip => ip.User.Wallets)
                  .Include(ip => ip.InfluencerType)
                  .FirstOrDefaultAsync(ip => ip.UID == uid);

            if (influenceProfile == null) return null;

            var skills = await GetUserSkillsAsync(uid);
            var contentAndStyles = await GetUserContentAndStylesAsync(uid);

            // Campaign statistics
            var completedCampaigns = influenceProfile.User?.KolsJoinCampaigns?
                .Count(kjc => kjc.Status == KolJoinCampaignStatus.Completed) ?? 0;
            var pendingCampaigns = influenceProfile.User?.KolsJoinCampaigns?
                .Count(kjc => kjc.Status == KolJoinCampaignStatus.Pending) ?? 0;
            var approvedCampaigns = influenceProfile.User?.KolsJoinCampaigns?
                .Count(kjc => kjc.Status == KolJoinCampaignStatus.Active) ?? 0;
            var totalCampaignsJoined = influenceProfile.User?.KolsJoinCampaigns?.Count ?? 0;

            return new InfluenceProfileModel
            {
                InfluenceId = influenceProfile.InfluenceId,
                UID = influenceProfile.UID,
                Nickname = influenceProfile.Nickname,
                Bio = influenceProfile.Bio,
                ContentCategory = influenceProfile.ContentCategory,
                Location = influenceProfile.Location,
                Languages = influenceProfile.Languages,
                PortfolioUrl = influenceProfile.PortfolioUrl,
                AverageRate = influenceProfile.AverageRate,
                Verified = influenceProfile.Verified,
                Facebook = influenceProfile.Facebook,
                Instagram = influenceProfile.Instagram,
                Tiktok = influenceProfile.Tiktok,
                Youtube = influenceProfile.Youtube,
                FollowerCount = influenceProfile.FollowerCount,
                Type = influenceProfile.InfluencerType.Name,
                EngagementRate = influenceProfile.EngagementRate,
                Gender = influenceProfile.Gender,
                DayOfBirth = influenceProfile.DayOfBirth,
                CreatedAt = influenceProfile.CreatedAt,
                UpdatedAt = influenceProfile.UpdatedAt,

                // User basic info
                Email = influenceProfile.User?.Email,
                FullName = influenceProfile.User?.FullName,
                Phone = influenceProfile.User?.Phone,
                ProfileImage = influenceProfile.User?.ProfileImage,
                Status = influenceProfile.User?.Status.ToString(),
                LastLogin = influenceProfile.User?.LastLogin,

                // Statistics
                TotalCampaignsJoined = totalCampaignsJoined,
                CompletedCampaigns = completedCampaigns,
                PendingCampaigns = pendingCampaigns,
                ApprovedCampaigns = approvedCampaigns,
                SkillsCount = skills.Count,
                ContentCount = contentAndStyles.Count,

                // Skills and Content
                Skills = skills,
                ContentAndStyles = contentAndStyles
            };
        }

        public async Task<List<ContentAndStyleModel>> GetUserContentAndStylesAsync(string uid)
        {
            return await _context.ContentAndStyles
                .Where(cs => cs.UID == uid)
                .Select(cs => new ContentAndStyleModel
                {
                    ContentId = cs.ContentId,
                    UID = cs.UID,
                    ContentName = cs.ContentName,
                    VideoUrl = cs.VideoUrl,
                    ImageUrl = cs.ImageUrl,
                    Description = cs.Description,
                    CreatedAt = cs.CreatedAt
                })
                .OrderByDescending(cs => cs.CreatedAt)
                .ToListAsync();
        }

        public async Task<string> GetUserRoleAsync(string uid)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UID == uid);

            return user?.Role?.RoleName;
        }

        public async Task<List<SkillModel>> GetUserSkillsAsync(string uid)
        {
            return await _context.Skills
                .Where(s => s.UID == uid)
                .Select(s => new SkillModel
                {
                    SkillId = s.SkillId,
                    UID = s.UID,
                    SkillName = s.SkillName,
                    ProficiencyLevel = s.ProficiencyLevel
                })
                .ToListAsync();
        }

        public async Task<bool> UserExistsAsync(string uid)
        {
            return await _context.Users.AnyAsync(u => u.UID == uid);
        }
        public async Task<User> GetByIdAsync(string uid)
        {
            return await _context.Users.FindAsync(uid);
        }

        public async Task<User> GetUserWithProfilesAsync(string uid)
        {
            return await _context.Users
                .Include(u => u.BrandProfile)
                .Include(u => u.InfluenceProfile)
                .FirstOrDefaultAsync(u => u.UID == uid);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task<List<User>> GetAllNewsUserInYear(int? year)
        {
            var userQuery = _context.Users
                .Include(u => u.Role)
                .AsQueryable();

            if (year.HasValue)
            {
                userQuery = userQuery.Where(u => u.CreatedAt >= new DateTime(year.Value, 1, 1)
                                              && u.CreatedAt < new DateTime(year.Value + 1, 1, 1));
            }

            return userQuery
                .Where(u => u.Status == UserStatus.Active
                    && (u.Role.RoleName == "Brand" || u.Role.RoleName == "Influencer"))
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

    }
}
