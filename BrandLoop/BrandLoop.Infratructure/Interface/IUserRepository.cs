using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Infratructure.Models.UserModel;

namespace BrandLoop.Infratructure.Interface
{
    public interface IUserRepository
    {
        Task<BasicAccountProfileModel> GetBasicAccountProfileAsync(string uid);
        Task<BrandProfileModel> GetBrandProfileAsync(string uid);
        Task<InfluenceProfileModel> GetInfluenceProfileAsync(string uid);
        Task<bool> UserExistsAsync(string uid);
        Task<string> GetUserRoleAsync(string uid);
        Task<List<SkillModel>> GetUserSkillsAsync(string uid);
        Task<List<ContentAndStyleModel>> GetUserContentAndStylesAsync(string uid);
    }
}
