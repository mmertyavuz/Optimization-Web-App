using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.OptimizationApp;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Corporations;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public class CorporationController : BaseAdminController
{
    #region Fields

    private readonly ICorporationModelFactory _corporationModelFactory;
    private readonly IPermissionService _permissionService;
    private readonly CorporationSettings _corporationSettings;
    private readonly ISettingService _settingService;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;
    private readonly ICorporationService _corporationService;
    private readonly ICourseService _courseService;

    #endregion

    #region Ctor

    public CorporationController(ICorporationModelFactory corporationModelFactory, IPermissionService permissionService, CorporationSettings corporationSettings, ISettingService settingService, INotificationService notificationService, ILocalizationService localizationService, ICorporationService corporationService, ICourseService courseService)
    {
        _corporationModelFactory = corporationModelFactory;
        _permissionService = permissionService;
        _corporationSettings = corporationSettings;
        _settingService = settingService;
        _notificationService = notificationService;
        _localizationService = localizationService;
        _corporationService = corporationService;
        _courseService = courseService;
    }

    #endregion

    public IActionResult Index()
    {
        return RedirectToAction(nameof(Details));
    }
    public virtual async Task<IActionResult> Details()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCorporations))
            return AccessDeniedView();

        //prepare model
        var model = _corporationModelFactory.PrepareCorporationSettingsModel(new CorporationSettingsModel());

        return View(model);
    }
    
    [HttpPost]
    public virtual async Task<IActionResult> Details(CorporationSettingsModel model, bool continueEditing)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCorporations))
            return AccessDeniedView();
        
        _corporationSettings.CorporationName = model.CorporationName;
        _corporationSettings.CorporationWebsite = model.CorporationWebsite;
        _corporationSettings.LogoUrl = model.LogoUrl;
        _corporationSettings.MiniLogoUrl = model.MiniLogoUrl;
        _corporationSettings.CorporationEmailSuffix = model.CorporationEmailSuffix;
       
        _settingService.SaveSettingAsync(_corporationSettings, settings => settings.CorporationName).Wait();
        _settingService.SaveSettingAsync(_corporationSettings, settings => settings.CorporationWebsite).Wait();
        _settingService.SaveSettingAsync(_corporationSettings, settings => settings.LogoUrl).Wait();
        _settingService.SaveSettingAsync(_corporationSettings, settings => settings.MiniLogoUrl).Wait();
        _settingService.SaveSettingAsync(_corporationSettings, settings => settings.CorporationEmailSuffix).Wait();
       
        
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Corporations.Updated"));

        return RedirectToAction("Details");
    }

    #region Utilities

    public virtual async Task<IActionResult> GetDepartmentsByCorporationId(string facultyId)
        {
            if (string.IsNullOrEmpty(facultyId))
                throw new ArgumentNullException(nameof(facultyId));

            var faculty = await _corporationService.GetFacultyByIdAsync(Convert.ToInt32(facultyId));

            var departments = await _corporationService.GetAllEducationalDepartmentsAsync();

            if (faculty is not null)
            {
                departments = departments.Where(x => x.FacultyId == faculty.Id).ToList();
            }
            
            var result = (from s in departments
                          select new { id = s.Id, name = s.Name }).ToList();

            result.Insert(0, new { id = 0, name = $"All" });
            
            return Json(result);
        }
    
    public virtual async Task<IActionResult> GetCoursesByDepartmentId(string departmentId)
    {
        if (string.IsNullOrEmpty(departmentId))
            throw new ArgumentNullException(nameof(departmentId));

        var department = await _corporationService.GetEducationalDepartmentByIdAsync(Convert.ToInt32(departmentId));

        var courses = await _courseService.GetAllCoursesAsync();

        if (department is not null)
        {
            courses = courses.Where(x => x.EducationalDepartmentId == department.Id).ToList();
        }
            
        var result = (from s in courses
            select new { id = s.Id, name = s.Name }).ToList();
        
        result.Insert(0, new { id = 0, name = $"All" });

        return Json(result);
    }
    
    public virtual async Task<IActionResult> GetCoursesByFacultyId(string facultyId)
    {
        if (string.IsNullOrEmpty(facultyId))
            throw new ArgumentNullException(nameof(facultyId));

        var faculty = await _corporationService.GetFacultyByIdAsync(Convert.ToInt32(facultyId));
        var departments = await _corporationService.GetAllEducationalDepartmentsAsync();
        
        var courses = await _courseService.GetAllCoursesAsync();

        if (faculty is not null)
        {
            courses = courses.Where(x =>
                departments.FirstOrDefault(d => d.Id == x.EducationalDepartmentId)?.FacultyId == faculty.Id).ToList();
        }
            
        var result = (from s in courses
            select new { id = s.Id, name = s.Name }).ToList();
        
        result.Insert(0, new { id = 0, name = $"All" });

        return Json(result);
    }
    
    #endregion
}