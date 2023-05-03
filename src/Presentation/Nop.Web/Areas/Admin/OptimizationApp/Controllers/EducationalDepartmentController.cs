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

public class EducationalDepartmentController : BaseAdminController
{
    #region Fields
    
    private readonly IEducationalDepartmentModelFactory _educationalDepartmentFactory;
    private readonly ICorporationService _corporationService;
    private readonly IPermissionService _permissionService;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;

    #endregion

    #region Ctor

    public EducationalDepartmentController(IEducationalDepartmentModelFactory educationalDepartmentFactory, ICorporationService corporationService, IPermissionService permissionService, INotificationService notificationService, ILocalizationService localizationService)
    {
        _educationalDepartmentFactory = educationalDepartmentFactory;
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
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageEducationalDepartments))
            return AccessDeniedView();

        //prepare model
        var model = await _educationalDepartmentFactory.PrepareEducationalDepartmentSearchModelAsync(new EducationalDepartmentSearchModel());

        return View(model);
    }
    [HttpPost]
    public virtual async Task<IActionResult> List(EducationalDepartmentSearchModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageEducationalDepartments))
            return await AccessDeniedDataTablesJson();

        //prepare model
        var model = await _educationalDepartmentFactory.PrepareEducationalDepartmentListModelAsync(searchModel);

        return Json(model);
    }

    #endregion

    #region Create / Edit / Delete

    public virtual async Task<IActionResult> Create()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageEducationalDepartments))
            return AccessDeniedView();

        //prepare model
        var model = await _educationalDepartmentFactory.PrepareEducationalDepartmentModelAsync(new EducationalDepartmentModel(), null);

        return View(model);
    }
    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public virtual async Task<IActionResult> Create(EducationalDepartmentModel model, bool continueEditing)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageEducationalDepartments))
            return AccessDeniedView();

        if (ModelState.IsValid)
        {
            var educationalDepartment = model.ToEntity<EducationalDepartment>();

            await _corporationService.InsertEducationalDepartmentAsync(educationalDepartment);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Corporations.EducationalDepartments.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = educationalDepartment.Id });
        }

        //prepare model
        model = await _educationalDepartmentFactory.PrepareEducationalDepartmentModelAsync(model, null, true);

        return View(model);

    }
    public virtual async Task<IActionResult> Edit(int id)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageEducationalDepartments))
            return AccessDeniedView();

        //try to get a educational departments with the specified id
        var educationalDepartment = await _corporationService.GetEducationalDepartmentByIdAsync(id);
        
        if (educationalDepartment == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _educationalDepartmentFactory.PrepareEducationalDepartmentModelAsync(null, educationalDepartment);

        return View(model);
    }
    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public virtual async Task<IActionResult> Edit(EducationalDepartmentModel model, bool continueEditing)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageEducationalDepartments))
            return AccessDeniedView();

        //try to get a educational department with the specified id
        var educationalDepartment = await _corporationService.GetEducationalDepartmentByIdAsync(model.Id);
        if (educationalDepartment == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            educationalDepartment = model.ToEntity(educationalDepartment);
            await _corporationService.UpdateEducationalDepartmentAsync(educationalDepartment);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Corporations.EducationalDepartments.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");
            
            return RedirectToAction("Edit", new { id = educationalDepartment.Id });
        }

        //prepare model
        model = await _educationalDepartmentFactory.PrepareEducationalDepartmentModelAsync(model, educationalDepartment, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }
    [HttpPost]
    public virtual async Task<IActionResult> Delete(int id)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageEducationalDepartments))
            return AccessDeniedView();

        //try to get a faculty with the specified id
        var educationalDepartment = await _corporationService.GetEducationalDepartmentByIdAsync(id);
        if (educationalDepartment == null)
            return RedirectToAction("List");

        await _corporationService.DeleteEducationalDepartmentAsync(educationalDepartment);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Corporations.EducationalDepartments.Deleted"));

        return RedirectToAction("List");
    }

    #endregion
}