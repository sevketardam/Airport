using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.Interface;

namespace Airport.UI.Models.ITransactions;

public class TGlobalSettings(IGlobalSettingsDAL globalSettings) : IGlobalSettings
{
    public GlobalSettings GetSettings()
    {
        return globalSettings.SelectByID(1);
    }
}
