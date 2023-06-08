using System.Threading.Tasks;

namespace Nop.Services.OptimizationApp;

public interface IOptimizationProcessingService
{
    public Task StartOptimizationProcessingAsync();

}

public class OptimizationProcessingService : IOptimizationProcessingService
{
    public Task StartOptimizationProcessingAsync()
    {
        throw new System.NotImplementedException();
    }
} 