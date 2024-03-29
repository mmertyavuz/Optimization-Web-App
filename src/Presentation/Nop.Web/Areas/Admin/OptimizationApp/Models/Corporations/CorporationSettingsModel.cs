using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Corporations;

public record CorporationSettingsModel : BaseNopModel, ISettingsModel
{
    
    public string CorporationName { get; set; }

    public string CorporationWebsite { get; set; }
    
    public int ActiveStoreScopeConfiguration { get; set; }

    public string LogoUrl { get; set; }
    
    public string MiniLogoUrl { get; set; }
    
    public string CorporationEmailSuffix { get; set; }
    
}