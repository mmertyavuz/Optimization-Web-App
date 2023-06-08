using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.OptimizationApp;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Optimization;

namespace Nop.Web.Controllers;

public class ApiController : BasePublicController
{
    #region Fields

    private readonly ICorporationService _corporationService;
    private readonly ISectionService _sectionService;

    public ApiController(ICorporationService corporationService, ISectionService sectionService)
    {
        _corporationService = corporationService;
        _sectionService = sectionService;
    }

    #endregion
    
    public class OptimizationRequest
    {
        public string OptimizationKey { get; set; }
    }

    [CheckAccessPublicStore(ignore: true)]
    [HttpPost]
    public virtual async Task<IActionResult> GetOptimizationData()
    {
        // var isKeyValid = _corporationService.IsOptimizationKeyValid(request.OptimizationKey);
        //
        // if (!isKeyValid)
        //     return BadRequest("Optimization key is not valid.");

        var model = new OptimizationModel();

        var classrooms = await _corporationService.GetAllClassroomsAsync();
        
        foreach (var classroom in classrooms)
        {
            var classroomModel = new ClassroomOptimizationModel()
            {
                Id = classroom.Id,
                Name = classroom.Name,
                Capacity = classroom.Capacity
            };

            model.Classrooms.Add(classroomModel);
        }

        var sections = await _sectionService.GetAllSectionsAsync();
        foreach (var section in sections)
        {
            var sectionModel = new SectionOptimizationModel()
            {
                Id = section.Id,
                Day = (int)section.Day,
                StartTime = section.StartTime,
                EndTime = section.EndTime,
                StudentCount = section.StudentCount
            };

            model.Sections.Add(sectionModel);
        }

        return Ok(model);
    }
}