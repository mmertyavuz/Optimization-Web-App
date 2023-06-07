using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain;
using Nop.Data;

namespace Nop.Services.OptimizationApp;

public interface ICourseService
{
    Task<Course> GetCourseByIdAsync(int courseId);

    Task<IList<Course>> GetAllCoursesAsync(
        int departmentId = 0,
        string name = null,
        string code = null);

    Task InsertCourseAsync(Course course);

    Task UpdateCourseAsync(Course course);

    Task DeleteCourseAsync(Course course);

    Task DeleteAllCoursesAsync();
}

public class CourseService : ICourseService
{
    #region Fields

    private readonly IRepository<Course> _courseRepository;

    #endregion

    #region Ctor

    public CourseService(IRepository<Course> courseRepository)
    {
        _courseRepository = courseRepository;
    }

    #endregion
    
    public async Task<Course> GetCourseByIdAsync(int courseId)
    {
        return await _courseRepository.GetByIdAsync(courseId);
    }

    public async Task<IList<Course>> GetAllCoursesAsync(int departmentId = 0, string name = null, string code = null)
    {
        var query = _courseRepository.Table;
    
        if (departmentId != 0)
        {
            query = query.Where(c => c.EducationalDepartmentId == departmentId);
        }
    
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(c => c.Name.Contains(name));
        }
    
        if (!string.IsNullOrEmpty(code))
        {
            query = query.Where(c => c.Code.Contains(code));
        }
    
        return await query.ToListAsync();
    }

    public async Task InsertCourseAsync(Course course)
    {
        await _courseRepository.InsertAsync(course);
    }

    public async Task UpdateCourseAsync(Course course)
    {
        await _courseRepository.UpdateAsync(course);
    }
    
    public async Task DeleteCourseAsync(Course course)
    { 
        await _courseRepository.DeleteAsync(course);
    }

    public async Task DeleteAllCoursesAsync()
    {
        await _courseRepository.TruncateAsync();
    }
}