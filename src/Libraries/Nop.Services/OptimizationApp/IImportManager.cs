using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Nop.Core.Domain;
using Nop.Core.Domain.Localization;
using Nop.Services.ExportImport.Help;

namespace Nop.Services.ExportImport;

public partial interface IImportManager
{
    Task ImportClassroomsFromExcelAsync(Stream stream);
    Task ImportFacultiesFromExcelAsync(Stream stream);
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

            var manager = new PropertyManager<Classroom, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

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

            var manager = new PropertyManager<Faculty, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

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
}