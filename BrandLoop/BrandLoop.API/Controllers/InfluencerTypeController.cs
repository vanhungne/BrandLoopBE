using BrandLoop.API.Models;
using BrandLoop.API.Response;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Models.CampainModel;
using BrandLoop.Infratructure.Models.UserModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrandLoop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class InfluencerTypeController : ControllerBase
    {
        private readonly IInfluencerTypeService _influencerTypeService;
        public InfluencerTypeController(IInfluencerTypeService influencerTypeService)
        {
            _influencerTypeService = influencerTypeService;
        }

        /// <summary>
        /// Lấy hết tất cả các loại Influencer với phân trang
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllInfluencerTypes([FromQuery] PaginationFilter filter)
        {
            try
            {
                var influencerTypes = await _influencerTypeService.GetAllInfluencerTypesAsync();
                var totalRecords = influencerTypes.Count;
                if (influencerTypes == null || !influencerTypes.Any())
                    return NotFound(ApiResponse<string>.ErrorResult("Can not found any Influencer type"));

                var pagedData = influencerTypes
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

                var response = new PaginationResponse<InfluTypeModel>(
                pagedData,
                filter.PageNumber,
                filter.PageSize,
                totalRecords
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Lấy thông tin loại Influencer theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInfluencerTypeById(int id)
        {
            try 
            { 
                var influencerType = await _influencerTypeService.GetInfluencerTypeByIdAsync(id);
                if (influencerType == null)
                    return NotFound(ApiResponse<string>.ErrorResult($"Influencer type with ID {id} not found."));
                return Ok(influencerType);
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(ApiResponse<string>.ErrorResult(knfEx.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Thêm một loại Influencer mới
        /// </summary>
        /// <param name="influencerType"></param>
        /// <returns></returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddInfluencerType([FromBody] InfluTypeModel influencerType)
        {
            var addedInfluencerType = await _influencerTypeService.AddInfluencerTypeAsync(influencerType);
            return CreatedAtAction(nameof(GetInfluencerTypeById), new { id = addedInfluencerType.Id }, addedInfluencerType);
        }

        /// <summary>
        /// Câp nhật thông tin loại Influencer
        /// </summary>
        /// <param name="influencerType"></param>
        /// <returns></returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateInfluencerType([FromBody] InfluTypeModel influencerType)
        {
            try
            {
                var updatedInfluencerType = await _influencerTypeService.UpdateInfluencerTypeAsync(influencerType);
                return Ok(updatedInfluencerType);
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(ApiResponse<string>.ErrorResult(knfEx.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
