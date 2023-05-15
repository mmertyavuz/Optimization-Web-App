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

[NopMigration("2023-04-22 15:33:00", "LocaleStringResource migration for 2023-04-22 Updates",
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

            #region Classroom

            new("Admin.Corporations.Classrooms.Fields.Name", "Ad", "Name"),
            new("Admin.Corporations.Classrooms.Fields.Description", "Açıklama", "Description"),
            new("Admin.Corporations.Classrooms.Fields.Capacity", "Kapasite", "Capacity"),
            new("Admin.Corporations.Classrooms.Fields.MinCapacity", "Min Kapasite", "MinCapacity"),
            new("Admin.Corporations.Classrooms.Fields.MaxCapacity", "Max Kapasite", "MaxCapacity"),

            #endregion

            #region Courses

            new("Admin.Courses.Fields.Code", "Kod", "Code"),
            new("Admin.Courses.Fields.Name", "Ad", "Name"),
            new("Admin.Courses.Fields.Description", "Açıklama", "Description"),
            new("Admin.Courses.Fields.Credit", "Kredi", "Credit"),
            new("Admin.Courses.Fields.Ects", "AKTS", "Ects"),
            new("Admin.Courses.Fields.EducationalDepartmentId", "Departman", "Department"),

            #endregion
            
            new("Admin.Corporations.Classrooms.Added", "Sınıf başarıyla eklendi.", "Classroom is successfully added."),
            new("Admin.Corporations.Classrooms.Updated", "Sınıf başarıyla güncellendi.", "Classroom is successfully updated."),
            new("Admin.Corporations.Classrooms.Deleted", "Sınıf başarıyla silindi.", "Classroom is successfully deleted."),
            new("Admin.Courses.Added", "Ders başarıyla eklendi.", "Course is successfully added."),
            new("Admin.Courses.Updated", "Ders başarıyla güncellendi.", "Course is successfully updated."),
            new("Admin.Courses.Deleted", "Ders başarıyla silindi.", "Course is successfully deleted."),
            new("Admin.Corporations.EducationalDepartments.Added", "Departman başarıyla eklendi.", "Educational department is successfully added."),
            new("Admin.Corporations.EducationalDepartments.Updated", "Departman başarıyla güncellendi.", "Educational department is successfully updated."),
            new("Admin.Corporations.EducationalDepartments.Deleted", "Departman başarıyla silindi.", "Educational department is successfully deleted."),
            new("Admin.Corporations.Faculties.Added", "Fakülte başarıyla eklendi.", "Faculty is successfully added."),
            new("Admin.Corporations.Faculties.Updated", "Fakülte başarıyla güncellendi.", "Faculty is successfully updated."),
            new("Admin.Corporations.Faculties.Deleted", "Fakülte başarıyla silindi.", "Faculty is successfully deleted."),
            
            new("Admin.Corporations.Classrooms", "Sınıflar", "Classrooms"),
            new("Admin.Corporations.Classrooms.AddNew", "Yeni sınıf ekle", "Add new Classroom"),
            new("Admin.Corporations.Classrooms.EditDetails", "Sınıf Detaylarını Düzenle", "Edit Classroom Details"),
            new("Admin.Corporations.Classrooms.Fields.Capacity", "Kapasite", "Capacity"),
            new("Admin.Corporations.Classrooms.Fields.Description", "Açıklama", "Description"),
            new("Admin.Corporations.Classrooms.Fields.Name", "Sınıf Adı", "Name"),
            new("Admin.Corporations.Classrooms.Info", "Sınıf Bilgileri", "Classroom Info"),
            new("Admin.Common.AddNew", "Yeni Ekle", "Add New"),
            new("Admin.Common.BackToList", "Listeye Dön", "Back to List"),
            new("Admin.Common.Delete", "Sil", "Delete"),
            new("Admin.Common.Edit", "Düzenle", "Edit"),
            new("Admin.Common.Save", "Kaydet", "Save"),
            new("Admin.Courses", "Dersler", "Courses"),
            new("Admin.Courses.AddNew", "Yeni Ders Ekle", "Add new Course"),
            new("Admin.Courses.EditDetails", "Ders Detaylarını Düzenle", "Edit Course Details"),
            new("Admin.Courses.Info", "Ders Bilgileri", "Course Info"),
            new("Admin.Courses.Sections.Fields.SectionNumber", "Bölüm Numarası", "Section Number"),
            new("Admin.Education.Courses.Fields.Code", "Kod", "Code"),
            new("Admin.Education.Courses.Fields.Credit", "Kredi", "Credit"),
            new("Admin.Education.Courses.Fields.Ects", "AKTS", "Ects"),
            new("Admin.Education.Courses.Fields.Name", "Ders Adı", "Name"),
            new("Admin.Sections.AddNew", "Yeni Bölüm Ekle", "Add new Section"),
            new("Admin.Sections.EditDetails", "Bölüm Detaylarını Düzenle", "Edit Section Details"),
            new("Admin.Corporations.EducationalDepartments", "Eğitim Departmanları", "Departments"),
            new("Admin.Corporations.EducationalDepartments.AddNew", "Yeni Eğitim Departmanı Ekle", "Add new Educational Department"),
            new("Admin.Corporations.EducationalDepartments.EditDetails", "Eğitim Departmanı Detaylarını Düzenle", "Edit Educational Department Details"),
            new("Admin.Corporations.EducationalDepartments.Info", "Eğitim Departmanı Bilgileri", "Educational Department Info"),
            new("Admin.Corporations.Faculties", "Fakülteler", "Faculties"),
            new("Admin.Corporations.Faculties.AddNew", "Yeni Fakülte Ekle", "Add new Faculty"),
            new("Admin.Corporations.Faculties.EditDetails", "Fakülte Detaylarını Düzenle", "Edit Faculty Details"),
            new("Admin.Corporations.Faculties.Info", "Fakülte Bilgileri", "Faculty Info"),
            new("admin.corporations.classrooms.fields.name.required", "Ad zorunludur.", "Name is required."),
            new("admin.corporations.classrooms.fields.capacity.required", "Kapasite zorunludur.", "Capacity is required."),
            new("Admin.Corporations.Faculties.Fields.DepartmentCount", "Departman sayısı", "Department count"),
            new("Admin.Corporations.Faculties.Fields.ShowOnlyWithoutDepartment", "Departmanı Olmayanları Göster", "Show Only Without Department"),
            
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