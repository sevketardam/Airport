using Airport.DBEntities.Entities;
using Airport.UI.Models.Interface;
using Airport.UI.Models.VM;
using System.Collections.Generic;
using System;
using Airport.UI.Models.IM;
using System.Linq;
using Airport.DBEntitiesDAL.Interfaces;

namespace Airport.UI.Models.ITransactions;

public class TReservationHelpers(ILocationCarsDAL locationCarDal, IUserDatasDAL userDatasDal, IReservationsDAL reservationsDal, IGetCarDetail getCarDal, IServiceItemsDAL serviceItemsDal, IServicePropertiesDAL servicePropertiesDal, IReservationServicesTableDAL reservationServicesTableDal, ILoginAuthDAL loginAuthDal, IGlobalSettings globalSettingsDal, ILocationsDAL locationsDal, ILocationCarsFareDAL locationCarsFareDal, ICouponsDAL couponsDal) : ITReservationHelpers
{

    public ReservationPriceVM ReservationPrice(int locationCarId, double locationKM, bool isSales, double extraServiceFee, bool returnStatus, bool isOutZone, string couponCode, double? specialPrice)
    {
        var locationCar = locationCarDal.SelectByID(locationCarId);
        locationCar.Location = locationsDal.SelectByID(locationCar.LocationId);

        var serviceFee = globalSettingsDal.GetSettings().GlobalPer;
        var salesFee = globalSettingsDal.GetSettings().SalesPer;
        var partnerFee = globalSettingsDal.GetSettings().PartnerPer;

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
            locationCar.LocationCarsFares = locationCarsFareDal.SelectByFunc(a => a.LocationCarId == locationCarId);

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
            var getCoupon = couponsDal.SelectByFunc(a => a.Active && a.CouponCode == couponCode && a.CouponStartDate <= DateTime.Now && a.CouponFinishDate >= DateTime.Now && (a.IsPerma || (!a.IsPerma && a.CouponLimit > a.UsingCount))).FirstOrDefault();

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
        var reservation = reservationsDal.SelectByID(id);
        if (reservation != null)
        {
            reservation.LocationCars = locationCarDal.SelectByID(reservation.LocationCarId);
            reservation.LocationCars.Car = getCarDal.CarDetail(reservation.LocationCars?.CarId);
            reservation.LocationCars.Location = locationsDal.SelectByID(reservation.LocationCars?.LocationId);

            var loginAuth = loginAuthDal.SelectByID(reservation.LocationCars.Location.UserId);

            reservation.LocationCars.Location.User = userDatasDal.SelectByID(loginAuth.UserId);

            reservation.ReservationServicesTables = reservationServicesTableDal.SelectByFunc(a => a.ReservationId == reservation.Id);

            var loginAuth2 = loginAuthDal.SelectByID(reservation.UserId);
            reservation.User = userDatasDal.SelectByID(loginAuth2.UserId);

            if (reservation.ReservationServicesTables.Any())
            {
                reservation.ReservationServicesTables.ForEach(a =>
                {
                    a.ServiceItem = serviceItemsDal.SelectByID(a.ServiceItemId);
                    a.ServiceItem.ServiceProperty = servicePropertiesDal.SelectByID(a.ServiceItem?.ServicePropertyId);
                });
            }

        }

        return reservation;
    }
}
