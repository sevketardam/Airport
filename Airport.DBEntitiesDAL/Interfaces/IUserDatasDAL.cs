using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface IUserDatasDAL : ISelectableRepo<UserDatas>, IEInsertableRepo<UserDatas>, IUpdatetableRepo<UserDatas>, IDeletableRepo<UserDatas>, IDisposable;

