using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Infratructure.Configurations;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Interfaces;
using BrandLoop.Infratructure.Mapper;
using BrandLoop.Infratructure.Reporitory;
using BrandLoop.Infratructure.ReporitorY;
using BrandLoop.Infratructure.Repository;
using BrandLoop.Infratructure.Service;
using Microsoft.Extensions.DependencyInjection;


namespace BrandLoop.Infratructure
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddInfratructure(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IAuthenRepository, AuthenRepository>();
            services.AddScoped<IEmailSender, EmailSenderRepository>();
            services.AddScoped<ICloundinaryRepository, CloundinaryRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<ICampaignRepository, CampaignRepository>();
            services.AddScoped<IAccountCleanupRepository, AccountCleanupRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<ICampaignInvitationRepository, CampaignInvitationRepository>();
            services.AddScoped<IDealRepository, DealRepository>();
            services.AddScoped<IImageCampainRepository, ImageCampainRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaySystem, PaySystem>();
            services.AddScoped<IBrandProfileRepository, BrandProfileRepository>();
            services.AddScoped<IInfluenceRepository, InfluenceProfileRepository>();
            services.AddScoped<IInfluencerTypeRepository, InfluencerTypeRepository>();
            services.AddScoped<IInfluencerReportRepository, InfluencerReportRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IBannerRepository, BannerRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<ISubscriptionRegisterRepository, SubscriptionRegisterRepository>();
            services.AddScoped<IEvidenceRepository, EvidenceRepository>();
            services.AddScoped<IKolsJoinCampaignRepository, KolsJoinCampaignRepository>();
            return services;
        }
    }
}
