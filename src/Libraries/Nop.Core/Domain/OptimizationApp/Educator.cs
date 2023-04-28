namespace Nop.Core.Domain;

public class Educator : BaseEntity
{
    /// <summary>
    /// The first name of the educator.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The last name of the educator.
    /// </summary>
    public string Surname { get; set; }
    
    /// <summary>
    /// The email address of the educator.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// The identifier of the associated department for the educator.
    /// </summary>
    public int DepartmentId { get; set; }
    
    /// <summary>
    /// The identifier of the customer user associated with the educator.
    /// </summary>
    public int CustomerId { get; set; }
}
