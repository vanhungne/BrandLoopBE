using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Application.Interfaces;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.UserModel;
using Microsoft.Extensions.Logging;

namespace BrandLoop.Application.Service
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _profileRepository;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(IUserRepository profileRepository, ILogger<ProfileService> logger)
        {
            _profileRepository = profileRepository;
            _logger = logger;
        }

        public async Task<BasicAccountProfileModel> GetBasicAccountProfileAsync(string uid)
        {
            return await _profileRepository.GetBasicAccountProfileAsync(uid);
        }

        public async Task<BrandProfileModel> GetBrandProfileAsync(string uid)
        {
            return await _profileRepository.GetBrandProfileAsync(uid);
        }

        public Task<InfluenceProfileModel> GetInfluenceProfileAsync(string uid)
        {
            return _profileRepository.GetInfluenceProfileAsync(uid);
        }

        public Task<List<ContentAndStyleModel>> GetUserContentAndStylesAsync(string uid)
        {
            return  _profileRepository.GetUserContentAndStylesAsync(uid);
        }

        public Task GetUserProfileAsync(string uid)
        {
            return  _profileRepository.UserExistsAsync(uid);
        }

        public Task<List<SkillModel>> GetUserSkillsAsync(string uid)
        {
            return _profileRepository.GetUserSkillsAsync(uid);
        }
    }
}
