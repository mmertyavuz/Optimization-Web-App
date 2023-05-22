using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain;
using Nop.Services.Localization;
using Nop.Services.OptimizationApp;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Corporations;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

public interface IFacultyModelFactory
{
    FacultySearchModel PrepareFacultySearchModel(FacultySearchModel searchModel);

    Task<FacultyListModel> PrepareFacultyListModelAsync(FacultySearchModel searchModel);

    Task<FacultyModel> PrepareFacultyModelAsync(FacultyModel model, Faculty faculty, bool excludeProperties = false);
}

public class FacultyModelFactory : IFacultyModelFactory
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly ICorporationService _corporationService;

    #endregion

    #region Ctor

    public FacultyModelFactory(ILocalizationService localizationService, ICorporationService corporationService)
    {
        _localizationService = localizationService;
        _corporationService = corporationService;
    }

    #endregion
    
    public FacultySearchModel PrepareFacultySearchModel(FacultySearchModel searchModel)
    {
        if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));
        
        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    public async Task<FacultyListModel> PrepareFacultyListModelAsync(FacultySearchModel searchModel)
    {
        if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

        var faculties = await _corporationService.GetAllFacultiesAsync(name: searchModel.Name, showOnlyWithoutDepartment: searchModel.ShowOnlyWithoutDepartment);

        var pagedFaculties = faculties.ToPagedList(searchModel);

        var departments = await _corporationService.GetAllEducationalDepartmentsAsync();
        
        //prepare grid model
        var model = new FacultyListModel().PrepareToGrid(searchModel, pagedFaculties, () =>
        {
            return pagedFaculties.Select(faculty =>
            {
                //fill in model values from the entity
                var facultyModel = faculty.ToModel<FacultyModel>();

                facultyModel.DepartmentCount = departments.Count(x => x.FacultyId == faculty.Id);
                
                return facultyModel;
            });
        });

        return model;
    }

    public async Task<FacultyModel> PrepareFacultyModelAsync(FacultyModel model, Faculty faculty, bool excludeProperties = false)
    {
        if (faculty != null)
        {
            //fill in model values from the entity
            if (model == null)
            {
                model = faculty.ToModel<FacultyModel>();
            }
        }
        return model;
    }
}
