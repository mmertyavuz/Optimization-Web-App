using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.RcMailService.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 0;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(name: RcMailServiceDefaults.ConfigurationRouteName,
                pattern: "Admin/RcMailService/Configure",
                defaults: new { controller = "RcMailService", action = "Configure" });
        }
    }
}
