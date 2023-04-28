using System;

namespace Nop.Core.Domain;

public class Course : BaseEntity
{
    /// <summary>
    /// The unique code for the course (e.g., "CS101").
    /// </summary>
    public string Code { get; set; }
    
    /// <summary>
    /// The name of the course (e.g., "Introduction to Computer Science").
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// A brief description of the course content.
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// The number of credits assigned to the course by the institution.
    /// </summary>
    public int Credit { get; set; }
    
    /// <summary>
    /// The number of ECTS credits assigned to the course.
    /// </summary>
    public int Ects { get; set; }
    
    /// <summary>
    /// The identifier of the associated department for the course.
    /// </summary>
    public int EducationalDepartmentId { get; set; }
}