using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.CampainModel;
using Microsoft.Extensions.Logging;

namespace BrandLoop.Application.Service
{
    public class CampaignService : ICampaignService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IImageCampainRepository _imageCampaignRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CampaignService> _logger;

        public CampaignService(
            ICampaignRepository campaignRepository,
            IMapper mapper,
            ILogger<CampaignService> logger,
            IImageCampainRepository imageCampaignRepository)
        {
            _campaignRepository = campaignRepository ?? throw new ArgumentNullException(nameof(campaignRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _imageCampaignRepository = imageCampaignRepository;
        }

        public async Task<IEnumerable<CampaignDto>> GetBrandCampaignsAsync(int brandId)
        {
            try
            {
                if (brandId <= 0)
                {
                    _logger.LogWarning("Invalid brandId: {BrandId}", brandId);
                    throw new ArgumentException("Brand ID phải lớn hơn 0", nameof(brandId));
                }

                var campaigns = await _campaignRepository.GetBrandCampaignsAsync(brandId);
                var result = _mapper.Map<IEnumerable<CampaignDto>>(campaigns);

                _logger.LogInformation("Retrieved {Count} campaigns for brand {BrandId}", result.Count(), brandId);
                return result;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting campaigns for brand {BrandId}", brandId);
                throw new InvalidOperationException("Lỗi khi lấy danh sách campaigns", ex);
            }
        }

        public async Task<CampaignDto> GetCampaignDetailAsync(int campaignId)
        {
            try
            {
                if (campaignId <= 0)
                {
                    _logger.LogWarning("Invalid campaignId: {CampaignId}", campaignId);
                    throw new ArgumentException("Campaign ID phải lớn hơn 0", nameof(campaignId));
                }

                var campaign = await _campaignRepository.GetCampaignDetailAsync(campaignId);
                if (campaign == null)
                {
                    _logger.LogWarning("Campaign not found: {CampaignId}", campaignId);
                    return null;
                }

                var result = _mapper.Map<CampaignDto>(campaign);
                _logger.LogInformation("Retrieved campaign detail for {CampaignId}", campaignId);
                return result;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting campaign detail {CampaignId}", campaignId);
                throw new InvalidOperationException("Lỗi khi lấy chi tiết campaign", ex);
            }
        }

        public async Task<CampaignDto> CreateCampaignAsync(CreateCampaignDto dto, string uid)
        {
            var branid =await _campaignRepository.getIdBrand(uid);
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                // Validate business rules
                if (string.IsNullOrWhiteSpace(dto.CampaignName))
                {
                    throw new ArgumentException("Tên campaign không được để trống");
                }

                if (dto.Deadline <= DateTime.Now)
                {
                    throw new ArgumentException("Deadline phải lớn hơn thời gian hiện tại");
                }

                if (dto.ImageUrls != null && dto.ImageUrls.Any())
                {
                    var invalidUrls = dto.ImageUrls.Where(url => string.IsNullOrWhiteSpace(url)).ToList();
                    if (invalidUrls.Any())
                    {
                        throw new ArgumentException("Các URL hình ảnh không được để trống");
                    }
                }

                var campaign = _mapper.Map<Campaign>(dto);
                campaign.UploadedDate = DateTime.Now;
                campaign.LastUpdate = DateTime.Now;
                campaign.Status = CampainStatus.Pending;
                campaign.CreatedBy = uid ?? throw new ArgumentNullException(nameof(uid), "User ID không được để trống");
                campaign.BrandId = branid;

                var result = await _campaignRepository.CreateCampaignAsync(campaign);

                // Upload images nếu có
                if (dto.ImageUrls != null && dto.ImageUrls.Any())
                {
                    try
                    {
                        await _imageCampaignRepository.AddMultipleImagesToCampaignAsync(
                            result.CampaignId,
                            dto.ImageUrls,
                            dto.ImageDescriptions);

                        _logger.LogInformation("Added {Count} images to campaign {CampaignId}",
                            dto.ImageUrls.Count, result.CampaignId);
                    }
                    catch (Exception imageEx)
                    {
                        _logger.LogError(imageEx, "Error uploading images for campaign {CampaignId}. Campaign created but without images.",
                            result.CampaignId);
                        // Không throw exception để campaign vẫn được tạo thành công
                    }
                }
                var mappedResult = _mapper.Map<CampaignDto>(result);

                _logger.LogInformation("Created new campaign {CampaignId} for brand {BrandId}", result.CampaignId, result.BrandId);
                return mappedResult;
            }
            catch (ArgumentNullException ex)
            {
                throw new InvalidOperationException("A required argument was null", ex);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException("Invalid argument provided", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating campaign for brand {BrandId}");
                throw new InvalidOperationException("Lỗi khi tạo campaign", ex);
            }
        }

        public async Task<CampaignDto> UpdateCampaignAsync(UpdateCampaignDto dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                if (dto.CampaignId <= 0)
                {
                    throw new ArgumentException("Campaign ID phải lớn hơn 0");
                }

                var existingCampaign = await _campaignRepository.GetCampaignDetailAsync(dto.CampaignId);
                if (existingCampaign == null)
                {
                    _logger.LogWarning("Campaign not found for update: {CampaignId}", dto.CampaignId);
                    return null;
                }

                // Validate business rules
                if (!string.IsNullOrWhiteSpace(dto.CampaignName) && dto.CampaignName != existingCampaign.CampaignName)
                {
                    // Additional validation if needed
                }

                if (dto.Deadline.HasValue && dto.Deadline <= DateTime.Now)
                {
                    throw new ArgumentException("Deadline phải lớn hơn thời gian hiện tại");
                }

                _mapper.Map(dto, existingCampaign);
                existingCampaign.LastUpdate = DateTime.Now;

                var result = await _campaignRepository.UpdateCampaignAsync(existingCampaign);
                var mappedResult = _mapper.Map<CampaignDto>(result);

                _logger.LogInformation("Updated campaign {CampaignId}", dto.CampaignId);
                return mappedResult;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating campaign {CampaignId}", dto?.CampaignId);
                throw new InvalidOperationException("Lỗi khi cập nhật campaign", ex);
            }
        }

        public async Task<bool> DeleteCampaignAsync(int campaignId)
        {
            try
            {
                if (campaignId <= 0)
                {
                    _logger.LogWarning("Invalid campaignId for delete: {CampaignId}", campaignId);
                    throw new ArgumentException("Campaign ID phải lớn hơn 0", nameof(campaignId));
                }

                var result = await _campaignRepository.DeleteCampaignAsync(campaignId);
                if (result)
                {
                    _logger.LogInformation("Deleted campaign {CampaignId}", campaignId);
                }
                else
                {
                    _logger.LogWarning("Campaign not found for delete: {CampaignId}", campaignId);
                }

                return result;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting campaign {CampaignId}", campaignId);
                throw new InvalidOperationException("Lỗi khi xóa campaign", ex);
            }
        }

        public async Task<CampaignDto> UpdateCampaignStatusAsync(int campaignId, CampainStatus status)
        {
            try
            {
                if (campaignId <= 0)
                {
                    throw new ArgumentException("Campaign ID phải lớn hơn 0", nameof(campaignId));
                }

                if (!Enum.IsDefined(typeof(CampainStatus), status))
                {
                    throw new ArgumentException("Trạng thái không hợp lệ", nameof(status));
                }

                var result = await _campaignRepository.UpdateCampaignStatusAsync(campaignId, status);
                if (result == null)
                {
                    _logger.LogWarning("Campaign not found for status update: {CampaignId}", campaignId);
                    return null;
                }

                var mappedResult = _mapper.Map<CampaignDto>(result);
                _logger.LogInformation("Updated campaign {CampaignId} status to {Status}", campaignId, status);
                return mappedResult;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating campaign {CampaignId} status", campaignId);
                throw new InvalidOperationException("Lỗi khi cập nhật trạng thái campaign", ex);
            }
        }

        public async Task<CampaignDto> DuplicateCampaignAsync(int campaignId)
        {
            try
            {
                if (campaignId <= 0)
                {
                    throw new ArgumentException("Campaign ID phải lớn hơn 0", nameof(campaignId));
                }

                var result = await _campaignRepository.DuplicateCampaignAsync(campaignId);
                if (result == null)
                {
                    _logger.LogWarning("Campaign not found for duplication: {CampaignId}", campaignId);
                    return null;
                }

                var mappedResult = _mapper.Map<CampaignDto>(result);
                _logger.LogInformation("Duplicated campaign {OriginalCampaignId} to new campaign {NewCampaignId}",
                    campaignId, result.CampaignId);
                return mappedResult;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while duplicating campaign {CampaignId}", campaignId);
                throw new InvalidOperationException("Lỗi khi nhân bản campaign", ex);
            }
        }
    }

}
