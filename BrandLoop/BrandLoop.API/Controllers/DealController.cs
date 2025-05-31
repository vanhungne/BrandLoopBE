using BrandLoop.API.Models;
using BrandLoop.API.Response;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Models.CampainModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrandLoop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DealController : ControllerBase
    {
        private readonly IDealService _dealService;
        public DealController(IDealService dealService)
        {
            _dealService = dealService;
        }

        [HttpGet("campaign/{campaignId}")]
        [Authorize(Roles = "Brand")]
        public async Task<IActionResult> GetAllDealsByCampaignId(int campaignId, [FromQuery] PaginationFilter filter)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var deals = await _dealService.GetAllDealsByCampaignId(campaignId, uid);
                var totalRecords = deals.Count;
                if (deals == null || !deals.Any())
                    return NotFound(ApiResponse<string>.ErrorResult("Can not found any deal"));

                var pagedData = deals
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

                var response = new PaginationResponse<DealDTO>(
                pagedData,
                filter.PageNumber,
                filter.PageSize,
                totalRecords
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-deals")]
        [Authorize(Roles = "KOL")]
        public async Task<IActionResult> GetAllKolDeals([FromQuery] PaginationFilter filter)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var deals = await _dealService.GetAllKolDeals(uid);
                var totalRecords = deals.Count;
                if (deals == null || !deals.Any())
                    return NotFound(ApiResponse<string>.ErrorResult("Can not found any deal"));

                var pagedData = deals
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

                var response = new PaginationResponse<DealDTO>(
                pagedData,
                filter.PageNumber,
                filter.PageSize,
                totalRecords
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("detail/{dealId}")]
        [Authorize]
        public async Task<IActionResult> GetDealByIdAsync(int dealId)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var deal = await _dealService.GetDealByIdAsync(dealId, uid);
                if (deal == null)
                    return NotFound(ApiResponse<string>.ErrorResult("Deal not found"));
                return Ok(deal);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateDeal(int id, [FromBody] string description)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var deal = await _dealService.UpdateDeal(id, description, uid);
                return Ok(deal);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
