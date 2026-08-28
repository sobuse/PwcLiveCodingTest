using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskDashboard.LiveCoding.Dtos
{
    public class RiskDto
    {
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid BusinessUnitId { get; set; }
    public string Title { get; set; }
    public DateTime CreatedDate { get; set; }
    }
}