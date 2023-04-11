using Nop.Core.Domain.Messages;
using Nop.Services.Logging;
using Nop.Services.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.RcSmsService.Services
{
    public class RcSmsService
    {
        private readonly RcSmsServiceSettings _rcSmsServiceSettings;

        public RcSmsService(RcSmsServiceSettings rcSmsServiceSettings)
        {
            _rcSmsServiceSettings = rcSmsServiceSettings;
        }

        public string GetServiceUrl()
        {
            return _rcSmsServiceSettings.ServiceUrl;
        }

        public string GetServicePassword()
        {
            return _rcSmsServiceSettings.ServicePassword;
        }
    }
}