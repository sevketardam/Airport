using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface ICarSeriesDAL : ISelectableRepo<CarSeries>, SelectableAsyncRepo<CarSeries>, IEInsertableRepo<CarSeries>, IUpdatetableRepo<CarSeries>, IDeletableRepo<CarSeries>, IDisposable;