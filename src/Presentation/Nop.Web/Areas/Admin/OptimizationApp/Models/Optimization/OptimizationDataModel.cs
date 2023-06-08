using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.OptimizationApp.Models;

public record OptimizationDataModel : BaseNopModel
{
    public int SectionId { get; set; }
    public int ClassroomId { get; set; }
}