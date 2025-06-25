using BrandLoop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface IBannerRepository
    {
        Task<List<Banner>> GetActiveBannersAsync();
    }
}
