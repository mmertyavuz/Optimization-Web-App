using FluentValidation;
using Nop.Core.Domain;
using Nop.Data.Mapping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Corporations;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators;

public class EducationalDepartmentValidator : BaseNopValidator<EducationalDepartmentModel>
{
    public EducationalDepartmentValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Corporations.EducationalDepartments.Fields.Name.Required"));
        
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Corporations.EducationalDepartments.Fields.Code.Required"));
        
        RuleFor(x => x.FacultyId)
            .GreaterThan(0)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Corporations.EducationalDepartments.Fields.FacultyId.Required"));
        
        SetDatabaseValidationRules<EducationalDepartment>(mappingEntityAccessor);
    }
}