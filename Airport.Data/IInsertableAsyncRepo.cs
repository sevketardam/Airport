using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airport.Data;

public interface IInsertableAsyncRepo<T> : IRepo<T> where T : class, IEntity
{
    Task InsertAsync(T addedData);
    Task InsertRangeAsync(List<T> addedListData);
}
