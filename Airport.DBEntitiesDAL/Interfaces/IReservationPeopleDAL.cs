using Airport.Data;
using Airport.DBEntities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntitiesDAL.Interfaces
{
    public interface IReservationPeopleDAL : ISelectableRepo<ReservationPeople>, IEInsertableRepo<ReservationPeople>, IUpdatetableRepo<ReservationPeople>, IDeletableRepo<ReservationPeople>, IDisposable
    {
    }
}
