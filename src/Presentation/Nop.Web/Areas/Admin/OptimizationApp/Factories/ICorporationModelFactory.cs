using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using Nop.Core.Domain.Corporations;
using Nop.Web.Areas.Admin.Models.Corporations;

namespace Nop.Web.Areas.Admin.Factories;

public interface ICorporationModelFactory
{
    public CorporationSettingsModel PrepareCorporationSettingsModel(CorporationSettingsModel model);
}

public class CorporationModelFactory : ICorporationModelFactory
{
    #region Fields

    private readonly CorporationSettings _corporationSettings;

    #endregion

    #region Ctor

    public CorporationModelFactory(CorporationSettings corporationSettings)
    {
        _corporationSettings = corporationSettings;
    }

    #endregion

    public CorporationSettingsModel PrepareCorporationSettingsModel(CorporationSettingsModel model)
    {
        if (model is null)
        {
            model = new CorporationSettingsModel
            {
                CorporationName = _corporationSettings.CorporationName,
                CorporationWebsite = _corporationSettings.CorporationWebsite
            };
        }

        return model;
    }
}