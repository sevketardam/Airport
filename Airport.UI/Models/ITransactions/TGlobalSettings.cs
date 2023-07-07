using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.Interface;

namespace Airport.UI.Models.ITransactions
{
    public class TGlobalSettings : IGlobalSettings
    {
        IGlobalSettingsDAL _globalSettings;
        public TGlobalSettings(IGlobalSettingsDAL globalSettings)
        {
            _globalSettings = globalSettings;
        }

        public GlobalSettings GetSettings()
        {
            return _globalSettings.SelectByID(1);
        }
    }
}
