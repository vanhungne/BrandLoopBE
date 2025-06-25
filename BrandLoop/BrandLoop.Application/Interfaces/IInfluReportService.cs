using BrandLoop.Infratructure.Models.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Interfaces
{
    public interface IInfluReportService
    {
        Task FinishReport(string userId, InfluReport influReportDto);
    }
}
