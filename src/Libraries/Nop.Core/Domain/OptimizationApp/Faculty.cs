namespace Nop.Core.Domain;

public class Faculty : BaseEntity
{
    /// <summary>
    /// The name of the faculty (e.g., "Faculty of Engineering").
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// A brief description of the faculty.
    /// </summary>
    public string Description { get; set; }
}
