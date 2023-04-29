using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.OptimizationApp;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Corporations;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public class FacultyController : BaseAdminController
{
    #region Fields

    private readonly IFacultyModelFactory _facultyModelFactory;
    private readonly ICorporationService _corporationService;
    private readonly IPermissionService _permissionService;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;

    #endregion

    #region Ctor

    public FacultyController(IFacultyModelFactory facultyModelFactory, ICorporationService corporationService,
        IPermissionService permissionService, INotificationService notificationService,
        ILocalizationService localizationService)
    {
        _facultyModelFactory = facultyModelFactory;
        _corporationService = corporationService;
        _permissionService = permissionService;
        _notificationService = notificationService;
        _localizationService = localizationService;
    }

    #endregion

    #region List

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    public async Task<IActionResult> List()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageFaculties))
            return AccessDeniedView();

        //prepare model
        var model = _facultyModelFactory.PrepareFacultySearchModel(new FacultySearchModel());

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> List(FacultySearchModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageFaculties))
            return await AccessDeniedDataTablesJson();

        //prepare model
        var model = await _facultyModelFactory.PrepareFacultyListModelAsync(searchModel);

        return Json(model);
    }

    #endregion

    #region Create / Edit / Delete

    public virtual async Task<IActionResult> Create()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageFaculties))
            return AccessDeniedView();

        //prepare model
        var model = await _facultyModelFactory.PrepareFacultyModelAsync(new FacultyModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public virtual async Task<IActionResult> Create(FacultyModel model, bool continueEditing)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageFaculties))
            return AccessDeniedView();

        if (ModelState.IsValid)
        {
            var faculty = model.ToEntity<Faculty>();

            await _corporationService.InsertFacultyAsync(faculty);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Corporations.Faculties.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = faculty.Id });
        }

        //prepare model
        model = await _facultyModelFactory.PrepareFacultyModelAsync(model, null, true);

        return View(model);

    }
    
    public virtual async Task<IActionResult> Edit(int id)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageFaculties))
            return AccessDeniedView();

        //try to get a faculty with the specified id
        var faculty = await _corporationService.GetFacultyByIdAsync(id);
        
        if (faculty == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _facultyModelFactory.PrepareFacultyModelAsync(null, faculty);

        return View(model);
    }
    
    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public virtual async Task<IActionResult> Edit(FacultyModel model, bool continueEditing)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageFaculties))
            return AccessDeniedView();

        //try to get a faculty with the specified id
        var faculty = await _corporationService.GetFacultyByIdAsync(model.Id);
        if (faculty == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            faculty = model.ToEntity(faculty);
            await _corporationService.UpdateFacultyAsync(faculty);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Corporations.Faculties.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");
            
            return RedirectToAction("Edit", new { id = faculty.Id });
        }

        //prepare model
        model = await _facultyModelFactory.PrepareFacultyModelAsync(model, faculty, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }
    
    [HttpPost]
    public virtual async Task<IActionResult> Delete(int id)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageFaculties))
            return AccessDeniedView();

        //try to get a faculty with the specified id
        var faculty = await _corporationService.GetFacultyByIdAsync(id);
        if (faculty == null)
            return RedirectToAction("List");

        await _corporationService.DeleteFacultyAsync(faculty);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Corporations.Faculties.Deleted"));

        return RedirectToAction("List");
    }
    
    #endregion
}