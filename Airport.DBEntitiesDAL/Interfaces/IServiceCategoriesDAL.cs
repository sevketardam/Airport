using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface IServiceCategoriesDAL : ISelectableRepo<ServiceCategories>, IEInsertableRepo<ServiceCategories>, IUpdatetableRepo<ServiceCategories>, IDeletableRepo<ServiceCategories>, IDisposable;