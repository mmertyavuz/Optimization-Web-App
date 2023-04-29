using FluentValidation;
using Nop.Core.Domain;
using Nop.Data.Mapping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Corporations;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators;

public class ClassroomValidator : BaseNopValidator<ClassroomModel>
{
    public ClassroomValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Corporations.Classrooms.Fields.Name.Required"));
        
        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Corporations.Classrooms.Fields.Capacity.Required"));
        
        SetDatabaseValidationRules<Classroom>(mappingEntityAccessor);
    }
}