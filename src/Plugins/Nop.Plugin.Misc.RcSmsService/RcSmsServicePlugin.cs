using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.RcSmsService
{
    public class RcSmsServicePlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public RcSmsServicePlugin(IWebHelper webHelper, IPermissionService permissionService,
            ISettingService settingService, IScheduleTaskService scheduleTaskService,
            ILocalizationService localizationService)
        {
            _webHelper = webHelper;
            _permissionService = permissionService;
            _settingService = settingService;
            _scheduleTaskService = scheduleTaskService;
            _localizationService = localizationService;
        }

        #endregion

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/RcSmsService/Configure";
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return;

            var config = rootNode.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Configuration"));
            if (config == null)
                return;

            var plugins = config.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Local plugins"));

            if (plugins == null)
                return;

            var index = config.ChildNodes.IndexOf(plugins);

            if (index < 0)
                return;

            config.ChildNodes.Insert(index, new SiteMapNode
            {
                SystemName = "Rc Sms Service",
                Title = "RC Sms Service",
                ControllerName = "RcSmsService",
                ActionName = "Configure",
                IconClass = "far fa-dot-circle",
                Visible = true,
                RouteValues = new RouteValueDictionary {{"area", AreaNames.Admin}}
            });
        }

        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new RcSmsServiceSettings
            {
                ApplicationName = "",
                ServicePassword = "",
                ServiceUrl = ""
            });

            //schedule task
            if (await _scheduleTaskService.GetTaskByTypeAsync(RcSmsServiceDefaults.RcSmsServiceTask.Type) is null)
            {
                await _scheduleTaskService.InsertTaskAsync(new()
                {
                    Enabled = false,
                    LastEnabledUtc = DateTime.UtcNow,
                    Seconds = RcSmsServiceDefaults.RcSmsServiceTask.Seconds,
                    Name = RcSmsServiceDefaults.RcSmsServiceTask.Name,
                    Type = RcSmsServiceDefaults.RcSmsServiceTask.Type
                });
            }

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Misc.RcSmsService.ApplicationName"] = "Application name",
                ["Plugins.Misc.RcSmsService.ServicePassword"] = "Service password",
                ["Plugins.Misc.RcSmsService.ServiceUrl"] = "Service url",
                ["Plugins.Misc.RcSmsService.ApplicationName.Required"] = "Application name is required.",
                ["Plugins.Misc.RcSmsService.ServicePassword.Required"] = "Service password is required",
                ["Plugins.Misc.RcSmsService.ServiceUrl.Required"] = "Service url is required."
            });

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<RcSmsServiceSettings>();

            //schedule task
            var task = await _scheduleTaskService.GetTaskByTypeAsync(RcSmsServiceDefaults.RcSmsServiceTask.Type);

            if (task is not null)
                await _scheduleTaskService.DeleteTaskAsync(task);

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.RcSmsService");

            await base.UninstallAsync();
        }
    }
}