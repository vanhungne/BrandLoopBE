using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BrandLoop.Application.Interfaces
{
    public interface ICloundinaryService
    {
        Task<string> UploadImage(IFormFile file);
    }
}
