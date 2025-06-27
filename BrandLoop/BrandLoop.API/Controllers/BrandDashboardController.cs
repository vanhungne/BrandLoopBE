using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace BrandLoop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Brand")]
    public class BrandDashboardController : ControllerBase
    {
        private readonly ICampaignService _campaignService;
        public BrandDashboardController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        /// <summary>
        /// Lay du lieu cho card tren dashboard cua brand
        /// </summary>
        /// 
        [HttpGet("card")]
        public async Task<IActionResult> GetDashboardCard()
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _campaignService.GetCampaignCard(uid);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// Lay du lieu cho bieu do tren dashboard cua brand theo nam
        /// </summary>
        /// 
        [HttpGet("chart")]
        public async Task<IActionResult> GetDashboardChart(int year)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _campaignService.GetCampaignChard(uid, year);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// Lay danh sach cac campaign cua brand theo trang thai
        /// </summary>
        /// 
        [HttpGet("campaigns")]
        public async Task<IActionResult> GetCampaignsOfBrand([FromQuery] CampaignStatus status)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _campaignService.GetCampaignsOf(uid, status);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// Lay du lieu chi tiet cua một campaign tren dashboard
        /// </summary>
        /// 
        [HttpGet("campaign/{campaignId}")]
        public async Task<IActionResult> GetCampaignDetailForDashboard(int campaignId)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _campaignService.GetCampaignDetailForDashboard(uid, campaignId);
                return Ok(result);
            }
            catch (AuthenticationException ae)
            {
                return Forbid();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
