﻿using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.IM;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Airport.UI.Controllers
{
    public class CarsController : BaseController
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
        public CarsController(ICarBrandsDAL carBrands, IServicesDAL services, IServiceItemsDAL serviceItems, ICarModelsDAL carModels, ICarSeriesDAL carSeries, ICarTrimsDAL carTrims, ICarClassesDAL carClasses, ICarTypesDAL carTypes, IMyCarsDAL myCars)
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
        }


        [HttpGet("panel/mycars")]
        public IActionResult Index()
        {

            var myCars = _myCars.Select();
            myCars.ForEach(a =>
            {
                a.Brand = _carBrands.SelectByID(a.BrandId);
            });

            return View(myCars);
        }


        [HttpGet("panel/addcar")]
        public IActionResult AddMyCar()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var addCarVM = new AddMyCarsVM();
            addCarVM.CarBrands = _carBrands.Select();

            var myService = _services.SelectByFunc(a => a.UserId == userId);

            addCarVM.ServiceItems = myService;
            return View(addCarVM);
        }

        public IActionResult GetModels(int id)
        {
            var carModels = _carModels.SelectByFunc(a => a.CarBrandId == id);
            return Json(new { result = 200, models = carModels });
        }

        public IActionResult GetSeries(int id)
        {
            var carSeries = _carSeries.SelectByFunc(a => a.CarModelId == id);
            return Json(new { result = 200, series = carSeries });
        }

        public IActionResult GetTrims(int id)
        {
            var carTrims = _carTrims.SelectByFunc(a => a.CarSeriesId == id);
            return Json(new { result = 200, trims = carTrims });
        }

        public IActionResult GetClassAndTypes(int id)
        {
            var carClass = _carClasses.SelectByFunc(a => a.CarTrimId == id);
            var carTypes = _carTypes.SelectByFunc(a => a.CarTrimId == id);
            return Json(new { result = 200, classes = carClass, types = carTypes });
        }

        public IActionResult CreateMyCar(AddMyCarIM myCar)
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
                    UserId = userId
                });

                return View();
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public JsonResult UpdateService(UpdateMyCarIM updateMyCar, int id)
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

        [Route("panel/updatemycar/{id}")]
        public IActionResult UpdateMyCarPage(int id)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var myCar = _myCars.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
                if (myCar != null)
                {
                    var updateBrand = new UpdateMyCarVM()
                    {
                        Id = myCar.Id,
                        Brands = _carBrands.Select(),
                        BrandId = myCar.BrandId,
                        Models = _carModels.SelectByFunc(a => a.CarBrandId == myCar.BrandId),
                        ModelId = myCar.ModelId,
                        Series = _carSeries.SelectByFunc(a => a.CarModelId == myCar.ModelId),
                        SeriesId = myCar.SeriesId,
                        Trims = _carTrims.SelectByFunc(a => a.CarSeriesId == myCar.TrimId),
                        TrimId = myCar.TrimId,
                        Types = _carTypes.SelectByFunc(a => a.CarTrimId == myCar.TrimId),
                        TypeId = myCar.TypeId,
                        Classes = _carClasses.SelectByFunc(a => a.CarTrimId == myCar.TrimId),
                        ClassId = myCar.ClassId,
                        MaxPassenger = myCar.MaxPassenger,
                        SmallBags = myCar.SmallBags,
                        SuitCase = myCar.SuitCase,
                        Services = _services.SelectByFunc(a => a.UserId == userId),
                        ServiceId = myCar.ServiceId
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

    }
}