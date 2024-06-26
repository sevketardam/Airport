using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface IReservationServicesTableDAL : ISelectableRepo<ReservationServicesTable>, IEInsertableRepo<ReservationServicesTable>, IUpdatetableRepo<ReservationServicesTable>, IDeletableRepo<ReservationServicesTable>, IDisposable;