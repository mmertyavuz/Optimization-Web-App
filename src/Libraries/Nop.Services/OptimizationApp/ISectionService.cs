using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain;
using Nop.Data;

namespace Nop.Services.OptimizationApp;

public interface ISectionService
{
    Task<Section> GetSectionByIdAsync(int sectionId);

    Task<IList<Section>> GetAllSectionsAsync(
        int courseId = 0,
        string sectionNumber = null);

    Task InsertSectionAsync(Section section);

    Task UpdateSectionAsync(Section section);
    
    Task DeleteSectionAsync(Section section);
}

public class SectionService : ISectionService
{
    #region Fields

    private readonly IRepository<Section> _sectionRepository;

    #endregion

    #region Ctor

    public SectionService(IRepository<Section> sectionRepository)
    {
        _sectionRepository = sectionRepository;
    }

    #endregion
    
    public async Task<Section> GetSectionByIdAsync(int sectionId)
    {
        return await _sectionRepository.GetByIdAsync(sectionId);
    }

    public async Task<IList<Section>> GetAllSectionsAsync(int courseId = 0,
        string sectionNumber = null)
    {
        var query = _sectionRepository.Table;
    
        if (!string.IsNullOrEmpty(sectionNumber))
        {
            query = query.Where(s => s.SectionNumber == sectionNumber);
        }
        
        if (courseId != 0)
        {
            query = query.Where(s => s.CourseId == courseId);
        }
    
        return await query.ToListAsync();
    }

    public async Task InsertSectionAsync(Section section)
    {
        await _sectionRepository.InsertAsync(section);
    }

    public async Task UpdateSectionAsync(Section section)
    {
        await _sectionRepository.UpdateAsync(section);
    }
    
    public async Task DeleteSectionAsync(Section section)
    {
        await _sectionRepository.DeleteAsync(section);
    }
}
