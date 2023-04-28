using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentMigrator;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Domain.Security;
using Nop.Data.Migrations;

namespace Nop.Data.OptimizationApp.Migrations.v00___Initial
{
    [NopMigration("2023-04-28 11:50:00", "4.60.0", UpdateMigrationType.Data, MigrationProcessType.Update)]
    public class DataMigration : Migration
    {
        private readonly INopDataProvider _dataProvider;

        public DataMigration(INopDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            InstallCustomerRoles();
        }

        public void InstallCustomerRoles()
        {
            var customerRoles = new List<CustomerRole>()
            {
                new CustomerRole
                {
                    Name = "Department Lead",
                    Active = true,
                    IsSystemRole = true,
                    SystemName = NopCustomerDefaults.DepartmentLeadRoleName
                },
            };
            
            foreach (var customerRole in customerRoles)
            {
                _dataProvider.InsertEntity(customerRole);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
