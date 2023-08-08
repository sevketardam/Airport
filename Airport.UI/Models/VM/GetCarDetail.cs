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
        ICarModelsDAL _myModels;
        ICarSeriesDAL _mySeries;
        ICarTypesDAL _myTypes;

        public GetCarDetail(IMyCarsDAL myCars, ICarBrandsDAL myBrands,  ICarModelsDAL carModels, ICarSeriesDAL carSeries, ICarTypesDAL carTypes)
        {
            _myCars = myCars;
            _myBrands = myBrands;
            _myModels = carModels;
            _mySeries = carSeries;
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
                car.Type = _myTypes.SelectByID(car.TypeId);
            });


            return listMyCarsList;

        }

        public MyCars CarDetail(int? CarId)
        {
            var MyCar = _myCars.SelectByID(CarId);
            if (MyCar != null)
            {
                MyCar.Brand = _myBrands.SelectByID(MyCar.BrandId);
                MyCar.Model = _myModels.SelectByID(MyCar.ModelId);
                MyCar.Series = _mySeries.SelectByID(MyCar.SeriesId);
                MyCar.Type = _myTypes.SelectByID(MyCar.TypeId);

                return MyCar;
            }

            return null;
        }
    }
}
