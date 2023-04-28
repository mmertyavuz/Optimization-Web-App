using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Corporations;

public record FacultyModel : BaseNopEntityModel
{
    [NopResourceDisplayName("Admin.Corporations.Faculties.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Corporations.Faculties.Fields.Description")]
    public string Description { get; set; }
}

public record FacultyListModel : BasePagedListModel<FacultyModel>
{
    
}

public record FacultySearchModel : BaseSearchModel
{
    [NopResourceDisplayName("Admin.Corporations.Faculties.Fields.Name")]
    public string Name { get; set; }
}