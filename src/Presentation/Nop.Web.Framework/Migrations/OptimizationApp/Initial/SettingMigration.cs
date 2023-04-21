using FluentMigrator;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Corporations;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Web.Framework.Migrations.Initial
{
    [NopMigration("2023-04-14 13:12:01", "4.60.0", UpdateMigrationType.Settings, MigrationProcessType.Update)]
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
           
            corporationSettings.CorporationName = $"Bahçeşehir Üniversitesi";
            settingService.SaveSettingAsync(corporationSettings, settings => settings.CorporationName).Wait();

            corporationSettings.CorporationWebsite = $"https://bau.edu.tr/";
            settingService.SaveSettingAsync(corporationSettings, settings => settings.CorporationWebsite).Wait();

            #endregion

        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}