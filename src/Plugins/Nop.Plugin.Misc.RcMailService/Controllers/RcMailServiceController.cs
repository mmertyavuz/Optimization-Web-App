using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.RcMailService.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.RcMailService.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class RcMailServiceController : BasePluginController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly RcMailServiceSettings _rcMailServiceSettings;
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public RcMailServiceController(IPermissionService permissionService, RcMailServiceSettings rcMailServiceSettings, ISettingService settingService, INotificationService notificationService, ILocalizationService localizationService)
        {
            _permissionService = permissionService;
            _rcMailServiceSettings = rcMailServiceSettings;
            _settingService = settingService;
            _notificationService = notificationService;
            _localizationService = localizationService;
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
                ApplicationName = _rcMailServiceSettings.ApplicationName,
                From = _rcMailServiceSettings.From,
                ServiceUrl = _rcMailServiceSettings.ServiceUrl
            };

            return View("~/Plugins/Misc.RcMailService/Views/Configure.cshtml", model);
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

            _rcMailServiceSettings.ApplicationName = model.ApplicationName;
            _rcMailServiceSettings.From = model.From;
            _rcMailServiceSettings.ServiceUrl = model.ServiceUrl;

            await _settingService.SaveSettingAsync(_rcMailServiceSettings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }


        #endregion
    }
}
