using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Entities;

namespace BrandLoop.Infratructure.Interface
{
    public interface IImageCampainRepository
    {
        Task<CampaignImage> AddImageToCampaignAsync(int campaignId, string imageUrl, string description = null);
        Task<IEnumerable<CampaignImage>> AddMultipleImagesToCampaignAsync(int campaignId, List<string> imageUrls, List<string> descriptions = null);
        Task<IEnumerable<CampaignImage>> GetCampaignImagesAsync(int campaignId);
        Task<CampaignImage> GetCampaignImageByIdAsync(int campaignImageId);
        Task<bool> DeleteCampaignImageAsync(int campaignImageId);
        Task<bool> DeleteAllCampaignImagesAsync(int campaignId);
        Task<CampaignImage> UpdateCampaignImageAsync(int campaignImageId, string imageUrl = null, string description = null);
    }
}
