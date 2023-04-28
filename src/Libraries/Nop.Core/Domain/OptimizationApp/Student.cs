namespace Nop.Core.Domain;

public class Student : BaseEntity
{
    /// <summary>
    /// The unique student number for the student (e.g., "1234567").
    /// </summary>
    public string StudentNumber { get; set; }
    
    /// <summary>
    /// The first name of the student.
    /// </summary>
    public string FirstName { get; set; }
    
    /// <summary>
    /// The last name of the student.
    /// </summary>
    public string LastName { get; set; }
    
    /// <summary>
    /// The email address of the student.
    /// </summary>
    public string Email { get; set; }
    
    /// <summary>
    /// The identifier of the associated department for the student.
    /// </summary>
    public int DepartmentId { get; set; }
    
    /// <summary>
    /// The identifier of the customer user associated with the student.
    /// </summary>
    public int CustomerId { get; set; }
}