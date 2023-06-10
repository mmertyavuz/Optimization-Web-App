using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Data;


namespace Nop.Services.OptimizationApp;

public interface IOptimizationResultService
{
    Task<List<Section>> GetSectionsByClassroomIdAsync(int classroomId);

    Task<IPagedList<OptimizationResult>> GetAllOptimizedSectionsAsync(
        int FacultyId = 0, //Faculty Level
        int EducationalDepartmentId = 0, //Educational Department Level
        int ClassroomId = 0, //Classroom Level
        int CourseId = 0, //Section Level
        int DayId = 0, //Section Level
        int MinStudentCount = 0, //Section Level
        int MaxStudentCount = 0, //Section Level
        TimeSpan? StartDate = default, //Section Level
        TimeSpan? EndDate = default, //Section Level
        int pageIndex = 0,
        int pageSize = int.MaxValue);

    Task<IList<Section>> GetOptimizedSectionsAsync(
        int FacultyId = 0, //Faculty Level
        int EducationalDepartmentId = 0, //Educational Department Level
        int ClassroomId = 0, //Classroom Level
        int CourseId = 0, //Section Level
        int DayId = 0, //Section Level
        int MinStudentCount = 0, //Section Level
        int MaxStudentCount = 0, //Section Level
        TimeSpan? StartDate = null, //Section Level
        TimeSpan? EndDate = null, //Section Level
        int pageIndex = 0,
        int pageSize = int.MaxValue);

    Task<OptimizationResult> GetOptimizationResultByIdAsync(int id);    
}

public class OptimizationResultService : IOptimizationResultService
{
    #region Fields

    private readonly IRepository<OptimizationResult> _optimizationResultRepository;
    private readonly IRepository<Section> _sectionRepository;
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<EducationalDepartment> _educationalDepartmentRepository;
    private readonly IRepository<Faculty> _facultyRepository;

    public OptimizationResultService(IRepository<OptimizationResult> optimizationResultRepository, IRepository<Section> sectionRepository, IRepository<Course> courseRepository, IRepository<EducationalDepartment> educationalDepartmentRepository, IRepository<Faculty> facultyRepository)
    {
        _optimizationResultRepository = optimizationResultRepository;
        _sectionRepository = sectionRepository;
        _courseRepository = courseRepository;
        _educationalDepartmentRepository = educationalDepartmentRepository;
        _facultyRepository = facultyRepository;
    }

    #endregion

    public async Task<List<Section>> GetSectionsByClassroomIdAsync(int classroomId)
    {
        var query = from section in _sectionRepository.Table
                    join optimizationResult in _optimizationResultRepository.Table on section.Id equals optimizationResult.SectionId
                    where optimizationResult.ClassroomId == classroomId
                    select section;
        
        query = query.OrderBy(x => x.DayId).ThenBy(x => x.StartTime);
        
        return await query.ToListAsync();
    }

    public async Task<IPagedList<OptimizationResult>> GetAllOptimizedSectionsAsync( 
    int FacultyId = 0, //Faculty Level
    int EducationalDepartmentId = 0, //Educational Department Level
    int ClassroomId = 0, //Classroom Level
    int CourseId = 0, //Section Level
    int DayId = 0, //Section Level
    int MinStudentCount = 0, //Section Level
    int MaxStudentCount = 0, //Section Level
    TimeSpan? StartDate = null , //Section Level
    TimeSpan? EndDate = null, //Section Level
    int pageIndex = 0,
    int pageSize = int.MaxValue)
    {
        var query = _optimizationResultRepository.Table;

        if (ClassroomId > 0)
            query = query.Where(x => x.ClassroomId == ClassroomId);
        
        var joinQuery = from section in _sectionRepository.Table
                        join optimizationResult in query on section.Id equals optimizationResult.SectionId
                        select new { section, optimizationResult };

        if (CourseId > 0)
            joinQuery = joinQuery.Where(x => x.section.CourseId == CourseId);
        
        if (MinStudentCount > 0)   
            joinQuery = joinQuery.Where(x => x.section.StudentCount >= MinStudentCount);
        
        if (MaxStudentCount > 0)
            joinQuery = joinQuery.Where(x => x.section.StudentCount <= MaxStudentCount);

        if (DayId > 0)
            joinQuery = joinQuery.Where(x => x.section.DayId == DayId);
        
        if (StartDate.HasValue && StartDate.Value != TimeSpan.Zero)
            joinQuery = joinQuery.Where(x => x.section.StartTime >= StartDate);
        
        if (EndDate.HasValue && EndDate.Value != TimeSpan.Zero)
            joinQuery = joinQuery.Where(x => x.section.EndTime <= EndDate);
        
        query = query.Where(x => joinQuery.Select(y => y.optimizationResult.Id).Contains(x.Id));
        
        if (CourseId > 0 || EducationalDepartmentId > 0 || FacultyId > 0)
        {
            var corporationQuery = from x in joinQuery
                                   join course in _courseRepository.Table on x.section.CourseId equals course.Id
                                   join educationalDepartment in _educationalDepartmentRepository.Table on course.EducationalDepartmentId equals educationalDepartment.Id
                                   join faculty in _facultyRepository.Table on educationalDepartment.FacultyId equals faculty.Id
                                   select new { x, course, educationalDepartment, faculty };
            
            if (EducationalDepartmentId > 0)
                corporationQuery = corporationQuery.Where(x => x.educationalDepartment.Id == EducationalDepartmentId);
            
            if (FacultyId > 0) 
                corporationQuery = corporationQuery.Where(x => x.faculty.Id == FacultyId);
            
            if (ClassroomId > 0) 
                corporationQuery = corporationQuery.Where(x => x.course.Id == ClassroomId);
            
            query = query.Where(x => corporationQuery.Select(y => y.x.optimizationResult.Id).Contains(x.Id));
        }

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }
    
