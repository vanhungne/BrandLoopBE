﻿using BrandLoop.API.Models;
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

        /// <summary>
        /// Brand mời KOL tham gia chiến dịch
        /// </summary>
        /// <returns>Trả vè thông tin của invitation đó</returns>
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
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// KOL xin tham gia vào chiến dịch
        /// </summary>
        /// <returns>Trả vè thông tin của invitation đó</returns>
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
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Láy tất cả các invitation của một chiến dịch
        /// </summary>
        /// <returns>Trả vè list các invitation của chiến dịch đó</returns>
        [HttpGet("campaign/{campaignId}")]
        [Authorize(Roles = "Brand")]
        public async Task<IActionResult> GetAllInvitationsOfCampaignAsync(int campaignId, CampaignInvitationStatus? status, [FromQuery] PaginationFilter filter)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var invitations = await _campaignInvitationService.GetAllInvitationsOfCampaignAsync(campaignId, uid, status);
                if (invitations == null || !invitations.Any())
                    return Ok(invitations);

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
                return Forbid();
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

        /// <summary>
        /// Láy tất cả các invitation ma brand đã gửi cho KOL
        /// </summary>
        /// <returns>Trả vè list các invitation của chiến dịch đó</returns>
        [HttpGet("get-all-invitation-of-brand")]
        [Authorize(Roles = "Brand")]
        public async Task<IActionResult> GetAllInvitationsOfBrandAsync(CampaignInvitationStatus? status, [FromQuery] PaginationFilter filter)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var invitations = await _campaignInvitationService.GetAllInvitationsOfBrandAsync(uid, status);
                if (invitations == null || !invitations.Invitations.Any())
                    return Ok();

                var totalRecords = invitations.Invitations.Count;
                var pagedData = invitations.Invitations
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

                var response = new PaginationResponseV2<InvitationDTO>(
                pagedData,
                filter.PageNumber,
                filter.PageSize,
                totalRecords,
                invitations.totalInvitation,
                invitations.totalWaitingInvitation,
                invitations.totalAcceptedInvitation,
                invitations.totalRejectedInvitation,
                invitations.totalExpiredInvitation
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Láy tất cả các invitation ma KOL đã gửi cho brand
        /// </summary>
        /// <returns>Trả vè list các invitation của chiến dịch đó</returns>
        [HttpGet("get-all-request-to-brand")]
        [Authorize(Roles = "Brand")]
        public async Task<IActionResult> GetAllRequestToBrandAsync(CampaignInvitationStatus? status, [FromQuery] PaginationFilter filter)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var invitations = await _campaignInvitationService.GetAllRequestOfBrandAsync(uid, status);
                if (invitations == null || !invitations.Invitations.Any())
                    return Ok();

                var totalRecords = invitations.Invitations.Count;
                var pagedData = invitations.Invitations
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

                var response = new PaginationResponseV2<InvitationDTO>(
                pagedData,
                filter.PageNumber,
                filter.PageSize,
                totalRecords,
                invitations.totalInvitation,
                invitations.totalWaitingInvitation,
                invitations.totalAcceptedInvitation,
                invitations.totalRejectedInvitation,
                invitations.totalExpiredInvitation
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Láy tất cả các invitaion ma brand moi KOL tham gia
        /// </summary>
        /// <returns>Trả vè list các invitation</returns>
        [HttpGet("get-all-kol-invitation")]
        [Authorize(Roles = "Influencer")]
        public async Task<IActionResult> GetInvitationsByKOLIdAsync([FromQuery]CampaignInvitationStatus? status, [FromQuery] PaginationFilter filter)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var invitations = await _campaignInvitationService.GetInvitationsByKOLIdAsync(uid, status);
                if (invitations == null || !invitations.Invitations.Any())
                    return Ok();

                var totalRecords = invitations.Invitations.Count;
                var pagedData = invitations.Invitations
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

                var response = new PaginationResponseV2<InvitationOfBrand>(
                pagedData,
                filter.PageNumber,
                filter.PageSize,
                totalRecords,
                invitations.totalInvitation,
                invitations.totalWaitingInvitation,
                invitations.totalAcceptedInvitation,
                invitations.totalRejectedInvitation,
                invitations.totalExpiredInvitation
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Láy tất cả các invitation của KOL request cho brand (nếu đang là KOL)
        /// </summary>
        /// <returns>Trả vè list các invitation</returns>
        [HttpGet("get-all-brand-invite-of-kol")]
        [Authorize(Roles = "Influencer")]
        public async Task<IActionResult> GetAllBrandInviteKOL([FromQuery] CampaignInvitationStatus? status, [FromQuery] PaginationFilter filter)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var invitations = await _campaignInvitationService.GetRequestedByKOLIdAsync(uid, status);
                if (invitations == null || !invitations.Invitations.Any())
                    return Ok();

                var totalRecords = invitations.Invitations.Count;
                var pagedData = invitations.Invitations
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

                var response = new PaginationResponseV2<InvitationOfBrand>(
                pagedData,
                filter.PageNumber,
                filter.PageSize,
                totalRecords,
                invitations.totalInvitation,
                invitations.totalWaitingInvitation,
                invitations.totalAcceptedInvitation,
                invitations.totalRejectedInvitation,
                invitations.totalExpiredInvitation
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thông tin của một invitation theo ID
        /// </summary>
        /// <returns>Trả vè thông tin của invitation đó</returns>
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
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        /// <summary>
        /// Accapt một invitation
        /// </summary>
        /// <returns> Tin nhắn</returns>
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
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        /// <summary>
        /// Trả lời một invitation (thương lượng giá cả, thời gian, ...)
        /// </summary>
        /// <returns>Trả vè thông báo</returns>
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
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        /// <summary>
        /// Từ chối một invitation
        /// </summary>
        /// <returns>Trả vè thông báo</returns>
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
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// API bị thừa, k biết có cần không, nhưng để lại cho chắc
        /// </summary>
        /// <returns>Trả vè True/False</returns>
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

        /// <summary>
        /// Kiểm tra xem có thể approve được invitation đó hay không
        /// </summary>
        /// <returns>Trả vè True/False</returns>
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

        /// <summary>
        /// Kiem tra xem có thể thương lượng được invitation đó hay không
        /// </summary>
        /// <returns>Trả vè True/False</returns>
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

        /// <summary>
        /// Kiểm tra xem có thể reject được invitation đó hay không
        /// </summary>
        /// <returns>Trả vè True/False</returns>
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
