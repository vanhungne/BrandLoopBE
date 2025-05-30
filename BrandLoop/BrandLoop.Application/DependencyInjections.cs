using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Application.Background;
using BrandLoop.Application.Interfaces;
using BrandLoop.Application.Service;
using Microsoft.Extensions.DependencyInjection;

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
            return services;
        } 
        }
}
