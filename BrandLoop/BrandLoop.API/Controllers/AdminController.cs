using BrandLoop.Application.Interfaces;
    using BrandLoop.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BrandLoop.Infratructure.Models.Authen;
using BrandLoop.Infratructure.Models.UserModel;

namespace BrandLoop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAuthenService _authenService;
        private readonly IInfluencerTypeService _influencerTypeService;

        public AdminController(IAuthenService authenService, IInfluencerTypeService influencerTypeService)
        {
            _authenService = authenService;
            _influencerTypeService = influencerTypeService;
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
        [HttpGet("approve-registrations")]
        public async Task<IActionResult> GetApproveRegistrations([FromQuery] PaginationFilter filter)
        {
            var allRegistrations = await _authenService.GetApproveRegistrations();
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

        [HttpGet("getall-influencer-types")]
        public async Task<IActionResult> GetAllInfluencerTypes()
        {
            try
            {
                var influencerTypes = await _influencerTypeService.GetAllInfluencerTypesAsync();
                if (influencerTypes == null || !influencerTypes.Any())
                    return NotFound(new { success = false, message = "No influencer types found" });

                return Ok(influencerTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "An error occurred while retrieving influencer types", error = ex.Message });
            }
        }

        [HttpPost("add-influencer-type")]
        public async Task<IActionResult> AddInfluencerType([FromBody] InfluTypeModel influencerTypeModel)
        {
            if (influencerTypeModel == null)
                return BadRequest(new { success = false, message = "Invalid influencer type data" });
            try
            {
                var addedInfluencerType = await _influencerTypeService.AddInfluencerTypeAsync(influencerTypeModel);
                return CreatedAtAction(nameof(GetAllInfluencerTypes), new { id = addedInfluencerType.Id }, addedInfluencerType);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "An error occurred while adding influencer type", error = ex.Message });
            }
        }

        [HttpPut("update-influencer-type")]
        public async Task<IActionResult> UpdateInfluencerType([FromBody] InfluTypeModel influencerTypeModel)
        {
            if (influencerTypeModel == null || influencerTypeModel.Id <= 0)
                return BadRequest(new { success = false, message = "Invalid influencer type data" });
            try
            {
                var updatedInfluencerType = await _influencerTypeService.UpdateInfluencerTypeAsync(influencerTypeModel);
                return Ok(updatedInfluencerType);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "An error occurred while updating influencer type", error = ex.Message });
            }
        }
    }
}
