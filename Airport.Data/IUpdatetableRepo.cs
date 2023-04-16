using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.Data
{
    public interface IUpdatetableRepo<T> : IRepo<T> where T : class,IEntity
    {
        T Update(T updatedData);
    }
}
