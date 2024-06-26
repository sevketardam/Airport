using Airport.Data;
using Airport.DBEntities.Entities;
using System;
namespace Airport.DBEntitiesDAL.Interfaces;

public interface IReservationPeopleDAL : ISelectableRepo<ReservationPeople>, IEInsertableRepo<ReservationPeople>, IUpdatetableRepo<ReservationPeople>, IDeletableRepo<ReservationPeople>, IDisposable;