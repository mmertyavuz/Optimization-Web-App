using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain;
using Nop.Data;

namespace Nop.Services.OptimizationApp;

public interface IOptimizationProcessingService
{
    public Task StartOptimizationProcessingAsync();

    Task InsertOptimizationDataAsync(OptimizationResult optimizationResult);

    Task DeleteAllOptimizationResultsAsync();

    bool IsOptimized();
}

public class OptimizationProcessingService : IOptimizationProcessingService
{
    #region Fields

    private readonly IRepository<OptimizationResult> _optimizationResultRepository;

    #endregion

    #region Ctor

    public OptimizationProcessingService(IRepository<OptimizationResult> optimizationResultRepository)
    {
        _optimizationResultRepository = optimizationResultRepository;
    }

    #endregion
    public Task StartOptimizationProcessingAsync()
    {
        throw new System.NotImplementedException();
    }
    
    public async Task InsertOptimizationDataAsync(OptimizationResult optimizationResult)
    {
        await _optimizationResultRepository.InsertAsync(optimizationResult);
    }
    
    public async Task DeleteAllOptimizationResultsAsync()
    {
        await _optimizationResultRepository.TruncateAsync(true);
    }
    
    public bool IsOptimized()
    {
        return _optimizationResultRepository.Table.Any();
    }
} 