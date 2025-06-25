using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Application.Background;
using BrandLoop.Application.Interfaces;
using BrandLoop.Application.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace BrandLoop.Application
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthenService, AuthenService>();
            services.AddScoped<ICloundinaryService, CloundinaryService>();
            services.AddScoped<ICampaignService, CampaignService>();
            services.AddHostedService<AccountCleanupBackgroundService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<ICampaignInvitationService, CampaignInvitationService>();
            services.AddScoped<IDealService, DealService>();
            services.AddScoped<ICampaignImageService, CampaignImageService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IInfluencerTypeService, InfluencerTypeService>();
            services.AddScoped<IChatAiService, GroqAiService>();
            services.AddScoped<IChatService, ChatService>();
            // Ensure the HttpClient extension method is available
            services.AddHttpClient<IChatAiService, GroqAiService>();

            return services;
        }
    }
}
