using Nop.Web.Framework.Models;

namespace Bau.Plugin.Optimization.Gurobi.Models;

public record ConfigurationModel : BaseNopModel
{
    public string GurobiLicenceKey { get; set; }

    public string BaseUrl { get; set; }

    public bool UseTestMode { get; set; }
}