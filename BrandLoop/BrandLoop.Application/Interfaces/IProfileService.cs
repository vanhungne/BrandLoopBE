using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Models.BannerDTO;
using BrandLoop.Infratructure.Models.FeartureDTO;
using BrandLoop.Infratructure.Models.Influence;
using BrandLoop.Infratructure.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Interfaces
{
    public interface IProfileService
    {
        Task<BasicAccountProfileModel> GetBasicAccountProfileAsync(string uid);
        Task<BrandProfileModel> GetBrandProfileAsync(string uid);
        Task<InfluenceProfileModel> GetInfluenceProfileAsync(string uid);
        Task<ProfileResponseDto> GetUserProfileAsync(string uid);
        Task<List<SkillModel>> GetUserSkillsAsync(string uid);
        Task<List<ContentAndStyleModel>> GetUserContentAndStylesAsync(string uid);
        Task<ProfileResponseDto> GetProfileAsync(string uid);
        Task<ProfileResponseDto> UpdateUserProfileAsync(string uid, UpdateUserProfileDto updateDto);
        Task<ProfileResponseDto> UpdateBrandProfileAsync(string uid, UpdateBrandProfileDto updateDto);
        Task<ProfileResponseDto> UpdateInfluenceProfileAsync(string uid, UpdateInfluenceProfileDto updateDto);
        Task<List<InfluenceProfile>> SearchInfluencersAsync(InfluenceSearchOptions opts);
        Task<List<InfluenceProfile>> SearchHomeFeaturedAsync(InfluenceSearchOptions opts);
        Task<List<BannerDto>> GetActiveBannersAsync();
    }
}
