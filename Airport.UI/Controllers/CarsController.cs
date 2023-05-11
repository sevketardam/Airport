using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.MessageExtension.Interfaces;
using Airport.MessageExtension.VM;
using Airport.UI.Models.IM;
using Airport.UI.Models.Interface;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Airport.UI.Controllers
{
    public class CarsController : PanelAuthController
    {
        IServicesDAL _services;
        IServiceItemsDAL _serviceItems;
        ICarBrandsDAL _carBrands;
        ICarModelsDAL _carModels;
        ICarSeriesDAL _carSeries;
        ICarTypesDAL _carTypes;
        IMyCarsDAL _myCars;
        IDriversDAL _drivers;
        IGetCarDetail _carDetail;
        IReservationsDAL _reservations;
        ILocationCarsDAL _locationCars;
        ILocationCarsFareDAL _locationCarsFare;
        ISMS _sms;

        public CarsController(ICarBrandsDAL carBrands, IServicesDAL services, IServiceItemsDAL serviceItems, ICarModelsDAL carModels, ICarSeriesDAL carSeries, ICarTypesDAL carTypes, IMyCarsDAL myCars, IDriversDAL drivers, IGetCarDetail getCarDetail, IReservationsDAL reservations, ILocationCarsDAL locationCars, ILocationCarsFareDAL locationCarsFare, ISMS sms)
        {
            _carBrands = carBrands;
            _services = services;
            _serviceItems = serviceItems;
            _carModels = carModels;
            _carSeries = carSeries;
            _carTypes = carTypes;
            _myCars = myCars;
            _drivers = drivers;
            _carDetail = getCarDetail;
            _reservations = reservations;
            _locationCars = locationCars;
            _locationCarsFare = locationCarsFare;
            _sms = sms;
        }


        public IActionResult SMSDeneme()
        {
            var allMessage = new List<Mesaj>();
            allMessage.Add(new Mesaj
            {
                dest = "905365278808",
                msg = "deneme mesaj1"
            });
            allMessage.Add(new Mesaj
            {
                dest = "905531878855",
                msg = "deneme mesaj1"
            });
            var mesaj = allMessage.ToArray();

            _sms.SmsForReservation(mesaj);


            return View();
        }


        [HttpGet("panel/my-cars")]
        public IActionResult Index()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var myCars = _myCars.SelectByFunc(a => a.UserId == userId);
            myCars.ForEach(a =>
            {
                a.Brand = _carBrands.SelectByID(a.BrandId);
                a.Model = _carModels.SelectByID(a.ModelId);
            });

            return View(myCars);
        }


        [HttpGet("panel/add-car")]
        public IActionResult AddMyCar()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var addCarVM = new AddMyCarsVM();
            addCarVM.CarBrands = _carBrands.Select();

            var myService = _services.SelectByFunc(a => a.UserId == userId);
            addCarVM.Drivers = _drivers.SelectByFunc(a => a.UserId == userId);
            addCarVM.CarTypes = _carTypes.Select();


            addCarVM.ServiceItems = myService;
            return View(addCarVM);
        }

        public IActionResult GetModels(int id)
        {
            var carModels = _carModels.SelectByFuncPer(a => a.CarBrandId == id);
            return Json(new { result = 200, models = carModels });
        }

        public IActionResult GetSeries(int id)
        {
            var carSeries = _carSeries.SelectByFuncPer(a => a.CarModelId == id);
            return Json(new { result = 200, series = carSeries });
        }

        [HttpPost("panel/add-car", Name = "AddCar")]
        public IActionResult AddMyCar(AddMyCarIM myCar)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                _myCars.Insert(new MyCars
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
                    Plate = myCar.Plate
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

                var myCar = _myCars.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
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

                    _myCars.Update(myCar);
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
                var myCar = _carDetail.CarDetail(id);
                if (myCar != null)
                {
                    var getCarDetails = _carDetail.CarDetail(myCar.Id);

                    var Brands = _carBrands.Select();
                    var Models = _carModels.SelectByFunc(a => a.CarBrandId == myCar.BrandId);
                    var Series = _carSeries.SelectByFunc(a => a.CarModelId == myCar.ModelId);
                    var Types = _carTypes.Select().ToImmutableList();
                    var Services = _services.SelectByFunc(a => a.UserId == userId);
                    var Drivers = _drivers.SelectByFunc(a => a.UserId == userId);


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
                var myCar = _myCars.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
                if (myCar != null)
                {
                    var check = false;
                    var locationCars = _locationCars.SelectByFunc(a => a.CarId == myCar.Id);
                    locationCars.ForEach(a =>
                    {
                        var checkReservation = _reservations.SelectByFunc(b => b.LocationCarId == a.Id).FirstOrDefault();
                        if (checkReservation is null)
                        {
                            var locationCarsFare = _locationCarsFare.SelectByFunc(b => b.LocationCarId == a.Id);
                            locationCarsFare.ForEach(b =>
                            {
                                _locationCarsFare.HardDelete(b);
                            });

                            _locationCars.HardDelete(a);
                        }
                        else
                        {
                            check = true;
                        }
                    });

                    if (check)
                    {
                        _myCars.SoftDelete(myCar);
                    }
                    else
                    {
                        _myCars.HardDelete(myCar);
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
}
