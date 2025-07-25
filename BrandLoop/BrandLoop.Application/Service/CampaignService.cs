﻿using AutoMapper;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.CampainModel;
using BrandLoop.Infratructure.Models.Dashboard;
using BrandLoop.Infratructure.Models.Report;
using BrandLoop.Infratructure.Repository;
using BrandLoop.Shared.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Service
{
    public class CampaignService : ICampaignService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IImageCampainRepository _imageCampaignRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CampaignService> _logger;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaySystem _paySystem;
        private readonly IUserRepository _userRepository;
        private readonly IInfluencerReportRepository _influencerReportRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ICampaignInvitationRepository _campaignInvitationRepository;
        private readonly IEvidenceRepository _evidenceRepository;
        private readonly IKolsJoinCampaignRepository _kolsJoinCampaignRepository;

        public CampaignService(
            ICampaignRepository campaignRepository,
            IMapper mapper,
            ILogger<CampaignService> logger,
            IImageCampainRepository imageCampaignRepository,
            IPaymentRepository paymentRepository,
            IPaySystem paySystem,
            IUserRepository userRepository,
            IInfluencerReportRepository influencerReportRepository,
            IFeedbackRepository feedbackRepository,
            ICampaignInvitationRepository campaignInvitationRepository,
            IEvidenceRepository evidenceRepository,
            IKolsJoinCampaignRepository kolsJoinCampaignRepository)
        {
            _campaignRepository = campaignRepository ?? throw new ArgumentNullException(nameof(campaignRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _imageCampaignRepository = imageCampaignRepository;
            _paymentRepository = paymentRepository;
            _paySystem = paySystem;
            _userRepository = userRepository;
            _influencerReportRepository = influencerReportRepository;
            _feedbackRepository = feedbackRepository;
            _campaignInvitationRepository = campaignInvitationRepository;
            _evidenceRepository = evidenceRepository;
            _kolsJoinCampaignRepository = kolsJoinCampaignRepository;
        }

        public async Task<IEnumerable<CampaignDto>> GetBrandCampaignsAsync(string uid)
        {
            try
            {
                if (uid == null)
                {
                    _logger.LogWarning("Invalid brandId: {BrandId}", uid);
                    throw new ArgumentException("Brand ID phải lớn hơn 0", nameof(uid));
                }

                var campaigns = await _campaignRepository.GetBrandCampaignsAsync(uid);
                var result = _mapper.Map<IEnumerable<CampaignDto>>(campaigns);

                _logger.LogInformation("Retrieved {Count} campaigns for brand {BrandId}", result.Count(), uid);
                return result;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting campaigns for brand {BrandId}", uid);
                throw new InvalidOperationException("Lỗi khi lấy danh sách campaigns", ex);
            }
        }

        public async Task<CampaignDtoVer2> GetCampaignDetailAsync(int campaignId)
        {
            try
            {
                if (campaignId <= 0)
                {
                    _logger.LogWarning("Invalid campaignId: {CampaignId}", campaignId);
                    throw new ArgumentException("Campaign ID phải lớn hơn 0", nameof(campaignId));
                }

                var campaign = await _campaignRepository.GetCampaignDetailAsync(campaignId);
                if (campaign == null)
                {
                    _logger.LogWarning("Campaign not found: {CampaignId}", campaignId);
                    return null;
                }

                var result = _mapper.Map<CampaignDtoVer2>(campaign);
                _logger.LogInformation("Retrieved campaign detail for {CampaignId}", campaignId);
                return result;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting campaign detail {CampaignId}", campaignId);
                throw new InvalidOperationException("Lỗi khi lấy chi tiết campaign", ex);
            }
        }

        public async Task<CampaignDto> CreateCampaignAsync(CreateCampaignDto dto, string uid)
        {
            var branid = await _campaignRepository.getIdBrand(uid);
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                // Validate business rules
                if (string.IsNullOrWhiteSpace(dto.CampaignName))
                {
                    throw new ArgumentException("Tên campaign không được để trống");
                }

                if (dto.Deadline <= DateTime.Now)
                {
                    throw new ArgumentException("Deadline phải lớn hơn thời gian hiện tại");
                }

                if (dto.ImageUrls != null && dto.ImageUrls.Any())
                {
                    var invalidUrls = dto.ImageUrls.Where(url => string.IsNullOrWhiteSpace(url)).ToList();
                    if (invalidUrls.Any())
                    {
                        throw new ArgumentException("Các URL hình ảnh không được để trống");
                    }
                }

                var campaign = _mapper.Map<Campaign>(dto);
                campaign.UploadedDate = DateTime.Now;
                campaign.LastUpdate = DateTime.Now;
                campaign.Status = CampaignStatus.Approved;
                campaign.CreatedBy = uid ?? throw new ArgumentNullException(nameof(uid), "User ID không được để trống");
                campaign.BrandId = branid;

                var result = await _campaignRepository.CreateCampaignAsync(campaign);

                // Upload images nếu có
                if (dto.ImageUrls != null && dto.ImageUrls.Any())
                {
                    try
                    {
                        await _imageCampaignRepository.AddMultipleImagesToCampaignAsync(
                            result.CampaignId,
                            dto.ImageUrls,
                            dto.ImageDescriptions);

                        _logger.LogInformation("Added {Count} images to campaign {CampaignId}",
                            dto.ImageUrls.Count, result.CampaignId);
                    }
                    catch (Exception imageEx)
                    {
                        _logger.LogError(imageEx, "Error uploading images for campaign {CampaignId}. Campaign created but without images.",
                            result.CampaignId);
                        // Không throw exception để campaign vẫn được tạo thành công
                    }
                }
                var mappedResult = _mapper.Map<CampaignDto>(result);

                _logger.LogInformation("Created new campaign {CampaignId} for brand {BrandId}", result.CampaignId, result.BrandId);
                return mappedResult;
            }
            catch (ArgumentNullException ex)
            {
                throw new InvalidOperationException("A required argument was null", ex);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException("Invalid argument provided", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating campaign for brand {BrandId}");
                throw new InvalidOperationException("Lỗi khi tạo campaign", ex);
            }
        }

        public async Task<CampaignDto> UpdateCampaignAsync(int campainId,UpdateCampaignDto dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                //if (dto.CampaignId <= 0)
                //{
                //    throw new ArgumentException("Campaign ID phải lớn hơn 0");
                //}

                var existingCampaign = await _campaignRepository.GetCampaignDetailAsync(campainId);
                if (existingCampaign == null)
                {
                    _logger.LogWarning("Campaign not found for update: {CampaignId}", campainId);
                    return null;
                }

                // Validate business rules
                if (!string.IsNullOrWhiteSpace(dto.CampaignName) && dto.CampaignName != existingCampaign.CampaignName)
                {
                    // Additional validation if needed
                }

                if (dto.Deadline.HasValue && dto.Deadline <= DateTime.Now)
                {
                    throw new ArgumentException("Deadline phải lớn hơn thời gian hiện tại");
                }

                _mapper.Map(dto, existingCampaign);
                existingCampaign.LastUpdate = DateTime.Now;

                var result = await _campaignRepository.UpdateCampaignAsync(existingCampaign);
                var mappedResult = _mapper.Map<CampaignDto>(result);

                _logger.LogInformation("Updated campaign {CampaignId}", campainId);
                return mappedResult;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating campaign {CampaignId}", campainId);
                throw new InvalidOperationException("Lỗi khi cập nhật campaign", ex);
            }
        }

        public async Task<bool> DeleteCampaignAsync(int campaignId)
        {
            try
            {
                if (campaignId <= 0)
                {
                    _logger.LogWarning("Invalid campaignId for delete: {CampaignId}", campaignId);
                    throw new ArgumentException("Campaign ID phải lớn hơn 0", nameof(campaignId));
                }

                var result = await _campaignRepository.DeleteCampaignAsync(campaignId);
                if (result)
                {
                    _logger.LogInformation("Deleted campaign {CampaignId}", campaignId);
                }
                else
                {
                    _logger.LogWarning("Campaign not found for delete: {CampaignId}", campaignId);
                }

                return result;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting campaign {CampaignId}", campaignId);
                throw new InvalidOperationException("Lỗi khi xóa campaign", ex);
            }
        }

        public async Task<CampaignDto> UpdateCampaignStatusAsync(int campaignId, CampaignStatus status)
        {
            try
            {
                if (campaignId <= 0)
                {
                    throw new ArgumentException("Campaign ID phải lớn hơn 0", nameof(campaignId));
                }

                if (!Enum.IsDefined(typeof(CampaignStatus), status))
                {
                    throw new ArgumentException("Trạng thái không hợp lệ", nameof(status));
                }

                var result = await _campaignRepository.UpdateCampaignStatusAsync(campaignId, status);
                if (result == null)
                {
                    _logger.LogWarning("Campaign not found for status update: {CampaignId}", campaignId);
                    return null;
                }

                var mappedResult = _mapper.Map<CampaignDto>(result);
                _logger.LogInformation("Updated campaign {CampaignId} status to {Status}", campaignId, status);
                return mappedResult;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating campaign {CampaignId} status", campaignId);
                throw new InvalidOperationException("Lỗi khi cập nhật trạng thái campaign", ex);
            }
        }

        public async Task<CampaignDto> DuplicateCampaignAsync(int campaignId)
        {
            try
            {
                if (campaignId <= 0)
                {
                    throw new ArgumentException("Campaign ID phải lớn hơn 0", nameof(campaignId));
                }

                var result = await _campaignRepository.DuplicateCampaignAsync(campaignId);
                if (result == null)
                {
                    _logger.LogWarning("Campaign not found for duplication: {CampaignId}", campaignId);
                    return null;
                }

                var mappedResult = _mapper.Map<CampaignDto>(result);
                _logger.LogInformation("Duplicated campaign {OriginalCampaignId} to new campaign {NewCampaignId}",
                    campaignId, result.CampaignId);
                return mappedResult;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while duplicating campaign {CampaignId}", campaignId);
                throw new InvalidOperationException("Lỗi khi nhân bản campaign", ex);
            }
        }

        public async Task<List<CampaignDto>> GetAllCampaignByUid(string uid)
        {
            var campaigns = await _campaignRepository.GetAllCampaignByUid(uid);
            if (campaigns == null || !campaigns.Any())
            {
                _logger.LogWarning("No campaigns found for user {Uid}", uid);
            }
            return _mapper.Map<List<CampaignDto>>(campaigns);
        }
        public async Task<List<CampaignDto>> GetAllCampaignByUid(CampaignStatus? status, string? name,string uid)
        {
            var campaigns = await _campaignRepository.GetAllCampaignByUid(status, name, uid);
            if (campaigns == null || !campaigns.Any())
            {
                _logger.LogWarning("No campaigns found for user {Uid}", uid);
            }
            var result = _mapper.Map<List<CampaignDto>>(campaigns);
            // Get campaign IDs và query count từ repository
            var campaignIds = campaigns.Select(c => c.CampaignId).ToList();
            var kolCounts = await _kolsJoinCampaignRepository.GetKolsCountByCampaignIdsAsync(campaignIds);

            // Gán count vào DTO
            foreach (var dto in result)
            {
                dto.TotalKolsJoined = kolCounts.ContainsKey(dto.CampaignId) ? kolCounts[dto.CampaignId] : 0;
            }

            return result;
        }

        public async Task<List<CampaignDto>> GetAllCampaignsAsync(CampaignFilterModel filter)
        {
            var query = _campaignRepository.GetAll()
                .Include(c => c.CampaignImages)
                .Include(c => c.KolsJoinCampaigns)
                 .ThenInclude(kjc => kjc.User)
                .Include(c => c.Brand)
                .Include(c => c.Creator)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(c => c.CampaignName.Contains(filter.Search) || c.Description.Contains(filter.Search));

            // Filter by status
            if (filter.Status.HasValue)
                query = query.Where(c => c.Status == filter.Status.Value);

            // Filter by date
            if (filter.FromDate.HasValue)
                query = query.Where(c => c.LastUpdate >= filter.FromDate.Value);
            if (filter.ToDate.HasValue)
                query = query.Where(c => c.LastUpdate <= filter.ToDate.Value);

            // Sort
            switch (filter.SortBy)
            {
                case "CampaignName":
                    query = filter.SortDesc ? query.OrderByDescending(c => c.CampaignName) : query.OrderBy(c => c.CampaignName);
                    break;
                case "UploadedDate":
                    query = filter.SortDesc ? query.OrderByDescending(c => c.UploadedDate) : query.OrderBy(c => c.UploadedDate);
                    break;
                default:
                    query = filter.SortDesc ? query.OrderByDescending(c => c.LastUpdate) : query.OrderBy(c => c.LastUpdate);
                    break;
            }

            // Paging
            var skip = (filter.PageNumber - 1) * filter.PageSize;
            var campaigns = await query.Skip(skip).Take(filter.PageSize).ToListAsync();

            var result = _mapper.Map<List<CampaignDto>>(campaigns);
            // Get campaign IDs và query count từ repository
            var campaignIds = campaigns.Select(c => c.CampaignId).ToList();
            var kolCounts = await _kolsJoinCampaignRepository.GetKolsCountByCampaignIdsAsync(campaignIds);

            // Gán count vào DTO
            foreach (var dto in result)
            {
                dto.TotalKolsJoined = kolCounts.ContainsKey(dto.CampaignId) ? kolCounts[dto.CampaignId] : 0;
            }

            return result;
        }

        public async Task<PaymentCampaign> StartCampaign(string creatorId, int campaignId)
        {
            var totalAmount = 0;
            var now = DateTimeHelper.GetVietnamNow();

            // Kiểm tra campaign tồn tại và quyền người dùng
            var checkcampaign = await _campaignRepository.GetCampaignDetailAsync(campaignId);
            if (checkcampaign == null)
                throw new InvalidOperationException($"Chiến dịch với id {campaignId} không tôn tại.");
            if (checkcampaign.CreatedBy != creatorId)
                throw new UnauthorizedAccessException($"Bạn không có quyền để bắt đầu chiến dịch này.");
            if (checkcampaign.Status != CampaignStatus.Approved)
                throw new InvalidOperationException($"Chiến dịch với id {campaignId} không trong trạng thái phù hơp để bắt đầu.");

            // Kiểm tra KOL đã join
            var kolJoinCampaigns = await _campaignRepository.GetKolsJoinCampaigns(campaignId);
            if (kolJoinCampaigns == null || !kolJoinCampaigns.Any())
                throw new Exception($"Chưa có Influencer nào tham gia chiến dịch này.");
            var existPayment = await _paymentRepository.GetPaymentByCamaignId(campaignId);

            // Kiểm tra payment đã tồn tại và trạng thái
            if (existPayment != null && existPayment.Status != PaymentStatus.Canceled)
            {
                return _mapper.Map<PaymentCampaign>(checkcampaign);
            }

            foreach (var kol in kolJoinCampaigns)
            {
                totalAmount += kol.User.InfluenceProfile.InfluencerType.PlatformFee;
            }

            var campaign = await _campaignRepository.StartCampaign(campaignId);

            var orderCode = await GenerateOrderCode();

            var payment = new Payment
            {
                PaymentId = orderCode,
                CreatedAt = DateTimeHelper.GetVietnamNow(),
                Amount = totalAmount,
                Status = PaymentStatus.pending,
                Type = PaymentType.campaign,
                CampaignId = campaignId,
                PaymentMethod = "Bank Transfer",
                TransactionCode = "Not bank yet"
            };
            await _paymentRepository.CreatePaymentAsync(payment);
            var paymentLink = await CreatePaymentLink(orderCode);

            // Update invitation status to expired
            var invitationList = await _campaignInvitationRepository.GetAllWaitingInvitationOfCampaign(campaignId);
            foreach (var invitation in invitationList)
                await _campaignInvitationRepository.UpdateInvitationStatus(invitation.InvitationId, CampaignInvitationStatus.expired);

            return _mapper.Map<PaymentCampaign>(campaign);
        }

        public async Task<CreatePaymentResult> CreatePaymentLink(long orderCode)
        {
            List<ItemData> items = new List<ItemData>();
            ItemData item;

            var payment = await _paymentRepository.GetPaymentByIdAsync(orderCode);
            if (payment == null)
                throw new Exception($"Không thể tìm thấy giao dịch với id: {orderCode}.");

            if (payment.Status != PaymentStatus.pending)
                throw new Exception("Giao dịch đang không trong trạng thái pending.");
            if (payment.Type != PaymentType.campaign)
                throw new Exception("Giaso dịch này không phải loại giao dịch dùng để thanh toán cho chiến dịch.");

            var campaign = await _campaignRepository.GetCampaignDetailAsync((int)payment.CampaignId);
            if (campaign.Creator == null)
                throw new Exception($"Không thể tìm thấy chiến dịch của giao dịch với id giao dịch: {(int)payment.CampaignId}.");

            var kolJoinCampaigns = await _campaignRepository.GetKolsJoinCampaigns(campaign.CampaignId);
            if (kolJoinCampaigns == null || !kolJoinCampaigns.Any())
                throw new Exception($"Chưa có Influencer nào tham gia chiến dịch này.");

            foreach (var kol in kolJoinCampaigns)
            {
                if (kol.User == null || kol.User.InfluenceProfile == null)
                    throw new Exception($"Influencer với UID {kol.UID} không có profile hợp lệ.");
                item = new ItemData(
                    $"{kol.User.InfluenceProfile.InfluencerType.Name} Fee for {kol.User.FullName}",
                    kol.User.InfluenceProfile.InfluencerType.PlatformFee,
                    1
                );
                items.Add(item);
            }
            var creator = await _userRepository.GetBasicAccountProfileAsync(campaign.CreatedBy);
            if (creator == null)
                throw new Exception($"Không thể tìm thấy người dùng với UID: {campaign.CreatedBy}.");

            var paymentInfo = await _paySystem.CreatePaymentAsync(
                creator,
                "Payment start campaign",
                orderCode,
                items
            );
            await _paymentRepository.UpdatePaymentTransactionCode(orderCode, paymentInfo.paymentLinkId);
            await _paymentRepository.UpdatePaymentLink(orderCode, paymentInfo.checkoutUrl);
            return paymentInfo;
        }

        public async Task ConfirmPayment(long orderCode)
        {
            var payment = await _paymentRepository.GetPaymentByOrderCodeAsync(orderCode);
            if (payment == null)
                throw new Exception($"Không thể tìm thấy giao dịch với id: {orderCode}.");
            if (payment.Status != PaymentStatus.pending)
                throw new Exception("Giao dịch đang không trong trạng thái peding.");

            var paymentLinkInfo = await _paySystem.getPaymentLinkInformation(orderCode);
            if (paymentLinkInfo.status != "PAID")
                throw new Exception($"Giao dịch chưa được hoàn thành thanh toán, status ({paymentLinkInfo.status})");

            await _paymentRepository.UpdatePaymentStatus(orderCode, PaymentStatus.Succeeded);
            await _campaignRepository.ConfirmPaymentToStartCampaign((int)payment.CampaignId);
        }

        public async Task<CampaignDto> EndCampaign(string creatorId, BrandReport brandReport)
        {
            var checkCampaign = await _campaignRepository.GetCampaignDetailAsync(brandReport.CampaignId);
            if (checkCampaign == null)
                throw new InvalidOperationException($"Không thể tìm thấy chiến dịch có ID: {brandReport.CampaignId}.");

            if (checkCampaign.CreatedBy != creatorId)
                throw new UnauthorizedAccessException($"Bạn không có quyền kết thúc chiến dich này.");
            var kolJoinCampaigns = await _campaignRepository.GetKolsJoinCampaigns(brandReport.CampaignId);
            foreach (var kol in kolJoinCampaigns)
            {
                if (kol.Status != KolJoinCampaignStatus.Feedbacked)
                    throw new InvalidOperationException($"Influencer {kol.User.FullName} cần phải báo cáo trước khi kết thúc chiến dịch.");
                var feedback = await _feedbackRepository.GetFeedbackForKolOfCampaignAsync(brandReport.CampaignId, kol.UID);
                if (feedback == null)
                    throw new InvalidOperationException($"Bạn cần phải cho nhận xét toàn bộ influencer đang tham gia trước khi kết thúc chiến dịch.");
            }

            // Update campaign report
            var campaignReport = await _campaignRepository.GetCampaignReportByCampaignIdAsync(brandReport.CampaignId);

            campaignReport.TotalRevenue = brandReport.TotalRevenue;
            campaignReport.TotalSpend += brandReport.TotalSpend;

            campaignReport.CostPerEngagement = campaignReport.TotalEngagement > 0 ? campaignReport.TotalSpend / campaignReport.TotalEngagement : 0;
            campaignReport.ROAS = campaignReport.TotalSpend > 0 ? campaignReport.TotalRevenue / campaignReport.TotalSpend : 0;
            await _influencerReportRepository.UpdateCampaignReport(campaignReport);

            return _mapper.Map<CampaignDto>(await _campaignRepository.EndCampaign(brandReport.CampaignId));
        }

        public async Task<CampaignDto> CancelCampaign(string creatorId, int campaignId)
        {
            var checkCampaign = await _campaignRepository.GetCampaignDetailAsync(campaignId);
            if (checkCampaign == null)
                throw new InvalidOperationException($"Không thể tìm thấy chiến dịch có id: {campaignId}.");

            if (checkCampaign.CreatedBy != creatorId)
                throw new UnauthorizedAccessException($"Bạn không có quyền để hủy chiến dịch này.");

            return _mapper.Map<CampaignDto>(await _campaignRepository.CancelCampaign(campaignId));
        }

        public async Task Cancelayment(long orderCode)
        {
            var payment = await _paymentRepository.GetPaymentByOrderCodeAsync(orderCode);
            if (payment == null)
                throw new Exception($"Không thể tìm thấy giao dịch có id: {orderCode}.");
            if (payment.Status != PaymentStatus.pending)
                throw new Exception("Giao dịch không trong trạng thái pending.");
            await _paymentRepository.UpdatePaymentStatus(orderCode, PaymentStatus.Canceled);
        }

        public async Task<long> GenerateOrderCode()
        {
            // Time-based + random số nhỏ, đảm bảo trong giới hạn Int64
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // 13 chữ số
            var random = new Random().Next(100, 999); // 3 chữ số
            var combined = $"{timestamp}{random}"; // Tổng: 16 chữ số

            var result = long.Parse(combined);
            var checkExistingPayment = await _paymentRepository.GetPaymentByOrderCodeAsync(result);
            if (checkExistingPayment != null)
                // Nếu đã tồn tại, gọi lại hàm để tạo mã mới
                return await GenerateOrderCode();

            return result;
        }

        public async Task GiveFeedback(CreateFeedback createFeedback, string userId)
        {
            var kolJoinCampaign = (await _campaignRepository.GetKolsJoinCampaigns(createFeedback.CampaignId))
                .FirstOrDefault(k => k.UID == createFeedback.ToUserId);
            if (kolJoinCampaign == null)
                throw new Exception($"Influencer với UID {createFeedback.ToUserId} không tham gia chiến dịch với ID {createFeedback.CampaignId}.");

            if (kolJoinCampaign.Status != KolJoinCampaignStatus.Completed)
                throw new Exception($"Influencer {kolJoinCampaign.User.FullName} chưa hoàn thành báo cáo, không thể đánh giá.");

            if (kolJoinCampaign.Status == KolJoinCampaignStatus.Feedbacked)
                throw new Exception($"Influencer {kolJoinCampaign.User.FullName} đã được đánh giá rồi.");

            var feedback = new Feedback
            {
                CampaignId = createFeedback.CampaignId,
                ToUserId = createFeedback.ToUserId,
                Rating = createFeedback.Rating,
                Description = createFeedback.Description
            };
            feedback.FromUserId = userId;
            feedback.CreatedAt = DateTimeHelper.GetVietnamNow();
            feedback.FeedbackFrom = Domain.Enums.FeedbackType.Brand;
            await _feedbackRepository.AddFeedbackAsync(feedback);

            // Update influencer's money received from campaign
            await _kolsJoinCampaignRepository.UpdateKolMoney(createFeedback.CampaignId, createFeedback.ToUserId, createFeedback.InfluencerMoney);
            // Update kol join campaign status to Feedbacked
            await _kolsJoinCampaignRepository.UpdateKolJoinCampaignStatus(createFeedback.CampaignId, createFeedback.ToUserId, KolJoinCampaignStatus.Feedbacked);

            // Create evidence for the feedback
            var evidence = new Evidence
            {
                Description = createFeedback.EvidenceDescription,
                Link = createFeedback.EvidenceLink,
                EvidenceOf = EvidenceType.Brand,
                KolsJoinCampaignId = kolJoinCampaign.KolsJoinCampaignId
            };
            await _evidenceRepository.AddEvidenceAsync(evidence);

            // Update campaign spend base on feedback
            var campaignReport = await _campaignRepository.GetCampaignReportByCampaignIdAsync(createFeedback.CampaignId);
            campaignReport.TotalSpend += createFeedback.InfluencerMoney;
            campaignReport.CostPerEngagement = campaignReport.TotalEngagement > 0 ? campaignReport.TotalSpend / campaignReport.TotalEngagement : 0;
            campaignReport.ROAS = campaignReport.TotalSpend > 0 ? campaignReport.TotalRevenue / campaignReport.TotalSpend : 0;
            await _influencerReportRepository.UpdateCampaignReport(campaignReport);
        }

        public async Task<CampaignTracking> GetCampaignDetail(int campaignId)
        {
            try
            {
                var campaign = await _campaignRepository.GetCampaignDetailAsync(campaignId);
                if (campaign == null)
                {
                    _logger.LogWarning("Campaign not found: {CampaignId}", campaignId);
                    return null;
                }
                var result = _mapper.Map<CampaignTracking>(campaign);
                _logger.LogInformation("Retrieved campaign detail for {CampaignId}", campaignId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting campaign detail {CampaignId}", campaignId);
                throw new InvalidOperationException("Lỗi khi lấy chi tiết campaign", ex);
            }
        }

        public async Task<CampaignCard> GetCampaignCard(string uid)
        {
            var campaignCard = new CampaignCard();
            var now = DateTimeHelper.GetVietnamNow();
            var campaigns = await _campaignRepository.GetBrandCampaignsAsync(uid);
            if (campaigns == null || !campaigns.Any())
                return campaignCard; // Return empty object if no campaigns found

            campaignCard.totalCampaigns = campaigns.Count();

            foreach (var campaign in campaigns)
            {
                switch (campaign.Status)
                {
                    case CampaignStatus.Approved:
                        campaignCard.totalUnstartedCampaigns++;
                        break;

                    case CampaignStatus.InProgress:
                        campaignCard.totalInprogressCampaigns++;
                        break;
                    case CampaignStatus.Overdue:
                        campaignCard.totalOverdueCampaigns++;
                        break;

                    case CampaignStatus.Completed:
                        campaignCard.totalCompletedCampaigns++;
                        break;
                }
            }
            return campaignCard;
        }

        public async Task<List<CampaignChart>> GetRevenueChard(string uid, int year)
        {
            var result = new List<CampaignChart>();

            // Khởi tạo 12 tháng
            for (int month = 1; month <= 12; month++)
            {
                result.Add(new CampaignChart
                {
                    month = month,
                    moneyIn = 0,
                    moneyOut = 0
                });
            }

            // Lấy tất cả campaigns theo năm
            var koljoinCampaigns = await _kolsJoinCampaignRepository.GetKolsJoinCampaignsOfBrand(uid, year);
            var campaigns = await _campaignRepository.GetBrandCampaignsByYear(uid, year);
            var payments = await _paymentRepository.GetPaymentOfBrandByYear(uid, year);
            

            // Tong hợp tiền ra và tiền vào theo tháng
            foreach (var item in koljoinCampaigns)
            {
                var month = item.AppliedAt.Month;
                result[month - 1].moneyOut += item.InfluencerEarning;
            }
            foreach (var item in payments)
            {
                var month = item.CreatedAt.Month;
                result[month - 1].moneyOut += item.Amount;
            }

            foreach (var item in campaigns)
            {
                var month = item.UploadedDate.Month;
                result[month - 1].moneyIn += (int)(item.CampaignReport?.TotalRevenue ?? 0);
            }

            return result;
        }

        public async Task<List<CampaignSelectOption>> GetCampaignsOf(string uid, CampaignStatus status)
        {
            var campaigns = await _campaignRepository.GetAllCampaignsOfBrandWithStatus(uid, status);
            return _mapper.Map<List<CampaignSelectOption>>(campaigns);
        }

        public async Task<CampaignDashboardDetail> GetCampaignDetailForDashboard(string uid, int campaignId)
        {
            var campaign = await _campaignRepository.GetCampaignDetailAsync(campaignId);
            if (campaign == null)
                return null; // Return null if campaign not found
            if (campaign.CreatedBy != uid)
                throw new AuthenticationException($"Bạn không có quyền xem thông tin của chiến dịch này.");

            var result = new CampaignDashboardDetail
            {
                StartTime = campaign.StartTime,
                Deadline = campaign.Deadline,
                Budget = campaign.Budget,
                Status = campaign.Status.ToString(),

                totalKolsJoin = campaign.KolsJoinCampaigns.Count(),
                totalInvitations = campaign.CampaignInvitations.Count(),
                totalAcceptedInvitations = campaign.CampaignInvitations.Where(ci => ci.Status == CampaignInvitationStatus.accepted).Count(),
                totalRejectedInvitations = campaign.CampaignInvitations.Where(ci => ci.Status == CampaignInvitationStatus.rejected).Count(),
                othersInvitations = campaign.CampaignInvitations.Where(ci => (ci.Status != CampaignInvitationStatus.accepted) && (ci.Status != CampaignInvitationStatus.rejected)).Count(),
                TotalSpend = campaign.CampaignReport?.TotalSpend ?? 0,
                TotalRevenue = campaign.CampaignReport?.TotalRevenue ?? 0,
                TotalReach = campaign.CampaignReport?.TotalReach ?? 0,
                TotalImpressions = campaign.CampaignReport?.TotalImpressions ?? 0,
                TotalEngagement = campaign.CampaignReport?.TotalEngagement ?? 0,
                TotalClicks = campaign.CampaignReport?.TotalClicks ?? 0,

                AvgEngagementRate = campaign.CampaignReport?.AvgEngagementRate ?? 0,
                CostPerEngagement = campaign.CampaignReport?.CostPerEngagement ?? 0,
                ROAS = campaign.CampaignReport?.ROAS ?? 0,
                feedbacks = _mapper.Map<List<ShowFeedback>>(await _feedbackRepository.GetFeedbacksByCampaignIdAsync(campaignId))
            };

            return result;
        }
    }
}