    public async Task<IList<Section>> GetOptimizedSectionsAsync( 
    int FacultyId = 0, //Faculty Level
    int EducationalDepartmentId = 0, //Educational Department Level
    int ClassroomId = 0, //Classroom Level
    int CourseId = 0, //Section Level
    int DayId = 0, //Section Level
    int MinStudentCount = 0, //Section Level
    int MaxStudentCount = 0, //Section Level
    TimeSpan? StartDate = null , //Section Level
    TimeSpan? EndDate = null, //Section Level
    int pageIndex = 0,
    int pageSize = int.MaxValue)
    {
        var query = _optimizationResultRepository.Table;

        if (ClassroomId > 0)
            query = query.Where(x => x.ClassroomId == ClassroomId);
        
        var joinQuery = from section in _sectionRepository.Table
                        join optimizationResult in query on section.Id equals optimizationResult.SectionId
                        select new { section, optimizationResult };

        if (CourseId > 0)
            joinQuery = joinQuery.Where(x => x.section.CourseId == CourseId);
        
        if (MinStudentCount > 0)   
            joinQuery = joinQuery.Where(x => x.section.StudentCount >= MinStudentCount);
        
        if (MaxStudentCount > 0)
            joinQuery = joinQuery.Where(x => x.section.StudentCount <= MaxStudentCount);

        if (DayId > 0)
            joinQuery = joinQuery.Where(x => x.section.DayId == DayId);
        
        if (StartDate.HasValue && StartDate.Value != TimeSpan.Zero)
            joinQuery = joinQuery.Where(x => x.section.StartTime >= StartDate);
        
        if (EndDate.HasValue && EndDate.Value != TimeSpan.Zero)
            joinQuery = joinQuery.Where(x => x.section.EndTime <= EndDate);
        
        query = query.Where(x => joinQuery.Select(y => y.optimizationResult.Id).Contains(x.Id));
        
        if (CourseId > 0 || EducationalDepartmentId > 0 || FacultyId > 0)
        {
            var corporationQuery = from x in joinQuery
                                   join course in _courseRepository.Table on x.section.CourseId equals course.Id
                                   join educationalDepartment in _educationalDepartmentRepository.Table on course.EducationalDepartmentId equals educationalDepartment.Id
                                   join faculty in _facultyRepository.Table on educationalDepartment.FacultyId equals faculty.Id
                                   select new { x, course, educationalDepartment, faculty };
            
            if (EducationalDepartmentId > 0)
                corporationQuery = corporationQuery.Where(x => x.educationalDepartment.Id == EducationalDepartmentId);
            
            if (FacultyId > 0) 
                corporationQuery = corporationQuery.Where(x => x.faculty.Id == FacultyId);
            
            if (ClassroomId > 0) 
                corporationQuery = corporationQuery.Where(x => x.course.Id == ClassroomId);
            
            query = query.Where(x => corporationQuery.Select(y => y.x.optimizationResult.Id).Contains(x.Id));
        }
        
        var lastQuery = from section in _sectionRepository.Table
            join optimizationResult in query on section.Id equals optimizationResult.SectionId
            select section;
        
        return lastQuery.ToList();
    }
    
    public async Task<OptimizationResult> GetOptimizationResultByIdAsync(int id)
    {
        return await _optimizationResultRepository.GetByIdAsync(id);
    }

}