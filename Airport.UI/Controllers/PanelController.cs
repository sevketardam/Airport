﻿using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Security.Claims;
using Airport.UI.Models.Interface;
using Airport.DBEntities.Entities;
using System.Security.Cryptography;
using System.IO;
using Airport.MessageExtensions.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using static iText.Svg.SvgConstants;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Airport.UI.Models.ITransactions;
using static Org.BouncyCastle.Utilities.Test.FixedSecureRandom;
using System.Net.Http;
using System.Text;

namespace Airport.UI.Controllers
{
    public class PanelController : PanelAuthController
    {
        IReservationsDAL _reservations;
        IDriversDAL _drivers;
        IGetCarDetail _carDetail;
        ILocationCarsDAL _locationCars;
        IReservationPeopleDAL _reservationPeople;
        ILocationsDAL _location;
        ILocationCarsFareDAL _locationCarsFare;
        IUserDatasDAL _user;
        IServicesDAL _services;
        IServiceItemsDAL _serviceItems;
        IServicePropertiesDAL _serviceProperties;
        IServiceCategoriesDAL _serviceCategories;
        IReservationServicesTableDAL _reservationServicesTable;
        IMail _mail;
        ILoginAuthDAL _loginAuth;
        ICouponsDAL _coupons;
        IMyCarsDAL _myCars;
        IFileOperation _fileOperation;
        IUserDocsDAL _docs;
        IPayment _payment;
        ITReservationHelpers _tReservationHelpers;
        IWithdrawalRequestDAL _withdrawalRequest;
        IPaymentDetailDAL _paymentDetail;
        public PanelController(IReservationsDAL reservations, IDriversDAL drivers, IGetCarDetail carDetail, ILocationCarsDAL locationCars, IReservationPeopleDAL reservationPeople, ILocationsDAL locations, ILocationCarsFareDAL locationCarsFare, IUserDatasDAL userDatas, IServicesDAL services, IServiceItemsDAL serviceItems, IServicePropertiesDAL serviceProperties, IServiceCategoriesDAL serviceCategories, IReservationServicesTableDAL reservationServicesTable, IWebHostEnvironment env, IMail mail, ILoginAuthDAL loginAuth, ICouponsDAL coupons, IMyCarsDAL myCars, IFileOperation fileOperation, IUserDocsDAL docs, IPayment payment, ITReservationHelpers tReservationHelpers, IWithdrawalRequestDAL withdrawalRequest, IPaymentDetailDAL paymentDetail)
        {
            _drivers = drivers;
            _reservations = reservations;
            _carDetail = carDetail;
            _locationCars = locationCars;
            _reservationPeople = reservationPeople;
            _location = locations;
            _locationCarsFare = locationCarsFare;
            _user = userDatas;
            _services = services;
            _serviceItems = serviceItems;
            _serviceProperties = serviceProperties;
            _serviceCategories = serviceCategories;
            _reservationServicesTable = reservationServicesTable;
            _mail = mail;
            _loginAuth = loginAuth;
            _coupons = coupons;
            _myCars = myCars;
            _fileOperation = fileOperation;
            _docs = docs;
            _payment = payment;
            _tReservationHelpers = tReservationHelpers;
            _withdrawalRequest = withdrawalRequest;
            _paymentDetail = paymentDetail;
        }

