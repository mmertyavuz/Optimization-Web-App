using System.Collections.Generic;
using System.Linq;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Data.Migrations;

namespace Nop.Data.OptimizationApp.Migrations.v00___Initial;

[NopMigration("2023-02-01 18:59:00", "PermissionRecordsDataMigration", UpdateMigrationType.Data,
    MigrationProcessType.Update)]
public class PermissionRecordsDataMigration : Migration
{
    private readonly INopDataProvider _dataProvider;
    private List<PermissionRecord> _allPermissionRecords;

    public PermissionRecordsDataMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public override void Up()
    {
        _allPermissionRecords = _dataProvider.GetTable<PermissionRecord>().Select(pr => pr).ToList();

        var adminRole = GetOrInsertCustomerRole(NopCustomerDefaults.AdministratorsRoleName);
        var departmentLeadRole = GetOrInsertCustomerRole(NopCustomerDefaults.DepartmentLeadRoleName);

        var manageCorporations = new PermissionRecord { Name = "Manage Corporations", SystemName = "ManageCorporations", Category = "OptimizationApp" };
        var manageClassrooms = new PermissionRecord
            {Name = "Manage Classrooms", SystemName = "ManageClassrooms", Category = "OptimizationApp"};
       
        
        InsertPermissionRecordCustomerRoleMapping(manageCorporations, adminRole);
        InsertPermissionRecordCustomerRoleMapping(manageClassrooms, adminRole);
        
    }

    void InsertPermissionRecordCustomerRoleMapping(PermissionRecord permissionRecord, params CustomerRole[] customerRoles)
    {
        var hasExistPermissionRecord = _allPermissionRecords.FirstOrDefault(x => x.SystemName == permissionRecord.SystemName);

        if (hasExistPermissionRecord == null)
            _dataProvider.InsertEntity(permissionRecord);
        else
            permissionRecord = hasExistPermissionRecord;

        foreach (var customerRole in customerRoles)
        {
            if (customerRole == null)
                continue;

            if (!_dataProvider.GetTable<PermissionRecordCustomerRoleMapping>().Any(x =>
                    x.PermissionRecordId == permissionRecord.Id && x.CustomerRoleId == customerRole.Id))
            {
                _dataProvider.InsertEntity(new PermissionRecordCustomerRoleMapping
                {
                    PermissionRecordId = permissionRecord.Id, CustomerRoleId = customerRole.Id
                });
            } 
        }
    }

    private CustomerRole GetOrInsertCustomerRole(string roleName)
    {
        var role = _dataProvider
            .GetTable<CustomerRole>()
            .FirstOrDefault(x => x.IsSystemRole && x.SystemName == roleName);

        if (role is null)
        {
            role = new CustomerRole
            {
                Name = "Administrators",
                Active = true,
                IsSystemRole = true,
                SystemName = NopCustomerDefaults.AdministratorsRoleName
            };

            _dataProvider.InsertEntity(role);
        }

        return role;
    }
    
    public override void Down()
    {
    }
}