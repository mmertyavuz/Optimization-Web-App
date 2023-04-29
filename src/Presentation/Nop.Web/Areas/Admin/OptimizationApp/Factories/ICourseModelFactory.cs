using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain;
using Nop.Services.Localization;
using Nop.Services.OptimizationApp;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Corporations;
using Nop.Web.Areas.Admin.Models.Education;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

public interface ICourseModelFactory
{
    Task<CourseSearchModel> PrepareCourseSearchModelAsync(CourseSearchModel searchModel);

    Task<CourseListModel> PrepareCourseListModelAsync(CourseSearchModel searchModel);

    Task<CourseModel> PrepareCourseModelAsync(CourseModel model, Course course, bool excludeProperties = false);

}

public class CourseModelFactory : ICourseModelFactory
{
    #region Fields

    private readonly ICourseService _courseService;
    private readonly ICorporationService _corporationService;
    private readonly IBaseOptimizationAppModelFactory _baseOptimizationAppModelFactory;
    private readonly ILocalizationService _localizationService;

    #endregion

    #region Ctor

    public CourseModelFactory(ICourseService courseService, ICorporationService corporationService, IBaseOptimizationAppModelFactory baseOptimizationAppModelFactory, ILocalizationService localizationService)
    {
        _courseService = courseService;
        _corporationService = corporationService;
        _baseOptimizationAppModelFactory = baseOptimizationAppModelFactory;
        _localizationService = localizationService;
    }

    #endregion
    
    public async Task<CourseSearchModel> PrepareCourseSearchModelAsync(CourseSearchModel searchModel)
    {
        if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));
        
        await _baseOptimizationAppModelFactory.PrepareEducationalDepartmentsAsync(searchModel
            .AvailableEducationalDepartments);
        
        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    public async Task<CourseListModel> PrepareCourseListModelAsync(CourseSearchModel searchModel)
    {
        if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));
        
        var courses = await _courseService.GetAllCoursesAsync(
            departmentId: searchModel.EducationalDepartmentId,
            name: searchModel.Name,
            code: searchModel.Code);
        
        var pagedCourses = courses.ToPagedList(searchModel);
        
        //prepare grid model
        var model = await new CourseListModel().PrepareToGridAsync(searchModel, pagedCourses, () =>
        {
            return pagedCourses.SelectAwait(async course =>
            {
                //fill in model values from the entity
                var courseModel = course.ToModel<CourseModel>();

                var educationalDepartment =
                    await _corporationService.GetEducationalDepartmentByIdAsync(course.EducationalDepartmentId);

                if (educationalDepartment is not null)
                {
                    courseModel.EducationalDepartmentName = educationalDepartment.Name;
                }
                
                return courseModel;
            });
        });

        return model;
    }

    public async Task<CourseModel> PrepareCourseModelAsync(CourseModel model, Course course, bool excludeProperties = false)
    {
        if (course != null)
        {
            //fill in model values from the entity
            if (model == null)
            {
                model = course.ToModel<CourseModel>();
            }
        }

        await _baseOptimizationAppModelFactory.PrepareEducationalDepartmentsAsync(model
            .AvailableEducationalDepartments, await _localizationService.GetResourceAsync("Admin.Common.Select"));
        
        return model;
    }
}