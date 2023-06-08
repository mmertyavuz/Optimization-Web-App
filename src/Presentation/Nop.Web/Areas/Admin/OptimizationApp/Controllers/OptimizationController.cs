using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.OptimizationApp;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.OptimizationApp.Models;

namespace Nop.Web.Areas.Admin.Controllers;

public class OptimizationController : BaseAdminController
{

    #region Fields

    private readonly IPermissionService _permissionService;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;
    private readonly ICorporationService _corporationService;
    private readonly ICourseService _courseService;
    private readonly ISectionService _sectionService;
    private readonly IOptimizationProcessingService _optimizationProcessingService;

    #endregion

    #region Ctor

    public OptimizationController(IPermissionService permissionService, INotificationService notificationService, ILocalizationService localizationService, ICorporationService corporationService, ICourseService courseService, ISectionService sectionService, IOptimizationProcessingService optimizationProcessingService)
    {
        _permissionService = permissionService;
        _notificationService = notificationService;
        _localizationService = localizationService;
        _corporationService = corporationService;
        _courseService = courseService;
        _sectionService = sectionService;
        _optimizationProcessingService = optimizationProcessingService;
    }

    #endregion
    
    public async Task<IActionResult> Index()
    {
        var sections = await _sectionService.GetAllSectionsAsync();
        
        var classrooms = await _corporationService.GetAllClassroomsAsync();

        var model = new OptimizationOverviewModel
        {
            SectionCount = sections.Count,
            ClassroomCount = classrooms.Count,
            IsReadyForOptimization = sections.Count > 0 && classrooms.Count > 0
        };

        return View(model);
    }

    public async Task<IActionResult> ClearData()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            return AccessDeniedView();

        await _corporationService.DeleteAllClassroomsAsync();
        
        await _sectionService.DeleteAllSectionsAsync();
        await _courseService.DeleteAllCoursesAsync();
        await _corporationService.DeleteAllEducationalDepartmentsAsync();
        await _corporationService.DeleteAllFacultiesAsync();

        _notificationService.SuccessNotification($"All data has been cleared successfully.");
        
        return RedirectToAction("Index");
        
    }

}