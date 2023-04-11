using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.RcSmsService.Infrastructure;

public class RouteProvider : IRouteProvider
{
    public int Priority => 0;
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute(name: RcSmsServiceDefaults.ConfigurationRouteName,
            pattern: "Admin/RcSmsService/Configure",
            defaults: new { controller = "RcSmsService", action = "Configure" });
    }
}