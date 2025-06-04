using BrandLoop.Domain.Entities;
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
    }
}
