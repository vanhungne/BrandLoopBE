using BrandLoop.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BrandLoop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompainImageController : ControllerBase
    {
        private readonly ICampaignImageService _campaignImageService;
        public CompainImageController(ICampaignImageService campaignImageService)
        {
            _campaignImageService = campaignImageService ?? throw new ArgumentNullException(nameof(campaignImageService));
        }
        [HttpPost("campaign/{campaignId}/images")]
        public async Task<IActionResult> AddImagesToCampaign(int campaignId, [FromForm] List<IFormFile> imageFiles)
        {
            try
            {
                if (imageFiles == null || !imageFiles.Any())
                {
                    return BadRequest("No image files provided");
                }

                var result = await _campaignImageService.AddImagesToCampaignAsync(campaignId, imageFiles);

                if (result)
                {
                    return Ok(new { message = "Images added successfully" });
                }

                return BadRequest("Failed to add images");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("campaign/{campaignId}/image")]
        public async Task<IActionResult> AddSingleImageToCampaign(
            int campaignId,
            [FromForm] CampaignImageUploadModel model)
        {
            try
            {
                if (model.ImageFile == null)
                {
                    return BadRequest("No image file provided");
                }

                var result = await _campaignImageService.AddSingleImageToCampaignAsync(
                    campaignId, model.ImageFile, model.Description);

                if (result)
                {
                    return Ok(new { message = "Image added successfully" });
                }

                return BadRequest("Failed to add image");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error occurred");
            }
        }
        [HttpGet("campaign/{campaignId}/images")]
        public async Task<IActionResult> GetCampaignImages(int campaignId)
        {
            try
            {
                var images = await _campaignImageService.GetCampaignImagesAsync(campaignId);
                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        [HttpDelete("campaign/image/{campaignImageId}")]
        public async Task<IActionResult> DeleteCampaignImage(int campaignImageId)
        {
            try
            {
                var result = await _campaignImageService.DeleteCampaignImageAsync(campaignImageId);
                if (result)
                {
                    return Ok(new { message = "Image deleted successfully" });
                }
                return NotFound("Image not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        [HttpDelete("campaign/{campaignId}/images")]
        public async Task<IActionResult> DeleteAllCampaignImages(int campaignId)
        {
            try
            {
                var result = await _campaignImageService.DeleteAllCampaignImagesAsync(campaignId);
                if (result)
                {
                    return Ok(new { message = "All images deleted successfully" });
                }
                return NotFound("No images found for this campaign");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        public class CampaignImageUploadModel
        {
            [FromForm(Name = "imageFile")]
            public IFormFile ImageFile { get; set; }

            [FromForm(Name = "description")]
            public string Description { get; set; }
        }
    }
}