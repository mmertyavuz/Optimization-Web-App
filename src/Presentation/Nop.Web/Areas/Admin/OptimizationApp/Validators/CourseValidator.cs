using FluentValidation;
using Nop.Data.Mapping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Education;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators;

public class CourseValidator : BaseNopValidator<CourseModel>
{
    public CourseValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required.");
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
        RuleFor(x => x.EducationalDepartmentId)
            .GreaterThan(0)
            .WithMessage("Department is required.");
    }
}