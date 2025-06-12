using BrandLoop.API.Models;
using BrandLoop.API.Response;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.CampainModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrandLoop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;

        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService ?? throw new ArgumentNullException(nameof(campaignService));
        }

        /// <summary>
        /// Lấy danh sách campaigns của một brand
        /// </summary>
        /// <param name="brandId">ID của brand</param>
        /// <returns>Danh sách campaigns</returns>
        [HttpGet("brand/{brandId}")]
        public async Task<ActionResult<ApiResponse<PaginationResponse<CampaignDto>>>> GetBrandCampaigns(
            int brandId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (brandId <= 0)
                {
                    return BadRequest(ApiResponse<PaginationResponse<CampaignDto>>.ErrorResult("Brand ID phải lớn hơn 0"));
                }

                var allCampaigns = await _campaignService.GetBrandCampaignsAsync(brandId);
                var totalRecords = allCampaigns.Count();

                var pagedData = allCampaigns
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var response = new PaginationResponse<CampaignDto>(
                    pagedData,
                    pageNumber,
                    pageSize,
                    totalRecords
                );

                return Ok(ApiResponse<PaginationResponse<CampaignDto>>.SuccessResult(response, "Lấy danh sách campaigns thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<PaginationResponse<CampaignDto>>.ErrorResult($"Lỗi server: {ex.Message}"));
            }
        }
        /// <summary>
        /// Lấy chi tiết campaign
        /// </summary>
        /// <param name="campaignId">ID của campaign</param>
        /// <returns>Chi tiết campaign</returns>
        [HttpGet("{campaignId}")]
        public async Task<ActionResult<ApiResponse<CampaignDto>>> GetCampaignDetail(int campaignId)
        {
            try
            {
                if (campaignId <= 0)
                {
                    return BadRequest(ApiResponse<CampaignDto>.ErrorResult("Campaign ID phải lớn hơn 0"));
                }

                var campaign = await _campaignService.GetCampaignDetailAsync(campaignId);
                if (campaign == null)
                {
                    return NotFound(ApiResponse<CampaignDto>.ErrorResult("Không tìm thấy campaign"));
                }

                return Ok(ApiResponse<CampaignDto>.SuccessResult(campaign, "Lấy chi tiết campaign thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<CampaignDto>.ErrorResult($"Lỗi server: {ex.Message}"));
            }
        }

        /// <summary>
        /// Tạo campaign mới
        /// </summary>
        /// <param name="dto">Thông tin campaign cần tạo</param>
        /// <returns>Campaign đã tạo</returns>
        [Authorize(Roles = "Brand,Admin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CampaignDto>>> CreateCampaign([FromBody] CreateCampaignDto dto)
        {

            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(ApiResponse<CampaignDto>.ErrorResult($"Dữ liệu không hợp lệ: {errors}"));
                }

                var result = await _campaignService.CreateCampaignAsync(dto,uid);
                if (result == null)
                {
                    return BadRequest(ApiResponse<CampaignDto>.ErrorResult("Không thể tạo campaign"));
                }

                return CreatedAtAction(nameof(GetCampaignDetail), new { campaignId = result.CampaignId },
                    ApiResponse<CampaignDto>.SuccessResult(result, "Tạo campaign thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<CampaignDto>.ErrorResult($"Lỗi server: {ex.Message}"));
            }
        }

        /// <summary>
        /// Cập nhật campaign
        /// </summary>
        /// <param name="campaignId">ID của campaign</param>
        /// <param name="dto">Thông tin cập nhật</param>
        /// <returns>Campaign đã cập nhật</returns>
        [HttpPut("{campaignId}")]
        public async Task<ActionResult<ApiResponse<CampaignDto>>> UpdateCampaign(int campaignId, [FromBody] UpdateCampaignDto dto)
        {
            try
            {
                if (campaignId <= 0)
                {
                    return BadRequest(ApiResponse<CampaignDto>.ErrorResult("Campaign ID phải lớn hơn 0"));
                }

                if (dto.CampaignId != campaignId)
                {
                    return BadRequest(ApiResponse<CampaignDto>.ErrorResult("Campaign ID trong URL và body không khớp"));
                }

                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(ApiResponse<CampaignDto>.ErrorResult($"Dữ liệu không hợp lệ: {errors}"));
                }

                var result = await _campaignService.UpdateCampaignAsync(dto);
                if (result == null)
                {
                    return NotFound(ApiResponse<CampaignDto>.ErrorResult("Không tìm thấy campaign để cập nhật"));
                }

                return Ok(ApiResponse<CampaignDto>.SuccessResult(result, "Cập nhật campaign thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<CampaignDto>.ErrorResult($"Lỗi server: {ex.Message}"));
            }
        }

        /// <summary>
        /// Xóa campaign
        /// </summary>
        /// <param name="campaignId">ID của campaign</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{campaignId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCampaign(int campaignId)
        {
            try
            {
                if (campaignId <= 0)
                {
                    return BadRequest(ApiResponse<bool>.ErrorResult("Campaign ID phải lớn hơn 0"));
                }

                var result = await _campaignService.DeleteCampaignAsync(campaignId);
                if (!result)
                {
                    return NotFound(ApiResponse<bool>.ErrorResult("Không tìm thấy campaign để xóa"));
                }

                return Ok(ApiResponse<bool>.SuccessResult(true, "Xóa campaign thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<bool>.ErrorResult($"Lỗi server: {ex.Message}"));
            }
        }

        /// <summary>
        /// Cập nhật trạng thái campaign
        /// </summary>
        /// <param name="campaignId">ID của campaign</param>
        /// <param name="status">Trạng thái mới</param>
        /// <returns>Campaign đã cập nhật</returns>
        [HttpPatch("{campaignId}/status")]
        public async Task<ActionResult<ApiResponse<CampaignDto>>> UpdateCampaignStatus(int campaignId, [FromBody] CampainStatus status)
        {
            try
            {
                if (campaignId <= 0)
                {
                    return BadRequest(ApiResponse<CampaignDto>.ErrorResult("Campaign ID phải lớn hơn 0"));
                }

                if (!Enum.IsDefined(typeof(CampainStatus), status))
                {
                    return BadRequest(ApiResponse<CampaignDto>.ErrorResult("Trạng thái không hợp lệ"));
                }

                var result = await _campaignService.UpdateCampaignStatusAsync(campaignId, status);
                if (result == null)
                {
                    return NotFound(ApiResponse<CampaignDto>.ErrorResult("Không tìm thấy campaign để cập nhật trạng thái"));
                }

                return Ok(ApiResponse<CampaignDto>.SuccessResult(result, "Cập nhật trạng thái campaign thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<CampaignDto>.ErrorResult($"Lỗi server: {ex.Message}"));
            }
        }

        /// <summary>
        /// Nhân bản campaign
        /// </summary>
        /// <param name="campaignId">ID của campaign gốc</param>
        /// <returns>Campaign đã nhân bản</returns>
        [HttpPost("{campaignId}/duplicate")]
        public async Task<ActionResult<ApiResponse<CampaignDto>>> DuplicateCampaign(int campaignId)
        {
            try
            {
                if (campaignId <= 0)
                {
                    return BadRequest(ApiResponse<CampaignDto>.ErrorResult("Campaign ID phải lớn hơn 0"));
                }

                var result = await _campaignService.DuplicateCampaignAsync(campaignId);
                if (result == null)
                {
                    return NotFound(ApiResponse<CampaignDto>.ErrorResult("Không tìm thấy campaign để nhân bản"));
                }

                return CreatedAtAction(nameof(GetCampaignDetail), new { campaignId = result.CampaignId },
                    ApiResponse<CampaignDto>.SuccessResult(result, "Nhân bản campaign thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<CampaignDto>.ErrorResult($"Lỗi server: {ex.Message}"));
            }
        }
    }
}
