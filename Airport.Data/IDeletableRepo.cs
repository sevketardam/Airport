using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.Data
{
    public interface IDeletableRepo<T> : IRepo<T> where T : class, IEntity
    {
        void HardDelete(T deletedData);
        void SoftDelete(T deletedData);
    }
}
