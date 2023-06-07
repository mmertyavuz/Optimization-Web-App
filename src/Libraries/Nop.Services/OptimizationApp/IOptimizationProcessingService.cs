using System.Threading.Tasks;

namespace Nop.Services.OptimizationApp;

public interface IOptimizationProcessingService
{
    public Task StartOptimizationProcessingAsync();
    
    public Task<bool> IsPluginInstalledAsync();
    
    public Task<bool> IsPluginAvailableAsync();
}

public class OptimizationProcessingService : IOptimizationProcessingService
{
    public Task StartOptimizationProcessingAsync()
    {
        throw new System.NotImplementedException();
    }

    public async Task<bool> IsPluginInstalledAsync()
    {
        return false;
    }
    
    public async Task<bool> IsPluginAvailableAsync()
    {
        return false;
    }
} 