        [HttpGet("denemeee")]
        public async Task<IActionResult> deneme(string data, bool json = false)
        {
            try
            {
                var newPayment = new PayDetail();

                newPayment.POST_URL = "https://posservicetest.esnekpos.com/api/pay/EYV3DPay";

                newPayment.Config = new PayConfig()
                {
                    MERCHANT = "TEST1234",
                    MERCHANT_KEY = "4oK26hK8MOXrIV1bzTRVPA==",
                    BACK_URL = "https://localhost:5001/fiyat-gel",
                    ORDER_AMOUNT = "80",
                    ORDER_REF_NUMBER = "RFN4786234",
                    PRICES_CURRENCY = "TRY"
                };

                newPayment.CreditCard = new PayCard()
                {
                    CC_NUMBER = "9792380000000009",
                    CC_CVV = "000",
                    EXP_MONTH = "12",
                    EXP_YEAR = "2023",
                    CC_OWNER = "TEST USER",
                    INSTALLMENT_NUMBER = "1"
                };

                newPayment.Customer = new PayCustomer()
                {
                    FIRST_NAME = "firstName",
                    LAST_NAME = "LastName",
                    MAIL = "asdasd@asdad.com",
                    PHONE = "05455456558",
                    CITY = "İstanbul",
                    STATE = "Kağıthane",
                    ADDRESS = "Merkez Mahallesi, Ayazma Cd. No:37/91 Papirus Plaza Kat:5, 34406 Kağıthane / İSTANBUL",
                    CLIENT_IP = HttpContext.Connection.RemoteIpAddress?.ToString()
                };

                newPayment.Product.Add(new PayProduct
                {
                    PRODUCT_AMOUNT = "80",
                    PRODUCT_CATEGORY = "TRANSFER",
                    PRODUCT_DESCRIPTION = "A'dan B'ye Transfer",
                    PRODUCT_NAME = "4565456 Reservasyon Kodu",
                    PRODUCT_ID = "4565456"
                });


                newPayment.HASH = EsnekposConfig.GetCheckKey("123456", "isyeri@bkm.com", "147258");

                var s = "";
                using (var client = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(newPayment), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(newPayment.POST_URL, content);
                    response.EnsureSuccessStatusCode();
                    s = await response.Content.ReadAsStringAsync();
                }

                //string dosyaYolu = "wwwroot/error.txt";

                //using (StreamWriter yazici = new StreamWriter(dosyaYolu))
                //{
                //    string metin = s;
                //    yazici.WriteLine(metin);
                //}

                return View(newPayment);
            }
            catch (Exception ex)
            {
                string dosyaYolu = "wwwroot/error.txt";

                using (StreamWriter yazici = new StreamWriter(dosyaYolu))
                {
                    string metin = ex.ToString();
                    yazici.WriteLine(metin);
                }
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("is-payment-success")]
        public IActionResult FiyatAl(string ORDER_REF_NUMBER, string RETURN_CODE, string RETURN_MESSAGE, string DATE)
        {
            try
            {
                var reservation = _reservations.SelectByFunc(a => a.ReservationCode == ORDER_REF_NUMBER && !a.IsThisReturn).FirstOrDefault();
                if (reservation is not null)
                {
                    if (RETURN_CODE == "0")
                    {

                        _reservations.SelectByFunc(a => a.ReservationCode == ORDER_REF_NUMBER).ForEach(a =>
                        {
                            a.PaymentStatus = "0";
                            a.PaymentStatusText = "Ödeme Başarılı";
                            _reservations.Update(a);
                        });


                        _paymentDetail.Insert(new PaymentDetail
                        {
                            Date = DateTime.Now,
                            PaymentPrice = reservation.TotalPrice.ToString(),
                            PaymentStatusCode = RETURN_CODE,
                            PaymentStatusText = RETURN_MESSAGE,
                            POSTDate = DATE,
                            ReservationCode = reservation.ReservationCode,
                            ReservationId = reservation.Id

                        });

                        return RedirectToAction("CreatedReservationDetail", "Reservation", new { id = reservation.Id });
                    }
                    else
                    {
                        _paymentDetail.Insert(new PaymentDetail
                        {
                            Date = DateTime.Now,
                            PaymentPrice = reservation.TotalPrice.ToString(),
                            PaymentStatusCode = RETURN_CODE,
                            PaymentStatusText = RETURN_MESSAGE,
                            POSTDate = DATE,
                            ReservationCode = reservation.ReservationCode,
                            ReservationId = reservation.Id

                        });

                        return RedirectToAction("CancelReservation", "Reservation", new { error_code = RETURN_CODE, error_text = RETURN_MESSAGE,reservationId = reservation.Id });
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            

            return BadRequest();
        }


        [Authorize(Roles = "0,2,4,5")]
        public IActionResult Index()
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                var userRole = Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Role).Select(a => a.Value).SingleOrDefault();

                var loginAuth = _loginAuth.SelectByID(userId);

                var today = DateTime.Today;
                var lastWeek = today.AddDays(7);

                var myLocationList = _location.SelectByFunc(a => a.UserId == userId && !a.IsDelete);
                var myCarsList = _myCars.SelectByFunc(a => a.UserId == userId && !a.IsDelete);
                var myReservations = _reservations.SelectByFunc(a => a.UserId == userId && !a.IsDelete);
                var myAWeekReservations = _reservations.SelectByFunc(a => a.ReservationDate >= today && a.ReservationDate < lastWeek && a.UserId == userId && !a.IsDelete);
                if (userRole == "0")
                {
                    myCarsList = _myCars.SelectByFunc(a => !a.IsDelete).Take(5).ToList();
                    myLocationList = _location.SelectByFunc(a => !a.IsDelete).Take(5).ToList();
                    myReservations = _reservations.SelectByFunc(a => !a.IsDelete).Take(50).ToList();
                    myAWeekReservations = _reservations.SelectByFunc(a => a.ReservationDate >= today && a.ReservationDate < lastWeek && !a.IsDelete).Take(50).ToList();
                }

                var myCars = new List<MyCars>();
                myCarsList.ForEach(a =>
                {
                    myCars.Add(_carDetail.CarDetail(a.Id));
                });

                var myDashboard = new DashboardVM()
                {
                    MyCars = myCars,
                    MyLocations = myLocationList,
                    User = _user.SelectByID(loginAuth.UserId),
                    Reservations = myReservations,
                    AWeekReservations = myAWeekReservations
                };

                return View(myDashboard);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }


        [HttpGet("createpaylink")]
        public IActionResult CreatePayLink()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                return Json(new { });
            }

        }


