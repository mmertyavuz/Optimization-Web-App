using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain;
using Nop.Data;

namespace Nop.Services.OptimizationApp;

public class CorporationService : ICorporationService
{
    #region Fields

    private readonly IRepository<Faculty> _facultyRepository;
    private readonly IRepository<EducationalDepartment> _educationalDepartmentRepository;
    private  readonly IRepository<Classroom> _classroomRepository;
    private readonly CorporationSettings _corporationSettings;

    #endregion

    #region Ctor

    public CorporationService(IRepository<Faculty> facultyRepository, IRepository<EducationalDepartment> educationalDepartmentRepository, IRepository<Classroom> classroomRepository, CorporationSettings corporationSettings)
    {
        _facultyRepository = facultyRepository;
        _educationalDepartmentRepository = educationalDepartmentRepository;
        _classroomRepository = classroomRepository;
        _corporationSettings = corporationSettings;
    }

    #endregion

    #region Faculty

    public async Task DeleteFacultyAsync(Faculty faculty)
    {
        await _facultyRepository.DeleteAsync(faculty);
    }

    public async Task<Faculty> GetFacultyByIdAsync(int facultyId)
    {
        return await _facultyRepository.GetByIdAsync(facultyId);
    }

    public async Task<IList<Faculty>> GetAllFacultiesAsync(string name = null, bool showOnlyWithoutDepartment = false)
    {
        var departments = _educationalDepartmentRepository.Table;
        
        var query = _facultyRepository.Table;

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(f => f.Name.Contains(name));
        }

        if (showOnlyWithoutDepartment)
        {
            query = query.Where(f => !departments.Any(d => d.FacultyId == f.Id));
        }

        return await query.ToListAsync();
    }

    public async Task InsertFacultyAsync(Faculty faculty)
    {
        await _facultyRepository.InsertAsync(faculty);
    }

    public async Task UpdateFacultyAsync(Faculty faculty)
    {
        await _facultyRepository.UpdateAsync(faculty);
    }
    
    public async Task DeleteAllFacultiesAsync()
    {
        await _facultyRepository.TruncateAsync(resetIdentity: true);
    }

    #endregion

    #region Educational Department

    public async Task DeleteEducationalDepartmentAsync(EducationalDepartment educationalDepartment)
    {
        await _educationalDepartmentRepository.DeleteAsync(educationalDepartment);
    }

    public async Task<EducationalDepartment> GetEducationalDepartmentByIdAsync(int educationalDepartmentId)
    {
        return await _educationalDepartmentRepository.GetByIdAsync(educationalDepartmentId);
    }

    public async Task<IList<EducationalDepartment>> GetAllEducationalDepartmentsAsync(int facultyId = 0, string name = null, string code = null)
    {
        var query = _educationalDepartmentRepository.Table;
        
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(ed => ed.Name.Contains(name));
        }

        if (!string.IsNullOrEmpty(code))
        {
            query = query.Where(ed => ed.Code.ToUpper().Contains(code.ToUpper()));
        }

        if (facultyId > 0)
        {
            query = query.Where(ed => ed.FacultyId == facultyId);
        }

        return await query.ToListAsync();
    }

    public async Task InsertEducationalDepartmentAsync(EducationalDepartment educationalDepartment)
    {
        await _educationalDepartmentRepository.InsertAsync(educationalDepartment);
    }

    public async Task UpdateEducationalDepartmentAsync(EducationalDepartment educationalDepartment)
    {
        await _educationalDepartmentRepository.UpdateAsync(educationalDepartment);
    }

    public async Task DeleteAllEducationalDepartmentsAsync()
    {
        await _educationalDepartmentRepository.TruncateAsync(resetIdentity: true);
    }
    
    #endregion

    #region Classroom

    public async Task DeleteClassroomAsync(Classroom classroom)
    {
        await _classroomRepository.DeleteAsync(classroom);
    }

    public async Task<Classroom> GetClassroomByIdAsync(int classroomId)
    {
        return await _classroomRepository.GetByIdAsync(classroomId);
    }

    public async Task<IList<Classroom>> GetAllClassroomsAsync(string name = null, int minCapacity = 0, int maxCapacity = 0, bool orderByCapacity = false)
    {
        var query = _classroomRepository.Table;
        
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(ed => ed.Name.Contains(name));
        }
        if (minCapacity > 0)
        {
            query = query.Where(ed => ed.Capacity >= minCapacity);
        }
        if (maxCapacity > 0)
        {
            query = query.Where(ed => ed.Capacity <= maxCapacity);
        }
        if (orderByCapacity)
        {
            query = query.OrderBy(x => x.Capacity);
        }
        else
        {
            query = query.OrderBy(x => x.Name);
        }

        return await query.ToListAsync();
    }

    public async Task InsertClassroomAsync(Classroom classroom)
    {
        await _classroomRepository.InsertAsync(classroom);
    }

    public async Task UpdateClassroomAsync(Classroom classroom)
    {
        await _classroomRepository.UpdateAsync(classroom);
    }
    
    public async Task<bool> IsThereAnyClassroomAsync()
    {
        return await _classroomRepository.Table.AnyAsync();
    }
    
    public async Task DeleteAllClassroomsAsync()
    {
        await _classroomRepository.TruncateAsync(resetIdentity: true);
    }

    #endregion

    public bool IsOptimizationKeyValid(string key)
    {
        if (string.IsNullOrEmpty(_corporationSettings.OptimizationKey))
        {
            return true;
        }
        return key == _corporationSettings.OptimizationKey;        
    }
}