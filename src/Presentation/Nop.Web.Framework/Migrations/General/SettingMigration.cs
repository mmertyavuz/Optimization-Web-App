﻿using FluentMigrator;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Web.Framework.Migrations.General
{
    [NopMigration("2022-11-17 00:00:01", "4.60.0", UpdateMigrationType.Settings, MigrationProcessType.Update)]
    public class SettingMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            var settingRepository = EngineContext.Current.Resolve<IRepository<Setting>>();
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            
            var customerSettings = settingService.LoadSettingAsync<CustomerSettings>().Result;
           
            customerSettings.CompanyEnabled = false;
            settingService.SaveSettingAsync(customerSettings, settings => settings.CompanyEnabled).Wait();

            customerSettings.NewsletterEnabled = false;
            settingService.SaveSettingAsync(customerSettings, settings => settings.NewsletterEnabled).Wait();

        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}