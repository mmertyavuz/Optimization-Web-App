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

    #endregion

    #region Ctor

    public CorporationService(IRepository<Faculty> facultyRepository, IRepository<EducationalDepartment> educationalDepartmentRepository, IRepository<Classroom> classroomRepository)
    {
        _facultyRepository = facultyRepository;
        _educationalDepartmentRepository = educationalDepartmentRepository;
        _classroomRepository = classroomRepository;
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

    public async Task<IList<Faculty>> GetAllFacultiesAsync(string name = null)
    {
        var query = _facultyRepository.Table;

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(f => f.Name.Contains(name));
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
            query = query.Where(ed => ed.Code.Contains(code));
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

    public async Task<IList<Classroom>> GetAllClassroomsAsync(string name = null, int minCapacity = 0, int maxCapacity = 0)
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

    #endregion
}