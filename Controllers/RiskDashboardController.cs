using Microsoft.AspNetCore.Mvc;
using RiskDashboard.LiveCoding.Models;
using RiskDashboard.LiveCoding.Services;

namespace RiskDashboard.LiveCoding.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RiskDashboardController : ControllerBase
{
    private readonly IRiskDashboardService _service;

    public RiskDashboardController(
        IRiskDashboardService service)
    {
        _service = service;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<List<RiskDashboardDto>>> GetSummary(
        [FromQuery] RiskDashboardRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetDashboardSummaryAsync(
            request,
            cancellationToken);

        return Ok(result);
    }
}