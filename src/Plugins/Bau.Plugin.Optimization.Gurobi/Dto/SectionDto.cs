using System;
using Nop.Web.Framework.Models;

namespace Bau.Plugin.Optimization.Gurobi.Domain;

public record SectionDto : BaseNopEntityModel
{
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int StudentCount { get; set; }
}