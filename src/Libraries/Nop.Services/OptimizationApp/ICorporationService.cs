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

    Task<IList<Faculty>> GetAllFacultiesAsync(string name = null);

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
}