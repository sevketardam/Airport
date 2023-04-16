using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.Data
{
    public interface ISelectableRepo<T> : IRepo<T> where T : class,IEntity
    {
        List<T> Select();
        T SelectByID(object Id);
        List<T> SelectByFunc(Func<T,bool> whereCondition);

    }
}
