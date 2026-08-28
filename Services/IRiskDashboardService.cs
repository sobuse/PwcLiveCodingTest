using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using

using RiskDashboard.LiveCoding.Models;

public interface IRiskDashboardService
{
    Task<List<RiskDashboardDto>> GetDashboardSummaryAsync(
        RiskDashboardRequest request,
        CancellationToken cancellationToken = default);
}