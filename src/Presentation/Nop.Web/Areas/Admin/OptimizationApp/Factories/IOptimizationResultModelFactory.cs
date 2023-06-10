using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain;
using Nop.Services;
using Nop.Services.Helpers;
using Nop.Services.OptimizationApp;
using Nop.Web.Areas.Admin.OptimizationApp.Models;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

public interface IOptimizationResultModelFactory
{
    Task<OptimizationResultModel> PrepareOptimizationResultModelAsync(OptimizationResultModel model, OptimizationResult optimizationResult);
    
    Task<OptimizationResultListModel> PrepareOptimizationResultListModelAsync(OptimizationResultSearchModel searchModel);
    
    Task<OptimizationResultSearchModel> PrepareOptimizationResultSearchModelAsync(OptimizationResultSearchModel searchModel);
}

public class OptimizationResultModelFactory : IOptimizationResultModelFactory
{
    #region Fields

    private readonly IFacultyModelFactory _facultyModelFactory;
    private readonly ICorporationService _corporationService;
    private readonly IClassroomModelFactory _classroomModelFactory;
    private readonly ISectionService _sectionService;
    private readonly ISectionModelFactory _sectionModelFactory;
    private readonly ICourseModelFactory _courseModelFactory;
    private readonly ICourseService _courseService;
    private readonly IEducationalDepartmentModelFactory _educationalDepartmentModelFactory;
    private readonly IOptimizationResultService _optimizationResultService;
    
    #endregion

    #region Ctor

    public OptimizationResultModelFactory(IFacultyModelFactory facultyModelFactory, ICorporationService corporationService, IClassroomModelFactory classroomModelFactory, ISectionService sectionService, ISectionModelFactory sectionModelFactory, ICourseModelFactory courseModelFactory, ICourseService courseService, IEducationalDepartmentModelFactory educationalDepartmentModelFactory, IOptimizationResultService optimizationResultService)
    {
        _facultyModelFactory = facultyModelFactory;
        _corporationService = corporationService;
        _classroomModelFactory = classroomModelFactory;
        _sectionService = sectionService;
        _sectionModelFactory = sectionModelFactory;
        _courseModelFactory = courseModelFactory;
        _courseService = courseService;
        _educationalDepartmentModelFactory = educationalDepartmentModelFactory;
        _optimizationResultService = optimizationResultService;
    }
    
    public async Task<OptimizationResultModel> PrepareOptimizationResultModelAsync(OptimizationResultModel model,
        OptimizationResult optimizationResult)
    {
        if (model is null)
            throw new ArgumentNullException(nameof(model));
        
        if (optimizationResult is null)
            throw new ArgumentNullException(nameof(optimizationResult));
        
        model.Id = optimizationResult.Id;
        model.ClassroomId = optimizationResult.ClassroomId;
        model.SectionId = optimizationResult.SectionId;

        #region Classroom

        var classroom = await _corporationService.GetClassroomByIdAsync(optimizationResult.ClassroomId);

        if (classroom is not null)
        {
            model.Classroom = classroom.Name;
            model.ClassroomModel = await _classroomModelFactory.PrepareClassroomModelAsync(null, classroom);
        }

        #endregion

        #region Section

        var section = await _sectionService.GetSectionByIdAsync(optimizationResult.SectionId);

        if (section is not null)
        {
            model.SectionModel = await _sectionModelFactory.PrepareSectionModelAsync(null, section);

            model.StartTime = section.StartTime;
            model.EndTime = section.EndTime;
            model.StudentCount = section.StudentCount;
            model.Day = TurkishDayConverter.ConvertToTurkishDay(section.Day);
            
            #region Course

            var course = await _courseService.GetCourseByIdAsync(section.CourseId);

            if (course is not null)
            {
                model.CourseModel = await _courseModelFactory.PrepareCourseModelAsync(null, course);
                model.CourseCode = course.Code;
                model.CourseName = course.Name;
                
                
                var department =
                    await _corporationService.GetEducationalDepartmentByIdAsync(course.EducationalDepartmentId);

                if (department is not null)
                {
                    model.EducationalDepartmentModel = await _educationalDepartmentModelFactory.PrepareEducationalDepartmentModelAsync(
                        null, department);

                    var faculty = await _corporationService.GetFacultyByIdAsync(department.FacultyId);

                    if (faculty is not null)
                    {
                        model.FacultyModel = await _facultyModelFactory.PrepareFacultyModelAsync(null, faculty);
                    }
                }
            }

            #endregion
        }
        
        #endregion
        
        return model;
    }

