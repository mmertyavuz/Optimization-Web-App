using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain;

namespace Nop.Services.OptimizationApp;

public interface ICorporationService
{
    #region Faculty

    Task DeleteFacultyAsync(Faculty faculty);

    Task<Faculty> GetFacultyByIdAsync(int facultyId);

    Task<IList<Faculty>> GetAllFacultiesAsync(string name = null, bool showOnlyWithoutDepartment = false);

    Task InsertFacultyAsync(Faculty faculty);

    Task UpdateFacultyAsync(Faculty faculty);

    #endregion

    #region Educational Department

    Task DeleteEducationalDepartmentAsync(EducationalDepartment educationalDepartment);

    Task<EducationalDepartment> GetEducationalDepartmentByIdAsync(int educationalDepartmentId);

    Task<IList<EducationalDepartment>> GetAllEducationalDepartmentsAsync(
        int facultyId = 0,
        string name = null,
        string code = null);

    Task InsertEducationalDepartmentAsync(EducationalDepartment educationalDepartment);

    Task UpdateEducationalDepartmentAsync(EducationalDepartment educationalDepartment);

    #endregion

    #region Classroom

    Task DeleteClassroomAsync(Classroom classroom);

    Task<Classroom> GetClassroomByIdAsync(int classroomId);

    Task<IList<Classroom>> GetAllClassroomsAsync(
        string name = null,
        int minCapacity = 0,
        int maxCapacity = 0,
        bool orderByCapacity = false);

    Task InsertClassroomAsync(Classroom classroom);

    Task UpdateClassroomAsync(Classroom classroom);


    #endregion
}