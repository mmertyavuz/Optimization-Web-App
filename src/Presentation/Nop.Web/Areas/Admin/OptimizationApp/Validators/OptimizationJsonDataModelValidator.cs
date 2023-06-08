using FluentValidation;
using Nop.Data.Mapping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.OptimizationApp.Models;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators;

public class OptimizationJsonDataModelValidator : BaseNopValidator<OptimizationJsonDataModel>
{
    public OptimizationJsonDataModelValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
    {
        RuleFor(x => x.JsonValue)
            .NotEmpty();
    }
}