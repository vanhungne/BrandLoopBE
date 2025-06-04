using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface IBrandProfileRepository
    {
        Task<BrandProfile> GetByUidAsync(string uid);
        Task UpdateAsync(BrandProfile brandProfile);
        Task SaveChangesAsync();
    }
}
