using AutoMapper;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.Authen;
using BrandLoop.Infratructure.Models.CampainModel;
using BrandLoop.Infratructure.Models.ChatDTO;
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

            CreateMap<InfluencerType, InfluTypeModel>().ReverseMap();

            CreateMap<Campaign, CampaignDto>().ReverseMap();
            CreateMap<Campaign, CampaignDto>()
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
                .ForMember(dest => dest.CampaignReports, opt => opt.Ignore())
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
                .ForMember(dest => dest.CampaignReports, opt => opt.Ignore())
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

            // CampaignImage mappings
            CreateMap<CampaignImage, CampaignImageDto>();
            CreateMap<CampaignImageDto, CampaignImage>()
                .ForMember(dest => dest.Campaign, opt => opt.Ignore());
            // CampaignInvitation mapping
            CreateMap<CampaignInvitation, InvitationDTO>();
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



            CreateMap<UserOnlineStatus, UserOnlineStatusDto>();

        }
    }
}
