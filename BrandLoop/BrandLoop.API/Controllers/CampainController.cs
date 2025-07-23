using BrandLoop.API.Models;
using BrandLoop.API.Response;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.CampainModel;
using BrandLoop.Infratructure.Models.Report;
using BrandLoop.Infratructure.Models.SubcriptionModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using System.Security.Authentication;
using System.Security.Claims;

namespace BrandLoop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;
        private readonly IInfluReportService _influReportService;

        public CampaignController(ICampaignService campaignService, IInfluReportService influReportService)
        {
            _campaignService = campaignService ?? throw new ArgumentNullException(nameof(campaignService));
            _influReportService = influReportService ?? throw new ArgumentNullException(nameof(influReportService));
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PaginationResponse<CampaignDto>>>> GetAllCampaigns([FromQuery] CampaignFilterModel filter)
        {
            var result = await _campaignService.GetAllCampaignsAsync(filter);
            var totalRecords = result.Count; // If your service supports total count, use that instead

            // If you want to support paging, apply Skip/Take here
            var pagedData = result
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var response = new PaginationResponse<CampaignDto>(
                pagedData,
                filter.PageNumber,
                filter.PageSize,
                totalRecords
            );

            return Ok(ApiResponse<PaginationResponse<CampaignDto>>.SuccessResult(response, "Lấy tất cả campaign thành công"));
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
                    return Ok();
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
                    ApiResponse<IEnumerable<CampaignDto>>.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// Lấy danh sách campaigns cua brand
        /// </summary>
        /// <returns>Danh sách campaigns</returns>
        [HttpGet("search-brand/my-brand")]
        [Authorize(Roles = "Brand")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CampaignDto>>>> GetMyCampaigns([FromQuery] PaginationFilter filter, CampaignStatus? status, string? name)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var campaigns = await _campaignService.GetAllCampaignByUid(status, name, uid);
                if (campaigns == null || !campaigns.Any())
                {
                    return Ok();
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
                    ApiResponse<IEnumerable<CampaignDto>>.ErrorResult(ex.Message));
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
                    ApiResponse<PaginationResponse<CampaignDto>>.ErrorResult(ex.Message));
            }
        }
        /// <summary>
        /// Lấy chi tiết campaign
        /// </summary>
        /// <param name="campaignId">ID của campaign</param>
        /// <returns>Chi tiết campaign</returns>
        [HttpGet("{campaignId}")]
        public async Task<ActionResult<ApiResponse<CampaignDtoVer2>>> GetCampaignDetail(int campaignId)
        {
            try
            {
                if (campaignId <= 0)
                {
                    return BadRequest(ApiResponse<CampaignDtoVer2>.ErrorResult("Campaign ID phải lớn hơn 0"));
                }

                var campaign = await _campaignService.GetCampaignDetailAsync(campaignId);
                if (campaign == null)
                {
                    return NotFound(ApiResponse<CampaignDtoVer2>.ErrorResult("Không tìm thấy campaign"));
                }

                return Ok(ApiResponse<CampaignDtoVer2>.SuccessResult(campaign, "Lấy chi tiết campaign thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<CampaignDtoVer2>.ErrorResult(ex.Message));
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
                    ApiResponse<CampaignDto>.ErrorResult(ex.Message));
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

                //if (dto.CampaignId != campaignId)
                //{
                //    return BadRequest(ApiResponse<CampaignDto>.ErrorResult("Campaign ID trong URL và body không khớp"));
                //}

                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(ApiResponse<CampaignDto>.ErrorResult($"Dữ liệu không hợp lệ: {errors}"));
                }

                var result = await _campaignService.UpdateCampaignAsync(campaignId, dto);
                if (result == null)
                {
                    return NotFound(ApiResponse<CampaignDto>.ErrorResult("Không tìm thấy campaign để cập nhật"));
                }

                return Ok(ApiResponse<CampaignDto>.SuccessResult(result, "Cập nhật campaign thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<CampaignDto>.ErrorResult(ex.Message));
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
                    ApiResponse<bool>.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// Cập nhật trạng thái campaign
        /// </summary>
        /// <param name="campaignId">ID của campaign</param>
        /// <param name="status">Trạng thái mới</param>
        /// <returns>Campaign đã cập nhật</returns>
        [HttpPatch("{campaignId}/status")]
        public async Task<ActionResult<ApiResponse<CampaignDto>>> UpdateCampaignStatus(int campaignId, [FromBody] CampaignStatus status)
        {
            try
            {
                if (campaignId <= 0)
                {
                    return BadRequest(ApiResponse<CampaignDto>.ErrorResult("Campaign ID phải lớn hơn 0"));
                }

                if (!Enum.IsDefined(typeof(CampaignStatus), status))
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
                    ApiResponse<CampaignDto>.ErrorResult(ex.Message));
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
                    ApiResponse<CampaignDto>.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// Bắt đầu campaign
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
                return BadRequest(ApiResponse<PaymentCampaign>.ErrorResult(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<PaymentCampaign>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<PaymentCampaign>.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// End campaign
        /// </summary>
        [HttpPost("end")]
        [Authorize(Roles = "Brand")]
        public async Task<ActionResult<ApiResponse<CampaignDto>>> EndCampaign([FromBody] BrandReport brandReport)
        {
            try
            {
                var creatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(creatorId))
                    return BadRequest(ApiResponse<CampaignDto>.ErrorResult("Không tìm thấy thông tin người dùng"));

                var result = await _campaignService.EndCampaign(creatorId, brandReport);
                if (result == null)
                    return NotFound(ApiResponse<CampaignDto>.ErrorResult("Không tìm thấy campaign để kết thúc"));

                return Ok(ApiResponse<CampaignDto>.SuccessResult(result, "Kết thúc campaign thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<CampaignDto>.ErrorResult(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<CampaignDto>.ErrorResult($"Truy cập bị từ chối: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<CampaignDto>.ErrorResult(ex.Message));
            }
        }

        ///// <summary>
        ///// Get payment link for campaign
        ///// </summary>
        //[HttpPost("payment-link/{orderCode}")]
        //[Authorize(Roles = "Brand")]
        //public async Task<ActionResult<ApiResponse<CreatePaymentResult>>> CreatePaymentLink(long orderCode)
        //{
        //    try
        //    {
        //        var result = await _campaignService.CreatePaymentLink(orderCode);
        //        if (result == null)
        //            return NotFound(ApiResponse<CreatePaymentResult>.ErrorResult("Không tìm thấy campaign để tạo payment link"));

        //        return Ok(ApiResponse<CreatePaymentResult>.SuccessResult(result, "Tạo payment link thành công"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            ApiResponse<CreatePaymentResult>.ErrorResult($"Lỗi server: {ex.Message}"));
        //    }
        //}

        /// <summary>
        /// Brand feedback cho influencer
        /// </summary>
        [HttpPut("give-feedback")]
        [Authorize(Roles = "Brand")]
        public async Task<ActionResult<ApiResponse<bool>>> GiveFeedback([FromBody] CreateFeedback createFeedback)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return BadRequest(ApiResponse<bool>.ErrorResult("Không tìm thấy thông tin người dùng"));
                await _campaignService.GiveFeedback(createFeedback, userId);
                return Ok(ApiResponse<bool>.SuccessResult(true, "Gửi feedback thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<bool>.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// Influencer nop báo cáo kết quả
        /// </summary>
        [HttpPost("influencer-report")]
        [Authorize(Roles = "Influencer")]
        public async Task<ActionResult<ApiResponse<InfluencerReport>>> SubmitInfluencerReport([FromBody] InfluReport report)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(uid))
                    return BadRequest(ApiResponse<InfluencerReport>.ErrorResult("Không tìm thấy thông tin người dùng"));

                await _influReportService.FinishReport(uid, report);
                return Ok(ApiResponse<string>.SuccessResult("Nộp báo cáo thành công"));
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(ApiResponse<InfluencerReport>.ErrorResult($"Bạn không có quyền thực hiện hành động này: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<InfluencerReport>.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// Lay chi tiết campaign bao gồm các thông tin tracking
        /// </summary>
        [HttpGet("tracking/{campaignId}")]
        [Authorize(Roles = "Brand")]
        public async Task<ActionResult<ApiResponse<CampaignTracking>>> GetCampaignDetailWithTracking(int campaignId)
        {
            try
            {
                if (campaignId <= 0)
                {
                    return BadRequest(ApiResponse<CampaignTracking>.ErrorResult("Campaign ID phải lớn hơn 0"));
                }
                var result = await _campaignService.GetCampaignDetail(campaignId);
                if (result == null)
                {
                    return NotFound(ApiResponse<CampaignTracking>.ErrorResult("Không tìm thấy campaign để lấy thông tin tracking"));
                }
                return Ok(ApiResponse<CampaignTracking>.SuccessResult(result, "Lấy chi tiết campaign thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<CampaignTracking>.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// Lay thong tin report của influencer o campaign (campaignId)
        /// </summary>
        /// 
        [HttpGet("influencer-report/{campaignId}")]
        [Authorize(Roles = "Influencer")]
        public async Task<ActionResult<ApiResponse<InfluencerReportModel>>> GetInfluencerReport(int campaignId)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(uid))
                    return BadRequest(ApiResponse<InfluencerReportModel>.ErrorResult("Không tìm thấy thông tin người dùng"));
                var report = await _influReportService.GetReportByCampaignId(campaignId, uid);
                if (report == null)
                {
                    return NotFound(ApiResponse<InfluencerReportModel>.ErrorResult("Không tìm thấy báo cáo cho campaign này"));
                }
                return Ok(ApiResponse<InfluencerReportModel>.SuccessResult(report, "Lấy báo cáo thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<InfluencerReportModel>.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// Lấy các feedback của brand cho influencer trong campaign
        /// </summary>
        /// 
        [HttpGet("feedback/{campaignId}")]
        [Authorize(Roles = "Brand")]
        public async Task<ActionResult<ApiResponse<IEnumerable<FeedbackDTO>>>> GetFeedbacks(int campaignId)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(uid))
                    return BadRequest(ApiResponse<IEnumerable<FeedbackDTO>>.ErrorResult("Không tìm thấy thông tin người dùng"));
                var feedbacks = await _influReportService.GetFeedbacksOfBrandByCampaignId(campaignId, uid);
                if (feedbacks == null || !feedbacks.Any())
                {
                    return NotFound(ApiResponse<IEnumerable<FeedbackDTO>>.ErrorResult("Không tìm thấy feedback cho campaign này"));
                }
                return Ok(ApiResponse<IEnumerable<FeedbackDTO>>.SuccessResult(feedbacks, "Lấy feedback thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<IEnumerable<FeedbackDTO>>.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// Lay feedback của influencer trong campaign
        /// </summary> 
        /// 
        [HttpGet("feedback/influencer")]
        [Authorize(Roles = "Brand")]
        public async Task<ActionResult<ApiResponse<BrandFeedbackDTO>>> GetFeedbackOfInfluencer(int campaignId, string influencerUID)
        {
            try
            {
                var brandUID = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(brandUID))
                    return BadRequest(ApiResponse<BrandFeedbackDTO>.ErrorResult("Không tìm thấy thông tin người dùng"));
                var feedback = await _influReportService.GetFeedbackOfInfluencerByCampaignId(campaignId, brandUID, influencerUID);
                if (feedback == null)
                    return NotFound(ApiResponse<BrandFeedbackDTO>.ErrorResult("Không tìm thấy feedback cho influencer trong campaign này"));

                

                return Ok(ApiResponse<BrandFeedbackDTO>.SuccessResult(feedback, "Lấy feedback thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<BrandFeedbackDTO>.ErrorResult(ex.Message));
            }
        }
    }
}
