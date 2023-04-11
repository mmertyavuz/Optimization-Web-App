using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.RcMailService
{
    public static class RcMailServiceDefaults
    {
        public const string VIEW_COMPONENT_NAME = "RcMailService";

        public static string SystemName = "RC.MailService";

        public static string ConfigurationRouteName => "Plugin.Misc.RcMailService.Configure";

        public static (string Name, string Type, int Seconds) RcMailServiceTask =>
            ($"RC Mail Service ({SystemName} Plugin)", "Nop.Plugin.Misc.RcMailService.Services.RcMailServiceTask", 60);
    }
}
