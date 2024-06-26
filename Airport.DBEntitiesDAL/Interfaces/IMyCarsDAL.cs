using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface IMyCarsDAL : ISelectableRepo<MyCars>, IEInsertableRepo<MyCars>, IUpdatetableRepo<MyCars>, IDeletableRepo<MyCars>, IDisposable;