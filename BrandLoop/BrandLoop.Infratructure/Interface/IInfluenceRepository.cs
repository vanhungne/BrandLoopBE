using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Models.FeartureDTO;
using BrandLoop.Infratructure.Models.Influence;
using BrandLoop.Infratructure.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface IInfluenceRepository
    {
        Task<InfluenceProfile> GetByUidAsync(string uid);
        Task UpdateAsync(InfluenceProfile influenceProfile);
        Task SaveChangesAsync();

        Task<List<InfluenceProfile>> SearchAsync(InfluenceSearchOptions opts);
        Task<List<InfluenceProfile>> SearchHomeFeaturedAsync(InfluenceSearchOptions opts);
    }
}
