using BrandLoop.API.Response;
using BrandLoop.Application.Interfaces;
using BrandLoop.Infratructure.Models.UserModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrandLoop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Brand,Influencer,Admin")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;


        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }   

        /// <summary>
        /// Lấy thông tin basic profile của user
        /// </summary>
        [HttpGet("basic")]
        public async Task<IActionResult> GetBasicAccountProfile()
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _profileService.GetBasicAccountProfileAsync(uid);
            if (result == null)
                return NotFound(ApiResponse<object>.ErrorResult("Không tìm thấy thông tin người dùng"));

            return Ok(ApiResponse<object>.SuccessResult(result));
        }

        /// <summary>
        /// Lấy thông tin brand profile
        /// </summary>
        /// 
        [HttpGet("brand")]
        public async Task<IActionResult> GetBrandProfile()
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _profileService.GetBrandProfileAsync(uid);
            if (result == null)
                return NotFound(ApiResponse<object>.ErrorResult("Không tìm thấy thông tin thương hiệu"));

            return Ok(ApiResponse<object>.SuccessResult(result));
        }

        /// <summary>
        /// Lấy thông tin influence profile
        /// </summary>
        [HttpGet("influence")]
        public async Task<IActionResult> GetInfluenceProfile()
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _profileService.GetInfluenceProfileAsync(uid);
            if (result == null)
                return NotFound(ApiResponse<object>.ErrorResult("Không tìm thấy thông tin influencer"));

            return Ok(ApiResponse<object>.SuccessResult(result));
        }

        /// <summary>
        /// Lấy thông tin profile của user (tự động detect role)
        /// </summary>
        [HttpGet("{uid}")]
        public async Task<IActionResult> GetUserProfile(string uid)
        {
            var result = await _profileService.GetUserProfileAsync(uid);
            if (result == null)
                return NotFound(ApiResponse<object>.ErrorResult("Không tìm thấy thông tin người dùng"));

            return Ok(ApiResponse<object>.SuccessResult(result));
        }


        /// <summary>
        /// Lấy danh sách skills của user
        /// </summary>
        [HttpGet("{uid}/skills")]
        public async Task<IActionResult> GetUserSkills(string uid)
        {
            var result = await _profileService.GetUserSkillsAsync(uid);
            return Ok(ApiResponse<object>.SuccessResult(result));
        }

        /// <summary>
        /// Lấy danh sách content và styles của user
        /// </summary>
        [HttpGet("{uid}/contents")]
        public async Task<IActionResult> GetUserContentAndStyles(string uid)
        {
            var result = await _profileService.GetUserContentAndStylesAsync(uid);
            return Ok(ApiResponse<object>.SuccessResult(result));
        }
        [HttpPut("user")]
        public async Task<ActionResult<ProfileResponseDto>> UpdateUserProfile([FromBody] UpdateUserProfileDto updateDto)
        {
            try
            {
                var uid = GetCurrentUserId();
                var updatedProfile = await _profileService.UpdateUserProfileAsync(uid, updateDto);
                return Ok(updatedProfile);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("brand")]
        public async Task<ActionResult<ProfileResponseDto>> UpdateBrandProfile([FromBody] UpdateBrandProfileDto updateDto)
        {
            try
            {
                var uid = GetCurrentUserId();
                var updatedProfile = await _profileService.UpdateBrandProfileAsync(uid, updateDto);
                return Ok(updatedProfile);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("influence")]
        public async Task<ActionResult<ProfileResponseDto>> UpdateInfluenceProfile([FromBody] UpdateInfluenceProfileDto updateDto)
        {
            try
            {
                var uid = GetCurrentUserId();
                var updatedProfile = await _profileService.UpdateInfluenceProfileAsync(uid, updateDto);
                return Ok(updatedProfile);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
