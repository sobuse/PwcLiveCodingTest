using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using RiskDashboard.LiveCoding.Data;
using RiskDashboard.LiveCoding.Models;

namespace RiskDashboard.LiveCoding.Services;

public class RiskDashboardService
{
    private readonly AppDbContext _db;
    private readonly IDistributedCache _cache;
    private readonly ILogger<RiskDashboardService> _logger;

    public RiskDashboardService(
        AppDbContext db,
        IDistributedCache cache,
        ILogger<RiskDashboardService> logger)
    {
        _db = db;
        _cache = cache;
        _logger = logger;
    }

    public class RiskDashboardService : IRiskDashboardService
{
    private readonly AppDbContext _db;
    private readonly IDistributedCache _cache;
    private readonly ILogger<RiskDashboardService> _logger;

    public RiskDashboardService(
        AppDbContext db,
        IDistributedCache cache,
        ILogger<RiskDashboardService> logger)
    {
        _db = db;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<RiskDashboardDto>> GetDashboardSummaryAsync(
        RiskDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildCacheKey(request);

        var cached = await _cache.GetStringAsync(
            cacheKey,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(cached))
        {
            _logger.LogInformation(
                "Returning risk dashboard from cache. TenantId: {TenantId}",
                request.TenantId);

            return JsonSerializer.Deserialize<List<RiskDashboardDto>>(cached)
                   ?? new List<RiskDashboardDto>();
        }

        var query = _db.Risks
            .AsNoTracking()
            .Where(r => r.TenantId == request.TenantId);

        if (request.BusinessUnitId.HasValue)
        {
            query = query.Where(r =>
                r.BusinessUnitId == request.BusinessUnitId.Value);
        }

        var result = await query
            .Select(r => new RiskDashboardDto
            {
                Id = r.Id,
                Name = r.Name,
                RiskLevel = r.RiskLevel,
                Status = r.Status,

                ControlsCount = r.Controls.Count(),
                AssessmentsCount = r.Assessments.Count()
            })
            .ToListAsync(cancellationToken);

        await CacheResultAsync(
            cacheKey,
            result,
            cancellationToken);

        return result;
    }

    private static string BuildCacheKey(RiskDashboardRequest request)
    {
        return request.BusinessUnitId.HasValue
            ? $"risk-dashboard:{request.TenantId}:{request.BusinessUnitId}"
            : $"risk-dashboard:{request.TenantId}";
    }

    private async Task CacheResultAsync(
        string cacheKey,
        List<RiskDashboardDto> result,
        CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(result);

        await _cache.SetStringAsync(
            cacheKey,
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(5)
            },
            cancellationToken);
    }
}
