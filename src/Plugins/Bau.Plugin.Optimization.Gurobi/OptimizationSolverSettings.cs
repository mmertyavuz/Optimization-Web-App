using Nop.Core.Configuration;

namespace Bau.Plugin.Optimization.Gurobi
{
    /// <summary>
    /// Represents a plugin settings
    /// </summary>
    public class OptimizationSolverSettings : ISettings
    {
        public string GurobiLicenceKey { get; set; }

        public string BaseUrl { get; set; }

        public bool UseTestMode { get; set; }
    }
}