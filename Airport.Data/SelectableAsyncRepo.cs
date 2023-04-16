using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.Data
{
    public interface SelectableAsyncRepo<T> : IRepo<T> where T : class,IEntity
    {
        Task<List<T>> SelectAsync();
        Task<T> SelectByAsync(object Id);

    }
}
