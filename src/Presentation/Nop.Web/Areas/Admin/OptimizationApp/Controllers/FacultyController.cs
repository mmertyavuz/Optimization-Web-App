using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Services.ExportImport;
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
    private readonly IImportManager _importManager;
    private readonly IExportManager _exportManager;
    private readonly CorporationSettings _corporationSettings;

    #endregion

    #region Ctor

    public FacultyController(IFacultyModelFactory facultyModelFactory, ICorporationService corporationService,
        IPermissionService permissionService, INotificationService notificationService,
        ILocalizationService localizationService, IImportManager importManager, IExportManager exportManager, CorporationSettings corporationSettings)
    {
        _facultyModelFactory = facultyModelFactory;
        _corporationService = corporationService;
        _permissionService = permissionService;
        _notificationService = notificationService;
        _localizationService = localizationService;
        _importManager = importManager;
        _exportManager = exportManager;
        _corporationSettings = corporationSettings;
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

        var departments = await _corporationService.GetAllEducationalDepartmentsAsync(facultyId: faculty.Id);

        if (departments.Any())
        {
            _notificationService.ErrorNotification("This faculty has departments. A faculty with departments cannot be deleted. Please delete the departments first or reset the optimization process from management page.");
            
            return RedirectToAction("Edit", new { id = faculty.Id });
        }
        
        await _corporationService.DeleteFacultyAsync(faculty);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Corporations.Faculties.Deleted"));

        return RedirectToAction("List");
    }
    
    [HttpPost]
    public virtual async Task<IActionResult> DeleteAll()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageFaculties))
            return AccessDeniedView();

        var departments = await _corporationService.GetAllEducationalDepartmentsAsync();
        
        var faculties = await _corporationService.GetAllFacultiesAsync(showOnlyWithoutDepartment: true);
        
        var isAllDeleted = true;
        
        foreach (var faculty in faculties)
        {
            if (departments.Any(x => x.FacultyId == faculty.Id))
            {
                isAllDeleted = false;
            }
            else
            {
                await _corporationService.DeleteFacultyAsync(faculty);
            }
        }

        if (isAllDeleted)
        {
             _notificationService.SuccessNotification("All faculties are successfully deleted.");
        }
        else
        {
            _notificationService.WarningNotification("Some faculties are not deleted. A faculty with departments cannot be deleted. Please delete the departments first or reset the optimization process from management page.");
        }
        
        return RedirectToAction("List");
    }
    
    public virtual async Task<IActionResult> ExportExcel()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageFaculties))
            return AccessDeniedView();

        try
        {
            var bytes = await _exportManager
                .ExportFacultiesToExcel((await _corporationService.GetAllFacultiesAsync()).ToList());

            var fileName = _corporationSettings.CorporationName + " faculties.xlsx";
            
            return File(bytes, MimeTypes.TextXlsx, fileName);
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }
    
    public virtual async Task<IActionResult> DownloadSampleExcel()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageFaculties))
            return AccessDeniedView();

        try
        {
            var bytes = await _exportManager
                .ExportFacultiesToExcel();

            var fileName = _corporationSettings.CorporationName + " sample faculties.xlsx";
            
            return File(bytes, MimeTypes.TextXlsx, fileName);
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }
    
    [HttpPost]
    public virtual async Task<IActionResult> ImportFromExcel(IFormFile importexcelfile)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageFaculties))
            return AccessDeniedView();

        try
        {
            if (importexcelfile is {Length: > 0})
            {
                await _importManager.ImportFacultiesFromExcelAsync(importexcelfile.OpenReadStream());
            }
            else
            {
                _notificationService.ErrorNotification("An error occured during importing data from excel. Please try again.");
                return RedirectToAction("List");
            }

            _notificationService.SuccessNotification("Successfully imported from given excel file.");

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }
    
    #endregion
}