    public async Task<OptimizationResultListModel> PrepareOptimizationResultListModelAsync(
        OptimizationResultSearchModel searchModel)
    {
        if (searchModel is null)
        {
            throw new ArgumentNullException(nameof(searchModel));
        }

        var optimizationResultTemp = await _optimizationResultService.GetAllOptimizedSectionsAsync(
            FacultyId: searchModel.FacultyId,
            EducationalDepartmentId : searchModel.EducationalDepartmentId,
            ClassroomId : searchModel.ClassroomId,
            CourseId : searchModel.CourseId,
            DayId : searchModel.DayId,
            MaxStudentCount: searchModel.MaxStudentCount,
            MinStudentCount: searchModel.MinStudentCount,
            StartDate : searchModel.StartDate,
            EndDate : searchModel.EndDate,
            pageIndex: searchModel.Page - 1,
            pageSize: searchModel.PageSize);
        
        //prepare grid model
        var model = await new OptimizationResultListModel().PrepareToGridAsync(searchModel, optimizationResultTemp, () =>
        {
            return optimizationResultTemp.SelectAwait(async optimizationResult =>
            {
                //fill in model values from the entity
                var optimizationResultModel = new OptimizationResultModel();

                await PrepareOptimizationResultModelAsync(optimizationResultModel, optimizationResult);

                return optimizationResultModel;
            });
        });

        return model;
    }

    public async Task<OptimizationResultSearchModel> PrepareOptimizationResultSearchModelAsync(
        OptimizationResultSearchModel searchModel)
    {
        if (searchModel is null)
            throw new ArgumentNullException(nameof(searchModel));

        #region Available Days

        var days = await DayOfWeek.Monday.ToSelectListAsync();
        
        foreach (var day in days)
        {
            if (day.Value == "0")
                continue;
            
            searchModel.AvailableDays.Add(day);
        }

        AddSelectListAllItem(searchModel.AvailableDays, -1);

        #endregion

        #region Available Faculties

        var faculties = await _corporationService.GetAllFacultiesAsync();
        
        foreach (var faculty in faculties)
        {
            searchModel.AvailableFaculties.Add(new SelectListItem
            {
                Text = faculty.Name,
                Value = faculty.Id.ToString()
            });
        }
        
        AddSelectListAllItem(searchModel.AvailableFaculties);

        #endregion
        
        #region Available Educational Departments
        
        var departments = await _corporationService.GetAllEducationalDepartmentsAsync();
        
        foreach (var department in departments)
        {
            searchModel.AvailableEducationalDepartments.Add(new SelectListItem
            {
                Text = department.Name,
                Value = department.Id.ToString()
            });
        }
        
        AddSelectListAllItem(searchModel.AvailableEducationalDepartments);
        
        #endregion
        
        #region Available Courses
        
        var courses = await _courseService.GetAllCoursesAsync();
        
        foreach (var course in courses)
        {
            searchModel.AvailableCourses.Add(new SelectListItem
            {
                Text = $"({course.Code}) {course.Name}",
                Value = course.Id.ToString()
            });
        }
        
        AddSelectListAllItem(searchModel.AvailableCourses);
        
        #endregion

        #region Available Classrooms

        var classrooms = await _corporationService.GetAllClassroomsAsync();
        
        foreach (var classroom in classrooms)
        {
            searchModel.AvailableClassrooms.Add(new SelectListItem
            {
                Text = classroom.Name,
                Value = classroom.Id.ToString()
            });
        }
        
        AddSelectListAllItem(searchModel.AvailableClassrooms);
        
        #endregion
        
        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }
    
    #endregion
    
    private void AddSelectListAllItem(IList<SelectListItem> selectList, int value = 0)
    {
        selectList.Insert(0, new SelectListItem { Text = "All", Value = value.ToString() });
    }
    
}