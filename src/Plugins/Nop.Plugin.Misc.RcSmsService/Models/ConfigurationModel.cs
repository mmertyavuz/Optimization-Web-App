using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.RcSmsService.Models;

public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName("Plugins.Misc.RcSmsService.ApplicationName")]
    public string ApplicationName { get; set; }
    
    [NopResourceDisplayName("Plugins.Misc.RcSmsService.ServicePassword")]
    public string ServicePassword { get; set; }
    
    [NopResourceDisplayName("Plugins.Misc.RcSmsService.ServiceUrl")]
    public string ServiceUrl { get; set; }
}