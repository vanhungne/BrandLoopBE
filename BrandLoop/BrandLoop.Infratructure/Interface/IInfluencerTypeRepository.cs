using BrandLoop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface IInfluencerTypeRepository
    {
        Task<List<InfluencerType>> GetAllInfluencerTypesAsync();
        Task<InfluencerType> GetInfluencerTypeByIdAsync(int id);
        Task<InfluencerType> AddInfluencerTypeAsync(InfluencerType influencerType);
        Task<InfluencerType> UpdateInfluencerTypeAsync(InfluencerType influencerType);
    }
}
