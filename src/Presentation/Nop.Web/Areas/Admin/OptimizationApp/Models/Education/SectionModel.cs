using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Web.Areas.Admin.Models.Education;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

public record SectionModel : BaseNopEntityModel

{
    public SectionModel()
    {
        AvailableCourses = new List<SelectListItem>();
        Periods = new List<CourseSectionPlanModel>();
    }
    #region Domain Objects

    /// <summary>
    /// The unique section number for the course section (e.g., "01").
    /// </summary>
    public string SectionNumber { get; set; }

    /// <summary>
    /// The identifier of the associated course for the section.
    /// </summary>
    public int CourseId { get; set; }
    
    /// <summary>
    /// The day of the week when the course section is scheduled.
    /// </summary>
    public DayOfWeek Day { get; set; }
    
    /// <summary>
    /// The starting time of the course section on the scheduled day.
    /// </summary>
    public TimeSpan StartTime { get; set; }
    
    /// <summary>
    /// The ending time of the course section on the scheduled day.
    /// </summary>
    public TimeSpan EndTime { get; set; }
    
    public int StudentCount { get; set; }

    public string DayName { get; set; }
    
    #endregion

    public string CourseName { get; set; }
    
    public IList<SelectListItem> AvailableCourses { get; set; }

    #region List Props

    public IList<CourseSectionPlanModel> Periods { get; set; }

    #endregion
}

public record SectionListModel : BasePagedListModel<SectionModel>
{
    
}

public record SectionSearchModel : BaseSearchModel
{
    public SectionSearchModel()
    {
        AvailableCourses = new List<SelectListItem>();
    }
    [NopResourceDisplayName("Admin.Courses.Sections.Fields.SectionNumber")]
    public string SectionNumber { get; set; }
        
    [NopResourceDisplayName("Admin.Courses.Sections.Fields.Course")]
    public int CourseId { get; set; }

    public int ClassroomId { get; set; }

    public IList<SelectListItem> AvailableCourses { get; set; }
    
}
