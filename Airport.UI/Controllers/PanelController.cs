using Airport.DBEntitiesDAL.Interfaces;
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
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Airport.UI.Models.ITransactions;
using System.Net.Http;
using System.Text;

namespace Airport.UI.Controllers;

public class PanelController(IReservationsDAL reservationsDal, IDriversDAL driversDal, IGetCarDetail carDetailDal, ILocationCarsDAL locationCarDal, IReservationPeopleDAL reservationPeopleDal, ILocationsDAL locationDal, ILocationCarsFareDAL locationCarsFareDal, IUserDatasDAL userDal, IServicesDAL servicesDal, IServiceItemsDAL serviceItemsDal, IServicePropertiesDAL servicePropertiesDal, IServiceCategoriesDAL serviceCategoriesDal, IReservationServicesTableDAL reservationServicesTableDal, ILoginAuthDAL loginAuthDal, ICouponsDAL couponsDal, IMyCarsDAL myCarsDal, IFileOperation fileOperationDal, IUserDocsDAL docsDal, ITReservationHelpers tReservationHelpersDal, IWithdrawalRequestDAL withdrawalRequestDal, IPaymentDetailDAL paymentDetailDal) : PanelAuthController
{

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
            var reservation = reservationsDal.SelectByFunc(a => a.ReservationCode == ORDER_REF_NUMBER && !a.IsThisReturn).FirstOrDefault();
            if (reservation is not null)
            {
                if (RETURN_CODE == "0")
                {

                    reservationsDal.SelectByFunc(a => a.ReservationCode == ORDER_REF_NUMBER).ForEach(a =>
                    {
                        a.PaymentStatus = "0";
                        a.PaymentStatusText = "Ödeme Başarılı";
                        reservationsDal.Update(a);
                    });


                    paymentDetailDal.Insert(new PaymentDetail
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
                    paymentDetailDal.Insert(new PaymentDetail
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

            var loginAuth = loginAuthDal.SelectByID(userId);

            var today = DateTime.Today;
            var lastWeek = today.AddDays(7);

            var myLocationList = locationDal.SelectByFunc(a => a.UserId == userId && !a.IsDelete);
            var myCarsList = myCarsDal.SelectByFunc(a => a.UserId == userId && !a.IsDelete);
            var myReservations = reservationsDal.SelectByFunc(a => a.UserId == userId && !a.IsDelete);
            var myAWeekReservations = reservationsDal.SelectByFunc(a => a.ReservationDate >= today && a.ReservationDate < lastWeek && a.UserId == userId && !a.IsDelete);
            if (userRole == "0")
            {
                myCarsList = myCarsDal.SelectByFunc(a => !a.IsDelete).Take(5).ToList();
                myLocationList = locationDal.SelectByFunc(a => !a.IsDelete).Take(5).ToList();
                myReservations = reservationsDal.SelectByFunc(a => !a.IsDelete).Take(50).ToList();
                myAWeekReservations = reservationsDal.SelectByFunc(a => a.ReservationDate >= today && a.ReservationDate < lastWeek && !a.IsDelete).Take(50).ToList();
            }

            var myCars = new List<MyCars>();
            myCarsList.ForEach(a =>
            {
                myCars.Add(carDetailDal.CarDetail(a.Id));
            });

            var myDashboard = new DashboardVM()
            {
                MyCars = myCars,
                MyLocations = myLocationList,
                User = userDal.SelectByID(loginAuth.UserId),
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
            var docs = docsDal.SelectByFunc(a => a.UserId == userId).FirstOrDefault();

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

                var userIsEmpty = docsDal.SelectByFunc(a => a.UserId == userId).FirstOrDefault();

                var docs1Link = "";
                var docs2Link = "";
                var docs3Link = "";

                if (docs1 != null)
                {
                    docs1Link = await fileOperationDal.UploadFile(docs1);
                    docs1Link = fileOperationDal.GetFile(docs1Link);
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
                    docs2Link = await fileOperationDal.UploadFile(docs2);
                    docs2Link = fileOperationDal.GetFile(docs2Link);
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
                    docs3Link = await fileOperationDal.UploadFile(docs3);
                    docs3Link = fileOperationDal.GetFile(docs3Link);
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
                    docsDal.Insert(new UserDocs
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
                    docsDal.Update(userIsEmpty);
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
                reservations = reservationsDal.SelectByFunc(a => a.SalesAgencyId == userId).OrderByDescending(a => a.ReservationDate).ToList();
            }
            else if (userRole == "0" || userRole == "4")
            {
                reservations = reservationsDal.Select().OrderByDescending(a => a.ReservationDate).ToList();
                reservations.ForEach(a =>
                {
                    a = tReservationHelpersDal.GetReservationAll(a.Id);
                });
            }
            else
            {
                reservations = reservationsDal.SelectByFunc(a => a.UserId == userId).OrderByDescending(a => a.ReservationDate).ToList();
            }

            var acceptedRequest = Math.Round(Convert.ToDecimal(withdrawalRequestDal.SelectByFunc(a => a.UserId == userId && a.Status == true).Select(a => a.Price).Sum()), 2);


            var PageVM = new FinancialAccountingPageVM()
            {
                Reservation = reservations,
                IsPendingRequest = withdrawalRequestDal.SelectByFunc(a => a.UserId == userId && a.Status == null).OrderBy(a => a.Date).FirstOrDefault()?.Status == null ? true : false,
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
        var withdrawalRequest = withdrawalRequestDal.Select().OrderBy(a => a.Status).ToList();

        withdrawalRequest.ForEach(a =>
        {
            var userId = loginAuthDal.SelectByID(a.UserId);

            a.User = userDal.SelectByID(userId.UserId);

        });

        return View(withdrawalRequest);
    }


    [HttpPost("post-withdrawal-request")]
    public IActionResult PostWithdrawalRequest()
    {
        try
        {

            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());

            if (withdrawalRequestDal.SelectByFunc(a => a.UserId == userId && a.Status == null).FirstOrDefault() is not null)
            {
                return Json(new { result = 2 });
            }

            var userRole = User.Claims.Where(a => a.Type == ClaimTypes.Role).Select(a => a.Value).SingleOrDefault();

            var reservations = new List<Reservations>();
            if (userRole == "5")
            {
                reservations = reservationsDal.SelectByFunc(a => a.SalesAgencyId == userId).OrderByDescending(a => a.ReservationDate).ToList();
            }
            else if (userRole == "0" || userRole == "4")
            {
                reservations = reservationsDal.Select().OrderByDescending(a => a.ReservationDate).ToList();
                reservations.ForEach(a =>
                {
                    a = tReservationHelpersDal.GetReservationAll(a.Id);
                });
            }
            else
            {
                reservations = reservationsDal.SelectByFunc(a => a.UserId == userId).OrderByDescending(a => a.ReservationDate).ToList();
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

            withdrawalRequestDal.Insert(new WithdrawalRequest
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

        var loginAuth = loginAuthDal.SelectByID(userId);

        var user = userDal.SelectByID(loginAuth.UserId);

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

            var loginAuth = loginAuthDal.SelectByID(userId);
            var user = userDal.SelectByID(loginAuth.UserId);

            if (user == null) { return NotFound(); }

            user.Name = updateUser.Name;
            user.PhoneNumber = updateUser.PhoneNumber;
            user.Profession = updateUser.Profession;
            user.Img = updateUser.Img;

            userDal.Update(user);


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

            var user = loginAuthDal.SelectByID(userId);
            if (user == null)
            {
                return Json(new { });
            }

            if (GetMD5(oldPassword) == user.Password)
            {
                user.Password = GetMD5(newPassword);
                loginAuthDal.Update(user);
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
        var loginAuth = loginAuthDal.SelectByID(userId);
        var user = userDal.SelectByID(loginAuth.UserId);
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

            var loginAuth = loginAuthDal.SelectByID(userId);
            var user = userDal.SelectByID(loginAuth.UserId);
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

            userDal.Update(user);

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

            var loginAuth = loginAuthDal.SelectByID(userId);

            var myReservations = reservationsDal.SelectByFunc(a => a.ReservationUserId == loginAuth.UserId);


            myReservations.ForEach(a =>
            {
                a.LocationCars = locationCarDal.SelectByID(a.LocationCarId);
                a.LocationCars.Car = carDetailDal.CarDetail(a.LocationCars.CarId);
                a.LocationCars.LocationCarsFares = locationCarsFareDal.SelectByFunc(b => b.LocationCarId == a.LocationCarId);
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
            var reservation = reservationsDal.SelectByFunc(a => a.Id == id).FirstOrDefault();
            if (reservation is not null)
            {
                var reservationLocationCars = locationCarDal.SelectByID(reservation.LocationCarId);

                reservation.LocationCars = reservationLocationCars;
                if (reservationLocationCars != null)
                {

                    reservation.LocationCars.Car = carDetailDal.CarDetail(reservation.LocationCars.CarId);
                    reservation.LocationCars.Car.Service = servicesDal.SelectByID(reservation.LocationCars?.Car?.SeriesId);


                    reservation.Driver = driversDal.SelectByID(reservation.DriverId);
                    if (reservation.Driver != null)
                    {
                        var loginAuth = loginAuthDal.SelectByFunc(a => a.DriverId == reservation.DriverId).FirstOrDefault();
                        reservation.Driver.LoginAuth = loginAuth;
                    }
                    reservation.ReservationPeoples = reservationPeopleDal.SelectByFunc(a => a.ReservationId == reservation.Id);

                    var ReservationServicesTable = reservationServicesTableDal.SelectByFunc(a => a.ReservationId == id);

                    ReservationServicesTable.ForEach(a =>
                    {
                        a.ServiceItem = serviceItemsDal.SelectByID(a.ServiceItemId);
                        a.ServiceItem.ServiceProperty = servicePropertiesDal.SelectByID(a.ServiceItem?.ServicePropertyId);
                        a.ServiceItem.Service = servicesDal.SelectByID(a.ServiceItem?.ServiceId);
                        a.ServiceItem.ServiceProperty.ServiceCategory = serviceCategoriesDal.SelectByID(a.ServiceItem?.ServiceProperty?.ServiceCategoryId);
                    });

                    if (reservation.Coupon != 0 && reservation.Coupon != null)
                    {
                        reservation.Coupons = couponsDal.SelectByID(reservation.Coupon);
                    }

                    var reservationVM = new ReservationManagementVM()
                    {
                        Reservation = reservation,
                        Drivers = driversDal.SelectByFunc(a => a.UserId == userId && !a.IsDelete),
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
            var reservation = reservationsDal.SelectByFunc(a => a.Id == id).FirstOrDefault();
            if (reservation is not null)
            {
                if (rate >= 5 && rate <= 0)
                {
                    rate = 0;
                }

                reservation.Rate = rate;
                reservationsDal.Update(reservation);

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



