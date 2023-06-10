using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Corporations;
using Nop.Web.Areas.Admin.Models.Education;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.OptimizationApp.Models;

public record OptimizationResultModel : BaseNopEntityModel
{
    public OptimizationResultModel()
    {
        FacultyModel = new FacultyModel();
        EducationalDepartmentModel = new EducationalDepartmentModel();
        CourseModel = new CourseModel();
        ClassroomModel = new ClassroomModel();
        SectionModel = new SectionModel();
    }
    
    public int ClassroomId { get; set; }
    public int SectionId { get; set; }

    public FacultyModel FacultyModel { get; set; }
    public EducationalDepartmentModel EducationalDepartmentModel { get; set; }
    public CourseModel CourseModel { get; set; }
    public ClassroomModel ClassroomModel { get; set; }
    public SectionModel SectionModel { get; set; }

    #region List Props

    public string CourseCode { get; set; }
    public string CourseName { get; set; }
    public string Classroom { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int StudentCount { get; set; }
    public string Day { get; set; }

    #endregion
}

public record OptimizationResultListModel : BasePagedListModel<OptimizationResultModel>
{
    
}

public record OptimizationResultSearchModel : BaseSearchModel
{
    public OptimizationResultSearchModel()
    {
        AvailableFaculties = new List<SelectListItem>();
        AvailableEducationalDepartments = new List<SelectListItem>();
        AvailableCourses = new List<SelectListItem>();
        AvailableClassrooms = new List<SelectListItem>();
        AvailableDays = new List<SelectListItem>();
    }
    public int FacultyId { get; set; }

    public int EducationalDepartmentId { get; set; }

    public int CourseId { get; set; }
    
    public int ClassroomId { get; set; }
    
    public int DayId { get; set; }

    public int MinStudentCount { get; set; }
    public int MaxStudentCount { get; set; }
    

    public TimeSpan StartDate { get; set; }

    public TimeSpan EndDate { get; set; }

    public IList<SelectListItem> AvailableFaculties { get; set; }

    public IList<SelectListItem> AvailableEducationalDepartments { get; set; }
    
    public IList<SelectListItem> AvailableCourses { get; set; }

    public IList<SelectListItem> AvailableClassrooms { get; set; }

    public IList<SelectListItem> AvailableDays { get; set; }
}