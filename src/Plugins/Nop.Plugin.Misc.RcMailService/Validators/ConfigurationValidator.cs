using FluentValidation;
using Nop.Plugin.Misc.RcMailService.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.RcMailService.Validators;

public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
{
    public ConfigurationValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.ApplicationName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.RcMailService.ApplicationName.Required"));
        RuleFor(x => x.ServiceUrl).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.RcMailService.ServiceUrl.Required"));
    
    }
}