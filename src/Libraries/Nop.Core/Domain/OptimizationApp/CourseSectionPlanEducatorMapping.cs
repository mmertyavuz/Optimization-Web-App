namespace Nop.Core.Domain;

public class CourseSectionPlanEducatorMapping : BaseEntity
{
    /// <summary>
    /// The identifier of the associated educator for the course section plan mapping.
    /// </summary>
    public int EducatorId { get; set; }
    
    /// <summary>
    /// The identifier of the associated course section plan for the educator mapping.
    /// </summary>
    public int CourseSectionPlanId { get; set; }
}