using Nop.Core.Configuration;

namespace Nop.Core.Domain;

public class CorporationSettings : ISettings
{
    /// <summary>
    /// The name of the corporation or institution.
    /// </summary>
    public string CorporationName { get; set; }

    /// <summary>
    /// The corporation's website URL.
    /// </summary>
    public string CorporationWebsite { get; set; }
    
    /// <summary>
    /// The URL of the corporation's logo.
    /// </summary>
    public string LogoUrl { get; set; }
}