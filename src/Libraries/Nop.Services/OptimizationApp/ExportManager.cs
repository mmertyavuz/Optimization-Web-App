using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain;
using Nop.Core.Domain.Localization;
using Nop.Services.ExportImport.Help;

namespace Nop.Services.ExportImport;

public partial interface IExportManager
{
    Task<byte[]> ExportClassroomsToExcel(IList<Classroom> classrooms);
    Task<byte[]> ExportClassroomsToExcel();

    Task<byte[]> ExportFacultiesToExcel(IList<Faculty> faculties);

    Task<byte[]> ExportFacultiesToExcel();
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
}