using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core.Domain;
using Nop.Services.OptimizationApp;
using Nop.Web.Areas.Admin.OptimizationApp.Models;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Optimization;

namespace Nop.Web.Controllers;

public class ApiController : BasePublicController
{
    #region Fields

    private readonly ICorporationService _corporationService;
    private readonly ISectionService _sectionService;
    private readonly ICourseService _courseService;
    private readonly IOptimizationProcessingService _optimizationProcessingService;

    public ApiController(ICorporationService corporationService, ISectionService sectionService, ICourseService courseService, IOptimizationProcessingService optimizationProcessingService)
    {
        _corporationService = corporationService;
        _sectionService = sectionService;
        _courseService = courseService;
        _optimizationProcessingService = optimizationProcessingService;
    }

    #endregion
    
    public class OptimizationRequest
    {
        public string OptimizationKey { get; set; }
    }

    [CheckAccessPublicStore(ignore: true)]
    [HttpPost]
    public virtual async Task<IActionResult> GetOptimizationData()
    {
        // var isKeyValid = _corporationService.IsOptimizationKeyValid(request.OptimizationKey);
        //
        // if (!isKeyValid)
        //     return BadRequest("Optimization key is not valid.");

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
                StudentCount = section.StudentCount,
            };

            var course = courses.FirstOrDefault(x => x.Id == section.CourseId);
            if (course is not null)
            { 
                sectionModel.CourseCode = course.Code;
            }

            model.Sections.Add(sectionModel);
        }

        return Ok(model);
    }


    [CheckAccessPublicStore(ignore: true)]
    [HttpPost]
    public virtual async Task<IActionResult> SetOptimizedData([FromBody] IList<OptimizationDataModel> items)
    {
        if (!items.Any())
        {
            return BadRequest("No items to save.");
        }
        
        var errorList = new List<string>();
        var optimizedList = new List<OptimizationResult>();

         var allSections = await _sectionService.GetAllSectionsAsync();
         var allClassrooms = await _corporationService.GetAllClassroomsAsync();

           
         foreach (var dataModel in items)
         {
             var section = allSections.FirstOrDefault(x => x.Id == dataModel.SectionId);
             var classRoom = allClassrooms.FirstOrDefault(x => x.Id == dataModel.ClassroomId);

             if (section is null)
             {
                 errorList.Add(
                     $"Section with id {dataModel.SectionId} not found. Object: {JsonConvert.SerializeObject(dataModel)}");
             }
             else if (classRoom is null)
             {
                 errorList.Add(
                     $"Classroom with id {dataModel.ClassroomId} not found. Object: {JsonConvert.SerializeObject(dataModel)}");
             }
             else
             {
                 var optimizationData = new OptimizationResult()
                 {
                     SectionId = dataModel.SectionId,
                     ClassroomId = dataModel.ClassroomId
                 };

                 if (optimizedList.Any(x =>
                         x.ClassroomId == dataModel.ClassroomId && x.SectionId == dataModel.SectionId))
                 {
                     errorList.Add($"Duplicate data found. Object: {JsonConvert.SerializeObject(dataModel)}");
                 }
                 else
                 {
                     optimizedList.Add(optimizationData);
                     await _optimizationProcessingService.InsertOptimizationDataAsync(optimizationData);
                 }
             }
         }
     
        return Ok(errorList);
    }
}