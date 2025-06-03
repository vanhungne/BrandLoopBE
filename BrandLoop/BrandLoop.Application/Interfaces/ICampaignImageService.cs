using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Infratructure.Models.CampainModel;
using Microsoft.AspNetCore.Http;

namespace BrandLoop.Application.Interfaces
{
    public interface ICampaignImageService
    {
        Task<bool> AddImagesToCampaignAsync(int campaignId, List<IFormFile> imageFiles);
        Task<bool> AddSingleImageToCampaignAsync(int campaignId, IFormFile imageFile, string description = null);
        Task<IEnumerable<CampaignImageDto>> GetCampaignImagesAsync(int campaignId);
        Task<bool> DeleteCampaignImageAsync(int campaignImageId);
        Task<bool> DeleteAllCampaignImagesAsync(int campaignId);
    }
}
