namespace Nop.Core.Domain;

public class StudentSectionMapping : BaseEntity
{
    /// <summary>
    /// The identifier of the associated student for the student-section mapping.
    /// </summary>
    public int StudentId { get; set; }
    
    /// <summary>
    /// The identifier of the associated section for the student-section mapping.
    /// </summary>
    public int SectionId { get; set; }
}