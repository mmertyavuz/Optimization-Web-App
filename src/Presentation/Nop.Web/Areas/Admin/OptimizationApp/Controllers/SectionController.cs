using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.OptimizationApp;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Corporations;
using Nop.Web.Areas.Admin.Models.Education;

namespace Nop.Web.Areas.Admin.Controllers;

public class SectionController : BaseAdminController
{
    #region Fields

    private readonly ISectionService _sectionService;
    private readonly ISectionModelFactory _sectionModelFactory;
    private readonly IPermissionService _permissionService;

    #endregion

    #region Ctor

    public SectionController(ISectionService sectionService, ISectionModelFactory sectionModelFactory, IPermissionService permissionService)
    {
        _sectionService = sectionService;
        _sectionModelFactory = sectionModelFactory;
        _permissionService = permissionService;
    }

    #endregion

    #region List

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }
    
    public async Task<IActionResult> List()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return AccessDeniedView();

        //prepare model
        var model = _sectionModelFactory.PrepareSectionSearchModelAsync(new SectionSearchModel());

        return View(model);
    }
    
    [HttpPost]
    public virtual async Task<IActionResult> List(SectionSearchModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return await AccessDeniedDataTablesJson();

        //prepare model
        var model = await _sectionModelFactory.PrepareSectionListModelAsync(searchModel);

        return Json(model);
    }

    #endregion
    
    public virtual async Task<IActionResult> Details(int id)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return AccessDeniedView();

        //try to get a classroom with the specified id
        var section = await _sectionService.GetSectionByIdAsync(id);
        
        if (section == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _sectionModelFactory.PrepareSectionModelAsync(null, section);

        return View(model);
    }
}