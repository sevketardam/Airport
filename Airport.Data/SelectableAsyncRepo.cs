using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airport.Data;

public interface SelectableAsyncRepo<T> : IRepo<T> where T : class,IEntity
{
    Task<List<T>> SelectAsync();
    Task<T> SelectByAsync(object Id);

}
