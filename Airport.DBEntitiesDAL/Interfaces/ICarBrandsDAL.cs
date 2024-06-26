using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface ICarBrandsDAL : ISelectableRepo<CarBrands>, IEInsertableRepo<CarBrands>, IUpdatetableRepo<CarBrands>, IDeletableRepo<CarBrands>, IDisposable;
