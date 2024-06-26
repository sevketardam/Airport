using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface IGlobalSettingsDAL : ISelectableRepo<GlobalSettings>, IUpdatetableRepo<GlobalSettings>, IDisposable;
