using System;
using System.Collections.Generic;
using System.Linq;
using FluentMigrator;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Migrations.Initial;

[NopMigration("2023-04-22 15:30:00", "LocaleStringResource migration for 2023-04-22 Updates",
    UpdateMigrationType.Localization, MigrationProcessType.Update)]
public class LocalizationMigration : MigrationBase
{
    private readonly INopDataProvider _dataProvider;

    public LocalizationMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

        var localeStringResources = new List<Tuple<string, string, string>>
        {
            new("Admin.Corporations.Updated", "Kurum bilgileri başarıyla güncellenmiştir.", "Corporation info is successfully updated."),
            new("Admin.Corporations", "Kurum Ayarları", "Corporation Settings"),
            new("Admin.Corporations.Info", "Kurum Bilgileri", "Info"),
            
            #region Faculty

            new("Admin.Corporations.Faculties.Fields.Name", "Ad", "Name"),
            new("Admin.Corporations.Faculties.Fields.Description", "Açıklama", "Description"),
            new("Admin.Corporations.Faculties.Fields.Name.Required", "Ad zorunludur", "Name is required"),
            

            #endregion

            #region EducationalDepartment

            new("Admin.Corporations.EducationalDepartments.Fields.Name", "Ad", "Name"),
            new("Admin.Corporations.EducationalDepartments.Fields.Name.Required", "Ad", "Name"),
            new("Admin.Corporations.EducationalDepartments.Fields.Code", "Kod", "Code"),
            new("Admin.Corporations.EducationalDepartments.Fields.Code.Required", "Kod", "Code"),
            new("Admin.Corporations.EducationalDepartments.Fields.Description", "Açıklama", "Description"),
            new("Admin.Corporations.EducationalDepartments.Fields.Faculty", "Fakülte", "Faculty"),
            new("Admin.Corporations.EducationalDepartments.Fields.FacultyId.Required", "Fakülte seçimi zorunludur", "Faculty is required"),
            new("Admin.Corporations.EducationalDepartments.Fields.DepartmentLeadCustomerId", "Yönetici Hesabı", "Department Lead Account"),

            #endregion
        };

        //table types
        var localeStringResourceTypeTable = _dataProvider.GetTable<LocaleStringResource>();

        #region Languages

        var enLanguage = _dataProvider.GetTable<Language>().FirstOrDefault(x => x.UniqueSeoCode == "en");
        var trLanguage = _dataProvider.GetTable<Language>().FirstOrDefault(x => x.UniqueSeoCode == "tr");

        #endregion

        #region English

        if (enLanguage is not null)
        {
            foreach (var localeItem in localeStringResources)
            {
                AddOrUpdateResource(localeStringResourceTypeTable, localeItem.Item1, localeItem.Item3,
                    enLanguage.Id);
            }
        }

        #endregion

        #region Turkish

        if (trLanguage is not null)
        {
            foreach (var localeItem in localeStringResources)
            {
                AddOrUpdateResource(localeStringResourceTypeTable, localeItem.Item1, localeItem.Item2,
                    trLanguage.Id);
            }
        }

        #endregion
    }

    public override void Down()
    {
    }

    private void AddOrUpdateResource(IQueryable<LocaleStringResource> localeStringResourceTypeTable,
        string resourceName, string resourceValue, int languageId)
    {
        if (!localeStringResourceTypeTable.Any(alt =>
                string.Compare(alt.ResourceName, resourceName, StringComparison.OrdinalIgnoreCase) == 0 &&
                alt.LanguageId == languageId))
        {
            _dataProvider.InsertEntity(new LocaleStringResource()
            {
                LanguageId = languageId, ResourceName = resourceName, ResourceValue = resourceValue
            });
        }
        else
        {
            var localeItems = _dataProvider.GetTable<LocaleStringResource>().Where(x =>
                string.Compare(x.ResourceName, resourceName, StringComparison.OrdinalIgnoreCase) == 0 &&
                x.LanguageId == languageId);

            foreach (var item in localeItems)
            {
                item.ResourceValue = resourceValue;

                _dataProvider.UpdateEntityAsync(item).GetAwaiter();
            }
        }
    }
}