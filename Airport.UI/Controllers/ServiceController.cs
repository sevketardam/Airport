using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Airport.UI.Controllers;

public class ServiceController(IServicePropertiesDAL servicePropertiesDal, IServiceCategoriesDAL serviceCategoryDal, IServicesDAL servicesDal, IServiceItemsDAL serviceItemsDal, IMyCarsDAL myCarsDal, IReservationServicesTableDAL reservationServicesTableDal) : PanelAuthController
{

    [HttpGet("panel/service")]
    public IActionResult Index()
    {
        var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
        var serviceList = servicesDal.SelectByFunc(a => a.UserId == userId);

        return View(serviceList);
    }

    [HttpGet("panel/add-service")]
    public IActionResult AddServicePage()
    {
        var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
        var serviceCategories = serviceCategoryDal.SelectByFunc(a => a.UserId == userId);

        serviceCategories.ForEach(a =>
        {
            a.ServiceProperties = servicePropertiesDal.SelectByFunc(b => b.ServiceCategoryId == a.Id);
        });

        return View(serviceCategories);
    }

    [HttpGet("panel/service-managament")]
    public async Task<IActionResult> ServiceManagement()
    {
        var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
        var serviceCategories = serviceCategoryDal.SelectByFunc(a => a.UserId == userId);
        var serviceProperties = new List<ServiceProperties>();
        serviceCategories.ForEach(a =>
        {
            a.ServiceProperties = servicePropertiesDal.SelectByFunc(b => b.ServiceCategoryId == a.Id);
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
            var service = servicesDal.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (service != null)
            {
                var serviceSelectedProps = new List<ServiceProperties>();

                var serviceItems = serviceItemsDal.SelectByFunc(a => a.ServiceId == id);

                var ServicePriceDataVM = new List<ServicePriceDataVM>();
                var ServiceCategoryPriceVM = new List<ServiceCategoryPriceVM>();
                serviceItems.ForEach(a =>
                {

                    a.ServiceProperty = servicePropertiesDal.SelectByID(a.ServicePropertyId);
                    ServiceCategoryPriceVM.Add(new ServiceCategoryPriceVM
                    {
                        Price = a.Price,
                        PropId = a.ServicePropertyId,
                        CategoryId = a.ServiceProperty.ServiceCategoryId,
                        ServiceProperties = servicePropertiesDal.SelectByID(a.ServicePropertyId)
                    });

                    serviceSelectedProps.Add(servicePropertiesDal.SelectByID(a.ServicePropertyId));
                });


                var s = ServiceCategoryPriceVM.GroupBy(a => a.CategoryId).ToList();

                var d = new List<ServiceLastCategoryPriceVM>();

                s.ForEach(a =>
                {
                    d.Add(new ServiceLastCategoryPriceVM
                    {
                        CategoryId = a.Key,
                        serviceCategoryPrices = a.Select(b => b),
                        CategoryName = serviceCategoryDal.SelectByID(a.Key).ServiceCategoryName
                    });
                });


                var updateServiceVM = new UpdatePageServiceVM()
                {
                    ServiceCategories = serviceCategoryDal.SelectByFunc(a => a.UserId == userId),
                    ServiceSelectedProperties = serviceSelectedProps,
                    Service = service,
                    ServicePriceDatas = d
                };

                updateServiceVM.ServiceCategories.ForEach(a =>
                {
                    a.ServiceProperties = servicePropertiesDal.SelectByFunc(b => b.ServiceCategoryId == a.Id);
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

            var service = servicesDal.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (service != null)
            {

                service.ServiceName = updateService.ServiceName;
                service.ServiceDescription = updateService.ServiceDescription;


                servicesDal.Update(service);

                var oldItems = serviceItemsDal.SelectByFunc(a => a.ServiceId == id);

                oldItems.ForEach(a =>
                {
                    serviceItemsDal.HardDelete(a);
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

                serviceItemsDal.InsertRage(serviceItemsList);

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
            serviceCategoryDal.Insert(new ServiceCategories { ServiceCategoryName = categoryName, UserId = userId });
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
            servicePropertiesDal.Insert(new ServiceProperties
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
    public JsonResult GetServiceProperty(int id)
    {
        try
        {
            var propData = servicePropertiesDal.SelectByID(id);
            if (propData != null)
            {
                return new JsonResult(new { result = 1, data = propData });
            }
            return new JsonResult(new { result = 2 });
        }
        catch (System.Exception)
        {
            return new JsonResult(new { });
        }
    }

    [HttpPost]
    public JsonResult UpdateServiceProperty(ServiceProperties prop)
    {
        try
        {
            var propData = servicePropertiesDal.SelectByID(prop.Id);

            if (propData != null)
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                propData.ServiceCategory = serviceCategoryDal.SelectByID(propData.ServiceCategoryId);
                if (propData.ServiceCategory.UserId == userId)
                {
                    propData.ServiceCategory = null;
                    propData.ServicePropertyDescription = prop.ServicePropertyDescription;
                    propData.ServicePropertyName = prop.ServicePropertyName;

                    servicePropertiesDal.Update(propData);
                    return new JsonResult(new { result = 1 });
                }
            }

            return new JsonResult(new { result = 2 });
        }
        catch (System.Exception)
        {
            return new JsonResult(new { });
        }
    }

    [HttpPost]
    public JsonResult DeleteServiceProp(int id)
    {
        try
        {
            var serviceProps = servicePropertiesDal.SelectByID(id);
            if (serviceProps != null)
            {
                var deleteCont = false;
                var serviceItems = serviceItemsDal.SelectByFunc(a=>a.ServicePropertyId == id);
                var reservationServices = reservationServicesTableDal.Select();

                var reservationServiceItemIds = new List<int>();

                reservationServices.ForEach(a =>
                {
                    reservationServiceItemIds.Add(a.ServiceItemId);
                });

                serviceItems.ForEach(a =>
                {
                    if (reservationServiceItemIds.Contains(a.Id))
                    {
                        deleteCont = true;
                    }
                });

                if (!deleteCont)
                {
                    var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                    serviceProps.ServiceCategory = serviceCategoryDal.SelectByID(serviceProps.ServiceCategoryId);
                    if (serviceProps.ServiceCategory.UserId == userId)
                    {
                        var items = serviceItemsDal.SelectByFunc(b => b.ServicePropertyId == id);
                        items.ForEach(b =>
                        {
                            serviceItemsDal.HardDelete(b);
                        });

                        servicePropertiesDal.HardDelete(serviceProps);
                        return new JsonResult(new { result = 1 });
                    }
                }
                else
                {
                    return new JsonResult(new { result = 3 });
                }                  
            }

            return new JsonResult(new { result = 2 });
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
            var service = serviceCategoryDal.SelectByFunc(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (service != null)
            {

                var reservationServices = reservationServicesTableDal.Select();
                var serviceProps = servicePropertiesDal.SelectByFunc(a => a.ServiceCategoryId == service.Id);

                var deleteCont = false;
                foreach (var a in serviceProps)
                {
                    var serviceItems = serviceItemsDal.SelectByFunc(a => a.ServicePropertyId == a.Id);


                    var reservationServiceItemIds = new List<int>();
                    reservationServices.ForEach(b =>
                    {
                        reservationServiceItemIds.Add(b.ServiceItemId);
                    });

                    serviceItems.ForEach(b =>
                    {
                        if (reservationServiceItemIds.Contains(b.Id))
                        {
                            deleteCont = true;
                            
                        }
                    });

                    if (deleteCont)
                    {
                        break;
                    }

                    var items = serviceItemsDal.SelectByFunc(b => b.ServicePropertyId == a.Id);
                    items.ForEach(b =>
                    {
                        var services = servicesDal.SelectByFunc(c => c.Id == b.ServiceId);
                        services.ForEach(c =>
                        {
                            var myCars = myCarsDal.SelectByFunc(d => d.ServiceId == c.Id);
                            myCars.ForEach(d =>
                            {
                                d.ServiceId = null;
                                myCarsDal.Update(d);
                            });

                            servicesDal.HardDelete(c);
                        });
                        serviceItemsDal.HardDelete(b);
                    });


                    servicePropertiesDal.HardDelete(a);
                }

                if (deleteCont)
                {
                    return new JsonResult(new { result = 2 });
                }
                else
                {
                    serviceCategoryDal.HardDelete(service);
                }

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
            var addedService = servicesDal.Insert(new Services
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

            serviceItemsDal.InsertRage(serviceItemsList);

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
                var serviceProp = servicePropertiesDal.SelectByID(item);

                if (serviceProp != null)
                {
                    list.Add(serviceProp.ServiceCategoryId);
                    serviceProp.ServiceCategory = serviceCategoryDal.SelectByID(serviceProp?.ServiceCategoryId);
                    serviceVM2.Add(new GetServiceCategoryItemVM()
                    {
                        Id = serviceProp.Id,
                        ServiceDescripton = serviceProp.ServicePropertyDescription,
                        ServiceCategoryName = serviceProp.ServiceCategory.ServiceCategoryName,
                        ServiceName = serviceProp.ServicePropertyName,
                        ServiceCategoryId = serviceProp.ServiceCategoryId,
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

    public JsonResult UpdateServiceCategory(string categoryName,int id)
    {
        try
        {
            var service = serviceCategoryDal.SelectByID(id);
            if (service is not null)
            {
                service.ServiceCategoryName = categoryName;
                serviceCategoryDal.Update(service);
                return Json(new { result = 1 });
            }
            return Json(new { result = 2 });
        }
        catch (Exception)
        {
            return Json(new {});
        }       
    }

    public JsonResult GetServiceCategory(int id)
    {
        try
        {
            var service = serviceCategoryDal.SelectByID(id);
            if (service is not null)
            {
                return Json(new { result = 1,data=service });
            }
            return Json(new { result = 2 });
        }
        catch (Exception)
        {
            return Json(new { });
        }

    }
}
