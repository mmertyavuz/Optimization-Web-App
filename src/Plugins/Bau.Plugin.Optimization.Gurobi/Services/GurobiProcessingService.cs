using System.Threading.Tasks;
using Nop.Services.OptimizationApp;

namespace Bau.Plugin.Optimization.Gurobi.Services;

public class GurobiProcessingService : IOptimizationProcessingService
{
    #region Fields

    private readonly OptimizationSolverSettings _optimizationSolverSettings;

    #endregion

    #region Ctor

    public GurobiProcessingService(OptimizationSolverSettings optimizationSolverSettings)
    {
        _optimizationSolverSettings = optimizationSolverSettings;
    }

    #endregion
    
    public Task StartOptimizationProcessingAsync()
    {
        throw new System.NotImplementedException();
    }

    public async Task<bool> IsPluginInstalledAsync()
    {
        return true;
    }

    public async Task<bool> IsPluginAvailableAsync()
    {
        if (string.IsNullOrEmpty(_optimizationSolverSettings.GurobiLicenceKey) || string.IsNullOrEmpty(_optimizationSolverSettings.BaseUrl))
            return false;
        
        return true;
    }
}