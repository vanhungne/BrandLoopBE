using BrandLoop.API.Models;
using BrandLoop.API.Response;
using BrandLoop.Application.Interfaces;
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
        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }
        [HttpGet("all-subscription")]
        [Authorize(Roles = "Admin")]
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

        [HttpGet("{subscriptionId}")]
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
        [HttpPut("update-subscription/{subscriptionId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubscription(int subscriptionId, [FromBody] SubscriptionDTO subscription)
        {
            try
            {
                if (subscription == null || subscription.SubscriptionId != subscriptionId)
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
        [HttpGet("my-subscription/{userId}")]
        [Authorize(Roles = "Influencer")]
        public async Task<IActionResult> GetSubscriptionRegistersOfUser(string userId)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var subscriptions = await _subscriptionService.GetSubscriptionRegistersOfUser(uid);
                if (subscriptions == null || !subscriptions.Any())
                    return NotFound("No subscriptions found for this user.");

                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("my-subscription-register/{id}")]
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

        [HttpPut("confirm")]
        public async Task<IActionResult> ConfirmPayment([FromQuery] long orderCode)
        {
            try
            {
                await _subscriptionService.ConfirmPayment(orderCode);
                return Ok("Thanh toan thanh cong");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("cancel")]
        public async Task<IActionResult> CancelPayment([FromQuery] long orderCode)
        {
            try
            {
                await _subscriptionService.CancelPayment(orderCode);
                return Ok("Thanh toan da huy");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
}
