using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.RcMailService
{
    public class RcMailServicePlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public RcMailServicePlugin(IWebHelper webHelper, IPermissionService permissionService, ISettingService settingService, IScheduleTaskService scheduleTaskService, ILocalizationService localizationService)
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
            return $"{_webHelper.GetStoreLocation()}Admin/RcMailService/Configure";
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
                SystemName = "Rc Mail Service",
                Title = "RC Mail Service",
                ControllerName = "RcMailService",
                ActionName = "Configure",
                IconClass = "far fa-dot-circle",
                Visible = true,
                RouteValues = new RouteValueDictionary { { "area", AreaNames.Admin } }
            });
        }

        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new RcMailServiceSettings
            {
                ApplicationName = "",
                From = "",
                ServiceUrl = ""
            });

            //schedule task
            if (await _scheduleTaskService.GetTaskByTypeAsync(RcMailServiceDefaults.RcMailServiceTask.Type) is null)
            {
                await _scheduleTaskService.InsertTaskAsync(new()
                {
                    Enabled = false,
                    LastEnabledUtc = DateTime.UtcNow,
                    Seconds = RcMailServiceDefaults.RcMailServiceTask.Seconds,
                    Name = RcMailServiceDefaults.RcMailServiceTask.Name,
                    Type = RcMailServiceDefaults.RcMailServiceTask.Type
                });
            }

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Misc.RcMailService.ApplicationName"] = "Application name",
                ["Plugins.Misc.RcMailService.From"] = "From",
                ["Plugins.Misc.RcMailService.ServiceUrl"] = "Service url",
                ["Plugins.Misc.RcMailService.ApplicationName.Required"] = "Application name is required.",
                ["Plugins.Misc.RcMailService.ServiceUrl.Required"] = "Service url is required."

            });

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<RcMailServiceSettings>();

            //schedule task
            var task = await _scheduleTaskService.GetTaskByTypeAsync(RcMailServiceDefaults.RcMailServiceTask.Type);

            if (task is not null)
                await _scheduleTaskService.DeleteTaskAsync(task);

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.RcMailService");

            await base.UninstallAsync();
        }
    }
}
