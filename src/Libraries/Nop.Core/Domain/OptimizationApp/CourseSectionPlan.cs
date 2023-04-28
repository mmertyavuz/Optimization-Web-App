using System;

namespace Nop.Core.Domain;

public class CourseSectionPlan : BaseEntity
{
    /// <summary>
    /// The identifier of the associated section for the course section plan.
    /// </summary>
    public int SectionId { get; set; }
    
    /// <summary>
    /// The day of the week when the course section is scheduled.
    /// </summary>
    public DayOfWeek Day { get; set; }
    
    /// <summary>
    /// The starting time of the course section on the scheduled day.
    /// </summary>
    public TimeSpan StartTime { get; set; }
    
    /// <summary>
    /// The ending time of the course section on the scheduled day.
    /// </summary>
    public TimeSpan EndTime { get; set; }
    
    /// <summary>
    /// The start date of the course section plan.
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// The end date of the course section plan.
    /// </summary>
    public DateTime EndDate { get; set; }
}