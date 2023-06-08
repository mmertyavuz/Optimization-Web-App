using System;

namespace Nop.Core.Domain;

public class Section : BaseEntity
{
    /// <summary>
    /// The unique section number for the course section (e.g., "01").
    /// </summary>
    public string SectionNumber { get; set; }

    /// <summary>
    /// The identifier of the associated course for the section.
    /// </summary>
    public int CourseId { get; set; }
    
    /// <summary>
    /// The day of the week when the course section is scheduled.
    /// </summary>
    public DayOfWeek Day
    {
        get => (DayOfWeek)DayId;
        set => DayId = (int)value;
    }

    public int DayId { get; set; }
    
    /// <summary>
    /// The starting time of the course section on the scheduled day.
    /// </summary>
    public TimeSpan StartTime { get; set; }
    
    /// <summary>
    /// The ending time of the course section on the scheduled day.
    /// </summary>
    public TimeSpan EndTime { get; set; }
    
    public int StudentCount { get; set; }
}
