using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.RcSmsService
{
    public class RcSmsServiceSettings : ISettings
    {
        public string ApplicationName { get; set; }

        public string ServicePassword { get; set; }

        public string ServiceUrl { get; set; }
    }
}