using System.Threading.Tasks;
using Nop.Services.OptimizationApp;
using Nop.Web.Areas.Admin.OptimizationApp.Models;
using Nop.Web.Models.Optimization;

namespace Nop.Web.Areas.Admin.Factories;

public interface IOptimizationModelFactory
{
    public Task<OptimizationModel> PrepareOptimizationModelAsync();
    
}

public class OptimizationModelFactory : IOptimizationModelFactory
{
    #region Fields

    private readonly ICourseService _courseService;
    private readonly ISectionService _sectionService;
    private readonly ICorporationService _corporationService;

    public OptimizationModelFactory(ICourseService courseService, ISectionService sectionService, ICorporationService corporationService)
    {
        _courseService = courseService;
        _sectionService = sectionService;
        _corporationService = corporationService;
    }

    #endregion
    
    public async Task<OptimizationModel> PrepareOptimizationModelAsync()
    {
        
    
        return new OptimizationModel();
    }
}