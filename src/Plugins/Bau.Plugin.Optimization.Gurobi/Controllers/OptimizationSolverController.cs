using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bau.Plugin.Optimization.Gurobi.Models;
using Bau.Plugin.Optimization.Gurobi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Messages;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Bau.Plugin.Optimization.Gurobi.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class OptimizationSolverController : BasePluginController
    {
        #region Fields

        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public OptimizationSolverController(INotificationService notificationService, ISettingService settingService, IStoreContext storeContext, IStoreService storeService, IWorkContext workContext)
        {
            _notificationService = notificationService;
            _settingService = settingService;
            _storeContext = storeContext;
            _storeService = storeService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        private async Task PrepareModelAsync(ConfigurationModel model)
        {
            //load settings for active store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<OptimizationSolverSettings>(storeId);

            model.GurobiLicenceKey = settings.GurobiLicenceKey;
            model.BaseUrl = settings.BaseUrl;
            model.UseTestMode = settings.UseTestMode;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            var model = new ConfigurationModel();
            await PrepareModelAsync(model);

            return View("~/Plugins/Bau.Plugin.Optimization.Gurobi/Views/Configure.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<OptimizationSolverSettings>(storeId);

            //set API key
            
            settings.GurobiLicenceKey = model.GurobiLicenceKey;
            settings.BaseUrl = model.BaseUrl;
            settings.UseTestMode = model.UseTestMode;
            
            await _settingService.SaveSettingAsync(settings, s => s.GurobiLicenceKey, clearCache: false);
            await _settingService.SaveSettingAsync(settings, s => s.BaseUrl, clearCache: false);
            await _settingService.SaveSettingAsync(settings, s => s.UseTestMode, clearCache: false);
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification("Settings have been saved successfully.");

            return await Configure();
        }

        
        #endregion
    }
}