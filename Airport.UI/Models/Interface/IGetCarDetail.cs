using Airport.DBEntities.Entities;
using Airport.UI.Models.VM;
using System;
using System.Collections.Generic;

namespace Airport.UI.Models.Interface
{
    public interface IGetCarDetail 
    {
        List<MyCars> GetCarsDetail(int[] myCarsId);
    }
}
