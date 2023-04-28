namespace Nop.Core.Domain;

public class Classroom : BaseEntity
{
    /// <summary>
    /// The unique name or identifier for the classroom (e.g., "A101").
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// A brief description of the classroom, which could include information about its features or location.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The maximum number of students that can be accommodated in the classroom.
    /// </summary>
    public int Capacity { get; set; }
}

