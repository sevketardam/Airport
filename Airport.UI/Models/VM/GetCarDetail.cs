using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.Interface;
using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class GetCarDetail : IGetCarDetail
    {
        IMyCarsDAL _myCars;
        ICarBrandsDAL _myBrands;
        ICarClassesDAL _myClasses;
        ICarModelsDAL _myModels;
        ICarSeriesDAL _mySeries;
        ICarTrimsDAL _myTrims;
        ICarTypesDAL _myTypes;

        public GetCarDetail(IMyCarsDAL myCars, ICarBrandsDAL myBrands,ICarClassesDAL carClasses, ICarModelsDAL carModels, ICarSeriesDAL carSeries, ICarTrimsDAL carTrims, ICarTypesDAL carTypes)
        {
            _myCars = myCars;
            _myBrands = myBrands;
            _myClasses = carClasses;
            _myModels = carModels;
            _mySeries = carSeries;
            _myTrims = carTrims;
            _myTypes = carTypes;
        }



        public List<MyCars> GetCarsDetail(int[] myCarsId)
        {
            var listMyCarsList = new List<MyCars>();
            foreach (var item in myCarsId)
            {
                var car = _myCars.SelectByID(item);
                if (car != null)
                {
                    listMyCarsList.Add(car);
                }
            }

            listMyCarsList.ForEach(car => { 
                car.Brand = _myBrands.SelectByID(car.BrandId);
                car.Model = _myModels.SelectByID(car.ModelId);
                car.Series = _mySeries.SelectByID(car.SeriesId);
                car.Trim = _myTrims.SelectByID(car.TrimId);
                car.Class = _myClasses.SelectByID(car.ClassId);
                car.Type = _myTypes.SelectByID(car.TypeId);
            
            });


            return listMyCarsList;

        }
    }
}
