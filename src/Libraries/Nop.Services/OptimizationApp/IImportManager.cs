using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.Extensions.Azure;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Services.ExportImport.Help;

namespace Nop.Services.ExportImport;

public partial interface IImportManager
{
    Task ImportClassroomsFromExcelAsync(Stream stream);
    Task ImportFacultiesFromExcelAsync(Stream stream);
    Task ImportDepartmentsFromExcelAsync(Stream stream);
    Task ImportCoursesAndSectionsFromExcelAsync(Stream stream);
}

public partial class ImportManager
{
    public async Task ImportClassroomsFromExcelAsync(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);

        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        //the columns
        var metadata = GetWorkbookMetadata<Classroom>(workbook, languages);
        var defaultWorksheet = metadata.DefaultWorksheet;
        var defaultProperties = metadata.DefaultProperties;
        var localizedProperties = metadata.LocalizedProperties;

        var manager =
            new PropertyManager<Classroom, Language>(defaultProperties, _catalogSettings, localizedProperties,
                languages);

        var iRow = 2;

        var clasrooms = await _corporationService.GetAllClassroomsAsync();

        while (true)
        {
            var allColumnsAreEmpty = manager.GetDefaultProperties
                .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

            if (allColumnsAreEmpty)
                break;

            manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

            var classroom = new Classroom();
            foreach (var property in manager.GetDefaultProperties)
            {
                switch (property.PropertyName)
                {
                    case nameof(Classroom.Name):
                        classroom.Name = property.StringValue;
                        break;
                    case nameof(Classroom.Description):
                        classroom.Description = property.StringValue;
                        break;
                    case nameof(Classroom.Capacity):
                        classroom.Capacity = Convert.ToInt32(property.StringValue);
                        break;
                }
            }

            var existingClassroom = clasrooms.FirstOrDefault(c => c.Name == classroom.Name);

            if (existingClassroom is null)
            {
                await _corporationService.InsertClassroomAsync(classroom);
            }
            else
            {
                existingClassroom.Description = classroom.Description;
                existingClassroom.Capacity = classroom.Capacity;
                await _corporationService.UpdateClassroomAsync(existingClassroom);
            }

            iRow++;
        }
    }

    public async Task ImportFacultiesFromExcelAsync(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);

        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        //the columns
        var metadata = GetWorkbookMetadata<Faculty>(workbook, languages);
        var defaultWorksheet = metadata.DefaultWorksheet;
        var defaultProperties = metadata.DefaultProperties;
        var localizedProperties = metadata.LocalizedProperties;

        var manager =
            new PropertyManager<Faculty, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

        var iRow = 2;

        var faculties = await _corporationService.GetAllFacultiesAsync();

        while (true)
        {
            var allColumnsAreEmpty = manager.GetDefaultProperties
                .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

            if (allColumnsAreEmpty)
                break;

            manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

            var faculty = new Faculty();
            foreach (var property in manager.GetDefaultProperties)
            {
                switch (property.PropertyName)
                {
                    case nameof(Faculty.Name):
                        faculty.Name = property.StringValue;
                        break;
                    case nameof(Faculty.Description):
                        faculty.Description = property.StringValue;
                        break;
                }
            }

            var existingFaculty = faculties.FirstOrDefault(c => c.Name == faculty.Name);

            if (existingFaculty is null)
            {
                await _corporationService.InsertFacultyAsync(faculty);
            }
            else
            {
                existingFaculty.Description = faculty.Description;
                await _corporationService.UpdateFacultyAsync(existingFaculty);
            }

            iRow++;
        }
    }

    public async Task ImportDepartmentsFromExcelAsync(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);

        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        //the columns
        var metadata = GetWorkbookMetadata<EducationalDepartment>(workbook, languages);
        var defaultWorksheet = metadata.DefaultWorksheet;
        var defaultProperties = metadata.DefaultProperties;
        var localizedProperties = metadata.LocalizedProperties;

        var manager = new PropertyManager<EducationalDepartment, Language>(defaultProperties, _catalogSettings,
            localizedProperties, languages);

        var iRow = 2;

        var departments = await _corporationService.GetAllEducationalDepartmentsAsync();
        var faculties = await _corporationService.GetAllFacultiesAsync();

        while (true)
        {
            var allColumnsAreEmpty = manager.GetDefaultProperties
                .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

            if (allColumnsAreEmpty)
                break;

            manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

            var department = new EducationalDepartment();
            foreach (var property in manager.GetDefaultProperties)
            {
                switch (property.PropertyName)
                {
                    case nameof(EducationalDepartment.Name):
                        department.Name = property.StringValue;
                        break;
                    case nameof(EducationalDepartment.Code):
                        department.Code = property.StringValue;
                        break;
                    case nameof(EducationalDepartment.Description):
                        department.Description = property.StringValue;
                        break;
                    case $"Faculty":
                        var faculty = faculties.FirstOrDefault(x => x.Name == property.StringValue);
                        if (faculty != null) department.FacultyId = faculty.Id;
                        break;
                }
            }
            
            var existingDepartment = departments.FirstOrDefault(c => c.Code == department.Code);

            if (existingDepartment is null)
            {
                await _corporationService.InsertEducationalDepartmentAsync(department);
            }
            else
            {
                existingDepartment.Name = existingDepartment.Name;
                existingDepartment.Code = department.Code;
                existingDepartment.Description = department.Description;
                    
                await _corporationService.UpdateEducationalDepartmentAsync(existingDepartment);
            }

            iRow++;
            
        }
    }

    public async Task ImportCoursesAndSectionsFromExcelAsync(Stream stream)
    {
        using var workbook = new XLWorkbook(stream);

        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        //the columns
        var metadata = GetWorkbookMetadata<Course>(workbook, languages);
        var defaultWorksheet = metadata.DefaultWorksheet;
        var defaultProperties = metadata.DefaultProperties;
        var localizedProperties = metadata.LocalizedProperties;

        var manager = new PropertyManager<Course, Language>(defaultProperties, _catalogSettings,
            localizedProperties, languages);

        var iRow = 2;

        var departments = await _corporationService.GetAllEducationalDepartmentsAsync();
        var courses = await _courseService.GetAllCoursesAsync();
        var sections = await _sectionService.GetAllSectionsAsync();
        
         while (true)
        {
            var allColumnsAreEmpty = manager.GetDefaultProperties
                .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

            if (allColumnsAreEmpty)
                break;

            manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

            var existingCourse = FindExistingCourse(manager, courses);

            //There is not a course with this code, so we create a new one
            if (existingCourse is null)
            {
                var course = new Course
                {
                    Description = "-"
                };

                var section = new Section
                {
                    SectionNumber = "1"
                };

                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName)
                    {
                        case "Kod":
                            course.Code = property.StringValue;
                            break;
                        case "Ad":
                            course.Name = property.StringValue;
                            break;
                        case "Departman":
                            var department = departments.FirstOrDefault(x => x.Name == property.StringValue);
                            if (department != null) course.EducationalDepartmentId = department.Id;
                            break;

                       #region Create a section for this new course
                        
                       case $"Gün":
                            
                       section.Day = property.StringValue switch
                       {
                           "Pazartesi" => DayOfWeek.Monday,
                           "Salı" => DayOfWeek.Tuesday,
                           "Çarşamba" => DayOfWeek.Wednesday,
                           "Perşembe" => DayOfWeek.Thursday,
                           "Cuma" => DayOfWeek.Friday,
                           "Cumartesi" => DayOfWeek.Saturday,
                           "Pazar" => DayOfWeek.Sunday,
                           _ => throw new ArgumentOutOfRangeException()
                       };

                            break;
                            case "Başlangıç":
                                section.StartTime =TimeSpan.Parse(property.StringValue);
                            break;
                            case "Bitiş":
                                section.EndTime =TimeSpan.Parse(property.StringValue);
                            break;
                            
                        case "Öğrenci Adet":
                            section.StudentCount = int.Parse(property.StringValue);
                            break;
                        #endregion
                    }
                }

                await _courseService.InsertCourseAsync(course);
                courses.Add(course);

                section.CourseId = course.Id;
                await _sectionService.InsertSectionAsync(section);
                sections.Add(section);

            }
            //There is a course with this code, so we create a new seciton for this course
            else
            {
                var allSections = sections.Where(x => x.CourseId == existingCourse.Id);
                
                var section = new Section
                {
                    SectionNumber = (allSections.Count() + 1).ToString(),
                    CourseId = existingCourse.Id
                };
                
                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName)
                    {
                       case $"Gün":
                       section.Day = property.StringValue switch
                       {
                           "Pazartesi" => DayOfWeek.Monday,
                           "Salı" => DayOfWeek.Tuesday,
                           "Çarşamba" => DayOfWeek.Wednesday,
                           "Perşembe" => DayOfWeek.Thursday,
                           "Cuma" => DayOfWeek.Friday,
                           "Cumartesi" => DayOfWeek.Saturday,
                           "Pazar" => DayOfWeek.Sunday,
                           _ => throw new ArgumentOutOfRangeException()
                       };
                            break;
                            case "Başlangıç":
                                section.StartTime =TimeSpan.Parse(property.StringValue);
                            break;
                            case "Bitiş":
                                section.EndTime =TimeSpan.Parse(property.StringValue);
                            break;
                            
                        case "Öğrenci Adet":
                            section.StudentCount = int.Parse(property.StringValue);
                            break;
                    }
                }
                
                await _sectionService.InsertSectionAsync(section);
                sections.Add(section);
            }
            

            iRow++;
            
        }
        
    }
    
    private Course FindExistingCourse(PropertyManager<Course, Language> manager, IList<Course> courses)
    {
        var codeProp = manager.GetDefaultProperties.FirstOrDefault(x => x.PropertyName == "Kod");
        
        if (codeProp is null)
            throw new Exception("Kod sütunu bulunamadı");
        
        var course = courses.FirstOrDefault(x => x.Code == codeProp.StringValue);
        
        return course;
    }

    #region Utilities

    private async Task<Customer> CreateEducationalDepartmentLead(EducationalDepartment educationalDepartment)
    {
        if (educationalDepartment is null)
            throw new ArgumentNullException(nameof(educationalDepartment));

        var customer = await _customerService.GetCustomerByIdAsync(educationalDepartment.DepartmentLeadCustomerId);

        if (customer is not null) return customer;

        var defaultStore = await _storeContext.GetCurrentStoreAsync();

        if (defaultStore == null)
            throw new Exception("No default store could be loaded");

        var storeId = defaultStore.Id;

        var email = educationalDepartment.Code.ToLower() + _corporationSettings.CorporationEmailSuffix;
        customer = new Customer
        {
            CustomerGuid = Guid.NewGuid(),
            Email = email,
            Username = email,
            Active = true,
            CreatedOnUtc = DateTime.UtcNow,
            LastActivityDateUtc = DateTime.UtcNow,
            RegisteredInStoreId = storeId
        };

        var address = new Address
        {
            FirstName = educationalDepartment.Name,
            LastName = "Department Lead",
            PhoneNumber = "1234567890",
            Email = email,
            FaxNumber = string.Empty,
            Company = _corporationSettings.CorporationName,
            Address1 = _corporationSettings.CorporationName,
            Address2 = string.Empty,
            City = "Istanbul",
            StateProvinceId = (await _stateProvinceService.GetStateProvincesAsync())
                .FirstOrDefault(sp => sp.Name == "California")?.Id,
            CountryId = (await _countryService.GetAllCountriesAsync())
                .FirstOrDefault(c => c.ThreeLetterIsoCode == "USA")?.Id,
            ZipPostalCode = "34000",
            CreatedOnUtc = DateTime.UtcNow
        };

        await _addressService.InsertAddressAsync(address);

        customer.BillingAddressId = address.Id;
        customer.ShippingAddressId = address.Id;
        customer.FirstName = address.FirstName;
        customer.LastName = address.LastName;

        await _customerService.InsertCustomerAsync(customer);

        await _customerService.InsertCustomerAddressAsync(customer, address);

        #region Registered Role

        var crRegistered =
            await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
        if (crRegistered != null)
            await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping
                {CustomerId = customer.Id, CustomerRoleId = crRegistered.Id});

        #endregion

        #region Department Lead Role

        var crEducationalDepartmentLead =
            await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.DepartmentLeadRoleName);
        if (crEducationalDepartmentLead != null)
            await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping
                {CustomerId = customer.Id, CustomerRoleId = crEducationalDepartmentLead.Id});

        #endregion

        var password = new CustomerPassword
        {
            CustomerId = customer.Id,
            Password = "123456",
            PasswordFormat = PasswordFormat.Clear,
            PasswordSalt = string.Empty,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _customerService.InsertCustomerPasswordAsync(password);

        return customer;
    }

    #endregion
}