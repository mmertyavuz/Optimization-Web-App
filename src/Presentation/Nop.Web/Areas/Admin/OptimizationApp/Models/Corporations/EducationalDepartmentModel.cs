using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Corporations;

public record EducationalDepartmentModel : BaseNopEntityModel
{
    public EducationalDepartmentModel()
    {
        AvailableFaculties = new List<SelectListItem>();
        AvailableCustomers = new List<SelectListItem>();
    }
    
    #region Domain Objects

    [NopResourceDisplayName("Admin.Corporations.EducationalDepartments.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Corporations.EducationalDepartments.Fields.Code")]
    public string Code { get; set; }

    [NopResourceDisplayName("Admin.Corporations.EducationalDepartments.Fields.Description")]
    public string Description { get; set; }

    [NopResourceDisplayName("Admin.Corporations.EducationalDepartments.Fields.Faculty")]
    public int FacultyId { get; set; }

    [NopResourceDisplayName("Admin.Corporations.EducationalDepartments.Fields.DepartmentLeadCustomerId")]
    public int DepartmentLeadCustomerId { get; set; }

    #endregion
    
    public string FacultyName { get; set; }
    public string DepartmentLeadCustomerName { get; set; }

    public IList<SelectListItem> AvailableFaculties { get; set; }
    public IList<SelectListItem> AvailableCustomers { get; set; }
}

public record EducationalDepartmentListModel : BasePagedListModel<EducationalDepartmentModel>
{
    
}

public record EducationalDepartmentSearchModel : BaseSearchModel
{
    public EducationalDepartmentSearchModel()
    {
        AvailableFaculties = new List<SelectListItem>();
    }
    [NopResourceDisplayName("Admin.Corporations.EducationalDepartments.Fields.Name")]
    public string Name { get; set; }
    
    [NopResourceDisplayName("Admin.Corporations.EducationalDepartments.Fields.Code")]
    public string Code { get; set; }
    
    [NopResourceDisplayName("Admin.Corporations.EducationalDepartments.Fields.Faculty")]
    public int FacultyId { get; set; }
    
    public IList<SelectListItem> AvailableFaculties { get; set; }
}