using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Misc.RcSmsService.Infrastructure;

public class NopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<Services.RcSmsService>();
        services.AddTransient<RCSmsService.IRcSmsService>(provider =>
        {
            var client = new RCSmsService.RcSmsServiceClient();
            var serviceUrl = provider.GetRequiredService<Services.RcSmsService>().GetServiceUrl();

            if(!string.IsNullOrEmpty(serviceUrl))
                client.Endpoint.Address = new System.ServiceModel.EndpointAddress(serviceUrl);

            return client;
        });
    }

    public void Configure(IApplicationBuilder application)
    {
        
    }

    public int Order => 11;
}