using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.RcMailService.Services;

namespace Nop.Plugin.Misc.RcMailService.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<RcEmailService>();
            services.AddTransient<RCEmailService.IRCEmailService>(provider =>
            {
                var client = new RCEmailService.RCEmailServiceClient();
                var serviceUrl = provider.GetRequiredService<RcEmailService>().GetServiceUrl();

                if (!string.IsNullOrEmpty(serviceUrl))
                {
                    client.Endpoint.Address = new System.ServiceModel.EndpointAddress(serviceUrl);
                }

                return client;
            });

        }

        public void Configure(IApplicationBuilder application)
        {

        }

        public int Order => 11;
    }
}