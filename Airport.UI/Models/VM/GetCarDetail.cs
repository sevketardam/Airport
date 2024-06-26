using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.Interface;
using System.Collections.Generic;

namespace Airport.UI.Models.VM;

public class GetCarDetail(IMyCarsDAL myCarsDal, ICarBrandsDAL myBrandsDal, ICarModelsDAL carModelsDal, ICarSeriesDAL carSeriesDal, ICarTypesDAL carTypesDal) : IGetCarDetail
{
    public List<MyCars> GetCarsDetail(int[] myCarsId)
    {
        var listMyCarsList = new List<MyCars>();
        foreach (var item in myCarsId)
        {
            var car = myCarsDal.SelectByID(item);
            if (car != null)
            {
                listMyCarsList.Add(car);
            }
        }

        listMyCarsList.ForEach(car =>
        {
            car.Brand = myBrandsDal.SelectByID(car.BrandId);
            car.Model = carModelsDal.SelectByID(car.ModelId);
            car.Series = carSeriesDal.SelectByID(car.SeriesId);
            car.Type = carTypesDal.SelectByID(car.TypeId);
        });


        return listMyCarsList;

    }

    public MyCars CarDetail(int? CarId)
    {
        var MyCar = myCarsDal.SelectByID(CarId);
        if (MyCar != null)
        {
            MyCar.Brand = myBrandsDal.SelectByID(MyCar.BrandId);
            MyCar.Model = carModelsDal.SelectByID(MyCar.ModelId);
            MyCar.Series = carSeriesDal.SelectByID(MyCar.SeriesId);
            MyCar.Type = carTypesDal.SelectByID(MyCar.TypeId);

            return MyCar;
        }

        return null;
    }
}
