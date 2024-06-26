using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface IServiceItemsDAL : ISelectableRepo<ServiceItems>, IEInsertableRepo<ServiceItems>, IUpdatetableRepo<ServiceItems>, IDeletableRepo<ServiceItems>, IDisposable;