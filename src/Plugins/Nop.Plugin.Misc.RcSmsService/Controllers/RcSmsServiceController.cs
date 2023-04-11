using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.RcSmsService;
using Nop.Plugin.Misc.RcSmsService.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.RcSmsService.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class RcSmsServiceController : BasePluginController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly RcSmsServiceSettings _rcSmsServiceSettings;
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        

        #endregion

        #region Ctor

        public RcSmsServiceController(IPermissionService permissionService, ISettingService settingService,
            INotificationService notificationService, ILocalizationService localizationService,
            RcSmsServiceSettings rcSmsServiceSettings)
        {
            _permissionService = permissionService;
            _settingService = settingService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _rcSmsServiceSettings = rcSmsServiceSettings;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                ApplicationName = _rcSmsServiceSettings.ApplicationName,
                ServicePassword = _rcSmsServiceSettings.ServicePassword,
                ServiceUrl = _rcSmsServiceSettings.ServiceUrl
            };

            return View("~/Plugins/Misc.RcSmsService/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            _rcSmsServiceSettings.ApplicationName = model.ApplicationName;
            _rcSmsServiceSettings.ServicePassword = model.ServicePassword;
            _rcSmsServiceSettings.ServiceUrl = model.ServiceUrl;

            await _settingService.SaveSettingAsync(_rcSmsServiceSettings);

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}