        [HttpGet("docs")]
        public IActionResult Docs()
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var docs = _docs.SelectByFunc(a => a.UserId == userId).FirstOrDefault();

                return View(docs);
            }
            catch (Exception)
            {
                return Json(new { });
            }

        }

        [HttpPost("docs")]
        public async Task<IActionResult> Docs(IFormFile docs1, IFormFile docs2, IFormFile docs3)
        {
            try
            {
                if (docs1 != null || docs2 != null || docs3 != null)
                {
                    var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                    var userIsEmpty = _docs.SelectByFunc(a => a.UserId == userId).FirstOrDefault();

                    var docs1Link = "";
                    var docs2Link = "";
                    var docs3Link = "";

                    if (docs1 != null)
                    {
                        docs1Link = await _fileOperation.UploadFile(docs1);
                        docs1Link = _fileOperation.GetFile(docs1Link);
                    }
                    else
                    {
                        if (userIsEmpty is not null)
                        {
                            docs1Link = userIsEmpty.Docs1;
                        }
                    }

                    if (docs2 != null)
                    {
                        docs2Link = await _fileOperation.UploadFile(docs2);
                        docs2Link = _fileOperation.GetFile(docs2Link);
                    }
                    else
                    {
                        if (userIsEmpty is not null)
                        {
                            docs2Link = userIsEmpty.Docs2;
                        }
                    }

                    if (docs3 != null)
                    {
                        docs3Link = await _fileOperation.UploadFile(docs3);
                        docs3Link = _fileOperation.GetFile(docs3Link);
                    }
                    else
                    {
                        if (userIsEmpty is not null)
                        {
                            docs3Link = userIsEmpty.Docs3;
                        }
                    }

                    if (userIsEmpty is null)
                    {
                        _docs.Insert(new UserDocs
                        {
                            Docs1 = docs1Link,
                            Docs2 = docs2Link,
                            Docs3 = docs3Link,
                            UserId = userId,
                            Docs1Status = docs1Link == "" ? false : true,
                            Docs2Status = docs2Link == "" ? false : true,
                            Docs3Status = docs3Link == "" ? false : true,
                            Docs1AdminStatus = false,
                            Docs2AdminStatus = false,
                            Docs3AdminStatus = false,
                            LastUpdate = DateTime.Now,
                        });
                    }
                    else
                    {
                        if (!userIsEmpty.Docs1AdminStatus)
                        {
                            userIsEmpty.Docs1 = docs1Link;
                            userIsEmpty.Docs1Status = docs1Link == "" ? false : true;
                        }

                        if (!userIsEmpty.Docs2AdminStatus)
                        {
                            userIsEmpty.Docs2 = docs2Link;
                            userIsEmpty.Docs2Status = docs2Link == "" ? false : true;
                        }

                        if (!userIsEmpty.Docs3AdminStatus)
                        {
                            userIsEmpty.Docs3 = docs3Link;
                            userIsEmpty.Docs3Status = docs3Link == "" ? false : true;
                        }

                        userIsEmpty.LastUpdate = DateTime.Now;
                        _docs.Update(userIsEmpty);
                    }

                    return Json(new { result = 1 });
                }
                return Json(new { result = 2 });
            }
            catch (Exception)
            {
                return Json(new { });
            }

        }

        [HttpGet("financial-accounting")]
        public IActionResult Satis()
        {
            try
            {
                var userRole = User.Claims.Where(a => a.Type == ClaimTypes.Role).Select(a => a.Value).SingleOrDefault();

                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var reservations = new List<Reservations>();
                if (userRole == "5")
                {
                    reservations = _reservations.SelectByFunc(a => a.SalesAgencyId == userId).OrderByDescending(a => a.ReservationDate).ToList();
                }
                else if (userRole == "0" || userRole == "4")
                {
                    reservations = _reservations.Select().OrderByDescending(a => a.ReservationDate).ToList();
                    reservations.ForEach(a =>
                    {
                        a = _tReservationHelpers.GetReservationAll(a.Id);
                    });
                }
                else
                {
                    reservations = _reservations.SelectByFunc(a => a.UserId == userId).OrderByDescending(a => a.ReservationDate).ToList();
                }

                var acceptedRequest = Math.Round(Convert.ToDecimal(_withdrawalRequest.SelectByFunc(a => a.UserId == userId && a.Status == true).Select(a => a.Price).Sum()), 2);


                var PageVM = new FinancialAccountingPageVM()
                {
                    Reservation = reservations,
                    IsPendingRequest = _withdrawalRequest.SelectByFunc(a => a.UserId == userId && a.Status == null).OrderBy(a => a.Date).FirstOrDefault()?.Status == null ? true : false,
                    RequestPrice = acceptedRequest
                };

                return View(PageVM);
            }
            catch (Exception ex)
            {
                string dosyaYolu = "wwwroot/error.txt";

                using (StreamWriter yazici = new StreamWriter(dosyaYolu))
                {
                    string metin = ex.ToString();
                    yazici.WriteLine(metin);
                }

                return BadRequest();
            }

        }

        [HttpGet("withdrawal-request")]
        public IActionResult GetWithdrawalRequest()
        {
            var withdrawalRequest = _withdrawalRequest.Select().OrderBy(a => a.Status).ToList();

            withdrawalRequest.ForEach(a =>
            {
                var userId = _loginAuth.SelectByID(a.UserId);

                a.User = _user.SelectByID(userId.UserId);

            });

            return View(withdrawalRequest);
        }


        [HttpPost("post-withdrawal-request")]
        public IActionResult PostWithdrawalRequest()
        {
            try
            {

                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                if (_withdrawalRequest.SelectByFunc(a => a.UserId == userId && a.Status == null).FirstOrDefault() is not null)
                {
                    return Json(new { result = 2 });
                }

                var userRole = User.Claims.Where(a => a.Type == ClaimTypes.Role).Select(a => a.Value).SingleOrDefault();

                var reservations = new List<Reservations>();
                if (userRole == "5")
                {
                    reservations = _reservations.SelectByFunc(a => a.SalesAgencyId == userId).OrderByDescending(a => a.ReservationDate).ToList();
                }
                else if (userRole == "0" || userRole == "4")
                {
                    reservations = _reservations.Select().OrderByDescending(a => a.ReservationDate).ToList();
                    reservations.ForEach(a =>
                    {
                        a = _tReservationHelpers.GetReservationAll(a.Id);
                    });
                }
                else
                {
                    reservations = _reservations.SelectByFunc(a => a.UserId == userId).OrderByDescending(a => a.ReservationDate).ToList();
                }

                decimal discountedSum = 0;

                if (userRole == "0")
                {
                    discountedSum = Math.Round(reservations.Where(a => a.Status != 4 && a.ReservationDate <= DateTime.Now.AddDays(-31)).Select(a => Convert.ToDecimal(a.GlobalPartnerFee)).Sum(), 2);
                    discountedSum = Math.Round(discountedSum + Math.Round(reservations.Where(a => a.Status != 4 && a.ReservationDate <= DateTime.Now.AddDays(-31)).Select(a => Convert.ToDecimal(a.ServiceFee)).Sum(), 2), 2);
                }
                else if (userRole == "2")
                {
                    discountedSum = Math.Round(reservations.Where(a => a.Status != 4 && a.ReservationDate <= DateTime.Now.AddDays(-31)).Select(a => Convert.ToDecimal(a.PartnerFee)).Sum(), 2);
                }
                else if (userRole == "5")
                {
                    discountedSum = Math.Round(reservations.Where(a => a.Status != 4 && a.ReservationDate <= DateTime.Now.AddDays(-31)).Select(a => Convert.ToDecimal(a.SalesFee)).Sum(), 2);
                }

                _withdrawalRequest.Insert(new WithdrawalRequest
                {
                    Date = DateTime.Now,
                    UserId = userId,
                    Price = (double)discountedSum,
                });




                return Json(new { result = 1 });
            }
            catch (Exception)
            {

                return Json(new { });
            }
        }


        [Authorize(Roles = "0,2,4,5")]
        [HttpGet("panel/profile")]
        public IActionResult Profile()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

            var loginAuth = _loginAuth.SelectByID(userId);

            var user = _user.SelectByID(loginAuth.UserId);

            if (user == null) { return NotFound(); }
            user.LoginAuth = loginAuth;

            return View(user);
        }


        [Authorize(Roles = "0,2,4,5")]
        [HttpPost("panel/profile")]
        public IActionResult Profile(UserDatas updateUser, IFormFile Img)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                var loginAuth = _loginAuth.SelectByID(userId);
                var user = _user.SelectByID(loginAuth.UserId);

                if (user == null) { return NotFound(); }

                user.Name = updateUser.Name;
                user.PhoneNumber = updateUser.PhoneNumber;
                user.Profession = updateUser.Profession;
                user.Img = updateUser.Img;

                _user.Update(user);


                return Json(new { result = 1 });
            }
            catch (Exception)
            {
                return Json(new { });
            }

        }


        [Authorize(Roles = "0,2,4,5")]
        [HttpGet("panel/settings")]
        public IActionResult Settings()
        {
            return View();
        }

        [Authorize(Roles = "0,2,4,5")]
        [HttpGet("panel/change-password")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize(Roles = "0,2,4,5")]
        [HttpPost("panel/change-password", Name = "panelChangePassword")]
        public IActionResult ChangePassword(string oldPassword, string newPassword)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                var user = _loginAuth.SelectByID(userId);
                if (user == null)
                {
                    return Json(new { });
                }

                if (GetMD5(oldPassword) == user.Password)
                {
                    user.Password = GetMD5(newPassword);
                    _loginAuth.Update(user);
                    //okey
                    return Json(new { result = 1 });
                }

                //wrong password
                return Json(new { result = 2 });
            }
            catch (Exception ex)
            {

                //error
                return Json(new { });
            }
        }


        [Authorize(Roles = "0,2,4,5")]
        [HttpGet("panel/bank-account")]
        public IActionResult BankAccount()
        {
            return View();
        }


        [Authorize(Roles = "0,2,4,5")]
        [HttpGet("panel/agreements")]
        public IActionResult Agreement()
        {
            return View();
        }

        [Authorize(Roles = "2,4")]
        [HttpGet("panel/my-company")]
        public IActionResult Company()
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var loginAuth = _loginAuth.SelectByID(userId);
            var user = _user.SelectByID(loginAuth.UserId);
            if (user == null) { return NotFound(); }
            user.LoginAuth = loginAuth;
            return View(user);
        }

        [Authorize(Roles = "2,4")]
        [HttpPost("panel/my-company")]
        public IActionResult Company(UserDatas updateUser)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                var loginAuth = _loginAuth.SelectByID(userId);
                var user = _user.SelectByID(loginAuth.UserId);
                if (user == null) { return NotFound(); }

                user.AboutUs = updateUser.AboutUs;
                user.CompanyPhoneNumber = updateUser.CompanyPhoneNumber;
                user.CompanyEmail = updateUser.CompanyEmail;
                user.CompanyWebsite = updateUser.CompanyWebsite;
                user.Address = updateUser.Address;
                user.CompanyName = updateUser.CompanyName;
                user.Country = updateUser.Country;
                user.Facebook = updateUser.Facebook;
                user.Linkedin = updateUser.Linkedin;

                _user.Update(user);

                return Json(new { result = 1 });
            }
            catch (Exception)
            {
                return Json(new { });
            }

        }

        [Authorize(Roles = "1")]
        [HttpGet("user-management")]
        public IActionResult UserManagement()
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

                var loginAuth = _loginAuth.SelectByID(userId);

                var myReservations = _reservations.SelectByFunc(a => a.ReservationUserId == loginAuth.UserId);


                myReservations.ForEach(a =>
                {
                    a.LocationCars = _locationCars.SelectByID(a.LocationCarId);
                    a.LocationCars.Car = _carDetail.CarDetail(a.LocationCars.CarId);
                    a.LocationCars.LocationCarsFares = _locationCarsFare.SelectByFunc(b => b.LocationCarId == a.LocationCarId);
                });

                var reservationVM = new ReservationsIndexVM()
                {
                    Reservations = myReservations
                };

                return View(reservationVM);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Authorize(Roles = "1")]
        [HttpGet("user-management/detail/{id}")]
        public IActionResult UserManagementDetail(int id)
        {
            try
            {
                var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                var reservation = _reservations.SelectByFunc(a => a.Id == id).FirstOrDefault();
                if (reservation is not null)
                {
                    var reservationLocationCars = _locationCars.SelectByID(reservation.LocationCarId);

                    reservation.LocationCars = reservationLocationCars;
                    if (reservationLocationCars != null)
                    {

                        reservation.LocationCars.Car = _carDetail.CarDetail(reservation.LocationCars.CarId);
                        reservation.LocationCars.Car.Service = _services.SelectByID(reservation.LocationCars?.Car?.SeriesId);


                        reservation.Driver = _drivers.SelectByID(reservation.DriverId);
                        if (reservation.Driver != null)
                        {
                            var loginAuth = _loginAuth.SelectByFunc(a => a.DriverId == reservation.DriverId).FirstOrDefault();
                            reservation.Driver.LoginAuth = loginAuth;
                        }
                        reservation.ReservationPeoples = _reservationPeople.SelectByFunc(a => a.ReservationId == reservation.Id);

                        var ReservationServicesTable = _reservationServicesTable.SelectByFunc(a => a.ReservationId == id);

                        ReservationServicesTable.ForEach(a =>
                        {
                            a.ServiceItem = _serviceItems.SelectByID(a.ServiceItemId);
                            a.ServiceItem.ServiceProperty = _serviceProperties.SelectByID(a.ServiceItem?.ServicePropertyId);
                            a.ServiceItem.Service = _services.SelectByID(a.ServiceItem?.ServiceId);
                            a.ServiceItem.ServiceProperty.ServiceCategory = _serviceCategories.SelectByID(a.ServiceItem?.ServiceProperty?.ServiceCategoryId);
                        });

                        if (reservation.Coupon != 0 && reservation.Coupon != null)
                        {
                            reservation.Coupons = _coupons.SelectByID(reservation.Coupon);
                        }

                        var reservationVM = new ReservationManagementVM()
                        {
                            Reservation = reservation,
                            Drivers = _drivers.SelectByFunc(a => a.UserId == userId && !a.IsDelete),
                            ReservationServicesTable = ReservationServicesTable
                        };
                        return View(reservationVM);
                    }

                    return RedirectToAction("Index", "Home");

                }
                return NotFound();
            }
            catch (Exception ex)
            {
                string dosyaYolu = "wwwroot/error.txt";

                using (StreamWriter yazici = new StreamWriter(dosyaYolu))
                {
                    string metin = ex.ToString();
                    yazici.WriteLine(metin);
                }

                return RedirectToAction("Index", "Home");
            }
        }

        public JsonResult RateDrive(int id, int rate)
        {
            try
            {
                var reservation = _reservations.SelectByFunc(a => a.Id == id).FirstOrDefault();
                if (reservation is not null)
                {
                    if (rate >= 5 && rate <= 0)
                    {
                        rate = 0;
                    }

                    reservation.Rate = rate;
                    _reservations.Update(reservation);

                    return Json(new { result = 1 });
                }
                return Json(new { });
            }
            catch (Exception ex)
            {
                string dosyaYolu = "wwwroot/error.txt";

                using (StreamWriter yazici = new StreamWriter(dosyaYolu))
                {
                    string metin = ex.ToString();
                    yazici.WriteLine(metin);
                }

                return Json(new { });
            }
        }

        public static string GetMD5(string value)
        {
            MD5 md5 = MD5.Create();
            byte[] md5Bytes = System.Text.Encoding.Default.GetBytes(value);
            byte[] cryString = md5.ComputeHash(md5Bytes);
            string md5Str = string.Empty;
            for (int i = 0; i < cryString.Length; i++)
            {
                md5Str += cryString[i].ToString("X");
            }
            return md5Str;
        }
    }
}



