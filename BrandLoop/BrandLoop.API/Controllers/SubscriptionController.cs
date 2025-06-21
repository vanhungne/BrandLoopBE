using BrandLoop.API.Models;
using BrandLoop.API.Response;
using BrandLoop.Application.Interfaces;
using BrandLoop.Infratructure.Models.CampainModel;
using BrandLoop.Infratructure.Models.SubcriptionModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrandLoop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ICampaignService _campaignService;
        public SubscriptionController(ISubscriptionService subscriptionService, ICampaignService campaignService)
        {
            _subscriptionService = subscriptionService;
            _campaignService = campaignService;
        }

        /// <summary>
        /// Lấy tất cả các gói đăng ký.
        /// </summary>
        /// <returns>Trả vè toàn bộ các gói</returns>
        [HttpGet("all-subscription")]
        [Authorize(Roles = "Admin, Influencer")]
        public async Task<IActionResult> GetAllSubscriptions()
        {
            try
            {
                var subscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
                if (subscriptions == null || !subscriptions.Any())
                    return NotFound("No subscriptions found.");
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        /// <summary>
        /// Láy thông tin gói đăng ký theo ID.
        /// </summary>
        /// <returns>Trả vè thông tin của gói đăng ký đó</returns>
        [HttpGet("get-detail/{subscriptionId}")]
        public async Task<IActionResult> GetSubscriptionById(int subscriptionId)
        {
            try
            {
                var subscription = await _subscriptionService.GetSubscriptionByIdAsync(subscriptionId);
                if (subscription == null)
                    return NotFound("Subscription not found.");
                return Ok(subscription);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        /// <summary>
        /// Admin thêm mới gói đăng ký.
        /// </summary>
        /// <returns>Trả vè thông tin gói đăng ký đó</returns>
        [HttpPost("add-subscription")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSubscription([FromBody] AddSubcription subscription)
        {
            try
            {
                if (subscription == null)
                    return BadRequest("Invalid subscription data.");
                var addedSubscription = await _subscriptionService.AddSubscriptionAsync(subscription);
                return Ok(addedSubscription);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        /// <summary>
        /// Admin cập nhật thông tin gói đăng ký.
        /// </summary>
        /// <returns>Trả vè thông tin gói đăng ký đã được cập nhật</returns>
        [HttpPut("update-subscription/{subscriptionId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubscription(int subscriptionId, [FromBody] SubscriptionDTO subscription)
        {
            try
            {
                if (subscription == null)
                    return BadRequest("Invalid subscription data.");

                var updatedSubscription = await _subscriptionService.UpdateSubscriptionAsync(subscription);
                if (updatedSubscription == null)
                    return NotFound("Subscription not found.");

                return Ok(updatedSubscription);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Admin xóa gói đăng ký theo ID.(chỉ set trạng thái là xóa, những người đã đăng ký có thể sử dụng đến hết hạn)
        /// </summary>
        /// <returns>Trả vè thông báo</returns>
        [HttpDelete("delete-subscription/{subscriptionId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubscription(int subscriptionId)
        {
            try
            {
                await _subscriptionService.DeleteSubscriptionAsync(subscriptionId);
                return Ok("Subscription deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // User subscription registation methods

        /// <summary>
        /// Lấy tất cả các gói đăng ký đã đăng ký của người dùng.
        /// </summary>
        /// <returns>Trả vè thông tin các gói đăng ký đã được đăng ký</returns>
        [HttpGet("my-registed-subscription")]
        [Authorize(Roles = "Influencer")]
        public async Task<IActionResult> GetSubscriptionRegistersOfUser([FromQuery] PaginationFilter filter)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var subscriptions = await _subscriptionService.GetSubscriptionRegistersOfUser(uid);
                if (subscriptions == null || !subscriptions.Any())
                    return NotFound("No subscriptions found for this user.");

                var totalRecords = subscriptions.Count;
                var pagedData = subscriptions
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

                var response = new PaginationResponse<SubscriptionRegisterDTO>(
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
        /// <summary>
        /// Lấy thông tin gói đăng ký đã đăng ký theo ID.
        /// </summary>
        /// <returns>Trả vè thông tin gói đăng ký đã được đăng ký</returns>
        [HttpGet("my-registed-subscription/{id}")]
        [Authorize(Roles = "Influencer")]
        public async Task<IActionResult> GetSubscriptionRegisterById(int id)
        {
            try
            {
                var subscriptionRegister = await _subscriptionService.GetSubscriptionRegisterByIdAsync(id);
                if (subscriptionRegister == null)
                    return NotFound("Subscription register not found.");

                return Ok(subscriptionRegister);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        /// <summary>
        /// Đang ký gói đăng ký cho người dùng. (Muốn hiển thị gì thì nhắn t)
        /// </summary>
        /// <returns>Trả vè thông tin các gói đăng ký đã được đăng ký</returns>
        [HttpPost("register-subscription/{subscriptionId}")]
        [Authorize(Roles = "Influencer")]
        public async Task<IActionResult> RegisterSubscription(int subscriptionId)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var subscriptionRegister = await _subscriptionService.RegisterSubscription(uid, subscriptionId);
                return Ok(subscriptionRegister);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy liên kết thanh toán cho gói đăng ký đã đăng ký.
        /// </summary>
        /// <returns>Trả vè thông tin liên quan đến PayOS</returns>
        [HttpPost("payment-link/{orderCode}")]
        [Authorize(Roles = "Influencer")]
        public async Task<IActionResult> CreatePaymentLink(long orderCode)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var paymentLink = await _subscriptionService.CreatePaymentLink(uid, orderCode);
                return Ok(paymentLink);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật lại trạng thái thanh toán cho gói đăng ký đã đăng ký.
        /// </summary>
        /// <returns>Trả vè thông báo</returns>
        [HttpPut("confirm-payment")]
        public async Task<IActionResult> ConfirmPayment(
            [FromQuery] string code,
            [FromQuery] string id,
            [FromQuery] bool? cancel,
            [FromQuery] string status,
            [FromQuery] long orderCode
            )
        {
            try
            {
                var payment = await _subscriptionService.GetPaymentByOrderCodeAsync(orderCode);
                if (payment.Type == Domain.Enums.PaymentType.subscription)
                    await _subscriptionService.ConfirmPayment(orderCode);
                else
                    await _campaignService.ConfirmPayment(orderCode);

                return Ok("Thanh toan thanh cong");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật lại trạng thái thanh toán cho gói đăng ký đã đăng ký.
        /// </summary>
        /// <returns>Trả vè thông báo</returns>
        [HttpPut("cancel-payment")]
        public async Task<IActionResult> CancelPayment(
            [FromQuery] long orderCode,
            [FromQuery] string code,
            [FromQuery] string id,
            [FromQuery] bool? cancel,
            [FromQuery] string status)
        {
            try
            {
                var payment = await _subscriptionService.GetPaymentByOrderCodeAsync(orderCode);
                if (payment.Type == Domain.Enums.PaymentType.subscription)
                    await _subscriptionService.CancelPayment(orderCode);
                return Ok("Thanh toan da huy");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
