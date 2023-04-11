﻿using System;
using System.Collections.Generic;
using System.Data;
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
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Migrations.UpgradeTo460
{
    [NopMigration("2022-02-03 00:00:00", "4.60.0", UpdateMigrationType.Data, MigrationProcessType.Update)]
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
            //#4601 customer attribute values to customer table column values
            var attributeKeys = new[] { nameof(Customer.FirstName), nameof(Customer.LastName), nameof(Customer.Gender),
                nameof(Customer.Company), nameof(Customer.StreetAddress), nameof(Customer.StreetAddress2), nameof(Customer.ZipPostalCode),
                nameof(Customer.City), nameof(Customer.County), nameof(Customer.Phone), nameof(Customer.Fax), nameof(Customer.VatNumber),
                nameof(Customer.TimeZoneId), nameof(Customer.CustomCustomerAttributesXML), nameof(Customer.CountryId),
                nameof(Customer.StateProvinceId), nameof(Customer.VatNumberStatusId), nameof(Customer.CurrencyId), nameof(Customer.LanguageId),
                nameof(Customer.TaxDisplayTypeId), nameof(Customer.DateOfBirth)};

            var languages = _dataProvider.GetTable<Language>().ToList();
            var currencies = _dataProvider.GetTable<Currency>().ToList();
            var customerRole = _dataProvider.GetTable<CustomerRole>().FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.RegisteredRoleName);
            var customerRoleId = customerRole?.Id ?? 0;

            var query =
                from c in _dataProvider.GetTable<Customer>()
                join crm in _dataProvider.GetTable<CustomerCustomerRoleMapping>() on c.Id equals crm.CustomerId
                where !c.Deleted && (customerRoleId == 0 || crm.CustomerRoleId == customerRoleId)
                select c;

            var pageIndex = 0;
            var pageSize = 500;

            int castToInt(string value)
            {
                return int.TryParse(value, out var result) ? result : default;
            }

            string castToString(string value)
            {
                return value;
            }

            DateTime? castToDateTime(string value)
            {
                return DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var dateOfBirth)
                    ? dateOfBirth
                    : default;
            }

            T getAttributeValue<T>(IList<GenericAttribute> attributes, string key, Func<string, T> castTo, int maxLength = 1000)
            {
                var str = CommonHelper.EnsureMaximumLength(attributes.FirstOrDefault(ga => ga.Key == key)?.Value, maxLength);

                return castTo(str);
            }

            while (true)
            {
                var customers = query.ToPagedListAsync(pageIndex++, pageSize).Result;

                if (!customers.Any())
                    break;

                var customerIds = customers.Select(c => c.Id).ToList();
                var genericAttributes = _dataProvider.GetTable<GenericAttribute>()
                    .Where(ga => ga.KeyGroup == nameof(Customer) && customerIds.Contains(ga.EntityId) && attributeKeys.Contains(ga.Key)).ToList();

                if (!genericAttributes.Any())
                    continue;

                foreach (var customer in customers)
                {
                    var customerAttributes = genericAttributes.Where(ga => ga.EntityId == customer.Id).ToList();
                    if (!customerAttributes.Any())
                        continue;

                    customer.FirstName = getAttributeValue(customerAttributes, nameof(Customer.FirstName), castToString);
                    customer.LastName = getAttributeValue(customerAttributes, nameof(Customer.LastName), castToString);
                    customer.Gender = getAttributeValue(customerAttributes, nameof(Customer.Gender), castToString);
                    customer.Company = getAttributeValue(customerAttributes, nameof(Customer.Company), castToString);
                    customer.StreetAddress = getAttributeValue(customerAttributes, nameof(Customer.StreetAddress), castToString);
                    customer.StreetAddress2 = getAttributeValue(customerAttributes, nameof(Customer.StreetAddress2), castToString);
                    customer.ZipPostalCode = getAttributeValue(customerAttributes, nameof(Customer.ZipPostalCode), castToString);
                    customer.City = getAttributeValue(customerAttributes, nameof(Customer.City), castToString);
                    customer.County = getAttributeValue(customerAttributes, nameof(Customer.County), castToString);
                    customer.Phone = getAttributeValue(customerAttributes, nameof(Customer.Phone), castToString);
                    customer.Fax = getAttributeValue(customerAttributes, nameof(Customer.Fax), castToString);
                    customer.VatNumber = getAttributeValue(customerAttributes, nameof(Customer.VatNumber), castToString);
                    customer.TimeZoneId = getAttributeValue(customerAttributes, nameof(Customer.TimeZoneId), castToString);
                    customer.CustomCustomerAttributesXML = getAttributeValue<string>(customerAttributes, nameof(Customer.CustomCustomerAttributesXML), castToString, int.MaxValue);
                    customer.CountryId = getAttributeValue(customerAttributes, nameof(Customer.CountryId), castToInt);
                    customer.StateProvinceId = getAttributeValue(customerAttributes, nameof(Customer.StateProvinceId), castToInt);
                    customer.VatNumberStatusId = getAttributeValue(customerAttributes, nameof(Customer.VatNumberStatusId), castToInt);
                    customer.CurrencyId = currencies.FirstOrDefault(c => c.Id == getAttributeValue(customerAttributes, nameof(Customer.CurrencyId), castToInt))?.Id;
                    customer.LanguageId = languages.FirstOrDefault(l => l.Id == getAttributeValue(customerAttributes, nameof(Customer.LanguageId), castToInt))?.Id;
                    customer.TaxDisplayTypeId = getAttributeValue(customerAttributes, nameof(Customer.TaxDisplayTypeId), castToInt);
                    customer.DateOfBirth = getAttributeValue(customerAttributes, nameof(Customer.DateOfBirth), castToDateTime);
                }

                _dataProvider.UpdateEntities(customers);
                _dataProvider.BulkDeleteEntities(genericAttributes);
            }

            //#3777 new activity log types
            var activityLogTypeTable = _dataProvider.GetTable<ActivityLogType>();

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportCustomers", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportCustomers",
                        Enabled = true,
                        Name = "Customers were exported"
                    }
                );

            // if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportCategories", StringComparison.InvariantCultureIgnoreCase) == 0))
            //     _dataProvider.InsertEntity(
            //         new ActivityLogType
            //         {
            //             SystemKeyword = "ExportCategories",
            //             Enabled = true,
            //             Name = "Categories were exported"
            //         }
            //     );
            

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportStates", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportStates",
                        Enabled = true,
                        Name = "States were exported"
                    }
                );
            
            //#5607
            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "EnableMultiFactorAuthentication", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var multifactorAuthenticationPermissionRecord = _dataProvider.InsertEntity(
                    new PermissionRecord
                    {
                        SystemName = "EnableMultiFactorAuthentication",
                        Name = "Security. Enable Multi-factor authentication",
                        Category = "Security"
                    }
                );

                var forceMultifactorAuthentication = _dataProvider.GetTable<Setting>()
                    .FirstOrDefault(s =>
                        string.Compare(s.Name, "MultiFactorAuthenticationSettings.ForceMultifactorAuthentication", StringComparison.InvariantCultureIgnoreCase) == 0 &&
                        string.Compare(s.Value, "True", StringComparison.InvariantCultureIgnoreCase) == 0)
                    is not null;

                var customerRoles = _dataProvider.GetTable<CustomerRole>();
                if (!forceMultifactorAuthentication)
                    customerRoles = customerRoles.Where(cr => cr.SystemName == NopCustomerDefaults.AdministratorsRoleName || cr.SystemName == NopCustomerDefaults.RegisteredRoleName);

                foreach (var role in customerRoles.ToList())
                {
                    _dataProvider.InsertEntity(
                        new PermissionRecordCustomerRoleMapping
                        {
                            CustomerRoleId = role.Id,
                            PermissionRecordId = multifactorAuthenticationPermissionRecord.Id
                        }
                    );
                }
            }

            var lastEnabledUtc = DateTime.UtcNow;
            if (!_dataProvider.GetTable<ScheduleTask>().Any(st => string.Compare(st.Type, "Nop.Services.Common.ResetLicenseCheckTask, Nop.Services", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new ScheduleTask
                {
                    Name = "ResetLicenseCheckTask",
                    Seconds = 2073600,
                    Type = "Nop.Services.Common.ResetLicenseCheckTask, Nop.Services",
                    Enabled = true,
                    LastEnabledUtc = lastEnabledUtc,
                    StopOnError = false
                });
            }
            
            // //#6395
            // var paRange = _dataProvider.GetTable<ProductAvailabilityRange>().FirstOrDefault(par => string.Compare(par.Name, "2 week", StringComparison.InvariantCultureIgnoreCase) == 0);
            // if (paRange is not null)
            // {
            //     paRange.Name = "2 weeks";
            //     _dataProvider.UpdateEntity(paRange);
            // }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
