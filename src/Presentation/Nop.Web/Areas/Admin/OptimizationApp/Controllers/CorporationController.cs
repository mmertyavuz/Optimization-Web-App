using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Corporations;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public class CorporationController : BaseAdminController
{
    #region Fields

    private readonly ICorporationModelFactory _corporationModelFactory;
    private readonly IPermissionService _permissionService;
    private readonly CorporationSettings _corporationSettings;
    private readonly ISettingService _settingService;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;

    #endregion

    #region Ctor

    public CorporationController(ICorporationModelFactory corporationModelFactory, IPermissionService permissionService, CorporationSettings corporationSettings, ISettingService settingService, INotificationService notificationService, ILocalizationService localizationService)
    {
        _corporationModelFactory = corporationModelFactory;
        _permissionService = permissionService;
        _corporationSettings = corporationSettings;
        _settingService = settingService;
        _notificationService = notificationService;
        _localizationService = localizationService;
    }

    #endregion

    public IActionResult Index()
    {
        return RedirectToAction(nameof(Details));
    }
    public virtual async Task<IActionResult> Details()
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCorporations))
            return AccessDeniedView();

        //prepare model
        var model = _corporationModelFactory.PrepareCorporationSettingsModel(new CorporationSettingsModel());

        return View(model);
    }
    
    [HttpPost]
    public virtual async Task<IActionResult> Details(CorporationSettingsModel model, bool continueEditing)
    {
        if (!await _permissionService.AuthorizeAsync(OptimizationAppPermissionProvider.ManageCorporations))
            return AccessDeniedView();
        
        _corporationSettings.CorporationName = model.CorporationName;
        _corporationSettings.CorporationWebsite = model.CorporationWebsite;
        _corporationSettings.LogoUrl = model.LogoUrl;
        _corporationSettings.MiniLogoUrl = model.MiniLogoUrl;
        _corporationSettings.CorporationEmailSuffix = model.CorporationEmailSuffix;
       
        _settingService.SaveSettingAsync(_corporationSettings, settings => settings.CorporationName).Wait();
        _settingService.SaveSettingAsync(_corporationSettings, settings => settings.CorporationWebsite).Wait();
        _settingService.SaveSettingAsync(_corporationSettings, settings => settings.LogoUrl).Wait();
        _settingService.SaveSettingAsync(_corporationSettings, settings => settings.MiniLogoUrl).Wait();
        _settingService.SaveSettingAsync(_corporationSettings, settings => settings.CorporationEmailSuffix).Wait();
       
        
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Corporations.Updated"));

        return RedirectToAction("Details");
    }
}