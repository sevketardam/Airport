using Airport.Data;
using Airport.DBEntities.Entities;
using System;
namespace Airport.DBEntitiesDAL.Interfaces;

public interface IServicePropertiesDAL : ISelectableRepo<ServiceProperties>, IEInsertableRepo<ServiceProperties>, IUpdatetableRepo<ServiceProperties>, IDeletableRepo<ServiceProperties>, IDisposable;