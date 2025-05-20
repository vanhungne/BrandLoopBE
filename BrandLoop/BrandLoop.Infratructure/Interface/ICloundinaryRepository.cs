using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BrandLoop.Infratructure.Interface
{
    public interface ICloundinaryRepository
    {
        Task<string> UploadImage(IFormFile file);
    }
}
