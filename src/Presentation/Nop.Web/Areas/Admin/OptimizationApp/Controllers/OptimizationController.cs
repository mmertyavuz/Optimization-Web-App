using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core.Domain;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.OptimizationApp;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.OptimizationApp.Models;
using Nop.Web.Models.Optimization;

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
            IsReadyForOptimization = sections.Count > 0 && classrooms.Count > 0,
            IsOptimized = _optimizationProcessingService.IsOptimized(),
        };

        return View(model);
    }

    public async Task<IActionResult> ClearData()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            return AccessDeniedView();

        await _optimizationProcessingService.DeleteAllOptimizationResultsAsync();
        
        await _corporationService.DeleteAllClassroomsAsync();
        
        await _sectionService.DeleteAllSectionsAsync();
        await _courseService.DeleteAllCoursesAsync();
        await _corporationService.DeleteAllEducationalDepartmentsAsync();
        await _corporationService.DeleteAllFacultiesAsync();

        _notificationService.SuccessNotification($"All data has been cleared successfully.");
        
        return RedirectToAction("Index");
        
    }

    public async Task<IActionResult> GetOptimizationData()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            return AccessDeniedView();
        
        var model = new OptimizationModel();

        var classrooms = await _corporationService.GetAllClassroomsAsync();
        var courses = await _courseService.GetAllCoursesAsync();
        
        foreach (var classroom in classrooms)
        {
            var classroomModel = new ClassroomOptimizationModel()
            {
                Id = classroom.Id,
                Name = classroom.Name,
                Capacity = classroom.Capacity
            };

            model.Classrooms.Add(classroomModel);
        }

        var sections = await _sectionService.GetAllSectionsAsync();
        foreach (var section in sections)
        {
            var sectionModel = new SectionOptimizationModel()
            {
                Id = section.Id,
                Day = (int)section.Day,
                StartTime = section.StartTime,
                EndTime = section.EndTime,
                StudentCount = section.StudentCount
            };
            
            var course = courses.FirstOrDefault(x => x.Id == section.CourseId);
            if (course is not null)
            { 
                sectionModel.CourseCode = course.Code;
            }

            model.Sections.Add(sectionModel);
        }

        return Json(model);
    }

    public async Task<IActionResult> OptimizeFromJson()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            return AccessDeniedView();

        var model = new OptimizationJsonDataModel();

        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> OptimizeFromJson(OptimizationJsonDataModel model)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            return AccessDeniedView();
        
        var errorList = new List<string>();
        var optimizedList = new List<OptimizationResult>();
        
        if (ModelState.IsValid)
        {
            try
            {
                var sectionClassrooms = JsonConvert.DeserializeObject<List<OptimizationDataModel>>(model.JsonValue);
                
                var allSections = await _sectionService.GetAllSectionsAsync();
                var allClassrooms = await _corporationService.GetAllClassroomsAsync();
                
                if (sectionClassrooms != null)
                    foreach (var dataModel in sectionClassrooms)
                    {
                        var section = allSections.FirstOrDefault(x => x.Id == dataModel.SectionId);
                        var classRoom = allClassrooms.FirstOrDefault(x => x.Id == dataModel.ClassroomId);

                        if (section is null)
                        {
                            errorList.Add($"Section with id {dataModel.SectionId} not found. Object: {JsonConvert.SerializeObject(dataModel)}");
                        }
                        else if (classRoom is null)
                        {
                            errorList.Add($"Classroom with id {dataModel.ClassroomId} not found. Object: {JsonConvert.SerializeObject(dataModel)}");
                        }
                        else
                        {
                            var optimizationData = new OptimizationResult()
                            {
                                SectionId = dataModel.SectionId,
                                ClassroomId = dataModel.ClassroomId
                            };

                            if (optimizedList.Any(x => x.ClassroomId == dataModel.ClassroomId && x.SectionId == dataModel.SectionId))
                            {
                                errorList.Add($"Duplicate data found. Object: {JsonConvert.SerializeObject(dataModel)}");
                            }
                            else
                            {
                                optimizedList.Add(optimizationData);
                            }
                            
                            await _optimizationProcessingService.InsertOptimizationDataAsync(optimizationData);
                        }
                    }

            }
            catch (JsonException ex)
            {
                ModelState.AddModelError("", "Unvalid JSON format: " + ex.Message);
            }
        }
        
        _notificationService.SuccessNotification("Optimization data has been saved successfully.");
        
        return RedirectToAction("List", $"OptimizationResult");
    }

}