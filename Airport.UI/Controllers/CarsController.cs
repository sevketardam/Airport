using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.IM;
using Airport.UI.Models.Interface;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Airport.UI.Controllers;

public class CarsController(IServicesDAL servicesDal, ICarBrandsDAL carBrandsDal, ICarModelsDAL carModelsDal, ICarSeriesDAL carSeriesDal, ICarTypesDAL carTypesDal, IMyCarsDAL myCarsDal, IDriversDAL driversDal, IGetCarDetail getCarDetailDal, IReservationsDAL reservationsDal, ILocationCarsDAL locationCarsDal) : PanelAuthController
{

    [HttpGet("panel/my-cars")]
    public IActionResult Index()
    {
        var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
        var myCars = myCarsDal.SelectByFunc(a => a.UserId == userId && !a.IsDelete);
        myCars.ForEach(a =>
        {
            a.Brand = carBrandsDal.SelectByID(a.BrandId);
            a.Model = carModelsDal.SelectByID(a.ModelId);
        });

        return View(myCars);
    }


    [HttpGet("panel/add-car")]
    public IActionResult AddMyCar()
    {
        var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
        var addCarVM = new AddMyCarsVM();
        addCarVM.CarBrands = carBrandsDal.Select();

        var myService = servicesDal.SelectByFunc(a => a.UserId == userId);
        addCarVM.Drivers = driversDal.SelectByFunc(a => a.UserId == userId && !a.IsDelete);
        addCarVM.CarTypes = carTypesDal.Select();


        addCarVM.ServiceItems = myService;
        return View(addCarVM);
    }

    public IActionResult GetModels(int id)
    {
        var carModels = carModelsDal.SelectByFuncPer(a => a.CarBrandId == id);
        return Json(new { result = 200, models = carModels });
    }

    public IActionResult GetSeries(int id)
    {
        var carSeries = carSeriesDal.SelectByFuncPer(a => a.CarModelId == id);
        return Json(new { result = 200, series = carSeries });
    }

    [HttpPost("panel/add-car", Name = "AddCar")]
    public IActionResult AddMyCar(AddMyCarIM myCar)
    {
        try
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

            myCarsDal.Insert(new MyCars
            {
                BrandId = myCar.Brand,
                ModelId = myCar.Model,
                MaxPassenger = myCar.MaxPassenger,
                SeriesId = myCar.Series,
                ServiceId = myCar.Service,
                SmallBags = myCar.SmallBags,
                SuitCase = myCar.SuitCase,
                TypeId = myCar.Type,
                UserId = userId,
                Armored = myCar.Armored,
                Charger = myCar.Charger,
                Disabled = myCar.Disabled,
                Partition = myCar.Partition,
                Water = myCar.Water,
                Wifi = myCar.Wifi,
                DriverId = myCar.Driver,
                Plate = myCar.Plate,
                IsDelete = false
            });

            return RedirectToAction("Index", "Cars");
        }
        catch (System.Exception)
        {
            return BadRequest();
        }
    }

    [HttpPost]
    public JsonResult UpdateMyCar(UpdateMyCarIM updateMyCar, int id)
    {
        try
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

            var myCar = myCarsDal.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (myCar != null)
            {
                myCar.SuitCase = updateMyCar.SuitCase;
                myCar.BrandId = updateMyCar.Brand;
                myCar.ServiceId = updateMyCar.Service;
                myCar.MaxPassenger = updateMyCar.MaxPassenger;
                myCar.ModelId = updateMyCar.Model;
                myCar.SeriesId = updateMyCar.Series;
                myCar.SmallBags = updateMyCar.SmallBags;
                myCar.Wifi = updateMyCar.Wifi;
                myCar.Armored = updateMyCar.Armored;
                myCar.Water = updateMyCar.Water;
                myCar.Charger = updateMyCar.Charger;
                myCar.Partition = updateMyCar.Partition;
                myCar.Disabled = updateMyCar.Disabled;
                myCar.DriverId = updateMyCar.Driver;
                myCar.Plate = updateMyCar.Plate;
                myCarsDal.Update(myCar);
                return new JsonResult(new { result = 1 });
            }
        }
        catch (System.Exception)
        {
            return new JsonResult(new { });
        }

        return new JsonResult(new { });
    }

    [HttpGet("panel/update-my-car/{id}")]
    public async Task<IActionResult> UpdateMyCarPage(int id)
    {
        try
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var myCar = getCarDetailDal.CarDetail(id);
            if (myCar != null)
            {
                var getCarDetails = getCarDetailDal.CarDetail(myCar.Id);

                var Brands = carBrandsDal.Select();
                var Models = carModelsDal.SelectByFunc(a => a.CarBrandId == myCar.BrandId);
                var Series = carSeriesDal.SelectByFunc(a => a.CarModelId == myCar.ModelId);
                var Types = carTypesDal.Select().ToImmutableList();
                var Services = servicesDal.SelectByFunc(a => a.UserId == userId);
                var Drivers = driversDal.SelectByFunc(a => a.UserId == userId && !a.IsDelete);


                var updateBrand = new UpdateMyCarVM()
                {
                    Id = myCar.Id,
                    Brands = Brands,
                    BrandId = myCar.BrandId,
                    Models = Models,
                    ModelId = myCar.ModelId,
                    Series = Series,
                    SeriesId = myCar.SeriesId,
                    Types = Types,
                    TypeId = myCar.TypeId,
                    MaxPassenger = myCar.MaxPassenger,
                    SmallBags = myCar.SmallBags,
                    SuitCase = myCar.SuitCase,
                    Services = Services,
                    ServiceId = myCar.ServiceId,
                    Armored = myCar.Armored,
                    Charger = myCar.Charger,
                    Disabled = myCar.Disabled,
                    Partition = myCar.Partition,
                    Water = myCar.Water,
                    Wifi = myCar.Wifi,
                    DriverId = myCar.DriverId,
                    Drivers = Drivers,
                    Plate = myCar.Plate
                };



                return View(updateBrand);
            }
            return BadRequest();
        }
        catch (System.Exception)
        {
            return BadRequest();
        }
    }

    [Authorize(Roles = "0")]
    [HttpGet("panel/car-management")]
    public IActionResult CarManagement()
    {
        try
        {

            return View();
        }
        catch (System.Exception)
        {
            return BadRequest();
        }
    }


    public async Task<IActionResult> DeleteMyCar(int id)
    {
        try
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var myCar = myCarsDal.SelectByFunc(a => a.Id == id && a.UserId == userId && !a.IsDelete).FirstOrDefault();
            if (myCar != null)
            {
                var check = false;
                var locationCars = locationCarsDal.SelectByFunc(a => a.CarId == myCar.Id);
                locationCars.ForEach(a =>
                {
                    var checkReservation = reservationsDal.SelectByFunc(b => b.LocationCarId == a.Id).FirstOrDefault();
                    if (checkReservation is not null)
                    {
                        check = true;
                    }
                });

                if (check)
                {
                    myCarsDal.SoftDelete(myCar);
                }
                else
                {
                    myCarsDal.HardDelete(myCar);
                }

                return Json(new { result = 1 });
            }
            return Json(new { result = 2 });
        }
        catch (System.Exception)
        {
            return Json(new { });
        }
    }

}
