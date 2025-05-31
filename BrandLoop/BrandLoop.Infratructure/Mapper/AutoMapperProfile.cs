using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.Authen;
using BrandLoop.Infratructure.Models.CampainModel;
using BrandLoop.Infratructure.Models.UserModel;


namespace BrandLoop.Infratructure.Mapper
{
    public class AutoMapperProfile : Profile
    {
            public AutoMapperProfile()
            {
            CreateMap<User, PendingRegistrationDto>()
     .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
     .ForMember(dest =>dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
     .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.BrandProfile.CompanyName))
     .ForMember(dest => dest.Industry, opt => opt.MapFrom(src => src.BrandProfile.Industry))
     .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.BrandProfile.Website))
     .ForMember(dest =>dest.logoUrl, opt =>opt.MapFrom(src=> src.BrandProfile.Logo))
     .ForMember(dest=>dest.imageUrl,opt =>opt.MapFrom(src=>src.ProfileImage))
     .ForMember(dest => dest.Nickname, opt => opt.MapFrom(src => src.InfluenceProfile.Nickname))
     .ForMember(dest => dest.ContentCategory, opt => opt.MapFrom(src => src.InfluenceProfile.ContentCategory))
     .ForMember(dest => dest.Facebook, opt => opt.MapFrom(src =>
         src.InfluenceProfile != null ? src.InfluenceProfile.Facebook : src.BrandProfile.Facebook))
     .ForMember(dest => dest.Instagram, opt => opt.MapFrom(src =>

         src.InfluenceProfile != null ? src.InfluenceProfile.Instagram : src.BrandProfile.Instagram));


            CreateMap<Campaign, CampaignDto>().ReverseMap();
            CreateMap<CreateCampaignDto, Campaign>();
            CreateMap<UpdateCampaignDto, Campaign>();

            // Skill mapping
            CreateMap<Skill, SkillModel>();

            // Content and Style mapping
            CreateMap<ContentAndStyle, ContentAndStyleModel>();


        }
        
    }
}
