using System;
using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Optimization;

public record OptimizationModel : BaseNopModel
{
    public OptimizationModel()
    {
        Classrooms = new List<ClassroomOptimizationModel>();
        Sections = new List<SectionOptimizationModel>();
    }
    
    public IList<ClassroomOptimizationModel> Classrooms { get; set; }
    public IList<SectionOptimizationModel> Sections { get; set; }
}

public record ClassroomOptimizationModel : BaseNopModel
{
    public int Id { get; set; }
    public int Capacity { get; set; }
    public string Name { get; set; }
}

public record SectionOptimizationModel : BaseNopModel
{
    public int Id { get; set; }
    public int Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int StudentCount { get; set; }
}