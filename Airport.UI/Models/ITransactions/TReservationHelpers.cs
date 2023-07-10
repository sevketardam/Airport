using Airport.DBEntities.Entities;
using Airport.UI.Models.Interface;
using Airport.UI.Models.VM;
using Microsoft.DotNet.MSIdentity.Shared;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using System;
using Airport.UI.Models.IM;
using System.Linq;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.MessageExtension.Interfaces;
using Airport.MessageExtensions.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using static Org.BouncyCastle.Utilities.Test.FixedSecureRandom;

namespace Airport.UI.Models.ITransactions
{
    public class TReservationHelpers : ITReservationHelpers
    {
        ILocationCarsDAL _locationCar;
        IUserDatasDAL _userDatas;
        IReservationsDAL _reservations;
        IGetCarDetail _getCar;
        IServiceItemsDAL _serviceItems;
        IServicePropertiesDAL _serviceProperties;
        IReservationServicesTableDAL _reservationServicesTable;
        ILoginAuthDAL _loginAuth;
        IGlobalSettings _globalSettings;
        ILocationsDAL _locations;
        ILocationCarsFareDAL _locationCarsFare;
        ICouponsDAL _coupons;
        public TReservationHelpers(ILocationCarsDAL locationCar, IUserDatasDAL userDatas, IReservationsDAL reservations, IGetCarDetail getCar, IServiceItemsDAL serviceItems, IServicePropertiesDAL serviceProperties, IReservationServicesTableDAL reservationServicesTable, ILoginAuthDAL loginAuth, IGlobalSettings globalSettings, ILocationsDAL locations, ILocationCarsFareDAL locationCarsFare, ICouponsDAL coupons)
        {
            _locationCar = locationCar;
            _userDatas = userDatas;
            _reservations = reservations;
            _getCar = getCar;
            _serviceItems = serviceItems;
            _serviceProperties = serviceProperties;
            _reservationServicesTable = reservationServicesTable;
            _loginAuth = loginAuth;
            _globalSettings = globalSettings;
            _locations = locations;
            _locationCarsFare = locationCarsFare;
            _coupons = coupons;
        }



        public ReservationPriceVM ReservationPrice(int locationCarId, double locationKM, bool isSales, double extraServiceFee, bool returnStatus, bool isOutZone, string couponCode, double? specialPrice)
        {
            var locationCar = _locationCar.SelectByID(locationCarId);
            locationCar.Location = _locations.SelectByID(locationCar.LocationId);

            var serviceFee = _globalSettings.GetSettings().GlobalPer;
            var salesFee = _globalSettings.GetSettings().SalesPer;
            var partnerFee = _globalSettings.GetSettings().PartnerPer;

            var priceVM = new ReservationPriceVM();

            double price = 0;
            var lastUp = 0;
            double lastPrice = 0;

            if (isOutZone)
            {
                price = locationCar.Location.DropCharge + (locationKM * locationCar.Location.OutZonePricePerKM);
            }
            else
            {
                locationCar.LocationCarsFares = _locationCarsFare.SelectByFunc(a => a.LocationCarId == locationCarId);

                locationCar.LocationCarsFares.ForEach(c =>
                {
                    var fare = Convert.ToDouble(c.Fare);

                    if (c.StartFrom < locationKM)
                    {
                        if (c.PriceType == 2)
                        {
                            if (c.UpTo < locationKM)
                            {
                                price += fare * (c.UpTo - c.StartFrom);
                            }
                            else
                            {
                                price += fare * (locationKM - c.StartFrom);
                            }
                        }
                        else
                        {
                            price += fare;
                        }
                    }

                    lastUp = c.UpTo;
                    lastPrice = fare;
                });

                if (lastUp < locationKM)
                {
                    var plusPrice = locationKM - lastUp;
                    price += lastPrice * plusPrice;
                }
            }

            if (returnStatus)
            {
                price *= 2;
            }

            //if (couponCode != null && couponCode != "")
            //{
            //    var getCoupon = _coupons.SelectByFunc(a => a.Active && a.CouponCode == couponCode && a.CouponStartDate <= DateTime.Now && a.CouponFinishDate >= DateTime.Now && (a.IsPerma || (!a.IsPerma && a.CouponLimit > a.UsingCount))).FirstOrDefault();

            //    if (getCoupon != null)
            //    { 
            //        price = priceVM.LastPrice - (priceVM.LastPrice * getCoupon.Discount / 100);
            //    }
            //}


            priceVM.OfferPrice = Math.Round(price, 2);
            priceVM.PartnerFee = Math.Round(priceVM.OfferPrice - (priceVM.OfferPrice * partnerFee) / 100, 2);
            priceVM.GlobalPartnerFee = Math.Round(priceVM.OfferPrice * partnerFee / 100, 2);
            priceVM.ServiceFee = Math.Round(price * serviceFee / 100, 2);
            priceVM.SalesFee = Math.Round(isSales ? price * salesFee / 100 : 0, 2);
            priceVM.ExtraServiceFee = Math.Round(extraServiceFee, 2);

            priceVM.LastPrice = Math.Round(priceVM.OfferPrice + priceVM.ServiceFee + priceVM.SalesFee + extraServiceFee, 2);


            if (couponCode != null && couponCode != "")
            {
                var getCoupon = _coupons.SelectByFunc(a => a.Active && a.CouponCode == couponCode && a.CouponStartDate <= DateTime.Now && a.CouponFinishDate >= DateTime.Now && (a.IsPerma || (!a.IsPerma && a.CouponLimit > a.UsingCount))).FirstOrDefault();

                if (getCoupon != null)
                {
                    priceVM.DiscountRate = getCoupon.Discount;

                    priceVM.LastPrice = Math.Round(priceVM.LastPrice - (priceVM.LastPrice * getCoupon.Discount / 100), 2);

                    priceVM.DiscountPrice = Math.Round(priceVM.LastPrice * getCoupon.Discount / 100, 2);


                    priceVM.PartnerFee = Math.Round(priceVM.LastPrice - (priceVM.LastPrice * partnerFee) / 100, 2);
                    priceVM.GlobalPartnerFee = Math.Round(priceVM.GlobalPartnerFee - (priceVM.GlobalPartnerFee * getCoupon.Discount / 100), 2);
                    priceVM.SalesFee = Math.Round(priceVM.SalesFee - (priceVM.SalesFee * getCoupon.Discount / 100), 2);
                    priceVM.DiscountExtraService = Math.Round(priceVM.ExtraServiceFee - (priceVM.ExtraServiceFee * getCoupon.Discount / 100), 2);
                    priceVM.DiscountServiceFee = Math.Round(priceVM.ServiceFee - (priceVM.ServiceFee * getCoupon.Discount / 100), 2);
                    priceVM.DiscountOfferPrice = Math.Round(price - (price * getCoupon.Discount / 100), 2);

                    priceVM.LastPrice = Math.Round(priceVM.DiscountOfferPrice + priceVM.DiscountServiceFee + priceVM.SalesFee + priceVM.DiscountExtraService, 2);
                }
            }

            priceVM.LastPrice = specialPrice != null ? Convert.ToDouble(specialPrice) : priceVM.LastPrice;

            return priceVM;
        }

