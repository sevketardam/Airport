using Airport.Data;
using Airport.DBEntities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntitiesDAL.Interfaces
{
    public interface ICarTrimsDAL : ISelectableRepo<CarTrims>, SelectableAsyncRepo<CarTrims>, IEInsertableRepo<CarTrims>, IUpdatetableRepo<CarTrims>, IDeletableRepo<CarTrims>, IDisposable
    {
    }
}
