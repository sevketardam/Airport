using Airport.Data;
using Airport.DBEntities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntitiesDAL.Interfaces
{
    public interface IMyCarsDAL : ISelectableRepo<MyCars>, IEInsertableRepo<MyCars>, IUpdatetableRepo<MyCars>, IDeletableRepo<MyCars>, IDisposable
    {
    }
}
