using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface ILocationCarsDAL : ISelectableRepo<LocationCars>, IEInsertableRepo<LocationCars>, IUpdatetableRepo<LocationCars>, IDeletableRepo<LocationCars>, IDisposable;
