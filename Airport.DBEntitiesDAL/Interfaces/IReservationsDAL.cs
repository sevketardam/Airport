using Airport.Data;
using Airport.DBEntities.Entities;
using System;

namespace Airport.DBEntitiesDAL.Interfaces;

public interface IReservationsDAL : ISelectableRepo<Reservations>, IEInsertableRepo<Reservations>, IUpdatetableRepo<Reservations>, IDeletableRepo<Reservations>, IDisposable;