using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using BrandLoop.Application.Interfaces;


namespace BrandLoop.Application.Service
{
    public class CloundinaryService : ICloundinaryService
    {
        private readonly IConfiguration _configuration;
        public CloundinaryService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<string> UploadImage(IFormFile file)
        {
            var cloudinary = new Cloudinary(new Account(
            cloud: _configuration.GetSection("Cloudinary:CloudName").Value,
            apiKey: _configuration.GetSection("Cloudinary:ApiKey").Value,
            apiSecret: _configuration.GetSection("Cloudinary:ApiSecret").Value
        ));
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream())
            };
            var uploadResult = cloudinary.Upload(uploadParams);

            return Task.FromResult(uploadResult.SecureUri.AbsoluteUri);
        }
    }
}
