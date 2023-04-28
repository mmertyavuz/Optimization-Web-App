using FluentValidation;
using Nop.Core.Domain;
using Nop.Data.Mapping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Corporations;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators;

public class FacultyValidator : BaseNopValidator<FacultyModel>
{
    public FacultyValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Corporations.Faculties.Fields.Name.Required"));
        
        SetDatabaseValidationRules<Faculty>(mappingEntityAccessor);
    }
}