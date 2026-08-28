using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using

using RiskDashboard.LiveCoding.Models;

namespace RiskDashboard.LiveCoding.Interfaces
{
    public class IRiskDashboardService 
    {
        Task<List<RiskDashboardDto>> GetDashboardSummaryAsync(RiskDashboardRequest request, CancellationToken cancellationToken = default);
    }
}