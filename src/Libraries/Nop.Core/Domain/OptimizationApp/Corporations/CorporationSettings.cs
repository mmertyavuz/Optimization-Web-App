using Nop.Core.Configuration;

namespace Nop.Core.Domain.Corporations;

public class CorporationSettings : ISettings
{
    public string CorporationName { get; set; }

    public string CorporationWebsite { get; set; }
}