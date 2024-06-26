using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface IServicesDAL : ISelectableRepo<Services>, IEInsertableRepo<Services>, IUpdatetableRepo<Services>, IDeletableRepo<Services>, IDisposable;