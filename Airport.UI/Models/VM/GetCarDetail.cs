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

        public GetCarDetail(IMyCarsDAL myCars, ICarBrandsDAL myBrands, ICarClassesDAL carClasses, ICarModelsDAL carModels, ICarSeriesDAL carSeries, ICarTrimsDAL carTrims, ICarTypesDAL carTypes)
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

            listMyCarsList.ForEach(car =>
            {
                car.Brand = _myBrands.SelectByID(car.BrandId);
                car.Model = _myModels.SelectByID(car.ModelId);
                car.Series = _mySeries.SelectByID(car.SeriesId);
                car.Trim = _myTrims.SelectByID(car.TrimId);
                car.Class = _myClasses.SelectByID(car.ClassId);
                car.Type = _myTypes.SelectByID(car.TypeId);

            });


            return listMyCarsList;

        }

        public MyCars CarDetail(int CarId)
        {
            var MyCar = _myCars.SelectByID(CarId);
            if (MyCar != null)
            {
                MyCar.Brand = _myBrands.SelectByID(MyCar.BrandId);
                MyCar.Model = _myModels.SelectByID(MyCar.ModelId);
                MyCar.Series = _mySeries.SelectByID(MyCar.SeriesId);
                MyCar.Trim = _myTrims.SelectByID(MyCar.TrimId);
                MyCar.Class = _myClasses.SelectByID(MyCar.ClassId);
                MyCar.Type = _myTypes.SelectByID(MyCar.TypeId);


                return MyCar;
            }

            return null;
        }
    }
}
