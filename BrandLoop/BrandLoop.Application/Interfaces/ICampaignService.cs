using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.CampainModel;
using BrandLoop.Infratructure.Models.Dashboard;
using BrandLoop.Infratructure.Models.Report;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Interfaces
{
    public interface ICampaignService
    {
        Task<IEnumerable<CampaignDto>> GetBrandCampaignsAsync(string uid);
        Task<CampaignDto> GetCampaignDetailAsync(int campaignId);
        Task<CampaignDto> CreateCampaignAsync(CreateCampaignDto dto,string uid);
        Task<CampaignDto> UpdateCampaignAsync(int campainId,UpdateCampaignDto dto);
        Task<bool> DeleteCampaignAsync(int campaignId);
        Task<CampaignDto> UpdateCampaignStatusAsync(int campaignId, CampaignStatus status);
        Task<CampaignDto> DuplicateCampaignAsync(int campaignId);
        Task<List<CampaignDto>> GetAllCampaignByUid(string uid);
        Task<List<CampaignDto>> GetAllCampaignByUid(CampaignStatus? status,string? name, string uid);
        Task<PaymentCampaign> StartCampaign(string creatorId, int campaignId);
        Task<CreatePaymentResult> CreatePaymentLink(long orderCode);
        Task ConfirmPayment(long orderCode);
        Task<CampaignDto> EndCampaign(string creatorId, BrandReport brandReport);
        Task<CampaignDto> CancelCampaign(string creatorId, int campaignId);
        Task<List<CampaignDto>> GetAllCampaignsAsync(CampaignFilterModel filter);
        Task Cancelayment(long orderCode);
        Task GiveFeedback(CreateFeedback createFeedback, string userId);
        Task<CampaignTracking> GetCampaignDetail(int campaignId);
        Task<CampaignCard> GetCampaignCard(string uid);
        Task<List<CampaignChart>> GetRevenueChard(string uid, int year);
        Task<List<CampaignSelectOption>> GetCampaignsOf(string uid, CampaignStatus status);
        Task<CampaignDashboardDetail> GetCampaignDetailForDashboard(string uid, int campaignId);
    }

}
