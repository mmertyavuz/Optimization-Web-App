using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.OptimizationApp;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Corporations;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

public interface IEducationalDepartmentModelFactory
{
    Task<EducationalDepartmentSearchModel> PrepareEducationalDepartmentSearchModelAsync(EducationalDepartmentSearchModel searchModel);

    Task<EducationalDepartmentListModel> PrepareEducationalDepartmentListModelAsync(EducationalDepartmentSearchModel searchModel);

    Task<EducationalDepartmentModel> PrepareEducationalDepartmentModelAsync(EducationalDepartmentModel model, EducationalDepartment educationalDepartment, bool excludeProperties = false);
}

public class EducationalDepartmentModelFactory : IEducationalDepartmentModelFactory
{
    #region Fields

    private readonly ICorporationService _corporationService;
    private readonly IBaseOptimizationAppModelFactory _baseOptimizationAppModelFactory;
    private readonly ICustomerService _customerService;
    private readonly ILocalizationService _localizationService;

    #endregion

    #region Ctor

    public EducationalDepartmentModelFactory(ICorporationService corporationService, IBaseOptimizationAppModelFactory baseOptimizationAppModelFactory, ICustomerService customerService, ILocalizationService localizationService)
    {
        _corporationService = corporationService;
        _baseOptimizationAppModelFactory = baseOptimizationAppModelFactory;
        _customerService = customerService;
        _localizationService = localizationService;
    }

    #endregion

    public async Task<EducationalDepartmentSearchModel> PrepareEducationalDepartmentSearchModelAsync(EducationalDepartmentSearchModel searchModel)
    {
        if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

        await _baseOptimizationAppModelFactory.PrepareFacultiesAsync(searchModel.AvailableFaculties);
        
        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    public async Task<EducationalDepartmentListModel> PrepareEducationalDepartmentListModelAsync(EducationalDepartmentSearchModel searchModel)
    {
        if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

        var educationalDepartments = await _corporationService.GetAllEducationalDepartmentsAsync(name: searchModel.Name);

        var pagedEducationalDepartments = educationalDepartments.ToPagedList(searchModel);

        //prepare grid model
        var model = await new EducationalDepartmentListModel().PrepareToGridAsync(searchModel, pagedEducationalDepartments, () =>
        {
            return pagedEducationalDepartments.SelectAwait(async educationalDepartment =>
            {
                //fill in model values from the entity
                var educationalDepartmentModel = educationalDepartment.ToModel<EducationalDepartmentModel>();

                var faculty = await _corporationService.GetFacultyByIdAsync(educationalDepartment.FacultyId);
                if (faculty is not null)
                    educationalDepartmentModel.FacultyName = faculty.Name;

                if (educationalDepartment.DepartmentLeadCustomerId > 0)
                {
                    var customer =
                        await _customerService.GetCustomerByIdAsync(educationalDepartment.DepartmentLeadCustomerId);

                    if (customer is not null)
                        educationalDepartmentModel.DepartmentLeadCustomerName =
                            await _customerService.GetCustomerFullNameAsync(customer);
                }

                return educationalDepartmentModel;
            });
        });

        return model;
    }

    public async Task<EducationalDepartmentModel> PrepareEducationalDepartmentModelAsync(EducationalDepartmentModel model,
        EducationalDepartment educationalDepartment, bool excludeProperties = false)
    {
            if (educationalDepartment != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = educationalDepartment.ToModel<EducationalDepartmentModel>();
                    
                }
            }
            //set default values for the new model
            if (educationalDepartment == null)
            {
                
            }

            await _baseOptimizationAppModelFactory.PrepareFacultiesAsync(model.AvailableFaculties, await _localizationService.GetResourceAsync($"Admin.Common.Select"));
            await _baseOptimizationAppModelFactory.PrepareDepartmentLeadsAsync(model.AvailableCustomers, await _localizationService.GetResourceAsync($"Admin.Common.Select"));

            return model;
    }
}
