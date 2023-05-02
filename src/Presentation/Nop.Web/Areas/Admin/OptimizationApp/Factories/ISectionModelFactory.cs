using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain;
using Nop.Services.Localization;
using Nop.Services.OptimizationApp;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Education;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

public interface ISectionModelFactory
{
    Task<SectionSearchModel> PrepareSectionSearchModelAsync(SectionSearchModel searchModel);

    Task<SectionListModel> PrepareSectionListModelAsync(SectionSearchModel searchModel);

    Task<SectionModel> PrepareSectionModelAsync(SectionModel model, Section section, bool excludeProperties = false);
}

public class SectionModelFactory : ISectionModelFactory
{
    #region Fields

    private readonly ISectionService _sectionService;
    private readonly IBaseOptimizationAppModelFactory _baseOptimizationAppModelFactory;
    private readonly ILocalizationService _localizationService;

    #endregion

    #region Ctor

    public SectionModelFactory(ISectionService sectionService, IBaseOptimizationAppModelFactory baseOptimizationAppModelFactory, ILocalizationService localizationService)
    {
        _sectionService = sectionService;
        _baseOptimizationAppModelFactory = baseOptimizationAppModelFactory;
        _localizationService = localizationService;
    }

    #endregion


    public async Task<SectionSearchModel> PrepareSectionSearchModelAsync(SectionSearchModel searchModel)
    {
        if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

        await _baseOptimizationAppModelFactory.PrepareCoursesAsync(searchModel.AvailableCourses);
        
        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    public async Task<SectionListModel> PrepareSectionListModelAsync(SectionSearchModel searchModel)
    {
        if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

        var sections = await _sectionService.GetAllSectionsAsync(courseId: searchModel.CourseId,
            sectionNumber: searchModel.SectionNumber
        );
        
        var pagedSections = sections.ToPagedList(searchModel);
        
        //prepare grid model
        var model =  await new SectionListModel().PrepareToGridAsync(searchModel, pagedSections, () =>
        {
            return pagedSections.SelectAwait(async section =>
            {
                //fill in model values from the entity
                var facultyModel = section.ToModel<SectionModel>();

                return facultyModel;
            });
        });

        return model;
    }

    public async Task<SectionModel> PrepareSectionModelAsync(SectionModel model, Section section, bool excludeProperties = false)
    {
        if (section != null)
        {
            //fill in model values from the entity
            if (model == null)
            {
                model = section.ToModel<SectionModel>();
                    
            }
        }
        //set default values for the new model
        if (section == null)
        {
                
        }

        await _baseOptimizationAppModelFactory.PrepareFacultiesAsync(model.AvailableCourses, await _localizationService.GetResourceAsync($"Admin.Common.Select"));

        return model;
    }
}