        public GetReservationValues CreateGetReservationValues(LocationCars b, double price, GetResevationIM reservation, GetGoogleAPIVM pickLocationValues, GetGoogleAPIVM dropLocationValues, List<Reservations> rate)
        {
            return new GetReservationValues
            {
                LocationCars = b,
                LastPrice = price,
                ReservationDate = reservation.FlightTime,
                PickLocationName = pickLocationValues.Result.formatted_address,
                DropLocationName = dropLocationValues.Result.formatted_address,
                PassangerCount = reservation.PeopleCount,
                PickLocationLatLng = $"{pickLocationValues.Result.Geometry.Location.lat},{pickLocationValues.Result.Geometry.Location.lng}",
                DropLocationLatLng = $"{dropLocationValues.Result.Geometry.Location.lat},{dropLocationValues.Result.Geometry.Location.lng}",
                DropLocationPlaceId = reservation.DropValue,
                PickLocationPlaceId = reservation.PickValue,
                Rate = rate.Count > 0 ? Math.Round(rate.Average(c => c.Rate), 2).ToString() : "0",
                RateCount = rate.Count > 0 ? rate.Count.ToString() : "0",
            };
        }

        public Reservations GetReservationAll(int id)
        {
            var reservation = _reservations.SelectByID(id);
            if (reservation != null)
            {
                reservation.LocationCars = _locationCar.SelectByID(reservation.LocationCarId);
                reservation.LocationCars.Car = _getCar.CarDetail(reservation.LocationCars.CarId);
                reservation.LocationCars.Location = _locations.SelectByID(reservation.LocationCars.LocationId);

                var loginAuth = _loginAuth.SelectByID(reservation.LocationCars.Location.UserId);

                reservation.LocationCars.Location.User = _userDatas.SelectByID(loginAuth.UserId);

                reservation.ReservationServicesTables = _reservationServicesTable.SelectByFunc(a => a.ReservationId == reservation.Id);

                var loginAuth2 = _loginAuth.SelectByID(reservation.UserId);
                reservation.User = _userDatas.SelectByID(loginAuth2.UserId);

                reservation.ReservationServicesTables.ForEach(a =>
                {
                    a.ServiceItem = _serviceItems.SelectByID(a.ServiceItemId);
                    a.ServiceItem.ServiceProperty = _serviceProperties.SelectByID(a.ServiceItem.ServicePropertyId);
                });
            }

            return reservation;
        }
    }
}
