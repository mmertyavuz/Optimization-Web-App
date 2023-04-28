using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Corporations;

public record ClassroomModel : BaseNopEntityModel
{
    [NopResourceDisplayName("Admin.Corporations.Classrooms.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Corporations.Classrooms.Fields.Description")]
    public string Description { get; set; }

    [NopResourceDisplayName("Admin.Corporations.Classrooms.Fields.Capacity")]
    public int Capacity { get; set; }
}

public record ClassroomListModel : BasePagedListModel<ClassroomModel>
{
}

public record ClassroomSearchModel : BaseSearchModel
{
    [NopResourceDisplayName("Admin.Corporations.Classrooms.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Corporations.Classrooms.Fields.MinCapacity")]
    public int MinCapacity { get; set; }
    
    [NopResourceDisplayName("Admin.Corporations.Classrooms.Fields.MaxCapacity")]
    public int MaxCapacity { get; set; }
}