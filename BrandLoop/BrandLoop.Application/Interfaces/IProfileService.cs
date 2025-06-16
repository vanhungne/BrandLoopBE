using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Infratructure.Models.UserModel;

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
    }
}
