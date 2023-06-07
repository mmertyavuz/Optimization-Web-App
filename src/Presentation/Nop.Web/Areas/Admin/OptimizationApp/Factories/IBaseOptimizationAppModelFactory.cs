using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.OptimizationApp;

namespace Nop.Web.Areas.Admin.Factories;

public interface IBaseOptimizationAppModelFactory
{
    public Task<IList<SelectListItem>> PrepareFacultiesAsync(IList<SelectListItem> items, string customText = null);

    public Task<IList<SelectListItem>> PrepareDepartmentLeadsAsync(IList<SelectListItem> items, string customText = null);
    
    public Task<IList<SelectListItem>> PrepareEducationalDepartmentsAsync(IList<SelectListItem> items, string customText = null);

    Task<IList<SelectListItem>> PrepareCoursesAsync(IList<SelectListItem> items);
}

public class BaseOptimizationAppModelFactory : IBaseOptimizationAppModelFactory
{
    #region Fields

    private readonly ICorporationService _corporationService;
    private readonly ILocalizationService _localizationService;
    private readonly ICustomerService _customerService;
    private readonly ICourseService _courseService;

    #endregion

    #region Ctor

    public BaseOptimizationAppModelFactory(ICorporationService corporationService, ILocalizationService localizationService, ICustomerService customerService, ICourseService courseService)
    {
        _corporationService = corporationService;
        _localizationService = localizationService;
        _customerService = customerService;
        _courseService = courseService;
    }

    #endregion
    
    public async Task<IList<SelectListItem>> PrepareFacultiesAsync(IList<SelectListItem> items, string customText = null)
    {
        items ??= new List<SelectListItem>();

        var faculties = await _corporationService.GetAllFacultiesAsync();
        
        foreach (var faculty in faculties)
        {
            items.Add(new SelectListItem
            {
                Text = faculty.Name,
                Value = faculty.Id.ToString()
            });
        }
        
        items.Insert(0, new SelectListItem
        {
            Text = customText ?? await _localizationService.GetResourceAsync("Admin.Common.All"),
            Value = "0"
        });
        
        return items;
    }

    public async Task<IList<SelectListItem>> PrepareDepartmentLeadsAsync(IList<SelectListItem> items, string customText = null)
    {
        var departmentLeadRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.DepartmentLeadRoleName);

        if (departmentLeadRole is not null)
        {
            var customers =
                await _customerService.GetAllCustomersAsync(customerRoleIds: new int[] {departmentLeadRole.Id});

            if (customers.Any())
            {
                foreach (var customer in customers)
                {
                    items.Add(new SelectListItem
                    {
                        Text = await _customerService.GetCustomerFullNameAsync(customer),
                        Value = customer.Id.ToString()
                    });
                }
                
                items.Add(new SelectListItem
                {
                    Text = customText ?? await _localizationService.GetResourceAsync("Admin.Common.All"),
                    Value = "0"
                });
            }
            else
            {
                items.Add(new SelectListItem
                {
                    Text = $"Department Lead Role is not found",
                    Value = "0"
                } );
            }
            
        }
        else
        {
            items.Add(new SelectListItem
            {
                Text = $"Department Lead Role is not found",
                Value = "0"
            } );
        }

        return items;
    }

    public async Task<IList<SelectListItem>> PrepareEducationalDepartmentsAsync(IList<SelectListItem> items,
        string customText = null)
    {
        var educationalDepartments = await _corporationService.GetAllEducationalDepartmentsAsync();
        
        foreach (var educationalDepartment in educationalDepartments)
        {
            items.Add(new SelectListItem
            {
                Text = educationalDepartment.Name,
                Value = educationalDepartment.Id.ToString()
            });
        }
                
        items.Insert(0, new SelectListItem
        {
            Text = customText ?? await _localizationService.GetResourceAsync("Admin.Common.All"),
            Value = "0"
        });

        return items;
    }

    public async Task<IList<SelectListItem>> PrepareCoursesAsync(IList<SelectListItem> items)
    {
        var courses = await _courseService.GetAllCoursesAsync();
        
        foreach (var course in courses)
        {
            items.Add(new SelectListItem
            {
                Text = course.Name,
                Value = course.Id.ToString()
            });
        }
        
        return items;
    }
}
