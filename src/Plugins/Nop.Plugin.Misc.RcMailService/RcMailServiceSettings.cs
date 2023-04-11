using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.RcMailService
{
    public class RcMailServiceSettings : ISettings
    {
        public string ApplicationName { get; set; }
        public string From { get; set; }
        public string ServiceUrl { get; set; }
    }
}
