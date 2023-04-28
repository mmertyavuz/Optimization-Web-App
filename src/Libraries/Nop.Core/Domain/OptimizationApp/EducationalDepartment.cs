namespace Nop.Core.Domain;

public class EducationalDepartment : BaseEntity
{
    /// <summary>
    /// The name of the educational department (e.g., "Computer Science").
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The unique code for the educational department (e.g., "CS").
    /// </summary>
    public string Code { get; set; }
    
    /// <summary>
    /// A brief description of the educational department.
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// The identifier of the associated faculty for the educational department.
    /// </summary>
    public int FacultyId { get; set; }
    
    /// <summary>
    /// The identifier of the customer user who leads the educational department.
    /// </summary>
    public int DepartmentLeadCustomerId { get; set; }
}