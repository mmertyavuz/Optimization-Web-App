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

public class ClassroomController : BaseAdminController
{
    #region Fields

    private readonly IClassroomModelFactory _classroomModelFactory;
    private readonly ICorporationService _corporationService;
    private readonly IPermissionService _permissionService;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;
    private readonly IExportManager _exportManager;
    private readonly CorporationSettings _corporationSettings;
    private readonly IImportManager _importManager;
    
    #endregion

    #region Ctor

    public ClassroomController(
        IClassroomModelFactory classroomModelFactory,
        ICorporationService corporationService,
        IPermissionService permissionService, INotificationService notificationService, ILocalizationService localizationService, IExportManager exportManager, CorporationSettings corporationSettings, IImportManager importManager)
    {
        _classroomModelFactory = classroomModelFactory;
        _corporationService = corporationService;
        _permissionService = permissionService;
        _notificationService = notificationService;
        _localizationService = localizationService;
        _exportManager = exportManager;
        _corporationSettings = corporationSettings;
        _importManager = importManager;
    }

    #endregion

    #region List

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }
    
    public async Task<IActionResult> List()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageClassrooms))
            return AccessDeniedView();

        //prepare model
        var model = _classroomModelFactory.PrepareClassroomSearchModel(new ClassroomSearchModel());

        return View(model);
    }
    
    [HttpPost]
    public virtual async Task<IActionResult> List(ClassroomSearchModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageClassrooms))
            return await AccessDeniedDataTablesJson();

        //prepare model
        var model = await _classroomModelFactory.PrepareClassroomListModelAsync(searchModel);

        return Json(model);
    }
    
    

    #endregion

    #region Create / Edit / Delete

    public virtual async Task<IActionResult> Create()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageClassrooms))
            return AccessDeniedView();

        //prepare model
        var model = await _classroomModelFactory.PrepareClassroomModelAsync(new ClassroomModel(), null);

        return View(model);
    }
    
    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public virtual async Task<IActionResult> Create(ClassroomModel model, bool continueEditing)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageClassrooms))
            return AccessDeniedView();

        if (ModelState.IsValid)
        {
            var classroom = model.ToEntity<Classroom>();

            await _corporationService.InsertClassroomAsync(classroom);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Corporations.Classrooms.Added"));

            if (!continueEditing)
                return RedirectToAction("List");
                
            return RedirectToAction("Edit", new { id = classroom.Id });
        }

        //prepare model
        model = await _classroomModelFactory.PrepareClassroomModelAsync(model, null, true);

        return View(model);
    }
    
    
    public virtual async Task<IActionResult> Edit(int id)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageClassrooms))
            return AccessDeniedView();

        //try to get a classroom with the specified id
        var classRoom = await _corporationService.GetClassroomByIdAsync(id);
        
        if (classRoom == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _classroomModelFactory.PrepareClassroomModelAsync(null, classRoom);

        return View(model);
    }
    
    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public virtual async Task<IActionResult> Edit(ClassroomModel model, bool continueEditing)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageClassrooms))
            return AccessDeniedView();

        //try to get a classroom with the specified id
        var classRoom = await _corporationService.GetClassroomByIdAsync(model.Id);
        if (classRoom == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            classRoom = model.ToEntity(classRoom);
            await _corporationService.UpdateClassroomAsync(classRoom);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Corporations.Classrooms.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");
            
            return RedirectToAction("Edit", new { id = classRoom.Id });
        }

        //prepare model
        model = await _classroomModelFactory.PrepareClassroomModelAsync(model, classRoom, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }
    
    [HttpPost]
    public virtual async Task<IActionResult> Delete(int id)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageClassrooms))
            return AccessDeniedView();

        //try to get a classroom with the specified id
        var classRoom = await _corporationService.GetClassroomByIdAsync(id);
        if (classRoom == null)
            return RedirectToAction("List");

        await _corporationService.DeleteClassroomAsync(classRoom);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Corporations.Classrooms.Deleted"));

        return RedirectToAction("List");
    }
    
    #endregion
    
    public virtual async Task<IActionResult> ExportExcel()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageClassrooms))
            return AccessDeniedView();

        try
        {
            var bytes = await _exportManager
                .ExportClassroomsToExcel((await _corporationService.GetAllClassroomsAsync()).ToList());

            var fileName = _corporationSettings.CorporationName + " classrooms.xlsx";
            
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
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageClassrooms))
            return AccessDeniedView();

        try
        {
            var bytes = await _exportManager
                .ExportClassroomsToExcel();

            var fileName = _corporationSettings.CorporationName + " sample classrooms.xlsx";
            
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
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageClassrooms))
            return AccessDeniedView();

        try
        {
            if (importexcelfile is {Length: > 0})
            {
                await _importManager.ImportClassroomsFromExcelAsync(importexcelfile.OpenReadStream());
            }
            else
            {
                _notificationService.ErrorNotification("An error occured during importing data from excel. Please try again.");
                return RedirectToAction("List");
            }

            _notificationService.SuccessNotification("Datas are successfully imported from given excel");

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }
    
}