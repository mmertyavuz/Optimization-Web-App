using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain;
using Nop.Core.Domain.Localization;
using Nop.Services.ExportImport.Help;
using Nop.Services.Helpers;

namespace Nop.Services.ExportImport;

public partial interface IExportManager
{
    Task<byte[]> ExportClassroomsToExcel(IList<Classroom> classrooms);
    Task<byte[]> ExportClassroomsToExcel();

    Task<byte[]> ExportFacultiesToExcel(IList<Faculty> faculties);

    Task<byte[]> ExportFacultiesToExcel();
    
    Task<byte[]> ExportDepartmentToExcel(IList<EducationalDepartment> departments);

    Task<byte[]> ExportDepartmentToExcel();

    Task<byte[]> ExportCoursesAndSectionsToExcel(List<Section> sections);
    Task<byte[]> ExportCoursesAndSectionsToExcel();

} 

public partial class ExportManager
{
    #region Clasroom

    public virtual async Task<byte[]> ExportClassroomsToExcel(IList<Classroom> classrooms)
    {
        
        //property manager 
        var manager = new PropertyManager<Classroom, Language>(new[]
        {
            new PropertyByName<Classroom, Language>("Id", (p, l) => p.Id),
            new PropertyByName<Classroom, Language>("Name", (p, l) => p.Name),
            new PropertyByName<Classroom, Language>("Description", (p, l) => p.Description),
            new PropertyByName<Classroom, Language>("Capacity", (p, l) => p.Capacity),
            
        }, _catalogSettings);

        return await manager.ExportToXlsxAsync(classrooms);
    }
    
    public virtual async Task<byte[]> ExportClassroomsToExcel()
    {
        var classrooms = new[]
        {
            new Classroom
            {
                Name = "",
                Description = "",
                Capacity = 0,
            }
        };
        //property manager 
        var manager = new PropertyManager<Classroom, Language>(new[]
        {
            new PropertyByName<Classroom, Language>("Name", (p, l) => p.Name),
            new PropertyByName<Classroom, Language>("Description", (p, l) => p.Description),
            new PropertyByName<Classroom, Language>("Capacity", (p, l) => p.Capacity),
            
        }, _catalogSettings);

        return await manager.ExportToXlsxAsync(classrooms);
    }

    #endregion

    #region Faculty
    
    public virtual async Task<byte[]> ExportFacultiesToExcel(IList<Faculty> faculties)
    {
        
        //property manager 
        var manager = new PropertyManager<Faculty, Language>(new[]
        {
            new PropertyByName<Faculty, Language>("Id", (p, l) => p.Id),
            new PropertyByName<Faculty, Language>("Name", (p, l) => p.Name),
            new PropertyByName<Faculty, Language>("Description", (p, l) => p.Description)
        }, _catalogSettings);

        return await manager.ExportToXlsxAsync(faculties);
    }
    
    public virtual async Task<byte[]> ExportFacultiesToExcel()
    {
        var faculties = new[]
        {
            new Faculty()
            {
                Name = "",
                Description = "",
            }
        };
        //property manager 
        var manager = new PropertyManager<Faculty, Language>(new[]
        {
            new PropertyByName<Faculty, Language>("Name", (p, l) => p.Name),
            new PropertyByName<Faculty, Language>("Description", (p, l) => p.Description)
            
        }, _catalogSettings);

        return await manager.ExportToXlsxAsync(faculties);
    }
    
    #endregion

    #region Department

        public async Task<byte[]> ExportDepartmentToExcel(IList<EducationalDepartment> departments)
    {
        var allFaculties = await _corporationService.GetAllFacultiesAsync();
        var allCustomers = await _customerService.GetAllCustomersAsync();
        
        //property manager 
        var manager = new PropertyManager<EducationalDepartment, Language>(new[]
        {
            new PropertyByName<EducationalDepartment, Language>("Id", (p, l) => p.Id),
            new PropertyByName<EducationalDepartment, Language>("Name", (p, l) => p.Name),
            new PropertyByName<EducationalDepartment, Language>("Code", (p, l) => p.Code),
            new PropertyByName<EducationalDepartment, Language>("Description", (p, l) => p.Description),
            new PropertyByName<EducationalDepartment, Language>("Faculty", (p, l) => allFaculties.FirstOrDefault(x => x.Id == p.FacultyId)?.Name),
            new PropertyByName<EducationalDepartment, Language>("Lead", (p, l) => allCustomers.FirstOrDefault(x => x.Id == p.DepartmentLeadCustomerId)?.FirstName + " " + allCustomers.FirstOrDefault(x => x.Id == p.DepartmentLeadCustomerId)?.LastName),
        }, _catalogSettings);

        return await manager.ExportToXlsxAsync(departments);
    }

    public async Task<byte[]> ExportDepartmentToExcel()
    {
        var departments = new[]
        {
            new EducationalDepartment()
            {
                Name = "",
                Code = "",
                Description = "",
                FacultyId = 0,
                DepartmentLeadCustomerId = 0
            }
        };
        
        //property manager 
        var manager = new PropertyManager<EducationalDepartment, Language>(new[]
        {
            new PropertyByName<EducationalDepartment, Language>("Name", (p, l) => p.Name),
            new PropertyByName<EducationalDepartment, Language>("Code", (p, l) => p.Code),
            new PropertyByName<EducationalDepartment, Language>("Description", (p, l) => p.Description),
            new PropertyByName<EducationalDepartment, Language>("Faculty", (p, l) => "")        
        }, _catalogSettings);

        return await manager.ExportToXlsxAsync(departments);
    }

    #endregion

    #region Course & Sections

        public async Task<byte[]> ExportCoursesAndSectionsToExcel(List<Section> sections)
        {
            var courses = await _courseService.GetAllCoursesAsync();
        
        //property manager 
        var manager = new PropertyManager<Section, Language>(new[]
        {
            new PropertyByName<Section, Language>("Id", (p, l) => p.Id),
            new PropertyByName<Section, Language>("Kod", (p, l) => courses.FirstOrDefault(x => x.Id == p.CourseId)?.Code),
            new PropertyByName<Section, Language>("Ad", (p, l) => courses.FirstOrDefault(x => x.Id == p.CourseId)?.Name),
            new PropertyByName<Section, Language>("Gün", (p, l) => TurkishDayConverter.ConvertToTurkishDay(p.Day)),
            new PropertyByName<Section, Language>("Başlangıç", (p, l) => p.StartTime.ToString()),
            new PropertyByName<Section, Language>("Bitiş", (p, l) => p.StartTime.ToString()),
            new PropertyByName<Section, Language>("Öğrenci Adet", (p, l) => p.StudentCount),
        }, _catalogSettings);

        return await manager.ExportToXlsxAsync(sections);
    }

    public async Task<byte[]> ExportCoursesAndSectionsToExcel()
    {
        var sections = new[]
        {
            new Section()
            {
            }
        };
        
        //property manager 
        var manager = new PropertyManager<Section, Language>(new[]
        {
            new PropertyByName<Section, Language>("Kod", (p, l) => ""),
            new PropertyByName<Section, Language>("Ad", (p, l) => ""),
            new PropertyByName<Section, Language>("Gün", (p, l) => ""),
            new PropertyByName<Section, Language>("Başlangıç", (p, l) => ""),
            new PropertyByName<Section, Language>("Bitiş", (p, l) => ""),
            new PropertyByName<Section, Language>("Öğrenci Adet", (p, l) => ""),        
        }, _catalogSettings);

        return await manager.ExportToXlsxAsync(sections);
    }

    #endregion
}