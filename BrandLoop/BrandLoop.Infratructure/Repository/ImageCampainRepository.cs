using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BrandLoop.Infratructure.Repository
{
    public class ImageCampainRepository : IImageCampainRepository
    {
        private readonly BLDBContext _context;
        private readonly ILogger<ImageCampainRepository> _logger;
        private readonly IMapper _mapper;

        public ImageCampainRepository(BLDBContext context, ILogger<ImageCampainRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<CampaignImage> AddImageToCampaignAsync(int campaignId, string imageUrl, string description = null)
        {
            try
            {
                if (campaignId <= 0)
                {
                    throw new ArgumentException("Campaign ID must be greater than 0", nameof(campaignId));
                }

                if (string.IsNullOrWhiteSpace(imageUrl))
                {
                    throw new ArgumentException("Image URL cannot be null or empty", nameof(imageUrl));
                }

                // Check if campaign exists
                var campaignExists = await _context.Campaigns.AnyAsync(c => c.CampaignId == campaignId);
                if (!campaignExists)
                {
                    throw new InvalidOperationException($"Campaign with ID {campaignId} does not exist");
                }

                var campaignImage = new CampaignImage
                {
                    CampaignId = campaignId,
                    ImageUrl = imageUrl,
                    Description = description
                };

                await _context.CampainImages.AddAsync(campaignImage);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Added image to campaign {CampaignId}, ImageId: {ImageId}", campaignId, campaignImage.CampaignImageId);
                return campaignImage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding image to campaign {CampaignId}", campaignId);
                throw;
            }
        }

        public async Task<IEnumerable<CampaignImage>> AddMultipleImagesToCampaignAsync(int campaignId, List<string> imageUrls, List<string> descriptions = null)
        {
            try
            {
                if (campaignId <= 0)
                {
                    throw new ArgumentException("Campaign ID must be greater than 0", nameof(campaignId));
                }

                if (imageUrls == null || !imageUrls.Any())
                {
                    throw new ArgumentException("Image URLs list cannot be null or empty", nameof(imageUrls));
                }

                // Check if campaign exists
                var campaignExists = await _context.Campaigns.AnyAsync(c => c.CampaignId == campaignId);
                if (!campaignExists)
                {
                    throw new InvalidOperationException($"Campaign with ID {campaignId} does not exist");
                }

                var campaignImages = new List<CampaignImage>();

                for (int i = 0; i < imageUrls.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(imageUrls[i]))
                    {
                        var campaignImage = new CampaignImage
                        {
                            CampaignId = campaignId,
                            ImageUrl = imageUrls[i],
                            Description = descriptions != null && i < descriptions.Count ? descriptions[i] : null
                        };
                        campaignImages.Add(campaignImage);
                    }
                }

                if (campaignImages.Any())
                {
                    await _context.CampainImages.AddRangeAsync(campaignImages);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Added {Count} images to campaign {CampaignId}", campaignImages.Count, campaignId);
                return campaignImages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding multiple images to campaign {CampaignId}", campaignId);
                throw;
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

                var campaignImages = await _context.CampainImages
                    .Where(ci => ci.CampaignId == campaignId)
                    .ToListAsync();

                if (!campaignImages.Any())
                {
                    _logger.LogWarning("No images found for campaign {CampaignId}", campaignId);
                    return false;
                }

                _context.CampainImages.RemoveRange(campaignImages);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted {Count} images for campaign {CampaignId}", campaignImages.Count, campaignId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all images for campaign {CampaignId}", campaignId);
                throw;
            }
        }

        public async Task<CampaignImage> UpdateCampaignImageAsync(int campaignImageId, string imageUrl = null, string description = null)
        {
            try
            {
                if (campaignImageId <= 0)
                {
                    throw new ArgumentException("Campaign Image ID must be greater than 0", nameof(campaignImageId));
                }

                var campaignImage = await _context.CampainImages.FindAsync(campaignImageId);
                if (campaignImage == null)
                {
                    _logger.LogWarning("Campaign image {CampaignImageId} not found for update", campaignImageId);
                    return null;
                }

                if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    campaignImage.ImageUrl = imageUrl;
                }

                if (description != null) // Allow setting empty description
                {
                    campaignImage.Description = description;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated campaign image {CampaignImageId}", campaignImageId);
                return campaignImage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating campaign image {CampaignImageId}", campaignImageId);
                throw;
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

                var campaignImage = await _context.CampainImages.FindAsync(campaignImageId);
                if (campaignImage == null)
                {
                    _logger.LogWarning("Campaign image {CampaignImageId} not found for deletion", campaignImageId);
                    return false;
                }

                _context.CampainImages.Remove(campaignImage);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted campaign image {CampaignImageId}", campaignImageId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting campaign image {CampaignImageId}", campaignImageId);
                throw;
            }
        }

        public async Task<IEnumerable<CampaignImage>> GetCampaignImagesAsync(int campaignId)
        {
            try
            {
                if (campaignId <= 0)
                {
                    throw new ArgumentException("Campaign ID must be greater than 0", nameof(campaignId));
                }

                return await _context.CampainImages
                    .Where(ci => ci.CampaignId == campaignId)
                    .OrderBy(ci => ci.CampaignImageId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting images for campaign {CampaignId}", campaignId);
                throw;
            }
        }

        public async Task<CampaignImage> GetCampaignImageByIdAsync(int campaignImageId)
        {
            try
            {
                if (campaignImageId <= 0)
                {
                    throw new ArgumentException("Campaign Image ID must be greater than 0", nameof(campaignImageId));
                }

                return await _context.CampainImages
                    .Include(ci => ci.Campaign)
                    .FirstOrDefaultAsync(ci => ci.CampaignImageId == campaignImageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campaign image {CampaignImageId}", campaignImageId);
                throw;
            }
        }
    }
}
