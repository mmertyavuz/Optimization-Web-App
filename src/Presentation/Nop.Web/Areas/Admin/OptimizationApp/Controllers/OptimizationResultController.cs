using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.OptimizationApp;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.OptimizationApp.Models;

namespace Nop.Web.Areas.Admin.Controllers;

public class OptimizationResultController : BaseAdminController
{
    #region Fields

    private readonly IOptimizationResultService _optimizationResultService;
    private readonly IOptimizationResultModelFactory _optimizationResultModelFactory;
    private readonly IPermissionService _permissionService;
    private readonly IOptimizationProcessingService _optimizationProcessingService;

    #endregion

    #region Ctor

    public OptimizationResultController(IOptimizationResultService optimizationResultService, IOptimizationResultModelFactory optimizationResultModelFactory, IPermissionService permissionService, IOptimizationProcessingService optimizationProcessingService)
    {
        _optimizationResultService = optimizationResultService;
        _optimizationResultModelFactory = optimizationResultModelFactory;
        _permissionService = permissionService;
        _optimizationProcessingService = optimizationProcessingService;
    }

    #endregion
    
    public async Task<IActionResult> List()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            return AccessDeniedView();
        
        var model = await _optimizationResultModelFactory.PrepareOptimizationResultSearchModelAsync(new OptimizationResultSearchModel());
        
        return View(model);
    }
    
    [HttpPost]
    public virtual async Task<IActionResult> List(OptimizationResultSearchModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            return AccessDeniedView();
        
        var model = await _optimizationResultModelFactory.PrepareOptimizationResultListModelAsync(searchModel);
        
        return Json(model);
    }
    
  
    public virtual async Task<IActionResult> Details(int id)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            return AccessDeniedView();
        
        var optimizationResult = await _optimizationResultService.GetOptimizationResultByIdAsync(id);
        
        if (optimizationResult == null)
            return RedirectToAction("List");
        
        var model = await _optimizationResultModelFactory.PrepareOptimizationResultModelAsync(new OptimizationResultModel(), optimizationResult);
        
        return View(model);
    }
    
    public virtual async Task<IActionResult> DeleteAll(int id)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            return AccessDeniedView();
        
        await _optimizationProcessingService.DeleteAllOptimizationResultsAsync();
        
        return RedirectToAction("Index", "Optimization");
    }
}