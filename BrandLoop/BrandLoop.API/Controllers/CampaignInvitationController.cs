using BrandLoop.API.Models;
using BrandLoop.API.Response;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.CampainModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace BrandLoop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignInvitationController : ControllerBase
    {
        private readonly ICampaignInvitationService _campaignInvitationService;
        public CampaignInvitationController(ICampaignInvitationService campaignInvitationService)
        {
            _campaignInvitationService = campaignInvitationService;
        }

        [HttpPost("brand-invite")]
        [Authorize(Roles = "Brand")]
        public async Task<IActionResult> BrandInviteKolsAsync([FromBody] JoinCampaign joinCampaign)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var invitation = await _campaignInvitationService.CreateInvitationAsync(joinCampaign, uid, JoinCampaignType.BrandInvited);
                return Ok(invitation);
            }
            catch (AuthenticationException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("kols-applie")]
        [Authorize(Roles = "Influencer")]
        public async Task<IActionResult> KolsApplyCampaignAsync([FromBody] JoinCampaign joinCampaign)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var invitation = await _campaignInvitationService.CreateInvitationAsync(joinCampaign, uid, JoinCampaignType.KolApplied);
                return Ok(invitation);
            }
            catch (AuthenticationException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("campaign/{campaignId}")]
        [Authorize(Roles = "Brand")]
        public async Task<IActionResult> GetAllInvitationsOfCampaignAsync(int campaignId, [FromQuery] PaginationFilter filter)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var invitations = await _campaignInvitationService.GetAllInvitationsOfCampaignAsync(campaignId, uid);
                if (invitations == null || !invitations.Any())
                    return NotFound(ApiResponse<string>.ErrorResult("Can not found any invitation"));

                var totalRecords = invitations.Count;
                var pagedData = invitations
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

                var response = new PaginationResponse<InvitationDTO>(
                pagedData,
                filter.PageNumber,
                filter.PageSize,
                totalRecords
                );
                return Ok(response);
            }
            catch (AuthenticationException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("get-all-kol-invitation")]
        [Authorize(Roles = "Influencer")]
        public async Task<IActionResult> GetInvitationsByKOLIdAsync([FromQuery] PaginationFilter filter)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var invitations = await _campaignInvitationService.GetInvitationsByKOLIdAsync(uid);
                if (invitations == null || !invitations.Any())
                    return NotFound(ApiResponse<string>.ErrorResult("Can not found any invitation"));

                var totalRecords = invitations.Count;
                var pagedData = invitations
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

                var response = new PaginationResponse<InvitationDTO>(
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
        [HttpGet("invitation/{invitationId}")]
        [Authorize]
        public async Task<IActionResult> GetInvitationByIdAsync(int invitationId)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var invitation = await _campaignInvitationService.GetInvitationByIdAsync(invitationId, uid);
                return Ok(invitation);
            }
            catch (AuthenticationException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("approve/{invitationId}")]
        [Authorize]
        public async Task<IActionResult> ApproveInvitationAsync(int invitationId)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _campaignInvitationService.ApproveInvitation(invitationId, uid);
                return Ok(new { message = "Invitation approved successfully." });
            }
            catch (AuthenticationException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("negotiate")]
        [Authorize]
        public async Task<IActionResult> NegotiateInvitationAsync([FromBody] InvitationResponse response)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _campaignInvitationService.Negotiate(response, uid);
                return Ok(new { message = "Negotiation sent successfully." });
            }
            catch (AuthenticationException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("reject/{invitationId}")]
        [Authorize]
        public async Task<IActionResult> RejectInvitationAsync(int invitationId)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _campaignInvitationService.RejectInvitation(invitationId, uid);
                return Ok(new { message = "Invitation rejected successfully." });
            }
            catch (AuthenticationException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("is-waiting-for-approve/{campaignId}")]
        public async Task<IActionResult> IsWaitingForApproveAsync(int campaignId)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isWaiting = await _campaignInvitationService.IsWaitingForApprove(campaignId, uid);
                return Ok(isWaiting);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("check-can-approve/{invitationId}")]
        [Authorize]
        public async Task<IActionResult> CheckCanApproveAsync(int invitationId)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var canApprove = await _campaignInvitationService.CheckCanApprove(invitationId, uid);
                return Ok(canApprove);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("check-can-negotiate/{invitationId}")]
        [Authorize]
        public async Task<IActionResult> CheckCanNegotiateAsync(int invitationId)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var canNegotiate = await _campaignInvitationService.CheckCanNegotiate(invitationId, uid);
                return Ok(canNegotiate);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("check-can-reject/{invitationId}")]
        [Authorize]
        public async Task<IActionResult> CheckCanRejectAsync(int invitationId)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var canReject = await _campaignInvitationService.CheckCanReject(invitationId, uid);
                return Ok(canReject);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
