using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.CampainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Interfaces
{
    public interface ICampaignService
    {
        Task<IEnumerable<CampaignDto>> GetBrandCampaignsAsync(int brandId);
        Task<CampaignDto> GetCampaignDetailAsync(int campaignId);
        Task<CampaignDto> CreateCampaignAsync(CreateCampaignDto dto,string uid);
        Task<CampaignDto> UpdateCampaignAsync(UpdateCampaignDto dto);
        Task<bool> DeleteCampaignAsync(int campaignId);
        Task<CampaignDto> UpdateCampaignStatusAsync(int campaignId, CampainStatus status);
        Task<CampaignDto> DuplicateCampaignAsync(int campaignId);
        Task<List<CampaignDto>> GetAllCampaignByUid(string uid);
    }

}
