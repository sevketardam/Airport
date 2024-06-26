using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface ILocationCarsFareDAL : ISelectableRepo<LocationCarsFare>, IEInsertableRepo<LocationCarsFare>, IUpdatetableRepo<LocationCarsFare>, IDeletableRepo<LocationCarsFare>, IDisposable;