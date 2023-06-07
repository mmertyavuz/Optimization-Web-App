using System;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Education;

public record CourseSectionPlanModel : BaseNopEntityModel
{
    public int SectionId { get; set; }
    public DayOfWeek Day { get; set; }
    
    public TimeSpan StartTime { get; set; }
    
    public TimeSpan EndTime { get; set; }
}

public record CourseSectionPlanSearchModel : BaseSearchModel
{
    public int SectionId { get; set; }
}

public record CourseSectionPlanListModel : BasePagedListModel<CourseSectionPlanModel>
{
    
}   