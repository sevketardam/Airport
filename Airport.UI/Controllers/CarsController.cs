using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
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
        ICarTrimsDAL _carTrims;
        ICarClassesDAL _carClasses;
        ICarTypesDAL _carTypes;
        IMyCarsDAL _myCars;
        IDriversDAL _drivers;
        IGetCarDetail _carDetail;
        public CarsController(ICarBrandsDAL carBrands, IServicesDAL services, IServiceItemsDAL serviceItems, ICarModelsDAL carModels, ICarSeriesDAL carSeries, ICarTrimsDAL carTrims, ICarClassesDAL carClasses, ICarTypesDAL carTypes, IMyCarsDAL myCars, IDriversDAL drivers, IGetCarDetail getCarDetail)
        {
            _carBrands = carBrands;
            _services = services;
            _serviceItems = serviceItems;
            _carModels = carModels;
            _carSeries = carSeries;
            _carTrims = carTrims;
            _carClasses = carClasses;
            _carTypes = carTypes;
            _myCars = myCars;
            _drivers = drivers;
            _carDetail = getCarDetail;
        }


        [HttpGet("panel/mycars")]
        public IActionResult Index()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var myCars = _myCars.SelectByFunc(a => a.UserId == userId);
            myCars.ForEach(a =>
            {
                a.Brand = _carBrands.SelectByID(a.BrandId);
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

        public IActionResult GetTrims(int id)
        {
            var carTrims = _carTrims.SelectByFuncPer(a => a.CarSeriesId == id);
            return Json(new { result = 200, trims = carTrims });
        }

        public IActionResult GetClassAndTypes(int id)
        {
            var carClass = _carClasses.Select();
            var carTypes = _carTypes.Select();
            return Json(new { result = 200, classes = carClass, types = carTypes });
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
                    ClassId = myCar.Class,
                    ModelId = myCar.Model,
                    MaxPassenger = myCar.MaxPassenger,
                    SeriesId = myCar.Series,
                    ServiceId = myCar.Service,
                    SmallBags = myCar.SmallBags,
                    SuitCase = myCar.SuitCase,
                    TrimId = myCar.Trim,
                    TypeId = myCar.Type,
                    UserId = userId,
                    Armored = myCar.Armored,
                    Charger = myCar.Charger,
                    Disabled = myCar.Disabled,
                    Partition = myCar.Partition,
                    Water = myCar.Water,
                    Wifi = myCar.Wifi,
                    DriverId = myCar.DriverId
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
                    myCar.ClassId = updateMyCar.Class;
                    myCar.MaxPassenger = updateMyCar.MaxPassenger;
                    myCar.ModelId = updateMyCar.Model;
                    myCar.SeriesId = updateMyCar.Series;
                    myCar.SmallBags = updateMyCar.SmallBags;
                    myCar.TrimId = updateMyCar.Trim;
                    myCar.Wifi = updateMyCar.Wifi;
                    myCar.Armored = updateMyCar.Armored;
                    myCar.Water = updateMyCar.Water;
                    myCar.Charger = updateMyCar.Charger;
                    myCar.Partition = updateMyCar.Partition;
                    myCar.Disabled = updateMyCar.Disabled;
                    myCar.DriverId = updateMyCar.DriverId;

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

        [HttpGet("panel/updatemycar/{id}")]
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
                    var Models = _carModels.SelectByFuncPer(a => a.CarBrandId == myCar.BrandId);
                    var Series = _carSeries.SelectByFuncPer(a => a.CarModelId == myCar.ModelId);
                    var Trims = _carTrims.SelectByFuncPer(a => a.CarSeriesId == myCar.SeriesId);
                    var Types = _carTypes.Select().ToImmutableList();
                    var Classes = _carClasses.Select().ToImmutableList();
                    var Services = _services.SelectByFuncPer(a => a.UserId == userId);
                    var Drivers = _drivers.SelectByFuncPer(a => a.UserId == userId);


                    var updateBrand = new UpdateMyCarVM()
                    {
                        Id = myCar.Id,
                        Brands = Brands,
                        BrandId = myCar.BrandId,
                        Models = Models,
                        ModelId = myCar.ModelId,
                        Series = Series,
                        SeriesId = myCar.SeriesId,
                        Trims = Trims,
                        TrimId = myCar.TrimId,
                        Types = Types,
                        TypeId = myCar.TypeId,
                        Classes = Classes,
                        ClassId = myCar.ClassId,
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
                        Drivers = Drivers
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

        //[HttpPost]
        //public IActionResult AddFirstData()
        //{
        //    try
        //    {
        //        //string line = "";


        //        string jsonText = System.IO.File.ReadAllText("wwwroot/trims.json");

        //        var list = System.Text.Json.JsonSerializer.Deserialize<List<TableEntry>>(jsonText);
        //        var list2 = new List<CarTrims>();
        //        list.ForEach(a =>
        //        {
        //            list2.Add(new CarTrims
        //            {
        //                CarSeriesId = a.serie_id,
        //                CarTrimName = a.name,
        //                Id = a.id,
        //                CarModelId = a.model_id
        //            });
        //        });

        //        _carTrims.InsertRage(list2);

        //        //_carBrands.InsertRage(list);


        //        return new JsonResult(new { result = 1 });
        //    }
        //    catch (System.Exception)
        //    {
        //        return BadRequest();
        //    }
        //}

    }
}
