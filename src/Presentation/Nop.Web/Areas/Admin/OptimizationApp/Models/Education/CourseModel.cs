using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Education;

public record CourseModel : BaseNopEntityModel
{
    public CourseModel()
    {
        AvailableEducationalDepartments = new List<SelectListItem>();
        SectionSearchModel = new SectionSearchModel();
    }
    
    [NopResourceDisplayName("Admin.Education.Courses.Fields.Code")]
    public string Code { get; set; }
    
    [NopResourceDisplayName("Admin.Education.Courses.Fields.Name")]
    public string Name { get; set; }
    
    [NopResourceDisplayName("Admin.Education.Courses.Fields.Description")]
    public string Description { get; set; }
    
    [NopResourceDisplayName("Admin.Education.Courses.Fields.Credit")]
    public int Credit { get; set; }
    
    [NopResourceDisplayName("Admin.Education.Courses.Fields.Ects")]
    public int Ects { get; set; }
    
    [NopResourceDisplayName("Admin.Courses.Education.Fields.EducationalDepartmentId")]
    public int EducationalDepartmentId { get; set; }

    public IList<SelectListItem> AvailableEducationalDepartments { get; set; }
    
    public string EducationalDepartmentName { get; set; }

    public SectionSearchModel SectionSearchModel { get; set; }
}

public record CourseListModel : BasePagedListModel<CourseModel>
{
    
}

public record CourseSearchModel : BaseSearchModel
{
    public CourseSearchModel()
    {
        AvailableEducationalDepartments = new List<SelectListItem>();
    }
    
    [NopResourceDisplayName("Admin.Courses.Education.Fields.EducationalDepartmentId")]
    public int EducationalDepartmentId { get; set; }

    [NopResourceDisplayName("Admin.Education.Courses.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Education.Courses.Fields.Code")]
    public string Code { get; set; }
    
    public IList<SelectListItem> AvailableEducationalDepartments { get; set; }
}