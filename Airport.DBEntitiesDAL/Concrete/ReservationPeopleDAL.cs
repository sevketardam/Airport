﻿using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntitiesDAL.Concrete
{
    public class ReservationPeopleDAL : EfRepo<AirportContext, ReservationPeople>, IReservationPeopleDAL
    {
        public void Dispose()
        {

        }
    }
}
