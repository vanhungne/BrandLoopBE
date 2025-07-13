using AutoMapper;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.BannerDTO;
using BrandLoop.Infratructure.Models.FeartureDTO;
using BrandLoop.Infratructure.Models.Influence;
using BrandLoop.Infratructure.Models.UserModel;
using BrandLoop.Infratructure.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Service
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _profileRepository;
        private readonly IBrandProfileRepository _brandProfileRepository;
        private readonly IInfluenceRepository _influenceProfileRepository;
        private readonly IBannerRepository _bannerRepo;
        private readonly ILogger<ProfileService> _logger;
        private readonly IMapper _mapper;
        private readonly IInfluencerTypeRepository _influencerTypeRepository;

        public ProfileService(IUserRepository profileRepository, ILogger<ProfileService> logger, IBrandProfileRepository brandProfileRepository, IInfluenceRepository influenceRepository, IMapper mapper, IBannerRepository bannerRepo, IInfluencerTypeRepository influencerTypeRepository)
        {
            _profileRepository = profileRepository;
            _logger = logger;
            _brandProfileRepository = brandProfileRepository;
            _influenceProfileRepository = influenceRepository;
            _mapper = mapper;
            _bannerRepo = bannerRepo;
            _influencerTypeRepository = influencerTypeRepository;
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

        public Task<ProfileResponseDto> GetProfileAsync(string uid)
        {
            throw new NotImplementedException();
        }

        public Task<List<ContentAndStyleModel>> GetUserContentAndStylesAsync(string uid)
        {
            return _profileRepository.GetUserContentAndStylesAsync(uid);
        }

        public async Task<ProfileResponseDto> GetUserProfileAsync(string uid)
        {
            var _ = await _profileRepository.GetByIdAsync(uid);
            if (_ == null)
                throw new ArgumentException("User not found");
            var profileResponse = _mapper.Map<ProfileResponseDto>(_);

            return profileResponse;
        }

        public Task<List<SkillModel>> GetUserSkillsAsync(string uid)
        {
            return _profileRepository.GetUserSkillsAsync(uid);
        }

        public async Task<ProfileResponseDto> UpdateUserProfileAsync(string uid, UpdateUserProfileDto updateDto)
        {
            var user = await _profileRepository.GetByIdAsync(uid);
            if (user == null)
                throw new ArgumentException("User not found");

            _mapper.Map(updateDto, user);
            await _profileRepository.UpdateAsync(user);
            await _profileRepository.SaveChangesAsync();

            return await GetProfileAsync(uid);
        }

        public async Task<ProfileResponseDto> UpdateBrandProfileAsync(string uid, UpdateBrandProfileDto updateDto)
        {
            var brandProfile = await _brandProfileRepository.GetByUidAsync(uid);
            if (brandProfile == null)
                throw new ArgumentException("Brand profile not found");

            _mapper.Map(updateDto, brandProfile);
            await _brandProfileRepository.UpdateAsync(brandProfile);
            await _brandProfileRepository.SaveChangesAsync();

            return await GetProfileAsync(uid);
        }

        public async Task<ProfileResponseDto> UpdateInfluenceProfileAsync(string uid, UpdateInfluenceProfileDto updateDto)
        {
            var influenceProfile = await _influenceProfileRepository.GetByUidAsync(uid);
            if (influenceProfile == null)
                throw new ArgumentException("Influence profile not found");

            _mapper.Map(updateDto, influenceProfile);
            await _influenceProfileRepository.UpdateAsync(influenceProfile);
            await _influenceProfileRepository.SaveChangesAsync();

            return await GetProfileAsync(uid);
        }
        public async Task<List<InfluencerList>> SearchInfluencersAsync(InfluenceSearchOptions opts)
        {
            var influ = await _influenceProfileRepository.SearchAsync(opts);
            return _mapper.Map<List<InfluencerList>>(influ);
        }

        public async Task<List<InfluencerList>> SearchHomeFeaturedAsync(InfluenceSearchOptions opts)
        {
            var influ = await _influenceProfileRepository.SearchHomeFeaturedAsync(opts);
            return _mapper.Map<List<InfluencerList>>(influ);
        }

        public async Task<List<BannerDto>> GetActiveBannersAsync()
        {
            var banners = await _bannerRepo.GetActiveBannersAsync();
            return banners.Select(b => new BannerDto
            {
                BannerId = b.BannerId,
                InfluenceId = b.InfluenceId,
                ImageUrl = b.ImageUrl,
                TargetUrl = b.TargetUrl,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                Nickname = b.InfluenceProfile.Nickname
            }).ToList();
        }

        public async Task<List<InfluencerTypeSelectionModel>> GetAllInfluencerType()
        {
            var influencerTypes = await _influencerTypeRepository.GetAllInfluencerTypesAsync();
            return _mapper.Map<List<InfluencerTypeSelectionModel>>(influencerTypes);
        }

        public async Task<List<InfluencerList>> SearchInfluencer(string? name, string? contentCategory, int? id)
        {
            var influencers = await _influenceProfileRepository.SearchInfluencer(name, contentCategory, id);
            return _mapper.Map<List<InfluencerList>>(influencers);
        }

        public async Task<InfluenceProfileModel> GetInfluenceProfileUsernameAsync(string username)
        {
            return await _profileRepository.GetInfluenceProfileByUsernameAsync(username);
        }

        public async Task<List<InfluenceProfileModel>> GetListInfluenceProfilesByUsernameAsync(string username)
        {
            return await _profileRepository.GetListInfluenceProfilesByUsernameAsync(username);
        }
    }
}
