using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.Data
{
    public  interface IEInsertableRepo<T> : IRepo<T> where T : class,IEntity
    {
        T Insert(T addedData);
        List<T> InsertRage(List<T> addedListData);
    }
}
