using Nop.Core.Domain.Messages;
using Nop.Services.Logging;
using Nop.Services.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.RcMailService.Services
{
    public class RcEmailService
    {
        private readonly RcMailServiceSettings _rcMailServiceSettings;
       
        public RcEmailService(RcMailServiceSettings rcMailServiceSettings)
        {
            _rcMailServiceSettings = rcMailServiceSettings;
        }

        public string GetServiceUrl()
        {
            return _rcMailServiceSettings.ServiceUrl;
        }
    }
}
