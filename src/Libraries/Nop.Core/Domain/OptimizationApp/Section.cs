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
}
