using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.OptimizationApp;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Corporations;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public class EducationalDepartmentController : BaseAdminController
{
    #region Fields
    
    private readonly IEducationalDepartmentModelFactory _educationalDepartmentFactory;
    private readonly ICorporationService _corporationService;
    private readonly IPermissionService _permissionService;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;
    private readonly ICustomerService _customerService;
    private readonly CorporationSettings _corporationSettings;
    private readonly IStoreContext _storeContext;
    private readonly IStateProvinceService _stateProvinceService;
    private readonly ICountryService _countryService;
    private readonly IAddressService _addressService;
    private readonly ICourseService _courseService;
    private readonly IExportManager _exportManager;
    public readonly IImportManager _importManager;

    #endregion

    #region Ctor

    public EducationalDepartmentController(IEducationalDepartmentModelFactory educationalDepartmentFactory, ICorporationService corporationService, IPermissionService permissionService, INotificationService notificationService, ILocalizationService localizationService, ICustomerService customerService, CorporationSettings corporationSettings, IStoreContext storeContext, IStateProvinceService stateProvinceService, ICountryService countryService, IAddressService addressService, ICourseService courseService, IExportManager exportManager)
    {
        _educationalDepartmentFactory = educationalDepartmentFactory;
        _corporationService = corporationService;
        _permissionService = permissionService;
        _notificationService = notificationService;
        _localizationService = localizationService;
        _customerService = customerService;
        _corporationSettings = corporationSettings;
        _storeContext = storeContext;
        _stateProvinceService = stateProvinceService;
        _countryService = countryService;
        _addressService = addressService;
        _courseService = courseService;
        _exportManager = exportManager;
    }
    

    #endregion

    #region List

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }
    public async Task<IActionResult> List()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageEducationalDepartments))
            return AccessDeniedView();

        //prepare model
        var model = await _educationalDepartmentFactory.PrepareEducationalDepartmentSearchModelAsync(new EducationalDepartmentSearchModel());

        return View(model);
    }
    [HttpPost]
    public virtual async Task<IActionResult> List(EducationalDepartmentSearchModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageEducationalDepartments))
            return await AccessDeniedDataTablesJson();

        //prepare model
        var model = await _educationalDepartmentFactory.PrepareEducationalDepartmentListModelAsync(searchModel);

        return Json(model);
    }

    #endregion

    #region Create / Edit / Delete

    public virtual async Task<IActionResult> Create()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageEducationalDepartments))
            return AccessDeniedView();

        //prepare model
        var model = await _educationalDepartmentFactory.PrepareEducationalDepartmentModelAsync(new EducationalDepartmentModel(), null);

        return View(model);
    }
    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public virtual async Task<IActionResult> Create(EducationalDepartmentModel model, bool continueEditing)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageEducationalDepartments))
            return AccessDeniedView();

        if (ModelState.IsValid)
        {
            var educationalDepartment = model.ToEntity<EducationalDepartment>();

            await _corporationService.InsertEducationalDepartmentAsync(educationalDepartment);

           // var departmentLead = await CreateEducationalDepartmentLead(educationalDepartment);
           //
           // educationalDepartment.DepartmentLeadCustomerId = departmentLead.Id;
           //
           // await _corporationService.UpdateEducationalDepartmentAsync(educationalDepartment);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Corporations.EducationalDepartments.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = educationalDepartment.Id });
        }

        //prepare model
        model = await _educationalDepartmentFactory.PrepareEducationalDepartmentModelAsync(model, null, true);

        return View(model);

    }
    public virtual async Task<IActionResult> Edit(int id)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageEducationalDepartments))
            return AccessDeniedView();

        //try to get a educational departments with the specified id
        var educationalDepartment = await _corporationService.GetEducationalDepartmentByIdAsync(id);
        
        if (educationalDepartment == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _educationalDepartmentFactory.PrepareEducationalDepartmentModelAsync(null, educationalDepartment);

        return View(model);
    }
    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public virtual async Task<IActionResult> Edit(EducationalDepartmentModel model, bool continueEditing)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageEducationalDepartments))
            return AccessDeniedView();

        //try to get a educational department with the specified id
        var educationalDepartment = await _corporationService.GetEducationalDepartmentByIdAsync(model.Id);
        if (educationalDepartment == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            educationalDepartment.Name = model.Name;
            educationalDepartment.Description = model.Description;
            
            await _corporationService.UpdateEducationalDepartmentAsync(educationalDepartment);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Corporations.EducationalDepartments.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");
            
            return RedirectToAction("Edit", new { id = educationalDepartment.Id });
        }

        //prepare model
        model = await _educationalDepartmentFactory.PrepareEducationalDepartmentModelAsync(model, educationalDepartment, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }
    
    [HttpPost]
    public virtual async Task<IActionResult> Delete(int id)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageEducationalDepartments))
            return AccessDeniedView();

        //try to get a faculty with the specified id
        var educationalDepartment = await _corporationService.GetEducationalDepartmentByIdAsync(id);
        if (educationalDepartment == null)
            return RedirectToAction("List");

        await _corporationService.DeleteEducationalDepartmentAsync(educationalDepartment);

        var customer = await _customerService.GetCustomerByIdAsync(educationalDepartment.DepartmentLeadCustomerId);

        if (customer is not null)
        {
            await _customerService.DeleteCustomerAsync(customer);
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Corporations.EducationalDepartments.Deleted"));

        return RedirectToAction("List");
    }
    
    [HttpPost]
    public virtual async Task<IActionResult> DeleteAll()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageFaculties))
            return AccessDeniedView();

        var courses = await _courseService.GetAllCoursesAsync();

        var educationalDepartments = await _corporationService.GetAllEducationalDepartmentsAsync();
        
        var isAllDeleted = true;
        
        foreach (var educationalDepartment in educationalDepartments)
        {
            if (courses.Any(x => x.EducationalDepartmentId == educationalDepartment.Id))
            {
                isAllDeleted = false;
            }
            else
            {
                await _corporationService.DeleteEducationalDepartmentAsync(educationalDepartment);
            }
        }

        if (isAllDeleted)
        {
            _notificationService.SuccessNotification("All departments are successfully deleted.");
        }
        else
        {
            _notificationService.WarningNotification("Some departments are not deleted. A department with course cannot be deleted. Please delete the course first or reset the optimization process from management page.");
        }
        
        return RedirectToAction("List");
    }
    
    public virtual async Task<IActionResult> ExportExcel()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageEducationalDepartments))
            return AccessDeniedView();

        try
        {
            var bytes = await _exportManager
                .ExportDepartmentToExcel((await _corporationService.GetAllEducationalDepartmentsAsync()).ToList());

            var fileName = _corporationSettings.CorporationName + " departments.xlsx";
            
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
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageEducationalDepartments))
            return AccessDeniedView();

        try
        {
            var bytes = await _exportManager
                .ExportDepartmentToExcel();

            var fileName = _corporationSettings.CorporationName + " sample department import file.xlsx";
            
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
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageEducationalDepartments))
            return AccessDeniedView();

        try
        {
            if (importexcelfile is {Length: > 0})
            {
                await _importManager.ImportDepartmentsFromExcelAsync(importexcelfile.OpenReadStream());
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

    #endregion
    
    #region Utilities

    public async Task<Customer> CreateEducationalDepartmentLead(EducationalDepartment educationalDepartment)
    {
        if (educationalDepartment is null)
            throw new ArgumentNullException(nameof(educationalDepartment));

        var customer = await _customerService.GetCustomerByIdAsync(educationalDepartment.DepartmentLeadCustomerId);

        if (customer is not null) return customer;
        
        var defaultStore = await _storeContext.GetCurrentStoreAsync();

        if (defaultStore == null)
            throw new Exception("No default store could be loaded");

        var storeId = defaultStore.Id;
            
        var email = educationalDepartment.Code.ToLower() + _corporationSettings.CorporationEmailSuffix;
        customer = new Customer
        {
            CustomerGuid = Guid.NewGuid(),
            Email = email,
            Username = email,
            Active = true,
            CreatedOnUtc = DateTime.UtcNow,
            LastActivityDateUtc = DateTime.UtcNow,
            RegisteredInStoreId = storeId
        };
            
        var address = new Address
        {
            FirstName = educationalDepartment.Name,
            LastName = "Department Lead",
            PhoneNumber = "1234567890",
            Email = email,
            FaxNumber = string.Empty,
            Company = _corporationSettings.CorporationName,
            Address1 = _corporationSettings.CorporationName,
            Address2 = string.Empty,
            City = "Istanbul",
            StateProvinceId = (await _stateProvinceService.GetStateProvincesAsync()).FirstOrDefault(sp => sp.Name == "California")?.Id,
            CountryId = (await _countryService.GetAllCountriesAsync()).FirstOrDefault(c => c.ThreeLetterIsoCode == "USA")?.Id,
            ZipPostalCode = "34000",
            CreatedOnUtc = DateTime.UtcNow
        };

        await _addressService.InsertAddressAsync(address);
            
        customer.BillingAddressId = address.Id;
        customer.ShippingAddressId = address.Id;
        customer.FirstName = address.FirstName;
        customer.LastName = address.LastName;

        await _customerService.InsertCustomerAsync(customer);

        await _customerService.InsertCustomerAddressAsync(customer, address);
            
        #region Registered Role

        var crRegistered = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
        if (crRegistered != null)
            await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = crRegistered.Id });
            
        #endregion

        #region Department Lead Role

        var crEducationalDepartmentLead = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.DepartmentLeadRoleName);
        if (crEducationalDepartmentLead != null)
            await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = crEducationalDepartmentLead.Id });

        #endregion
                
        var password = new CustomerPassword
        {
            CustomerId = customer.Id,
            Password = "123456",
            PasswordFormat = PasswordFormat.Clear,
            PasswordSalt = string.Empty,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _customerService.InsertCustomerPasswordAsync(password);

        return customer;
    }

    #endregion
}