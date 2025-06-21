using BrandLoop.Infratructure.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Interfaces
{
    public interface IInfluencerTypeService
    {
        Task<List<InfluTypeModel>> GetAllInfluencerTypesAsync();
        Task<InfluTypeModel> GetInfluencerTypeByIdAsync(int id);
        Task<InfluTypeModel> AddInfluencerTypeAsync(InfluTypeModel influencerType);
        Task<InfluTypeModel> UpdateInfluencerTypeAsync(InfluTypeModel influencerType);
    }
}
