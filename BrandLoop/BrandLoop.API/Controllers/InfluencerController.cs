using BrandLoop.Application.Interfaces;
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
    }
}
