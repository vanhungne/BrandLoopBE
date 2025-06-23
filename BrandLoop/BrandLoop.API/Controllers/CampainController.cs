using BrandLoop.API.Models;
using BrandLoop.API.Response;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.CampainModel;
using BrandLoop.Infratructure.Models.SubcriptionModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
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

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<CampaignDto>>>> GetAllCampaigns([FromQuery] CampaignFilterModel filter)
        {
            var result = await _campaignService.GetAllCampaignsAsync(filter);
            return Ok(ApiResponse<List<CampaignDto>>.SuccessResult(result, "Lấy tất cả campaign thành công"));
        }

        /// <summary>
        /// Lấy danh sách campaigns cua brand
        /// </summary>
        /// <returns>Danh sách campaigns</returns>
        [HttpGet("brand/my-brand")]
        [Authorize(Roles = "Brand")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CampaignDto>>>> GetMyCampaigns([FromQuery] PaginationFilter filter)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var campaigns = await _campaignService.GetAllCampaignByUid(uid);
                if (campaigns == null || !campaigns.Any())
                {
                    return NotFound(ApiResponse<List<CampaignDto>>.ErrorResult("Không tìm thấy campaigns cho brand này"));
                }
                var totalRecords = campaigns.Count;
                var pagedData = campaigns
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

                var response = new PaginationResponse<CampaignDto>(
                pagedData,
                filter.PageNumber,
                filter.PageSize,
                totalRecords
                );
                return Ok(ApiResponse<PaginationResponse<CampaignDto>>.SuccessResult(response, "Lấy danh sách campaigns thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<IEnumerable<CampaignDto>>.ErrorResult($"Lỗi server: {ex.Message}"));
            }
        }

        /// <summary>
        /// Lấy danh sách campaigns của một brand
        /// </summary>
        /// <param name="brandId">ID của brand</param>
        /// <returns>Danh sách campaigns</returns>
        [HttpGet("brand")]
        public async Task<ActionResult<ApiResponse<PaginationResponse<CampaignDto>>>> GetBrandCampaigns(
            int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (uid == null)
                {
                    return BadRequest(ApiResponse<PaginationResponse<CampaignDto>>.ErrorResult("Brand ID phải lớn hơn 0"));
                }

                var allCampaigns = await _campaignService.GetBrandCampaignsAsync(uid);
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

                var result = await _campaignService.CreateCampaignAsync(dto, uid);
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

        /// <summary>
        /// Start campaign
        /// </summary>
        [HttpPost("{campaignId}/start")]
        [Authorize(Roles = "Brand")]
        public async Task<ActionResult<ApiResponse<PaymentCampaign>>> StartCampaign(int campaignId)
        {
            try
            {
                var creatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(creatorId))
                    return BadRequest(ApiResponse<PaymentCampaign>.ErrorResult("Không tìm thấy thông tin người dùng"));

                var result = await _campaignService.StartCampaign(creatorId, campaignId);
                if (result == null)
                    return NotFound(ApiResponse<PaymentCampaign>.ErrorResult("Không tìm thấy campaign để bắt đầu"));

                return Ok(ApiResponse<PaymentCampaign>.SuccessResult(result, "Bắt đầu campaign thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<PaymentCampaign>.ErrorResult($"Lỗi: {ex.Message}"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<PaymentCampaign>.ErrorResult($"Truy cập bị từ chối: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<PaymentCampaign>.ErrorResult($"Lỗi server: {ex.Message}"));
            }
        }

        /// <summary>
        /// End campaign
        /// </summary>
        [HttpPost("{campaignId}/end")]
        [Authorize(Roles = "Brand")]
        public async Task<ActionResult<ApiResponse<CampaignDto>>> EndCampaign(int campaignId)
        {
            try
            {
                var creatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(creatorId))
                    return BadRequest(ApiResponse<CampaignDto>.ErrorResult("Không tìm thấy thông tin người dùng"));

                var result = await _campaignService.EndCampaign(creatorId, campaignId);
                if (result == null)
                    return NotFound(ApiResponse<CampaignDto>.ErrorResult("Không tìm thấy campaign để kết thúc"));

                return Ok(ApiResponse<CampaignDto>.SuccessResult(result, "Kết thúc campaign thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<CampaignDto>.ErrorResult($"Lỗi: {ex.Message}"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<CampaignDto>.ErrorResult($"Truy cập bị từ chối: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<CampaignDto>.ErrorResult($"Lỗi server: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get payment link for campaign
        /// </summary>
        [HttpPost("payment-link/{orderCode}")]
        [Authorize(Roles = "Brand")]
        public async Task<ActionResult<ApiResponse<CreatePaymentResult>>> CreatePaymentLink(long orderCode)
        {
            try
            {
                var result = await _campaignService.CreatePaymentLink(orderCode);
                if (result == null)
                    return NotFound(ApiResponse<CreatePaymentResult>.ErrorResult("Không tìm thấy campaign để tạo payment link"));

                return Ok(ApiResponse<CreatePaymentResult>.SuccessResult(result, "Tạo payment link thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<CreatePaymentResult>.ErrorResult($"Lỗi server: {ex.Message}"));
            }
        }
    }
}
