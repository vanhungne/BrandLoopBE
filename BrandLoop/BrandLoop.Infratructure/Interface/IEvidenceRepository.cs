using BrandLoop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface IEvidenceRepository
    {
        Task AddEvidenceAsync(Evidence evidence);
        Task<List<Evidence>> GetEvidences(int influencerReportId);
    }
}
