using BrandLoop.API.Models;
using BrandLoop.API.Response;
using BrandLoop.Application.Interfaces;
using BrandLoop.Application.Service;
using BrandLoop.Infratructure.Models.CampainModel;
using BrandLoop.Infratructure.Models.Influence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BrandLoop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfluencerController : ControllerBase
    {
        private readonly IProfileService _profileService;
        public InfluencerController(IProfileService profileService)
            => _profileService = profileService;

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] InfluenceSearchOptions opts
        )
        {
            var list = await _profileService.SearchInfluencersAsync(opts);
            return Ok(list);
        }
        [HttpGet("featured-home")]
        public async Task<IActionResult> FeaturedHome([FromQuery] InfluenceSearchOptions opts)
        {
            var list = await _profileService.SearchHomeFeaturedAsync(opts);
            return Ok(list);
        }
        [HttpGet("banners")]
        public async Task<IActionResult> GetBanners()
    => Ok(await _profileService.GetActiveBannersAsync());

        [HttpGet("influencer-types")]
        public async Task<IActionResult> GetInfluencerTypes()
        {
            try
            {
                var influencerTypes = await _profileService.GetAllInfluencerType();
                return Ok(influencerTypes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

            [HttpGet("search-influencer")]
            public async Task<IActionResult> SearchInfluencer(string? name, string? contentCategory, int? influencerTypeId, [FromQuery] PaginationFilter filter)
            {
                try
                {
                    var influencers = await _profileService.SearchInfluencer(name, contentCategory, influencerTypeId);
                    if (influencers == null || !influencers.Any())
                    {
                        return Ok();
                    }
                    var totalRecords = influencers.Count;
                    var pagedData = influencers
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                    var response = new PaginationResponse<InfluencerList>(
                    pagedData,
                    filter.PageNumber,
                    filter.PageSize,
                    totalRecords
                    );
                    return Ok(ApiResponse<PaginationResponse<InfluencerList>>.SuccessResult(response, "Lấy danh sách influencer thành công"));
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
    }
}
