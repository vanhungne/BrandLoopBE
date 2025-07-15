using AutoMapper;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.Authen;
using BrandLoop.Infratructure.Models.CampainModel;
using BrandLoop.Infratructure.Models.ChatDTO;
using BrandLoop.Infratructure.Models.NewDTO;
using BrandLoop.Infratructure.Models.Dashboard;
using BrandLoop.Infratructure.Models.Influence;
using BrandLoop.Infratructure.Models.Report;
using BrandLoop.Infratructure.Models.SubcriptionModel;
using BrandLoop.Infratructure.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;


namespace BrandLoop.Infratructure.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, PendingRegistrationDto>()
                             .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
                             .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
                             .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.BrandProfile.CompanyName))
                             .ForMember(dest => dest.Industry, opt => opt.MapFrom(src => src.BrandProfile.Industry))
                             .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.BrandProfile.Website))
                             .ForMember(dest => dest.logoUrl, opt => opt.MapFrom(src => src.BrandProfile.Logo))
                             .ForMember(dest => dest.imageUrl, opt => opt.MapFrom(src => src.ProfileImage))
                             .ForMember(dest => dest.Nickname, opt => opt.MapFrom(src => src.InfluenceProfile.Nickname))
                             .ForMember(dest => dest.ContentCategory, opt => opt.MapFrom(src => src.InfluenceProfile.ContentCategory))
                             .ForMember(dest => dest.Facebook, opt => opt.MapFrom(src =>
                                 src.InfluenceProfile != null ? src.InfluenceProfile.Facebook : src.BrandProfile.Facebook))
                             .ForMember(dest => dest.Instagram, opt => opt.MapFrom(src =>

                                 src.InfluenceProfile != null ? src.InfluenceProfile.Instagram : src.BrandProfile.Instagram));

            CreateMap<UpdateUserProfileDto, User>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // Fixed the error by replacing 'ForAllOtherMembers' with 'ForAllMembers'

            CreateMap<User, ProfileResponseDto>();

            // Brand Profile mappings
            CreateMap<UpdateBrandProfileDto, BrandProfile>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<BrandProfile, BrandProfileResponseDto>();

            // Influence Profile mappings
            CreateMap<UpdateInfluenceProfileDto, InfluenceProfile>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // Fixed the error by replacing 'ForAllOtherMembers' with 'ForAllMembers'

            CreateMap<InfluenceProfile, InfluenceProfileResponseDto>();

            CreateMap<InfluenceProfile, InfluencerList>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.User.ProfileImage))
                .ForMember(dest => dest.InfluencerType, opt => opt.MapFrom(src => src.InfluencerType.Name))
                .ForMember(dest => dest.PlatformFee, opt => opt.MapFrom(src => src.InfluencerType.PlatformFee));

            CreateMap<InfluencerType, InfluTypeModel>().ReverseMap();
            CreateMap<InfluencerType, InfluencerTypeSelectionModel>();

            CreateMap<Campaign, CampaignDto>().ReverseMap();
            CreateMap<Campaign, CampaignDto>()
                .ForMember(dest => dest.BrandIndustry,
                          opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Industry : null))
                //.ForMember(dest => dest.TotalKolsJoined,
                //          opt => opt.MapFrom(src => src.KolsJoinCampaigns != null ? src.KolsJoinCampaigns.Count : 0))
                .ForMember(dest => dest.Images,
                          opt => opt.MapFrom(src => src.CampaignImages));
            // Campaign mappings
            CreateMap<Campaign, CampaignDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.CampaignImages));

            CreateMap<CreateCampaignDto, Campaign>()
                .ForMember(dest => dest.CampaignId, opt => opt.Ignore())
                .ForMember(dest => dest.BrandId, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdate, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.Creator, opt => opt.Ignore())
                .ForMember(dest => dest.KolsJoinCampaigns, opt => opt.Ignore())
                .ForMember(dest => dest.Feedbacks, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignInvitations, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignReport, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignImages, opt => opt.Ignore())
                .ForMember(dest => dest.Payments, opt => opt.Ignore());

            CreateMap<UpdateCampaignDto, Campaign>()
                .ForMember(dest => dest.CampaignId, opt => opt.Ignore())
                .ForMember(dest => dest.BrandId, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.Creator, opt => opt.Ignore())
                .ForMember(dest => dest.KolsJoinCampaigns, opt => opt.Ignore())
                .ForMember(dest => dest.Feedbacks, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignInvitations, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignReport, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignImages, opt => opt.Ignore())
                .ForMember(dest => dest.Payments, opt => opt.Ignore());

            CreateMap<Campaign, PaymentCampaign>()
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.Payments.FirstOrDefault(p => p.Status != PaymentStatus.Canceled).PaymentId))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Payments.FirstOrDefault(p => p.Status != PaymentStatus.Canceled).Amount))
                .ForMember(dest => dest.paymentType, opt => opt.MapFrom(src => src.Payments.FirstOrDefault().Type))
                .ForMember(dest => dest.PaymentLink, opt => opt.MapFrom(src => src.Payments.FirstOrDefault(p => p.Status != PaymentStatus.Canceled).PaymentLink))
                .ForMember(dest => dest.CampaignName, opt => opt.MapFrom(src => src.CampaignName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Campaign, CampaignSelectOption>();
            // CampaignTracking mappings
            CreateMap<Campaign, CampaignTracking>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.CampaignImages))
                .ForMember(dest => dest.kolInCampaignTrackings, opt => opt.MapFrom(src => src.KolsJoinCampaigns))

            // Mapping từ CampaignReport
                .ForMember(dest => dest.TotalSpend, opt => opt.MapFrom(src => src.CampaignReport.TotalSpend))
                .ForMember(dest => dest.TotalRevenue, opt => opt.MapFrom(src => src.CampaignReport.TotalRevenue))
                .ForMember(dest => dest.TotalReach, opt => opt.MapFrom(src => src.CampaignReport.TotalReach ?? 0))
                .ForMember(dest => dest.TotalImpressions, opt => opt.MapFrom(src => src.CampaignReport.TotalImpressions ?? 0))
                .ForMember(dest => dest.TotalEngagement, opt => opt.MapFrom(src => src.CampaignReport.TotalEngagement ?? 0))
                .ForMember(dest => dest.TotalClicks, opt => opt.MapFrom(src => src.CampaignReport.TotalClicks ?? 0))
                .ForMember(dest => dest.AvgEngagementRate, opt => opt.MapFrom(src => src.CampaignReport.AvgEngagementRate ?? 0))
                .ForMember(dest => dest.CostPerEngagement, opt => opt.MapFrom(src => src.CampaignReport.CostPerEngagement ?? 0))
                .ForMember(dest => dest.ROAS, opt => opt.MapFrom(src => src.CampaignReport.ROAS ?? 0))

            // Bỏ qua null
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<KolsJoinCampaign, KolInCampaignTracking>()
                .ForMember(dest => dest.KolName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.TotalContent, opt => opt.MapFrom(src => src.InfluencerReport != null ? src.InfluencerReport.TotalContent : "0"))
                .ForMember(dest => dest.TotalReach, opt => opt.MapFrom(src => src.InfluencerReport != null ? src.InfluencerReport.TotalReach : 0))
                .ForMember(dest => dest.TotalImpressions, opt => opt.MapFrom(src => src.InfluencerReport != null ? src.InfluencerReport.TotalImpressions : 0))
                .ForMember(dest => dest.TotalEngagement, opt => opt.MapFrom(src => src.InfluencerReport != null ? src.InfluencerReport.TotalEngagement : 0))
                .ForMember(dest => dest.AvgEngagementRate, opt => opt.MapFrom(src => src.InfluencerReport != null ? src.InfluencerReport.AvgEngagementRate : 0.0))
                .ForMember(dest => dest.TotalClicks, opt => opt.MapFrom(src => src.InfluencerReport != null ? src.InfluencerReport.TotalClicks : 0))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // CampaignImage mappings
            CreateMap<CampaignImage, CampaignImageDto>();
            CreateMap<CampaignImageDto, CampaignImage>()
                .ForMember(dest => dest.Campaign, opt => opt.Ignore());
            // CampaignInvitation mapping
            CreateMap<CampaignInvitation, InvitationDTO>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.User.ProfileImage));
            CreateMap<Deal, DealDTO>();

            // Skill mapping
            CreateMap<Skill, SkillModel>();

            // Content and Style mapping
            CreateMap<ContentAndStyle, ContentAndStyleModel>();

            //Subscription mapping
            CreateMap<Subscription, SubscriptionDTO>();
            CreateMap<SubscriptionRegister, SubscriptionRegisterDTO>()
                .ForMember(dest => dest.SubscriptionName, opt => opt.MapFrom(src => src.Subscription.SubscriptionName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Subscription.Description));
            CreateMap<SubscriptionRegister, PaymentSubscription>()
                .ForMember(dest => dest.SubscriptionName, opt => opt.MapFrom(src => src.Subscription.SubscriptionName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Subscription.Description))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Subscription.Price))
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.Payments.FirstOrDefault().PaymentId))
                .ForMember(dest => dest.paymentType, opt => opt.MapFrom(src => src.Payments.FirstOrDefault().Type));

            // Feedback mapping
            CreateMap<Feedback, FeedbackDTO>().ReverseMap();
            CreateMap<Feedback, ShowFeedback>();



            CreateMap<UserOnlineStatus, UserOnlineStatusDto>();

            // Mapping for CampaignDtoDetail with BrandInfo
            CreateMap<Campaign, CampaignDtoDetail>()
                .ForMember(dest => dest.BrandIndustry,
                          opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Industry : null))
                .ForMember(dest => dest.TotalKolsJoined,
                          opt => opt.MapFrom(src => src.KolsJoinCampaigns != null ? src.KolsJoinCampaigns.Count : 0))
                .ForMember(dest => dest.Images,
                          opt => opt.MapFrom(src => src.CampaignImages))
                .ForMember(dest => dest.BrandInfo,
                          opt => opt.MapFrom(src => src.Brand != null ? new BrandInfoDto
                          {
                              Logo = src.Brand.Logo,
                              Avatar = src.Brand.User != null ? src.Brand.User.ProfileImage : null,
                              CompanyName = src.Brand.CompanyName,
                              Industry = src.Brand.Industry,
                              Email = src.Brand.User != null ? src.Brand.User.Email : null,
                              Phone = src.Brand.User != null ? src.Brand.User.Phone : null
                          } : null));

            // Alternative: Separate mapping for BrandInfo
            CreateMap<BrandProfile, BrandInfoDto>()
                .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.Logo))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User != null ? src.User.ProfileImage : null))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.Industry, opt => opt.MapFrom(src => src.Industry))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User != null ? src.User.Phone : null));

            // If using separate mapping, update the Campaign to CampaignDtoDetail mapping
            CreateMap<Campaign, CampaignDtoDetail>()
                .ForMember(dest => dest.BrandIndustry,
                          opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Industry : null))
                .ForMember(dest => dest.TotalKolsJoined,
                          opt => opt.MapFrom(src => src.KolsJoinCampaigns != null ? src.KolsJoinCampaigns.Count : 0))
                .ForMember(dest => dest.Images,
                          opt => opt.MapFrom(src => src.CampaignImages))
                .ForMember(dest => dest.BrandInfo,
                          opt => opt.MapFrom(src => src.Brand));

            // News to NewsDto
            CreateMap<News, NewsDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // News to NewsListDto (for listing pages)
            CreateMap<News, NewsListDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // News to NewsDetailDto (for single news detail)
            CreateMap<News, NewsDetailDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.AuthorUser));

            // User to AuthorDto
            CreateMap<User, AuthorDto>();

            // News to MyNewsDto (for user's own news)
            CreateMap<News, MyNewsDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // News to PendingNewsDto (for admin pending news)
            CreateMap<News, PendingNewsDto>();
        }
    }
}
