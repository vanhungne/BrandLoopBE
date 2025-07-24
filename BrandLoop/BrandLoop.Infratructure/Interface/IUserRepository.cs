using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface IUserRepository
    {
        Task<BasicAccountProfileModel> GetBasicAccountProfileAsync(string uid);
        Task<BrandProfileModel> GetBrandProfileAsync(string uid);
        Task<InfluenceProfileModel> GetInfluenceProfileAsync(string uid);
        Task<InfluenceProfileModel> GetInfluenceProfileByUsernameAsync(string username);
        Task<List<InfluenceProfileModel>> GetListInfluenceProfilesByUsernameAsync(string username);
        Task<bool> UserExistsAsync(string uid);
        Task<string> GetUserRoleAsync(string uid);
        Task<List<SkillModel>> GetUserSkillsAsync(string uid);
        Task<List<ContentAndStyleModel>> GetUserContentAndStylesAsync(string uid);
        Task<User> GetByIdAsync(string uid);
        Task<User> GetUserWithProfilesAsync(string uid);
        Task UpdateAsync(User user);
        Task SaveChangesAsync();
        Task<List<User>> GetAllNewsUserInYear(int? year);
        Task UpdateUserStatus(string uid, UserStatus status);
    }
}
