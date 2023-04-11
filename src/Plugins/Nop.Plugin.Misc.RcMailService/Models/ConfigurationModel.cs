using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.RcMailService.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Misc.RcMailService.ApplicationName")]
        public string ApplicationName { get; set; }

        [NopResourceDisplayName("Plugins.Misc.RcMailService.From")]
        public string From { get; set; }

        [NopResourceDisplayName("Plugins.Misc.RcMailService.ServiceUrl")]
        public string ServiceUrl { get; set; }

    }
}
