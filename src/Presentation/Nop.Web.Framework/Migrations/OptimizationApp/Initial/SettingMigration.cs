using FluentMigrator;
using Nop.Core.Domain;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Web.Framework.Migrations.Initial
{
    [NopMigration("2023-04-14 13:15:01", "4.60.0", UpdateMigrationType.Settings, MigrationProcessType.Update)]
    public class SettingMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            var settingRepository = EngineContext.Current.Resolve<IRepository<Setting>>();
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            #region Corporation Settings

            var corporationSettings = settingService.LoadSettingAsync<CorporationSettings>().Result;

            if (string.IsNullOrEmpty(corporationSettings.CorporationName))
            {
                corporationSettings.CorporationName = $"Bahçeşehir Üniversitesi";
                settingService.SaveSettingAsync(corporationSettings, settings => settings.CorporationName).Wait();
            }

            if (string.IsNullOrEmpty(corporationSettings.CorporationWebsite))
            {
                corporationSettings.CorporationWebsite = $"https://bau.edu.tr/";
                settingService.SaveSettingAsync(corporationSettings, settings => settings.CorporationWebsite).Wait();
            }

            if (string.IsNullOrEmpty(corporationSettings.LogoUrl))
            {
                corporationSettings.LogoUrl = $"https://cdn.bau.edu.tr/public/bau-logo-25-yil-white.png";
                settingService.SaveSettingAsync(corporationSettings, settings => settings.LogoUrl).Wait();
            }

            if (string.IsNullOrEmpty(corporationSettings.MiniLogoUrl))
            {
                corporationSettings.MiniLogoUrl = $"https://cdn.bau.edu.tr/news/klkvssdz4ko9k-baunew.jpg";
                settingService.SaveSettingAsync(corporationSettings, settings => settings.MiniLogoUrl).Wait();
            }

            if (string.IsNullOrEmpty(corporationSettings.CorporationEmailSuffix))
            {
                corporationSettings.CorporationEmailSuffix = $"@bahcesehir.edu.tr";
                settingService.SaveSettingAsync(corporationSettings, settings => settings.CorporationEmailSuffix).Wait();
            }


            #endregion

        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}