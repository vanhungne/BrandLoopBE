using BrandLoop.Application.Interfaces;
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
    }
}
