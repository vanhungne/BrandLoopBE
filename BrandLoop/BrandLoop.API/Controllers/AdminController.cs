using BrandLoop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetPendingRegistrations()
        {
            var pendingRegistrations = await _authenService.GetPendingRegistrations();
            return Ok(pendingRegistrations);
        }
        [HttpPost("approve-registration/{email}")]
        public async Task<IActionResult> ApproveRegistration(string email)
        {
            var result = await _authenService.ApproveRegistration(email);

            if (result)
                return Ok(new { success = true, message = "Registration approved successfully" });

            return NotFound(new { success = false, message = "User not found or already approved" });
        }
        [HttpPost("reject-registration/{email}")]
        public async Task<IActionResult> RejectRegistration(string email, [FromBody] RejectRegistrationModel model)
        {
            var result = await _authenService.RejectRegistration(email, model.Reason);

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
