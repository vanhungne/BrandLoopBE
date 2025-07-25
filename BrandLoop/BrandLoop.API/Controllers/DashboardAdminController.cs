using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrandLoop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardAdminController : ControllerBase
    {
        private readonly IAdminDashboardService _adminDashboardService;
        public DashboardAdminController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        /// <summary>
        /// Lay bieu do doanh thu theo nam
        /// </summary>
        [HttpGet("payment-chart")]
        public async Task<IActionResult> GetPaymentChart(int? year)
        {
            try
            {
                var result = await _adminDashboardService.GetPaymentChart(year);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// Lay bieu do nguoi dung theo nam
        /// </summary>
        [HttpGet("user-chart")]
        public async Task<IActionResult> GetUserChart(int? year)
        {
            try
            {
                var result = await _adminDashboardService.GetUserChart(year);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// Ban tai khoan nguoi dung
        /// </summary>
        /// 
        [HttpPut("ban-user/{uid}")]
        public async Task<IActionResult> BanUser(string uid)
        {
            try
            {
                await _adminDashboardService.BanUser(uid);
                return Ok(new { message = "User banned successfully." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// Go ban tai khoan nguoi dung
        /// </summary>
        /// 
        [HttpPut("unban-user/{uid}")]
        public async Task<IActionResult> UnBanUser(string uid)
        {
            try
            {
                await _adminDashboardService.UnBanUser(uid);
                return Ok(new { message = "User unbanned successfully." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả giao dịch
        /// </summary>
        /// 
        [HttpGet("payments")]
        public async Task<IActionResult> GetAllPayment(int? year, PaymentStatus? status, PaymentType? type)
        {
            try
            {
                var result = await _adminDashboardService.GetAllPayment(year, status, type);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// Láy chi tiết giao dịch
        /// </summary>
        /// 
        [HttpGet("payment-detail/{paymentId}")]
        public async Task<IActionResult> GetPaymentDetail(long paymentId)
        {
            try
            {
                var result = await _adminDashboardService.GetPaymentDetail(paymentId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
