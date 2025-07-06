using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrandLoop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Influencer")]
    public class DashboardInfluencerController : ControllerBase
    {
        private readonly IInfluencerDashboardService _influencerDashboardService;
        public DashboardInfluencerController(IInfluencerDashboardService influencerDashboardService)
        {
            _influencerDashboardService = influencerDashboardService;
        }


        /// <summary>
        /// Lay hong tin cho card tren dashboard cua influencer
        /// </summary>
        [HttpGet("campaign-card")]
        public async Task<IActionResult> GetCampaignCard()
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _influencerDashboardService.GetCampaignCardAsync(uid);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// Lay thong tin cho bieu do tren dashboard cua influencer theo nam
        /// </summary>
        [HttpGet("revenue-chart-by-year")]
        public async Task<IActionResult> GetRevenueChartByYear(int year)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _influencerDashboardService.GetRevenueChart(uid, year);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// Lay thong tin cac goi subscription dang hoat dong cua influencer
        /// </summary>
        [HttpGet("subscription-registers")]
        public async Task<IActionResult> GetActiveSubscriptionRegisters()
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _influencerDashboardService.GetActiveSubscriptionRegisterAsync(uid);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// Lay thong tin cac option de chon campaign tren dashboard cua influencer
        /// </summary>
        /// 
        [HttpGet("campaign-select-option")]
        public async Task<IActionResult> GetCampaignSelectOption(CampaignStatus status)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _influencerDashboardService.GetCampaignSelectOption(uid, status);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// Lay thong tin bao cao cua influencer ve campaign
        /// </summary>
        /// 
        [HttpGet("campaign-report/{campaignId}")]
        public async Task<IActionResult> GetCampaignReportOfInfluencer(int campaignId)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _influencerDashboardService.GetCampaignReportOfInfluencerAsync(uid, campaignId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
