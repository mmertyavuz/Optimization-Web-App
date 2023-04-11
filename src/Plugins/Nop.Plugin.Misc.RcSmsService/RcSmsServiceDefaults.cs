namespace Nop.Plugin.Misc.RcSmsService
{
    public static class RcSmsServiceDefaults
    {
        public const string VIEW_COMPONENT_NAME = "RcSmsService";

        public static string SystemName = "RC.SmsService";

        public static string ConfigurationRouteName => "Plugin.Misc.RcSmsService.Configure";

        public static (string Name, string Type, int Seconds) RcSmsServiceTask =>
            ($"RC Sms Service ({SystemName} Plugin)", "Nop.Plugin.Misc.RcSmsService.Services.RcSmsServiceTask", 60);
    }
}