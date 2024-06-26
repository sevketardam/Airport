using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface IUserDocsDAL : ISelectableRepo<UserDocs>, IEInsertableRepo<UserDocs>, IUpdatetableRepo<UserDocs>, IDeletableRepo<UserDocs>, IDisposable
{
}
