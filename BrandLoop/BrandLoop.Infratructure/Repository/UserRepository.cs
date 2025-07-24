using AutoMapper;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.UserModel;
using BrandLoop.Infratructure.Persistence;
using BrandLoop.Shared.Helper;
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

        public async Task<InfluenceProfileModel> GetInfluenceProfileByUsernameAsync(string username)
        {
            var influenceProfile = await _context.InfluenceProfiles
                  .Include(ip => ip.User)
                  .ThenInclude(u => u.Role)
                  .Include(ip => ip.User.KolsJoinCampaigns)
                  .ThenInclude(kjc => kjc.Campaign)
                  .Include(ip => ip.User.Wallets)
                  .Include(ip => ip.InfluencerType)
                  .FirstOrDefaultAsync(ip => ip.User.FullName == username);
            if (influenceProfile == null) return null;
            var uid = influenceProfile.UID;

            if (uid == null) return null;

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
        public async Task<List<InfluenceProfileModel>> GetListInfluenceProfilesByUsernameAsync(string username)
        {
            // Nếu không nhập gì thì trả về tất cả
            if (string.IsNullOrWhiteSpace(username))
            {
                username = "";
            }

            var keywords = username
                .Trim()
                .ToLower()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var query = _context.InfluenceProfiles
                .Include(ip => ip.User)
                    .ThenInclude(u => u.Role)
                .Include(ip => ip.User.KolsJoinCampaigns)
                    .ThenInclude(kjc => kjc.Campaign)
                .Include(ip => ip.User.Wallets)
                .Include(ip => ip.InfluencerType)
                .AsQueryable();

            // Ưu tiên 1: Trùng khớp hoàn toàn
            var exact = query.Where(ip => ip.User.FullName.ToLower() == username.ToLower());

            // Ưu tiên 2: Chứa tất cả từ khóa (AND)
            var allKeywords = query.Where(ip =>
                keywords.All(kw => ip.User.FullName.ToLower().Contains(kw))
            );

            // Ưu tiên 3: Chứa ít nhất một từ khóa (OR)
            var anyKeyword = query.Where(ip =>
                keywords.Any(kw => ip.User.FullName.ToLower().Contains(kw))
            );

            // Ưu tiên 4: Chứa bất kỳ ký tự nào trong tên nhập vào
            var charContains = query;
            if (keywords.Length > 0)
            {
                var chars = keywords.SelectMany(kw => kw.ToCharArray()).Distinct().ToArray();
                charContains = query.Where(ip => chars.Any(c => ip.User.FullName.ToLower().Contains(c)));
            }

            // Lấy danh sách theo ưu tiên
            List<InfluenceProfile> resultList = await exact.ToListAsync();
            if (resultList.Count == 0 && keywords.Length > 0)
                resultList = await allKeywords.ToListAsync();
            if (resultList.Count == 0 && keywords.Length > 0)
                resultList = await anyKeyword.ToListAsync();
            if (resultList.Count == 0 && keywords.Length > 0)
                resultList = await charContains.ToListAsync();
            if (resultList.Count == 0)
                resultList = await query.ToListAsync();

            var results = new List<InfluenceProfileModel>();

            foreach (var influenceProfile in resultList)
            {
                if (influenceProfile?.UID == null)
                    continue;

                var uid = influenceProfile.UID;
                var skills = await GetUserSkillsAsync(uid);
                var contentAndStyles = await GetUserContentAndStylesAsync(uid);

                var kolsJoin = influenceProfile.User?.KolsJoinCampaigns ?? new List<KolsJoinCampaign>();
                var completedCampaigns = kolsJoin.Count(k => k.Status == KolJoinCampaignStatus.Completed);
                var pendingCampaigns = kolsJoin.Count(k => k.Status == KolJoinCampaignStatus.Pending);
                var approvedCampaigns = kolsJoin.Count(k => k.Status == KolJoinCampaignStatus.Active);
                var totalCampaigns = kolsJoin.Count;

                results.Add(new InfluenceProfileModel
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
                    Type = influenceProfile.InfluencerType?.Name,
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
                    TotalCampaignsJoined = totalCampaigns,
                    CompletedCampaigns = completedCampaigns,
                    PendingCampaigns = pendingCampaigns,
                    ApprovedCampaigns = approvedCampaigns,
                    SkillsCount = skills.Count,
                    ContentCount = contentAndStyles.Count,

                    // Skills and Content
                    Skills = skills,
                    ContentAndStyles = contentAndStyles
                });
            }

            return results;
        }

        public async Task UpdateUserStatus(string uid, UserStatus status)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UID == uid);
            if (user == null)
                throw new Exception("Không tìm thấy tài khoản người dùng.");

            user.Status = status;
            user.UpdatedAt = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();
        }

    }
}
