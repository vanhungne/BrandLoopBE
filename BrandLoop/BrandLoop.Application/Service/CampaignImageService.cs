using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BrandLoop.Application.Interfaces;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.CampainModel;
using BrandLoop.Infratructure.Reporitory;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BrandLoop.Application.Service
{
    public class CampaignImageService : ICampaignImageService
    {
        private readonly IImageCampainRepository _imageCampaignRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CampaignImageService> _logger;
        private readonly IConfiguration _configuration;

        public CampaignImageService(
            IImageCampainRepository imageCampaignRepository,
            IMapper mapper,
            ILogger<CampaignImageService> logger,
            IConfiguration configuration)
        {
            _imageCampaignRepository = imageCampaignRepository ?? throw new ArgumentNullException(nameof(imageCampaignRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<bool> AddImagesToCampaignAsync(int campaignId, List<IFormFile> imageFiles)
        {
            try
            {
                if (campaignId <= 0)
                {
                    throw new ArgumentException("Campaign ID must be greater than 0", nameof(campaignId));
                }

                if (imageFiles == null || !imageFiles.Any())
                {
                    throw new ArgumentException("Image files list cannot be null or empty", nameof(imageFiles));
                }

                var cloudinaryService = new CloundinaryRepository(_configuration);
                var imageUrls = new List<string>();

                // Upload all images to Cloudinary
                foreach (var imageFile in imageFiles)
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var imageUrl = await cloudinaryService.UploadImage(imageFile);
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            imageUrls.Add(imageUrl);
                        }
                    }
                }

                if (imageUrls.Any())
                {
                    // Save image URLs to database
                    await _imageCampaignRepository.AddMultipleImagesToCampaignAsync(campaignId, imageUrls);
                    _logger.LogInformation("Successfully added {Count} images to campaign {CampaignId}", imageUrls.Count, campaignId);
                    return true;
                }

                _logger.LogWarning("No valid images were uploaded for campaign {CampaignId}", campaignId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding images to campaign {CampaignId}", campaignId);
                throw new InvalidOperationException("Error occurred while adding images to campaign", ex);
            }
        }

        public async Task<bool> AddSingleImageToCampaignAsync(int campaignId, IFormFile imageFile, string description = null)
        {
            try
            {
                if (campaignId <= 0)
                {
                    throw new ArgumentException("Campaign ID must be greater than 0", nameof(campaignId));
                }

                if (imageFile == null || imageFile.Length == 0)
                {
                    throw new ArgumentException("Image file cannot be null or empty", nameof(imageFile));
                }

                var cloudinaryService = new CloundinaryRepository(_configuration);
                var imageUrl = await cloudinaryService.UploadImage(imageFile);

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    await _imageCampaignRepository.AddImageToCampaignAsync(campaignId, imageUrl, description);
                    _logger.LogInformation("Successfully added single image to campaign {CampaignId}", campaignId);
                    return true;
                }

                _logger.LogWarning("Failed to upload image for campaign {CampaignId}", campaignId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding single image to campaign {CampaignId}", campaignId);
                throw new InvalidOperationException("Error occurred while adding image to campaign", ex);
            }
        }

        public async Task<IEnumerable<CampaignImageDto>> GetCampaignImagesAsync(int campaignId)
        {
            try
            {
                if (campaignId <= 0)
                {
                    throw new ArgumentException("Campaign ID must be greater than 0", nameof(campaignId));
                }

                var images = await _imageCampaignRepository.GetCampaignImagesAsync(campaignId);
                var result = _mapper.Map<IEnumerable<CampaignImageDto>>(images);

                _logger.LogInformation("Retrieved {Count} images for campaign {CampaignId}", result.Count(), campaignId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting images for campaign {CampaignId}", campaignId);
                throw new InvalidOperationException("Error occurred while getting campaign images", ex);
            }
        }

        public async Task<bool> DeleteCampaignImageAsync(int campaignImageId)
        {
            try
            {
                if (campaignImageId <= 0)
                {
                    throw new ArgumentException("Campaign Image ID must be greater than 0", nameof(campaignImageId));
                }

                var result = await _imageCampaignRepository.DeleteCampaignImageAsync(campaignImageId);

                if (result)
                {
                    _logger.LogInformation("Successfully deleted campaign image {CampaignImageId}", campaignImageId);
                }
                else
                {
                    _logger.LogWarning("Campaign image {CampaignImageId} not found for deletion", campaignImageId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting campaign image {CampaignImageId}", campaignImageId);
                throw new InvalidOperationException("Error occurred while deleting campaign image", ex);
            }
        }

        public async Task<bool> DeleteAllCampaignImagesAsync(int campaignId)
        {
            try
            {
                if (campaignId <= 0)
                {
                    throw new ArgumentException("Campaign ID must be greater than 0", nameof(campaignId));
                }

                var result = await _imageCampaignRepository.DeleteAllCampaignImagesAsync(campaignId);

                if (result)
                {
                    _logger.LogInformation("Successfully deleted all images for campaign {CampaignId}", campaignId);
                }
                else
                {
                    _logger.LogWarning("No images found to delete for campaign {CampaignId}", campaignId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all images for campaign {CampaignId}", campaignId);
                throw new InvalidOperationException("Error occurred while deleting campaign images", ex);
            }
        }
    }
}
