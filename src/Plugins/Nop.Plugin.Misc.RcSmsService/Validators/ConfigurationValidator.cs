using FluentValidation;
using Nop.Plugin.Misc.RcSmsService.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.RcSmsService.Validators;

public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
{
    public ConfigurationValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.ApplicationName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.RcSmsService.ApplicationName.Required"));
        RuleFor(x => x.ServicePassword).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.RcSmsService.ServicePassword.Required"));
        RuleFor(x => x.ServiceUrl).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.RcSmsService.ServiceUrl.Required"));
    }
}