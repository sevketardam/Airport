using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Airport.UI.Controllers
{
    [Authorize]
    public class ServiceController : BaseController
    {
        IServiceCategoriesDAL _serviceCategory;
        IServicePropertiesDAL _serviceProperties;
        IServicesDAL _services;
        IServiceItemsDAL _items;
        public ServiceController(IServicePropertiesDAL serviceProperties, IServiceCategoriesDAL serviceCategories, IServicesDAL services, IServiceItemsDAL items)
        {
            _serviceCategory = serviceCategories;
            _serviceProperties = serviceProperties;
            _services = services;
            _items = items;
        }

        [Route("panel/service")]
        public IActionResult Index()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var serviceList = _services.SelectByFunc(a => a.UserId == userId);

            return View(serviceList);
        }

        [Route("panel/add-service")]
        public IActionResult AddServicePage()
        {
            var serviceCategories = _serviceCategory.Select();

            serviceCategories.ForEach(a =>
            {
                a.ServiceProperties = _serviceProperties.SelectByFunc(b => b.ServiceCategoryId == a.Id);
            });

            return View(serviceCategories);
        }

        [Route("panel/servicemanagament")]
        public IActionResult ServiceManagement()
        {
            var serviceCategories = _serviceCategory.Select();
            var serviceProperties = _serviceProperties.Select();

            var ServiceItems = new ServiceManagementVM()
            {
                ServiceCategories = serviceCategories,
                ServiceProperties = serviceProperties,
            };


            return View(ServiceItems);
        }

        [Route("panel/updateservice/{id}")]
        public IActionResult UpdateServicePage(int id)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var service = _services.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
                if (service != null)
                {
                    var serviceSelectedProps = new List<ServiceProperties>();
                    var serviceItems = _items.SelectByFunc(a => a.ServiceId == id);

                    serviceItems.ForEach(a =>
                    {
                        serviceSelectedProps.Add(_serviceProperties.SelectByID(a.ServicePropertyId));
                    });


                    var updateServiceVM = new UpdatePageServiceVM()
                    {
                        ServiceCategories = _serviceCategory.Select(),
                        ServiceSelectedProperties = serviceSelectedProps,
                        Service = service,
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

        public JsonResult UpdateService(UpdateServiceVM updateService, int id)
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

                    var oldServiceItems = _items.SelectByFunc(a => a.ServiceId == id);

                    foreach (var item in oldServiceItems)
                    {
                        if (!updateService.ServiceItems.Contains(item.ServicePropertyId))
                        {
                            _items.HardDelete(item);
                        }
                    }

                    foreach (var item2 in updateService.ServiceItems)
                    {
                        var service2 = _items.SelectByFunc(a => a.ServicePropertyId == item2 && a.ServiceId == id);
                        service2.ForEach(a =>
                        {
                            a.Service = _services.SelectByID(id);
                        });

                        var thisservice = service2.Where(a => a.Service.UserId == userId && a.ServicePropertyId == item2).FirstOrDefault();

                        if (!oldServiceItems.Contains(thisservice))
                        {
                            _items.Insert(new ServiceItems
                            {
                                ServiceId = id,
                                ServicePropertyId = item2,
                                Price = "0"
                            });
                        }
                    }

                    return new JsonResult(new { result = 1 });
                }
            }
            catch (System.Exception)
            {
                return new JsonResult(new { });
            }

            return new JsonResult(new { });
        }


        public JsonResult AddServiceCategory(string categoryName)
        {
            try
            {
                _serviceCategory.Insert(new ServiceCategories { ServiceCategoryName = categoryName });
                return new JsonResult(new { result = 1 });
            }
            catch (System.Exception)
            {
                return new JsonResult(new { });
            }
        }

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

                foreach (var item in service.ServiceItems)
                {
                    serviceItemsList.Add(new ServiceItems
                    {
                        ServicePropertyId = item,
                        ServiceId = addedService.Id,
                        Price = "0"
                    });
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
                var serviceVM = new List<GetServiceItemDetailVM>();
                foreach (var item in serviceProId)
                {
                    var serviceProp = _serviceProperties.SelectByID(item);

                    if (serviceProp != null)
                    {
                        serviceProp.ServiceCategory = _serviceCategory.SelectByID(serviceProp?.ServiceCategoryId);
                        serviceVM.Add(new GetServiceItemDetailVM()
                        {
                            Id = serviceProp.Id,
                            ServiceCategoryName = serviceProp.ServiceCategory?.ServiceCategoryName,
                            ServiceDescripton = serviceProp.ServicePropertyDescription,
                            ServiceName = serviceProp.ServicePropertyName
                        });
                    }
                }

                return new JsonResult(new { result = 1, data = serviceVM });
            }
            catch (System.Exception)
            {
                return new JsonResult(new { });
            }
        }
    }
}
