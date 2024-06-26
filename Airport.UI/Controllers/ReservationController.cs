using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.MessageExtensions.Interfaces;
using Airport.UI.Models.Extendions;
using Airport.UI.Models.IM;
using Airport.UI.Models.Interface;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Airport.MessageExtension.VM;
using Airport.MessageExtension.Interfaces;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Airport.UI.Controllers;

public class ReservationController(ILocationsDAL locationDal, ILocationCarsDAL locationCarDal,  IGetCarDetail carDetailDal, IUserDatasDAL userDatasDal, IReservationsDAL reservationsDal, IGetCarDetail getCarDal, IReservationPeopleDAL reservationsPeopleDal, IMail mailDal, IWebHostEnvironment envDal, IServicesDAL servicesDal, IServiceItemsDAL serviceItemsDal, IServicePropertiesDAL servicePropertiesDal, IServiceCategoriesDAL serviceCategoriesDal, IReservationServicesTableDAL reservationServicesTableDal, ICouponsDAL couponsDal, ISMS smsDal, ILoginAuthDAL loginAuthDal, ITReservations reservationTDal, IApiResult apiResultDal, ITReservationHelpers tReservationHelpersDal, IPayment paymentDal, IConfiguration configuration) : Controller
{
    [HttpGet("reservation", Name = "getLocationValue")]
    public async Task<IActionResult> ReservationStepTwo(GetResevationIM reservation)
    {
        try
        {
            var pickLocationValues = await apiResultDal.LocationValues(reservation.PickValue);
            var dropLocationValues = await apiResultDal.LocationValues(reservation.DropValue);

            var betweenLocation = await apiResultDal.DistanceMatrixValues(pickLocationValues.Result.Geometry.Location.lat, pickLocationValues.Result.Geometry.Location.lng, dropLocationValues.Result.Geometry.Location.lat, dropLocationValues.Result.Geometry.Location.lng);

            if (betweenLocation.status == "OK")
            {
                var allDatas = await reservationTDal.GetLocationAllDataList(pickLocationValues, dropLocationValues);

                var getreservation = new List<GetReservationValues>();
                var selectedLocationsMini = new List<LocationIsOutMiniVM>();

                var selectedLocations = await reservationTDal.GetLocationIsOutList(allDatas);

                double minKm = 0;

                if (betweenLocation.rows[0].elements[0].status == "OK")
                {
                    var lastKm = Math.Round(Convert.ToDouble(betweenLocation.rows[0].elements[0].distance.value) / 100) * 100;
                    minKm = lastKm / 1000;
                    var lastLocations = await reservationTDal.ReservationList(selectedLocations, reservation, minKm, pickLocationValues, dropLocationValues);

                    getreservation = lastLocations.Locations;
                    selectedLocationsMini = lastLocations.MiniLocations;
                }

                var lastVM = new ReservationStepTwoVM()
                {
                    ReservationValues = getreservation,
                    DropLocationLatLng = $"lat:{dropLocationValues.Result.Geometry.Location.lat},lng:{dropLocationValues.Result.Geometry.Location.lng}",
                    PickLocationLatLng = $"lat:{pickLocationValues.Result.Geometry.Location.lat},lng:{pickLocationValues.Result.Geometry.Location.lng}",
                    DropLocationPlaceId = reservation.DropValue,
                    PickLocationPlaceId = reservation.PickValue,
                    Distance = betweenLocation.rows[0].elements[0].distance?.text,
                    Duration = betweenLocation.rows[0].elements[0].duration?.text,
                    SelectedReservationValues = reservation,

                };

                var reservationDatas = new ReservationDatasVM()
                {
                    DropLocationLatLng = $"lat:{dropLocationValues.Result.Geometry.Location.lat},lng:{dropLocationValues.Result.Geometry.Location.lng}",
                    PickLocationLatLng = $"lat:{pickLocationValues.Result.Geometry.Location.lat},lng:{pickLocationValues.Result.Geometry.Location.lng}",
                    DropLocationPlaceId = reservation.DropValue,
                    PickLocationPlaceId = reservation.PickValue,
                    PickLocationName = pickLocationValues.Result.formatted_address,
                    DropLocationName = dropLocationValues.Result.formatted_address,
                    KM = minKm,
                    ReservationValues = reservation,
                    Distance = betweenLocation.rows[0].elements[0].distance?.text,
                    Duration = betweenLocation.rows[0].elements[0].duration?.text
                };



                HttpContext.Session.Remove("reservationData");
                HttpContext.Session.MySet("reservationData", reservationDatas);
                HttpContext.Session.MySet("selectedLocationMini", selectedLocationsMini);


                lastVM.ReservationValues = lastVM.ReservationValues.OrderBy(a => a.LastPrice).ToList();

                return View(lastVM);
            }


            return RedirectToAction("Index", "Home");
            //return View(lastVM);
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

    [HttpGet("reservation-step-three/{id}")]
    public async Task<IActionResult> ReservationStepThree(int id)
    {
        try
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var user = new UserDatas();

            if (userId != 0)
            {
                var loginAuth = loginAuthDal.SelectByID(userId);
                user = userDatasDal.SelectByID(loginAuth?.UserId);
                user.LoginAuth = loginAuth;
            }
            else
            {
                user = null;
            }


            var datas = HttpContext.Session.MyGet<ReservationDatasVM>("reservationData");

            if (datas == null)
            {
                return NotFound();
            }

            var selectedDatasMini = HttpContext.Session.MyGet<List<LocationIsOutMiniVM>>("selectedLocationMini").Where(a => a.LocationCarId == id).FirstOrDefault();

            if (selectedDatasMini != null)
            {
                var locationCar = locationCarDal.SelectByID(selectedDatasMini.LocationCarId);

                //fixed= 1
                //per = 2 

                var prices = tReservationHelpersDal.ReservationPrice(locationCar.Id, datas.KM, false, 0, datas.ReservationValues.ReturnStatus, selectedDatasMini.IsOutZone);

                datas.OfferPrice = prices.OfferPrice;
                datas.ServiceFee = prices.ServiceFee;
                datas.TotalPrice = prices.LastPrice;
                datas.ExtraServiceFee = prices.ExtraServiceFee;
                datas.SalesFee = prices.SalesFee;
                datas.PartnerFee = prices.PartnerFee;
                datas.GlobalPartnerFee = prices.GlobalPartnerFee;

                datas.LocationCar = locationCar;
                datas.LocationCar.Car = carDetailDal.CarDetail(locationCar.CarId);
                datas.LocationCar.Car.Service = servicesDal.SelectByID(datas.LocationCar.Car.ServiceId);
                datas.IsOutZone = selectedDatasMini.IsOutZone;
                var selectedCarItems = new List<PriceServiceList>();
                if (datas.LocationCar.Car.Service != null)
                {
                    datas.LocationCar.Car.Service.ServiceItems = serviceItemsDal.SelectByFunc(a => a.ServiceId == datas.LocationCar.Car.ServiceId);

                    datas.LocationCar.Car.Service.ServiceItems.ForEach(a =>
                    {
                        a.ServiceProperty = servicePropertiesDal.SelectByID(a.ServicePropertyId);
                    });
                }


                HttpContext.Session.Remove("reservationData");

                HttpContext.Session.MySet("reservationData", datas);

                var reservation = new ReservationStepThreeVM()
                {
                    SelectedData = datas,
                    User = user,
                    ServiceItems = selectedCarItems,
                };

                return View(reservation);
            }

            return RedirectToAction("Index", "Home");
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

    [HttpPost("reservation-get-code")]
    public async Task<IActionResult> ReservationLastStep(ReservationLastStepVM reservation)
    {
        try
        {
            var createReservation = HttpContext.Session.MyGet<ReservationDatasVM>("reservationData");

            if (createReservation == null)
            {
                return NotFound();
            }

            Random random = new Random();
            int kodUzunlugu = 6;
            string karakterler = "0123456789";
            string kod = string.Concat(Enumerable.Range(0, kodUzunlugu).Select(_ => karakterler[random.Next(karakterler.Length)]));

            var totalServiceFee = reservation.ServiceList?.Sum(item1 => serviceItemsDal.SelectByID(item1.SelectedValue).Price * item1.PeopleCountInput) ?? 0.0;

            var getCoupon = couponsDal.SelectByFunc(a => a.Active && a.CouponCode == reservation.CouponCode && a.CouponStartDate <= DateTime.Now
                && a.CouponFinishDate >= DateTime.Now && (a.IsPerma || (a.CouponLimit > a.UsingCount && !a.IsPerma))).FirstOrDefault();

            var calcPrice = tReservationHelpersDal.ReservationPrice(createReservation.LocationCar.Id, createReservation.KM, false, totalServiceFee, createReservation.ReservationValues.ReturnStatus, createReservation.IsOutZone, reservation.CouponCode);

            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Sid)?.Value);

            var loginAuth = userId == 0 ? null : loginAuthDal.SelectByID(userId);
            var user = loginAuth == null ? null : userDatasDal.SelectByID(loginAuth.UserId);
            if (user != null)
            {
                user.LoginAuth = loginAuth;
            }

            var item = new Reservations()
            {
                DropLatLng = createReservation.DropLocationLatLng,
                PickLatLng = createReservation.PickLocationLatLng,
                Phone = reservation.PassengerPhone,
                DropPlaceId = createReservation.DropLocationPlaceId,
                PickPlaceId = createReservation.PickLocationPlaceId,
                Email = reservation.PassengerEmail,
                LocationCarId = createReservation.LocationCar.Id,
                Name = reservation.PassengerName,
                ReservationCode = kod,

                PartnerFee = calcPrice.PartnerFee,
                SalesFee = calcPrice.SalesFee,
                ServiceFee = calcPrice.ServiceFee,
                OfferPrice = calcPrice.OfferPrice,
                TotalPrice = calcPrice.LastPrice,
                GlobalPartnerFee = calcPrice.GlobalPartnerFee,
                DiscountServiceFee = calcPrice.DiscountServiceFee,
                DiscountOfferPrice = calcPrice.DiscountOfferPrice,
                DiscountExtraService = calcPrice.DiscountExtraService,

                Surname = reservation.PassengerSurname,
                DropFullName = createReservation.DropLocationName,
                PickFullName = createReservation.PickLocationName,
                PeopleCount = createReservation.ReservationValues.PeopleCount,
                ReservationDate = createReservation.ReservationValues.FlightTime,
                ReturnDate = createReservation.ReservationValues.ReturnDate,
                ReturnStatus = createReservation.ReservationValues.ReturnStatus,
                DistanceText = createReservation.Distance,
                DurationText = createReservation.Duration,
                Discount = 0,
                IsDiscount = false,
                UserId = locationDal.SelectByID(createReservation.LocationCar.LocationId).UserId,
                ExtraServiceFee = totalServiceFee,
                Comment = reservation.PassengerComment,
                Status = 1,
                IsDelete = false,
                HidePrice = reservation.HidePrice,
                Coupon = getCoupon?.Id,
                RealPhone = reservation.PassengerRealPhone,
                DiscountText = getCoupon?.Comment,
                ReservationUserId = user?.Id,
                Rate = 0,
                DiscountRate = getCoupon?.Discount,
                LastUpdate = DateTime.Now,
                CreateDate = DateTime.Now,

            };

            item.Coupons = getCoupon;

            item.ReservationServicesTables = reservation.ServiceList?
                .Select(item1 => new ReservationServicesTable
                {
                    ReservationId = item.Id,
                    PeopleCount = item1.PeopleCountInput,
                    Price = serviceItemsDal.SelectByID(item1.SelectedValue).Price,
                    ServiceItemId = item1.SelectedValue,
                })
                .ToList();

            item.LocationCars = locationCarDal.SelectByID(item.LocationCarId);
            item.LocationCars.Car = getCarDal.CarDetail(item.LocationCars.CarId);

            var loginAuth2 = loginAuthDal.SelectByID(item.UserId);
            item.User = userDatasDal.SelectByID(loginAuth2.UserId);

            item.ReservationPeoples = reservation.PassengerList?
                .Select(a => new ReservationPeople
                {
                    Name = a.PassengerName,
                    Surname = a.PassengerSurname,
                    ReservationId = item.Id
                })
                .ToList();

            item.ReservationServicesTables = reservationServicesTableDal.SelectByFunc(a => a.ReservationId == item.Id);
            item.ReservationServicesTables.ForEach(a =>
            {
                a.ServiceItem = serviceItemsDal.SelectByID(a.ServiceItemId);
                a.ServiceItem.ServiceProperty = servicePropertiesDal.SelectByID(a.ServiceItem.ServicePropertyId);
            });

            HttpContext.Session.MySet("reservation", item);
            return Json(new { result = 1 });
            //return RedirectToAction("ReservationStepPayment", "Reservation");
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

    [HttpGet("reservation-step-payment")]
    public async Task<IActionResult> ReservationStepPayment()
    {
        try
        {
            var reservation = HttpContext.Session.MyGet<Reservations>("reservation");
            if (reservation != null)
            {
                return View(reservation);
            }
            return NotFound();
        }
        catch (Exception)
        {
            throw new Exception();
        }
    }

    [HttpPost("reservation-step-payment")]
    public async Task<IActionResult> ReservationStepPayment(PaymentCardDetailVM cardDetail)
    {
        try
        {
            var reservation = HttpContext.Session.MyGet<Reservations>("reservation");
            if (reservation != null)
            {
                cardDetail.InstallmentNumber = "1";
                var returnStatus = await paymentDal.CreatePayment(cardDetail, reservation);
                if (returnStatus.RETURN_CODE == "0")
                {
                    if (!reservationsDal.SelectByFunc(a => a.ReservationCode == reservation.ReservationCode).Any())
                    {
                        var getCoupon = couponsDal.Select().FirstOrDefault(a => a.Active && a.Id == reservation.Coupon && a.CouponStartDate <= DateTime.Now
&& a.CouponFinishDate >= DateTime.Now && (a.IsPerma || a.CouponLimit > a.UsingCount));

                        if (getCoupon != null)
                        {
                            getCoupon.UsingCount++;
                            couponsDal.Update(getCoupon);
                        }
                        var createdReservation = reservationsDal.Insert(new Reservations
                        {
                            DropLatLng = reservation.DropLatLng,
                            PickLatLng = reservation.PickLatLng,
                            Phone = reservation.Phone,
                            DropPlaceId = reservation.DropPlaceId,
                            PickPlaceId = reservation.PickPlaceId,
                            Email = reservation.Email,
                            LocationCarId = reservation.LocationCars.Id,
                            Name = reservation.Name,
                            ReservationCode = reservation.ReservationCode,
                            Surname = reservation.Surname,
                            DropFullName = reservation.DropFullName,
                            PickFullName = reservation.PickFullName,
                            PeopleCount = reservation.PeopleCount,
                            ReservationDate = reservation.ReservationDate,
                            ReturnDate = reservation.ReturnDate,
                            ReturnStatus = reservation.ReturnStatus,
                            DistanceText = reservation.DistanceText,
                            DurationText = reservation.DurationText,
                            Discount = reservation.Discount,
                            IsDiscount = reservation.IsDiscount,
                            UserId = reservation.UserId,
                            ExtraServiceFee = reservation.ExtraServiceFee,
                            Comment = reservation.Comment,
                            Status = reservation.Status,
                            IsDelete = reservation.IsDelete,
                            HidePrice = reservation.HidePrice,
                            Coupon = reservation.Coupon,
                            RealPhone = reservation.RealPhone,
                            DiscountText = reservation.DiscountText,
                            ReservationUserId = reservation.ReservationUserId,
                            Rate = reservation.Rate,
                            LastUpdate = DateTime.Now,
                            CreateDate = DateTime.Now,
                            IsThisReturn = false,
                            PartnerFee = reservation.PartnerFee,
                            SalesFee = reservation.SalesFee,
                            ServiceFee = reservation.ServiceFee,
                            OfferPrice = reservation.OfferPrice,
                            TotalPrice = reservation.TotalPrice,
                            GlobalPartnerFee = reservation.GlobalPartnerFee,
                            DiscountServiceFee = reservation.DiscountServiceFee,
                            DiscountOfferPrice = reservation.DiscountOfferPrice,
                            DiscountExtraService = reservation.DiscountExtraService,
                            PaymentStatus = "1",
                            PaymentStatusText = "Ödeme Yapılmadı"
                        });

                        if (createdReservation.ReturnStatus)
                        {
                            reservationsDal.Insert(new Reservations
                            {
                                DropLatLng = reservation.DropLatLng,
                                PickLatLng = reservation.PickLatLng,
                                Phone = reservation.Phone,
                                DropPlaceId = reservation.DropPlaceId,
                                PickPlaceId = reservation.PickPlaceId,
                                Email = reservation.Email,
                                LocationCarId = reservation.LocationCars.Id,
                                Name = reservation.Name,
                                ReservationCode = reservation.ReservationCode,
                                Surname = reservation.Surname,
                                DropFullName = reservation.DropFullName,
                                PickFullName = reservation.PickFullName,
                                PeopleCount = reservation.PeopleCount,
                                ReservationDate = reservation.ReturnDate,
                                ReturnDate = reservation.ReturnDate,
                                ReturnStatus = false,
                                DistanceText = reservation.DistanceText,
                                DurationText = reservation.DurationText,
                                Discount = reservation.Discount,
                                IsDiscount = reservation.IsDiscount,
                                UserId = reservation.UserId,
                                ExtraServiceFee = reservation.ExtraServiceFee,
                                Comment = reservation.Comment,
                                Status = reservation.Status,
                                IsDelete = reservation.IsDelete,
                                HidePrice = reservation.HidePrice,
                                Coupon = reservation.Coupon,
                                RealPhone = reservation.RealPhone,
                                DiscountText = reservation.DiscountText,
                                ReservationUserId = reservation.ReservationUserId,
                                Rate = reservation.Rate,
                                LastUpdate = DateTime.Now,
                                CreateDate = DateTime.Now,
                                IsThisReturn = true,

                                PartnerFee = reservation.PartnerFee,
                                SalesFee = reservation.SalesFee,
                                ServiceFee = reservation.ServiceFee,
                                OfferPrice = reservation.OfferPrice,
                                TotalPrice = reservation.TotalPrice,
                                GlobalPartnerFee = reservation.GlobalPartnerFee,
                                DiscountServiceFee = reservation.DiscountServiceFee,
                                DiscountOfferPrice = reservation.DiscountOfferPrice,
                                DiscountExtraService = reservation.DiscountExtraService,
                                PaymentStatus = "1",
                                PaymentStatusText = "Ödeme Yapılmadı"
                            });
                        }

                        reservation.Id = createdReservation.Id;

                        reservation.ReservationPeoples?.ForEach(item => item.ReservationId = createdReservation.Id);

                        reservation.ReservationServicesTables?.ForEach(item => item.ReservationId = createdReservation.Id);

                        if (reservation.ReservationPeoples != null)
                        {
                            reservationsPeopleDal?.InsertRage(reservation.ReservationPeoples);
                        }

                        reservation.ReservationServicesTables = reservation.ReservationServicesTables.Where(a => a.PeopleCount > 0).ToList();
                        if (reservation.ReservationServicesTables != null && reservation.ReservationServicesTables.Count > 0)
                        {
                            reservationServicesTableDal.InsertRage(reservation.ReservationServicesTables);
                        }

                        PdfCreator pdfCreator = new PdfCreator(envDal);
                        //pdfCreator.CreateReservationPDF(reservation.ReservationCode + "-" + reservation.Id, reservation);

                        mailDal.SendReservationMail(reservation);

                        var mesaj = new Mesaj[]
                        {
                        new Mesaj
                        {
                            dest = reservation.RealPhone,
                            msg = @$"Your reservation code {reservation.ReservationCode} has been created. Voucher Link {configuration["PageLinks:PageGlobalLink"]}/pdf/{reservation.ReservationCode}-{reservation.Id}.pdf"
                        }
                        };

                        smsDal.SmsForReservation(mesaj);

                        HttpContext.Session.MySet("reservation", reservation);
                    }


                    return Redirect(returnStatus.URL_3DS);
                }
                else
                {
                    return RedirectToAction("CancelReservation", "Reservation", new { error_code = returnStatus.RETURN_CODE, error_text = returnStatus.RETURN_MESSAGE, reservationId = reservation.Id });
                }
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpGet("reservation-success")]
    public IActionResult CreatedReservationDetail(int id)
    {
        var reservation = HttpContext.Session.MyGet<Reservations>("reservation");
        if (reservation != null && reservation.Id == id)
        {
            reservation = tReservationHelpersDal.GetReservationAll(id);

            return View(reservation);
        }

        return NotFound();
    }

    [HttpGet("reservation-cancel")]
    public IActionResult CancelReservation(string error_code, string error_text, string reservationId)
    {
        var reservation = HttpContext.Session.MyGet<Reservations>("reservation");
        if (reservation != null)
        {
            var PageVM = new CancelPaymentVM()
            {
                Error_Code = error_code,
                Error_Text = error_text,
                ReservationId = reservationId
            };

            return View(PageVM);
        }
        return NotFound();
    }

    [HttpPost("manage-reservation", Name = "checkReservation")]
    public async Task<IActionResult> GetReservation(string reservationCode, string email)
    {
        try
        {
            var reservation = reservationsDal.SelectByFunc(a => a.Email == email && a.ReservationCode == reservationCode).FirstOrDefault();
            if (reservation == null)
            {
                ViewBag.Warning = "Warning";
                return RedirectToAction("ManageReservation", "Home");
            }

            reservation.LocationCars = locationCarDal.SelectByID(reservation.LocationCarId);
            reservation.LocationCars.Car = getCarDal.CarDetail(reservation.LocationCars.CarId);

            return View(reservation);
        }
        catch (Exception)
        {
            ViewBag.Error = "Error";
            return RedirectToAction("Index", "Home");
        }

    }

    [Authorize(Roles = "0,2,4,5")]
    [HttpGet("panel/manual-reservation-one")]
    public async Task<IActionResult> ManualReservationStepOne()
    {
        return View();
    }

    [Authorize(Roles = "0,2,4,5")]
    [HttpGet("panel/manual-reservation-two", Name = "getManualLocationValue")]
    public async Task<IActionResult> ManualReservationStepTwo(GetResevationIM reservation)
    {
        try
        {
            var pickLocationValues = await apiResultDal.LocationValues(reservation.PickValue);
            var dropLocationValues = await apiResultDal.LocationValues(reservation.DropValue);

            var betweenLocation = await apiResultDal.DistanceMatrixValues(pickLocationValues.Result.Geometry.Location.lat, pickLocationValues.Result.Geometry.Location.lng, dropLocationValues.Result.Geometry.Location.lat, dropLocationValues.Result.Geometry.Location.lng);

            if (betweenLocation.status == "OK")
            {
                var allDatas = await reservationTDal.GetLocationAllDataList(pickLocationValues, dropLocationValues);

                var getreservation = new List<GetReservationValues>();
                var selectedLocationsMini = new List<LocationIsOutMiniVM>();

                var selectedLocations = await reservationTDal.GetLocationIsOutList(allDatas);

                double minKm = 0;

                if (betweenLocation.rows[0].elements[0].status == "OK")
                {
                    var lastKm = Math.Round(Convert.ToDouble(betweenLocation.rows[0].elements[0].distance.value) / 100) * 100;
                    minKm = lastKm / 1000;
                    var lastLocations = await reservationTDal.ReservationList(selectedLocations, reservation, minKm, pickLocationValues, dropLocationValues);

                    getreservation = lastLocations.Locations;
                    selectedLocationsMini = lastLocations.MiniLocations;
                }

                var lastVM = new ReservationStepTwoVM()
                {
                    ReservationValues = getreservation,
                    DropLocationLatLng = $"lat:{dropLocationValues.Result.Geometry.Location.lat},lng:{dropLocationValues.Result.Geometry.Location.lng}",
                    PickLocationLatLng = $"lat:{pickLocationValues.Result.Geometry.Location.lat},lng:{pickLocationValues.Result.Geometry.Location.lng}",
                    DropLocationPlaceId = reservation.DropValue,
                    PickLocationPlaceId = reservation.PickValue,
                    Distance = betweenLocation.rows[0].elements[0].distance?.text,
                    Duration = betweenLocation.rows[0].elements[0].duration?.text,
                    SelectedReservationValues = reservation
                };

                var reservationDatas = new ReservationDatasVM()
                {
                    DropLocationLatLng = $"lat:{dropLocationValues.Result.Geometry.Location.lat},lng:{dropLocationValues.Result.Geometry.Location.lng}",
                    PickLocationLatLng = $"lat:{pickLocationValues.Result.Geometry.Location.lat},lng:{pickLocationValues.Result.Geometry.Location.lng}",
                    DropLocationPlaceId = reservation.DropValue,
                    PickLocationPlaceId = reservation.PickValue,
                    PickLocationName = pickLocationValues.Result.formatted_address,
                    DropLocationName = dropLocationValues.Result.formatted_address,
                    KM = minKm,
                    ReservationValues = reservation,
                    Distance = betweenLocation.rows[0].elements[0].distance?.text,
                    Duration = betweenLocation.rows[0].elements[0].duration?.text
                };



                HttpContext.Session.Remove("reservationData");
                HttpContext.Session.MySet("reservationData", reservationDatas);
                HttpContext.Session.MySet("selectedLocationMini", selectedLocationsMini);


                lastVM.ReservationValues = lastVM.ReservationValues.OrderBy(a => a.LastPrice).ToList();

                return View(lastVM);
            }


            return RedirectToAction("Index", "Home");
            //return View(lastVM);
        }
        catch (Exception)
        {
            ViewBag.Error = "Error";
            return RedirectToAction("Index", "Home");
        }

    }

    [Authorize(Roles = "0,2,4,5")]
    [HttpGet("panel/manual-reservation-three/{id}")]
    public IActionResult ManualReservationStepThree(int id)
    {
        try
        {
            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var user = new UserDatas();

            if (userId != 0)
            {
                var loginAuth = loginAuthDal.SelectByID(userId);
                user = userDatasDal.SelectByID(loginAuth?.UserId);
                user.LoginAuth = loginAuth;
            }
            else
            {
                user = null;
            }

            var datas = HttpContext.Session.MyGet<ReservationDatasVM>("reservationData");

            if (datas == null)
            {
                return NotFound();
            }


            var selectedDatasMini = HttpContext.Session.MyGet<List<LocationIsOutMiniVM>>("selectedLocationMini").Where(a => a.LocationCarId == id).FirstOrDefault();

            if (selectedDatasMini != null)
            {
                var locationCar = locationCarDal.SelectByID(selectedDatasMini.LocationCarId);


                var prices = tReservationHelpersDal.ReservationPrice(locationCar.Id, datas.KM, false, 0, datas.ReservationValues.ReturnStatus, selectedDatasMini.IsOutZone);

                datas.OfferPrice = prices.OfferPrice;
                datas.ServiceFee = prices.ServiceFee;
                datas.TotalPrice = prices.LastPrice;
                datas.ExtraServiceFee = prices.ExtraServiceFee;
                datas.SalesFee = prices.SalesFee;
                datas.PartnerFee = prices.PartnerFee;
                datas.GlobalPartnerFee = prices.GlobalPartnerFee;
                datas.IsOutZone = selectedDatasMini.IsOutZone;

                datas.LocationCar = locationCar;
                datas.LocationCar.Car = carDetailDal.CarDetail(locationCar.CarId);
                datas.LocationCar.Car.Service = servicesDal.SelectByID(datas.LocationCar.Car.ServiceId);
                var selectedCarItems = new List<PriceServiceList>();
                if (datas.LocationCar.Car.Service != null)
                {
                    datas.LocationCar.Car.Service.ServiceItems = serviceItemsDal.SelectByFunc(a => a.ServiceId == datas.LocationCar.Car.ServiceId);

                    var listServices = new List<ServiceCategories>();

                    var c = new List<PriceService>();

                    datas.LocationCar.Car.Service.ServiceItems.ForEach(a =>
                    {
                        a.ServiceProperty = servicePropertiesDal.SelectByID(a.ServicePropertyId);
                        a.ServiceProperty.ServiceCategory = serviceCategoriesDal.SelectByID(a.ServiceProperty.ServiceCategoryId);
                        c.Add(new PriceService
                        {
                            Category = a.ServiceProperty.ServiceCategory,
                            CategoryProp = a.ServiceProperty
                        });
                    });

                    selectedCarItems = c.GroupBy(c => c.Category)
                                .Select(g => new PriceServiceList
                                {
                                    Category = g.Key,
                                    CategoryProps = g.Select(c => c.CategoryProp).ToList(),
                                }).ToList();
                }

                HttpContext.Session.Remove("reservationData");

                HttpContext.Session.MySet("reservationData", datas);

                var reservation = new ReservationStepThreeVM()
                {
                    SelectedData = datas,
                    User = user,
                    ServiceItems = selectedCarItems
                };

                return View(reservation);
            }

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            ViewBag.Error = "Error";
            return BadRequest(ex.ToString());
        }

    }

    [Authorize(Roles = "0,2,4,5")]
    [HttpPost("panel/manual-reservation-three")]
    public IActionResult ManualReservationLastStep(ReservationLastStepVM reservation)
    {
        try
        {
            var userRole = User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role)?.Value;

            var createReservation = HttpContext.Session.MyGet<ReservationDatasVM>("reservationData");

            if (createReservation == null)
            {
                return NotFound();
            }

            var totalServiceFee = reservation.ServiceList?.Sum(item1 => serviceItemsDal.SelectByID(item1.SelectedValue).Price * item1.PeopleCountInput) ?? 0.0;

            Random random = new Random();
            int kodUzunlugu = 6;
            string karakterler = "0123456789";
            string kod = string.Concat(Enumerable.Range(0, kodUzunlugu).Select(_ => karakterler[random.Next(karakterler.Length)]));

            createReservation.LocationCar.Location = locationDal.SelectByID(createReservation.LocationCar.LocationId);

            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Sid)?.Value);
            var user = userId != 0 ? userDatasDal.SelectByID(loginAuthDal.SelectByID(userId)?.UserId) : null;

            var totalprice = createReservation.TotalPrice;

            if (userRole == "0" || userRole == "4")
            {
                totalprice = reservation.IsDiscount ? Convert.ToDouble(reservation.Discount) : totalprice;
            }


            var salesAgency = userRole == "5" ? userId : 0;

            totalprice = Math.Round(totalprice, 2);
            var item = reservationsDal.Insert(new Reservations
            {
                DropLatLng = createReservation.DropLocationLatLng,
                PickLatLng = createReservation.PickLocationLatLng,
                Phone = reservation.PassengerPhone,
                DropPlaceId = createReservation.DropLocationPlaceId,
                PickPlaceId = createReservation.PickLocationPlaceId,
                Email = reservation.PassengerEmail,
                LocationCarId = createReservation.LocationCar.Id,
                Name = reservation.PassengerName,
                ReservationCode = kod,
                Surname = reservation.PassengerSurname,
                DropFullName = createReservation.DropLocationName,
                PickFullName = createReservation.PickLocationName,
                PeopleCount = createReservation.ReservationValues.PeopleCount,
                ReservationDate = createReservation.ReservationValues.FlightTime,
                ReturnDate = createReservation.ReservationValues.ReturnDate,
                ReturnStatus = createReservation.ReservationValues.ReturnStatus,
                DistanceText = createReservation.Distance,
                DurationText = createReservation.Duration,
                IsDiscount = reservation.IsDiscount,
                Discount = reservation.Discount,
                UserId = locationDal.SelectByID(createReservation.LocationCar.LocationId).UserId,
                ExtraServiceFee = totalServiceFee,
                Comment = reservation.PassengerComment,
                Status = 1,
                IsDelete = false,
                HidePrice = reservation.HidePrice,
                RealPhone = reservation.PassengerRealPhone,
                DiscountText = reservation.DiscountText,
                ReservationUserId = user?.Id,
                Rate = 0,
                SalesAgencyId = salesAgency,
                LastUpdate = DateTime.Now,
                CreateDate = DateTime.Now,
                IsThisReturn = false,

                PartnerFee = createReservation.PartnerFee,
                SalesFee = createReservation.SalesFee,
                ServiceFee = createReservation.ServiceFee,
                OfferPrice = createReservation.OfferPrice,
                TotalPrice = totalprice,
                GlobalPartnerFee = createReservation.GlobalPartnerFee
            });


            if (item.ReturnStatus)
            {
                reservationsDal.Insert(new Reservations
                {
                    DropLatLng = createReservation.DropLocationLatLng,
                    PickLatLng = createReservation.PickLocationLatLng,
                    Phone = reservation.PassengerPhone,
                    DropPlaceId = createReservation.DropLocationPlaceId,
                    PickPlaceId = createReservation.PickLocationPlaceId,
                    Email = reservation.PassengerEmail,
                    LocationCarId = createReservation.LocationCar.Id,
                    Name = reservation.PassengerName,
                    ReservationCode = kod,
                    Surname = reservation.PassengerSurname,
                    DropFullName = createReservation.DropLocationName,
                    PickFullName = createReservation.PickLocationName,
                    PeopleCount = createReservation.ReservationValues.PeopleCount,
                    ReservationDate = createReservation.ReservationValues.ReturnDate,
                    ReturnDate = createReservation.ReservationValues.ReturnDate,
                    ReturnStatus = false,
                    DistanceText = createReservation.Distance,
                    DurationText = createReservation.Duration,
                    IsDiscount = reservation.IsDiscount,
                    Discount = reservation.Discount,
                    UserId = locationDal.SelectByID(createReservation.LocationCar.LocationId).UserId,
                    ExtraServiceFee = totalServiceFee,
                    Comment = reservation.PassengerComment,
                    Status = 1,
                    IsDelete = false,
                    HidePrice = reservation.HidePrice,
                    RealPhone = reservation.PassengerRealPhone,
                    DiscountText = reservation.DiscountText,
                    ReservationUserId = user?.Id,
                    Rate = 0,
                    SalesAgencyId = salesAgency,
                    LastUpdate = DateTime.Now,
                    CreateDate = DateTime.Now,
                    IsThisReturn = true,

                    PartnerFee = createReservation.PartnerFee,
                    SalesFee = createReservation.SalesFee,
                    ServiceFee = createReservation.ServiceFee,
                    OfferPrice = createReservation.OfferPrice,
                    TotalPrice = totalprice,
                    GlobalPartnerFee = createReservation.GlobalPartnerFee
                });
            }

            var reservationItemsList = reservation.ServiceList?.Select(item1 => new ReservationServicesTable
            {
                ReservationId = item.Id,
                PeopleCount = item1.PeopleCountInput,
                Price = serviceItemsDal.SelectByID(item1.SelectedValue).Price,
                ServiceItemId = item1.SelectedValue
            }).ToList();

            reservationItemsList = reservationItemsList?.Where(a => a.PeopleCount > 0).ToList();
            if (reservationItemsList != null && reservationItemsList.Count > 0)
            {
                reservationServicesTableDal.InsertRage(reservationItemsList);
            }

            item.LocationCars = locationCarDal.SelectByID(item.LocationCarId);
            item.LocationCars.Car = getCarDal.CarDetail(item.LocationCars.CarId);

            var loginAuth2 = loginAuthDal.SelectByID(item.UserId);
            item.User = userDatasDal.SelectByID(loginAuth2.UserId);

            var reservationPeople = reservation.PassengerList?.Select(a => new ReservationPeople
            {
                Name = a.PassengerName,
                Surname = a.PassengerSurname,
                ReservationId = item.Id
            }).ToList();

            if (reservationPeople != null)
            {
                reservationsPeopleDal.InsertRage(reservationPeople);
            }

            item.ReservationServicesTables = reservationServicesTableDal.SelectByFunc(a => a.ReservationId == item.Id);
            item.ReservationServicesTables.ForEach(a =>
            {
                a.ServiceItem = serviceItemsDal.SelectByID(a.ServiceItemId);
                a.ServiceItem.ServiceProperty = servicePropertiesDal.SelectByID(a.ServiceItem.ServicePropertyId);
            });

            PdfCreator pdfCreator = new PdfCreator(envDal);
            //pdfCreator.CreateReservationPDF($"{kod}-{item.Id}", item);

        
            mailDal.SendReservationMail(item);

            var allMessage = new List<Mesaj>
            {
                new Mesaj
                {
                    dest = reservation.PassengerRealPhone,
                    msg = @$"Your reservation code {item.ReservationCode} has been created. Voucher Link {configuration["PageLinks:PageGlobalLink"]}/pdf/{item.ReservationCode}-{item.Id}.pdf"
                }
            };

            smsDal.SmsForReservation(allMessage.ToArray());

            return Json(new { result = 1, id = item.Id });
        }
        catch (Exception)
        {
            return RedirectToAction("Index", "Home");
        }
    }

    [Authorize(Roles = "0,2,4,5")]
    [HttpGet("panel/reservation-success")]
    public IActionResult ManualCreatedReservationDetail(int id)
    {
        var reservation = reservationsDal.SelectByID(id);
        if (reservation != null)
        {
            reservation = tReservationHelpersDal.GetReservationAll(id);

            return View(reservation);
        }

        return NotFound();
    }

    public JsonResult GetServiceItem(int[] serviceProId, int serviceId)
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
                    serviceProp.ServiceCategory = serviceCategoriesDal.SelectByID(serviceProp?.ServiceCategoryId);
                    serviceVM2.Add(new GetServiceCategoryItemVM()
                    {
                        Id = serviceProp.Id,
                        ServiceDescripton = serviceProp.ServicePropertyDescription,
                        ServiceCategoryName = serviceProp.ServiceCategory.ServiceCategoryName,
                        ServiceName = serviceProp.ServicePropertyName,
                        ServiceCategoryId = serviceProp.ServiceCategoryId,
                        Price = serviceItemsDal.SelectByFunc(a => a.ServiceId == serviceId && a.ServicePropertyId == serviceProp.Id).FirstOrDefault()?.Price
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

    public JsonResult CheckCoupon(string coupon, List<SelectServiceVM> selectedServices)
    {
        try
        {

            var createReservation = HttpContext.Session.MyGet<ReservationDatasVM>("reservationData");
            if (createReservation != null)
            {
                var totalServiceFee = 0.0;
                if (selectedServices != null)
                {
                    foreach (var item1 in selectedServices)
                    {
                        var serviceFee = serviceItemsDal.SelectByID(item1.SelectedValue);
                        totalServiceFee += serviceFee.Price * item1.PeopleCountInput;
                    }
                }

                var prices = tReservationHelpersDal.ReservationPrice(createReservation.LocationCar.Id, createReservation.KM, false, totalServiceFee, createReservation.ReservationValues.ReturnStatus, createReservation.IsOutZone, coupon);

                var coupons = couponsDal.SelectByFunc(a => a.Active && a.CouponCode == coupon && a.CouponStartDate <= DateTime.Now
                                                                                        && a.CouponFinishDate >= DateTime.Now && a.IsPerma).FirstOrDefault();

                var JsonData = new JsonResult(new { price = prices.LastPrice, oldPrice = Math.Round(prices.OfferPrice + prices.ServiceFee + prices.SalesFee + prices.ExtraServiceFee, 2), discount = coupons?.Discount });
                return new JsonResult(new { result = coupons == null ? 2 : 1, data = JsonData, discount = coupons?.Discount });
            }


            return new JsonResult(new { result = 2 });
        }
        catch (System.Exception)
        {
            return new JsonResult(new { });
        }
    }

}
