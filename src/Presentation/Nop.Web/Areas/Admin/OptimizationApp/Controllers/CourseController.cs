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
using Nop.Web.Areas.Admin.Models.Education;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public class CourseController : BaseAdminController
{
    #region Fields

    private readonly IPermissionService _permissionService;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;
    private readonly ICourseService _courseService;
    private readonly ICourseModelFactory _courseModelFactory;
    private readonly ISectionModelFactory _sectionModelFactory;
    private readonly ISectionService _sectionService;
    private readonly IImportManager _importManager;
    private readonly IExportManager _exportManager;
    private readonly CorporationSettings _corporationSettings;

    #endregion

    #region Ctor

    public CourseController(IPermissionService permissionService, INotificationService notificationService, ILocalizationService localizationService, ICourseService courseService, ICourseModelFactory courseModelFactory, ISectionModelFactory sectionModelFactory, ISectionService sectionService, IImportManager importManager, IExportManager exportManager, CorporationSettings corporationSettings)
    {
        _permissionService = permissionService;
        _notificationService = notificationService;
        _localizationService = localizationService;
        _courseService = courseService;
        _courseModelFactory = courseModelFactory;
        _sectionModelFactory = sectionModelFactory;
        _sectionService = sectionService;
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
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageClassrooms))
            return AccessDeniedView();

        //prepare model
        var model = await _courseModelFactory.PrepareCourseSearchModelAsync(new CourseSearchModel());

        return View(model);
    }
    
    [HttpPost]
    public virtual async Task<IActionResult> List(CourseSearchModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return await AccessDeniedDataTablesJson();

        //prepare model
        var model = await _courseModelFactory.PrepareCourseListModelAsync(searchModel);

        return Json(model);
    } 

    #endregion

    public virtual async Task<IActionResult> Create()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return AccessDeniedView();

        //prepare model
        var model = await _courseModelFactory.PrepareCourseModelAsync(new CourseModel(), null);

        return View(model);
    }
    
    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public virtual async Task<IActionResult> Create(CourseModel model, bool continueEditing)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return AccessDeniedView();

        if (ModelState.IsValid)
        {
            var course = model.ToEntity<Course>();

            await _courseService.InsertCourseAsync(course);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Courses.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = course.Id });
        }

        //prepare model
        model = await _courseModelFactory.PrepareCourseModelAsync(model, null, true);

        return View(model);
    }

    public virtual async Task<IActionResult> Edit(int id)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return AccessDeniedView();

        //try to get a course with the specified id
        var course = await _courseService.GetCourseByIdAsync(id);
        
        if (course == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _courseModelFactory.PrepareCourseModelAsync(null, course);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public virtual async Task<IActionResult> Edit(CourseModel model, bool continueEditing)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return AccessDeniedView();

        //try to get a course with the specified id
        var course = await _courseService.GetCourseByIdAsync(model.Id);
        if (course == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            course = model.ToEntity(course);
            await _courseService.UpdateCourseAsync(course);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Courses.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = course.Id });
        }

        //prepare model
        model = await _courseModelFactory.PrepareCourseModelAsync(model, course, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Delete(int id)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return AccessDeniedView();

        //try to get a course with the specified id
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null)
            return RedirectToAction("List");

        await _courseService.DeleteCourseAsync(course);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Courses.Deleted"));

        return RedirectToAction("List");
    }

    #region Sections

    [HttpPost]
    public virtual async Task<IActionResult> SectionList(SectionSearchModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return await AccessDeniedDataTablesJson();

        var model = await _sectionModelFactory.PrepareSectionListModelAsync(searchModel);

        return Json(model);
    }

    public virtual async Task<IActionResult> SectionCreatePopup()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return AccessDeniedView();

        var model = await _sectionModelFactory.PrepareSectionModelAsync(new SectionModel(), null);

        return View(model);
    }
    
    [HttpPost]
    public virtual async Task<IActionResult> SectionCreatePopup(SectionModel model)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return AccessDeniedView();

        if (model.CourseId == 0)
            throw new ArgumentNullException(nameof(model.CourseId));
        
        if (ModelState.IsValid)
        {
            //fill entity from model
            var section = model.ToEntity<Section>();

            await _sectionService.InsertSectionAsync(section);
            
            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _sectionModelFactory.PrepareSectionModelAsync(model, null);

        //if we got this far, something failed, redisplay form
        return View(model);
    }
    
    public virtual async Task<IActionResult> SectionEditPopup(int id)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return AccessDeniedView();

        var section = await _sectionService.GetSectionByIdAsync(id)
            ?? throw new ArgumentException("No section found with the specified id");

        var model = await _sectionModelFactory.PrepareSectionModelAsync(null, section); 

        return View(model);
    }
    
    [HttpPost]
    public virtual async Task<IActionResult> SectionEditPopup(SectionModel model)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return AccessDeniedView();

        //try to get a section with the specified id
        var section = await _sectionService.GetSectionByIdAsync(model.Id)
                      ?? throw new ArgumentException("No section found with the specified id");

        if (model.CourseId == 0)
        {
            throw new ArgumentNullException(nameof(model.CourseId));
        }

        if (ModelState.IsValid)
        {
            section = model.ToEntity(section);
            await _sectionService.UpdateSectionAsync(section);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _sectionModelFactory.PrepareSectionModelAsync(model, section, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }
    
    [HttpPost]
    public virtual async Task<IActionResult> SectionDelete(int id)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return AccessDeniedView();

        //try to get a section with the specified id
        var section = await _sectionService.GetSectionByIdAsync(id)
                      ?? throw new ArgumentException("No section found with the specified id", nameof(id));

        await _sectionService.DeleteSectionAsync(section);

        return new NullJsonResult();
    }

    public virtual async Task<IActionResult> ExportExcel()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return AccessDeniedView();

        try
        {
            var bytes = await _exportManager
                .ExportCoursesAndSectionsToExcel((await _sectionService.GetAllSectionsAsync()).ToList());

            var fileName = _corporationSettings.CorporationName + " courses.xlsx";
            
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
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return AccessDeniedView();

        try
        {
            var bytes = await _exportManager
                .ExportCoursesAndSectionsToExcel();

            var fileName = _corporationSettings.CorporationName + " sample course import file.xlsx";
            
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
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return AccessDeniedView();

        var courses = await _courseService.GetAllCoursesAsync();
        var sections = await _sectionService.GetAllSectionsAsync();

        if (courses.Any() || sections.Any())
        {
            _notificationService.ErrorNotification("You can not import courses and sections from excel file because there are already courses or sections in the system. Please reset the optimization process.");
            return RedirectToAction("List");
        }
        
        try
        {
            if (importexcelfile is {Length: > 0})
            {
                await _importManager.ImportCoursesAndSectionsFromExcelAsync(importexcelfile.OpenReadStream());
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
    
    [HttpPost]
    public virtual async Task<IActionResult> DeleteAll()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCourses))
            return AccessDeniedView();


        var courses = await _courseService.GetAllCoursesAsync();
        var sections = await _sectionService.GetAllSectionsAsync();

        foreach (var section in sections)
        {
            await _sectionService.DeleteSectionAsync(section);
        }

        foreach (var course in courses)
        {
            await _courseService.DeleteCourseAsync(course);
        }
        
        return RedirectToAction("List");
    }

    #endregion

}