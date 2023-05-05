using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Airport.UI.Controllers
{
    public class ServiceController : PanelAuthController
    {
        IServiceCategoriesDAL _serviceCategory;
        IServicePropertiesDAL _serviceProperties;
        IServicesDAL _services;
        IServiceItemsDAL _items;
        IMyCarsDAL _myCars;
        public ServiceController(IServicePropertiesDAL serviceProperties, IServiceCategoriesDAL serviceCategories, IServicesDAL services, IServiceItemsDAL items, IMyCarsDAL myCars)
        {
            _serviceCategory = serviceCategories;
            _serviceProperties = serviceProperties;
            _services = services;
            _items = items;
            _myCars = myCars;
        }

        [HttpGet("panel/service")]
        public IActionResult Index()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var serviceList = _services.SelectByFunc(a => a.UserId == userId);

            return View(serviceList);
        }

        [HttpGet("panel/add-service")]
        public IActionResult AddServicePage()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var serviceCategories = _serviceCategory.SelectByFunc(a=>a.UserId == userId);

            serviceCategories.ForEach(a =>
            {
                a.ServiceProperties = _serviceProperties.SelectByFunc(b => b.ServiceCategoryId == a.Id);
            });

            return View(serviceCategories);
        }

        [HttpGet("panel/service-managament")]
        public async Task<IActionResult> ServiceManagement()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var serviceCategories = _serviceCategory.SelectByFunc(a=>a.UserId == userId);
            var serviceProperties = new List<ServiceProperties>();
            serviceCategories.ForEach(a =>
            {
                a.ServiceProperties = _serviceProperties.SelectByFunc(b => b.ServiceCategoryId == a.Id);
            });


            var ServiceItems = new ServiceManagementVM()
            {
                ServiceCategories = serviceCategories,
            };


            return View(ServiceItems);
        }

        [HttpGet("panel/update-service/{id}")]
        public async Task<IActionResult> UpdateServicePage(int id)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var service = _services.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
                if (service != null)
                {
                    var serviceSelectedProps = new List<ServiceProperties>();

                    var serviceItems = _items.SelectByFunc(a => a.ServiceId == id);

                    var ServicePriceDataVM = new List<ServicePriceDataVM>();
                    var ServiceCategoryPriceVM = new List<ServiceCategoryPriceVM>();
                    serviceItems.ForEach(a =>
                    {

                        a.ServiceProperty = _serviceProperties.SelectByID(a.ServicePropertyId);
                        ServiceCategoryPriceVM.Add(new ServiceCategoryPriceVM
                        {
                            Price = a.Price,
                            PropId = a.ServicePropertyId,
                            CategoryId = a.ServiceProperty.ServiceCategoryId,
                            ServiceProperties = _serviceProperties.SelectByID(a.ServicePropertyId)                          
                        });

                        serviceSelectedProps.Add(_serviceProperties.SelectByID(a.ServicePropertyId));
                    });


                    var s = ServiceCategoryPriceVM.GroupBy(a => a.CategoryId).ToList();

                    var d = new List<ServiceLastCategoryPriceVM>();

                    s.ForEach(a => 
                    {
                        d.Add(new ServiceLastCategoryPriceVM
                        {
                            CategoryId = a.Key,
                            serviceCategoryPrices = a.Select(b=>b),
                            CategoryName = _serviceCategory.SelectByID(a.Key).ServiceCategoryName
                        });
                    });


                    var updateServiceVM = new UpdatePageServiceVM()
                    {
                        ServiceCategories = _serviceCategory.SelectByFunc(a=>a.UserId == userId),
                        ServiceSelectedProperties = serviceSelectedProps,
                        Service = service,
                        ServicePriceDatas = d
                    };

                    updateServiceVM.ServiceCategories.ForEach(a =>
                    {
                        a.ServiceProperties = _serviceProperties.SelectByFunc(b => b.ServiceCategoryId == a.Id);
                    });

                    return View(updateServiceVM);
                }
                return BadRequest();
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }


        [HttpPost]
        public JsonResult UpdateService(AddServiceVM updateService, int id)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                var service = _services.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
                if (service != null)
                {

                    service.ServiceName = updateService.ServiceName;
                    service.ServiceDescription = updateService.ServiceDescription;


                    _services.Update(service);

                    var oldItems = _items.SelectByFunc(a=>a.ServiceId == id);
                  
                    oldItems.ForEach(a =>
                    {
                        _items.HardDelete(a);
                    });

                    var serviceItemsList = new List<ServiceItems>();

                    foreach (var item in updateService.PriceData)
                    {
                        foreach (var item2 in item.PriceData)
                        {
                            serviceItemsList.Add(new ServiceItems
                            {
                                ServicePropertyId = item2.PropId,
                                ServiceId = id,
                                Price = item2.Price
                            });
                        }
                    }

                    _items.InsertRage(serviceItemsList);

                    return new JsonResult(new { result = 1 });
                }
            }
            catch (System.Exception)
            {
                return new JsonResult(new { });
            }

            return new JsonResult(new { });
        }

        [HttpPost]
        public JsonResult AddServiceCategory(string categoryName)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                _serviceCategory.Insert(new ServiceCategories { ServiceCategoryName = categoryName,UserId = userId });
                return new JsonResult(new { result = 1 });
            }
            catch (System.Exception)
            {
                return new JsonResult(new { });
            }
        }

        [HttpPost]
        public JsonResult AddServiceProperty(AddServicePropertyVM serviceProperty)
        {
            try
            {
                _serviceProperties.Insert(new ServiceProperties
                {
                    ServiceCategoryId = serviceProperty.ServiceCategoryId,
                    ServicePropertyDescription = serviceProperty.PropertyDescription,
                    ServicePropertyName = serviceProperty.PropertyName
                });

                return new JsonResult(new { result = 1 });
            }
            catch (System.Exception)
            {
                return new JsonResult(new { });
            }
        }

        [HttpPost]
        public JsonResult DeleteService(int id)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var service = _serviceCategory.SelectByFunc(a=>a.Id == id && a.UserId == userId).FirstOrDefault();
                if (service != null)
                {
                    var serviceProps = _serviceProperties.SelectByFunc(a=>a.ServiceCategoryId == service.Id);
                    serviceProps.ForEach(a =>
                    {


                        var items = _items.SelectByFunc(b => b.ServicePropertyId == a.Id);
                        items.ForEach(b =>
                        {
                            var services = _services.SelectByFunc(c => c.Id == b.ServiceId);
                            services.ForEach(c =>
                            {
                                var myCars = _myCars.SelectByFunc(d=>d.ServiceId == c.Id);
                                myCars.ForEach(d =>
                                {
                                    d.ServiceId = null;
                                    _myCars.Update(d);
                                });

                                _services.HardDelete(c);
                            });
                            _items.HardDelete(b);
                        });


                        _serviceProperties.HardDelete(a);
                    });

                    _serviceCategory.HardDelete(service);

                }

                return new JsonResult(new { result = 1 });
            }
            catch (System.Exception)
            {
                return new JsonResult(new { });
            }
        }


        [HttpPost]
        public JsonResult AddService(AddServiceVM service)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var addedService = _services.Insert(new Services
                {
                    ServiceDescription = service.ServiceDescription,
                    ServiceName = service.ServiceName,
                    UserId = userId
                });

                var serviceItemsList = new List<ServiceItems>();

                foreach (var item in service.PriceData)
                {
                    foreach (var item2 in item.PriceData)
                    {
                        serviceItemsList.Add(new ServiceItems
                        {
                            ServicePropertyId = item2.PropId,
                            ServiceId = addedService.Id,
                            Price = item2.Price
                        });
                    }
                }

                _items.InsertRage(serviceItemsList);

                return new JsonResult(new { result = 1 });
            }
            catch (System.Exception)
            {
                return new JsonResult(new { });
            }
        }

        [HttpPost]
        public JsonResult GetServiceItem(int[] serviceProId)
        {
            try
            {

                var list = new List<int>();
                var serviceVM = new List<GetServiceItemDetailVM>();
                var serviceVM2 = new List<GetServiceCategoryItemVM>();
                foreach (var item in serviceProId)
                {
                    var serviceProp = _serviceProperties.SelectByID(item);

                    if (serviceProp != null)
                    {
                        list.Add(serviceProp.ServiceCategoryId);
                        serviceProp.ServiceCategory = _serviceCategory.SelectByID(serviceProp?.ServiceCategoryId);
                        serviceVM2.Add(new GetServiceCategoryItemVM()
                        {
                            Id = serviceProp.Id,
                            ServiceDescripton = serviceProp.ServicePropertyDescription,
                            ServiceCategoryName = serviceProp.ServiceCategory.ServiceCategoryName,
                            ServiceName = serviceProp.ServicePropertyName,
                            ServiceCategoryId = serviceProp.ServiceCategoryId
                        });
                    }
                }

                list = list.Distinct().ToList();

                list.ForEach(a =>
                {
                    serviceVM.Add(new GetServiceItemDetailVM
                    {
                        ServiceCategoryId = a,
                        ServiceCategoryName = serviceVM2.Where(b => b.ServiceCategoryId == a).FirstOrDefault().ServiceCategoryName,
                        CategoryItems = serviceVM2.Where(b => b.ServiceCategoryId == a).ToList()
                    });
                });



                return new JsonResult(new { result = 1, data = serviceVM });
            }
            catch (System.Exception)
            {
                return new JsonResult(new { });
            }
        }
    }
}
