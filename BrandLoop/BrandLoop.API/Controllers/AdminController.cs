using BrandLoop.Application.Interfaces;
    using BrandLoop.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BrandLoop.Infratructure.Models.Authen;

namespace BrandLoop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAuthenService _authenService;

        public AdminController(IAuthenService authenService)
        {
            _authenService = authenService;
        }

        [HttpGet("pending-registrations")]
        public async Task<IActionResult> GetPendingRegistrations([FromQuery] PaginationFilter filter)
        {
            var allRegistrations = await _authenService.GetPendingRegistrations();
            var totalRecords = allRegistrations.Count;
            
            var pagedData = allRegistrations
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var response = new PaginationResponse<PendingRegistrationDto>(
                pagedData, 
                filter.PageNumber,
                filter.PageSize,
                totalRecords
            );
            
            return Ok(response);
        }

        [HttpPost("approve-registration/{uid}")]
        public async Task<IActionResult> ApproveRegistration(string uid)
        {
            var result = await _authenService.ApproveRegistration(uid);

            if (result)
                return Ok(new { success = true, message = "Registration approved successfully" });

            return NotFound(new { success = false, message = "User not found or already approved" });
        }

        [HttpPost("reject-registration/{uid}")]
        public async Task<IActionResult> RejectRegistration(string uid, [FromBody] RejectRegistrationModel model)
        {
            var result = await _authenService.RejectRegistration(uid, model.Reason);

            if (result)
                return Ok(new { success = true, message = "Registration rejected successfully" });

            return NotFound(new { success = false, message = "User not found or already processed" });
        }

        public class RejectRegistrationModel
        {
            public string Reason { get; set; }
        }
    }
}
