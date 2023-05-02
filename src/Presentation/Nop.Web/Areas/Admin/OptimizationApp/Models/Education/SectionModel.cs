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
    }
    #region Domain Objects

    [NopResourceDisplayName("Admin.Courses.Sections.Fields.SectionNumber")]
    public string SectionNumber { get; set; }
        
    [NopResourceDisplayName("Admin.Courses.Sections.Fields.Course")]
    public int CourseId { get; set; }

    #endregion

    public string CourseName { get; set; }
    
    public IList<SelectListItem> AvailableCourses { get; set; }
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

    public IList<SelectListItem> AvailableCourses { get; set; }
    
}
