using AutoMapper;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.CampainModel;
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

        public CampaignService(
            ICampaignRepository campaignRepository,
            IMapper mapper,
            ILogger<CampaignService> logger,
            IImageCampainRepository imageCampaignRepository,
            IPaymentRepository paymentRepository,
            IPaySystem paySystem,
            IUserRepository userRepository,
            IInfluencerReportRepository influencerReportRepository,
            IFeedbackRepository feedbackRepository)
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

        public async Task<CampaignDto> GetCampaignDetailAsync(int campaignId)
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

                var result = _mapper.Map<CampaignDto>(campaign);
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

        public async Task<CampaignDto> UpdateCampaignAsync(UpdateCampaignDto dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                if (dto.CampaignId <= 0)
                {
                    throw new ArgumentException("Campaign ID phải lớn hơn 0");
                }

                var existingCampaign = await _campaignRepository.GetCampaignDetailAsync(dto.CampaignId);
                if (existingCampaign == null)
                {
                    _logger.LogWarning("Campaign not found for update: {CampaignId}", dto.CampaignId);
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

                _logger.LogInformation("Updated campaign {CampaignId}", dto.CampaignId);
                return mappedResult;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating campaign {CampaignId}", dto?.CampaignId);
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
        public async Task<List<CampaignDto>> GetAllCampaignsAsync(CampaignFilterModel filter)
        {
            var query = _campaignRepository.GetAll()
                .Include(c => c.CampaignImages)
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

            return _mapper.Map<List<CampaignDto>>(campaigns);
        }

        public async Task<PaymentCampaign> StartCampaign(string creatorId, int campaignId)
        {
            var totalAmount = 0;
            var now = DateTimeHelper.GetVietnamNow();

            // Kiểm tra campaign tồn tại và quyền người dùng
            var checkcampaign = await _campaignRepository.GetCampaignDetailAsync(campaignId);
            if (checkcampaign == null)
                throw new InvalidOperationException($"Campaign with ID {campaignId} not found or cannot be started.");
            if (checkcampaign.CreatedBy != creatorId)
                throw new UnauthorizedAccessException($"User {creatorId} is not authorized to start this campaign.");

            // Kiểm tra KOL đã join
            var kolJoinCampaigns = await _campaignRepository.GetKolsJoinCampaigns(campaignId);
            if (kolJoinCampaigns == null || !kolJoinCampaigns.Any())
                throw new Exception($"No KOLs joined the campaign with ID {campaignId}.");
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
            return _mapper.Map<PaymentCampaign>(campaign);
        }

        public async Task<CreatePaymentResult> CreatePaymentLink(long orderCode)
        {
            List<ItemData> items = new List<ItemData>();
            ItemData item;

            var payment = await _paymentRepository.GetPaymentByIdAsync(orderCode);
            if (payment == null)
                throw new Exception($"Payment with ID {orderCode} not found.");

            if (payment.Status != PaymentStatus.pending)
                throw new Exception("Payment is not in pending status.");
            if (payment.Type != PaymentType.campaign)
                throw new Exception("Payment type is not campaign.");

            var campaign = await _campaignRepository.GetCampaignDetailAsync((int)payment.CampaignId);
            if (campaign.Creator == null)
                throw new Exception($"Campaign with ID {(int)payment.CampaignId} not found.");

            var kolJoinCampaigns = await _campaignRepository.GetKolsJoinCampaigns(campaign.CampaignId);
            if (kolJoinCampaigns == null || !kolJoinCampaigns.Any())
                throw new Exception($"No KOLs joined the campaign with ID {campaign.CampaignId}.");

            foreach (var kol in kolJoinCampaigns)
            {
                if (kol.User == null || kol.User.InfluenceProfile == null)
                    throw new Exception($"KOL with ID {kol.UID} does not have a valid profile.");
                item = new ItemData(
                    $"{kol.User.InfluenceProfile.InfluencerType.Name} Fee for {kol.User.FullName}",
                    kol.User.InfluenceProfile.InfluencerType.PlatformFee,
                    1
                );
                items.Add(item);
            }
            var creator = await _userRepository.GetBasicAccountProfileAsync(campaign.CreatedBy);
            if (creator == null)
                throw new Exception($"User with ID {campaign.CreatedBy} not found.");

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
                throw new Exception($"Payment with order code {orderCode} not found.");
            if (payment.Status != PaymentStatus.pending)
                throw new Exception("Payment is not in pending status.");

            var paymentLinkInfo = await _paySystem.getPaymentLinkInformation(orderCode);
            if (paymentLinkInfo.status != "PAID")
                throw new Exception($"Payment is not paid yet: {paymentLinkInfo.status}");

            await _paymentRepository.UpdatePaymentStatus(orderCode, PaymentStatus.Succeeded);
            await _campaignRepository.ConfirmPaymentToStartCampaign((int)payment.CampaignId);
        }

        public async Task<CampaignDto> EndCampaign(string creatorId, BrandReport brandReport)
        {
            var checkCampaign = await _campaignRepository.GetCampaignDetailAsync(brandReport.CampaignId);
            if (checkCampaign == null)
                throw new InvalidOperationException($"Campaign with ID {brandReport.CampaignId} not found.");

            if (checkCampaign.CreatedBy != creatorId)
                throw new UnauthorizedAccessException($"User {creatorId} is not authorized to end this campaign.");
            var kolJoinCampaigns = await _campaignRepository.GetKolsJoinCampaigns(brandReport.CampaignId);
            foreach (var kol in kolJoinCampaigns)
            {
                if (kol.Status != KolJoinCampaignStatus.Completed)
                    throw new InvalidOperationException($"KOL {kol.User.FullName} is not completed in this campaign yet.");
                var feedback = _feedbackRepository.GetFeedbackForKolOfCampaignAsync(brandReport.CampaignId, kol.UID);
                if (feedback == null)
                    throw new InvalidOperationException($"You need to give feedback to all influencer before end this campaign.");
            }

            // Create campaign report
            var campaignReport = new CampaignReport
            {
                CampaignId = brandReport.CampaignId,
                TotalRevenue = brandReport.TotalRevenue,
                TotalSpend = brandReport.TotalSpend,
                CreatedAt = DateTimeHelper.GetVietnamNow()
            };
            var influencerReport = await _influencerReportRepository.GetReportsByCampaignId(brandReport.CampaignId);
            foreach (var report in influencerReport)
            {
                campaignReport.TotalReach += report.TotalReach;
                campaignReport.TotalImpressions += report.TotalImpressions;
                campaignReport.TotalEngagement += report.TotalEngagement;
                campaignReport.TotalClicks += report.TotalClicks;
                campaignReport.AvgEngagementRate += report.AvgEngagementRate;
            }
            campaignReport.AvgEngagementRate = campaignReport.AvgEngagementRate / influencerReport.Count;
            campaignReport.CostPerEngagement = campaignReport.TotalSpend / campaignReport.TotalEngagement;
            campaignReport.ROAS = campaignReport.TotalRevenue / campaignReport.TotalSpend;
            await _influencerReportRepository.AddCampaignReport(campaignReport);

            return _mapper.Map<CampaignDto>(await _campaignRepository.EndCampaign(brandReport.CampaignId));
        }

        public async Task<CampaignDto> CancelCampaign(string creatorId, int campaignId)
        {
            var checkCampaign = await _campaignRepository.GetCampaignDetailAsync(campaignId);
            if (checkCampaign == null)
                throw new InvalidOperationException($"Campaign with ID {campaignId} not found.");

            if (checkCampaign.CreatedBy != creatorId)
                throw new UnauthorizedAccessException($"User {creatorId} is not authorized to cancel this campaign.");

            return _mapper.Map<CampaignDto>(await _campaignRepository.CancelCampaign(campaignId));
        }

        public async Task Cancelayment(long orderCode)
        {
            var payment = await _paymentRepository.GetPaymentByOrderCodeAsync(orderCode);
            if (payment == null)
                throw new Exception($"Payment with order code {orderCode} not found.");
            if (payment.Status != PaymentStatus.pending)
                throw new Exception("Payment is not in pending status.");
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
                throw new Exception("No campaigns found.");

            campaignCard.totalCampaigns = campaigns.Count();

            foreach (var campaign in campaigns)
            {
                switch (campaign.Status)
                {
                    case CampaignStatus.Approved:
                        campaignCard.totalUnstartedCampaigns++;
                        break;

                    case CampaignStatus.Pending:
                        campaignCard.totalUnpaidCampaigns++;
                        break;

                    case CampaignStatus.InProgress:
                        if (campaign.Deadline >= now)
                            campaignCard.totalInprogressCampaigns++;
                        else
                            campaignCard.totalOverdueCampaigns++;
                        break;

                    case CampaignStatus.Completed:
                        campaignCard.totalCompletedCampaigns++;
                        break;
                }
            }
            return campaignCard;
        }

        public async Task<List<CampaignChart>> GetCampaignChard(string uid, int year)
        {
            var result = new List<CampaignChart>();

            // Khởi tạo 12 tháng
            for (int month = 1; month <= 12; month++)
            {
                result.Add(new CampaignChart
                {
                    month = month,
                    approvedCampaigns = 0,
                    inprogressCampaigns = 0,
                    completedCampaigns = 0
                });
            }

            // Lấy tất cả campaigns theo năm
            var campaigns = await _campaignRepository.GetBrandCampaignsByYear(uid, year);
            if (campaigns == null || !campaigns.Any())
                throw new Exception($"No campaigns found in year {year}");

            foreach (var campaign in campaigns)
            {
                var campaignDate = campaign.StartTime ?? campaign.UploadedDate;

                int month = campaignDate.Month;

                var chart = result.FirstOrDefault(c => c.month == month);
                if (chart == null) continue;

                switch (campaign.Status)
                {
                    case CampaignStatus.Approved:
                        chart.approvedCampaigns++;
                        break;

                    case CampaignStatus.InProgress:
                        chart.inprogressCampaigns++;
                        break;

                    case CampaignStatus.Completed:
                        chart.completedCampaigns++;
                        break;
                }
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
            if(campaign.CreatedBy != uid)
                throw new AuthenticationException($"You are can not view this campaign detail.");

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