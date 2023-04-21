using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Corporations;

public record CorporationSettingsModel : BaseNopModel, ISettingsModel
{
    public string CorporationName { get; set; }

    public string CorporationWebsite { get; set; }
    
    public int ActiveStoreScopeConfiguration { get; set; }
}