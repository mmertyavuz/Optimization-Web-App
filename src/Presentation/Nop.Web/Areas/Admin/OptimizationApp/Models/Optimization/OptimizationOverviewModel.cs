using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.OptimizationApp.Models;

public record OptimizationOverviewModel : BaseNopModel
{
    public int ClassroomCount { get; set; }

    public int SectionCount { get; set; }

    public bool IsReadyForOptimization { get; set; }

    public bool IsOptimized { get; set; }
}