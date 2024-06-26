using Airport.Data;
using Airport.DBEntities.Entities;
using System;
namespace Airport.DBEntitiesDAL.Interfaces;

public interface ILocationsDAL : ISelectableRepo<Locations>, IEInsertableRepo<Locations>, IUpdatetableRepo<Locations>, IDeletableRepo<Locations>, IDisposable;