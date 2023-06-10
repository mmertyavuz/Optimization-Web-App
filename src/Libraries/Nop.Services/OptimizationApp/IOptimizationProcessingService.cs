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

    Task<OptimizationStatus> GetOptimizationStatus();
}

public class OptimizationProcessingService : IOptimizationProcessingService
{
    #region Fields

    private readonly IRepository<OptimizationResult> _optimizationResultRepository;
    private readonly ICorporationService _corporationService;
    private readonly ISectionService _sectionService;

    #endregion

    #region Ctor

    public OptimizationProcessingService(IRepository<OptimizationResult> optimizationResultRepository, ICorporationService corporationService, ISectionService sectionService)
    {
        _optimizationResultRepository = optimizationResultRepository;
        _corporationService = corporationService;
        _sectionService = sectionService;
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
    
    public async Task<OptimizationStatus> GetOptimizationStatus()
    {
        var anyClassroom = await _corporationService.IsThereAnyClassroomAsync();

        var anySection = await _sectionService.IsThereAnySectionAsync();
        
        var anyOptimizationResult = IsOptimized();

        if (!anyClassroom || !anySection)
            return OptimizationStatus.WaitingData;

        if (!anyOptimizationResult)
           return OptimizationStatus.WaitingOptimization;
        
        return OptimizationStatus.Optimized;
    }
    
} 