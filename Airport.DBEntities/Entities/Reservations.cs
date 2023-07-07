using Airport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Entities
{
    public class Reservations : IEntity
    {
        public int Id { get; set; }
        public string DropLatLng { get; set; }
        public string PickLatLng { get; set; }
        public string Name { get; set; }
        public string? Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string DropPlaceId { get; set; }
        public string PickPlaceId { get; set; }
        public int LocationCarId { get; set; }
        public string ReservationCode { get; set; }
        public string DropFullName { get; set; }
        public string PickFullName { get; set; }
        public int PeopleCount { get; set; }
        public DateTime ReservationDate { get; set; }
        public bool ReturnStatus { get; set; }
        public DateTime ReturnDate { get; set; }
        public string DistanceText { get; set; }
        public string DurationText { get; set; }
        public bool IsDiscount { get; set; }
        public int UserId { get; set; }
        public string Comment { get; set; }
        public byte Status { get; set; }
        public string? FinishComment { get; set; }
        public int? DriverId { get; set; }
        public bool IsDelete { get; set; }
        public bool HidePrice { get; set; }
        public int? Coupon { get; set; }
        public string RealPhone { get; set; }
        public string? DiscountText { get; set; }
        public int? ReservationUserId { get; set; }
        public int Rate { get; set; }
        public int? SalesAgencyId { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime CreateDate { get; set; }
        public double OfferPrice { get; set; }
        public bool? IsManuelDriver { get; set; }
        public string? DriverName { get; set; }
        public string? DriverSurname { get; set; }
        public string? DriverPhone { get; set; }


        public double TotalPrice { get; set; }
        public double? ServiceFee { get; set; }
        public double? SalesFee { get; set; }
        public double? PartnerFee { get; set; }
        public double? ExtraServiceFee { get; set; }
        public double? DriverFee { get; set; }
        public double? DiscountRate { get; set; }
        public double? GlobalPartnerFee { get; set; }
        public double? Discount { get; set; }



        public LocationCars LocationCars { get; set; }
        public UserDatas User { get; set; }
        public List<ReservationPeople> ReservationPeoples { get; set; }
        public Drivers Driver { get; set; }
        public List<ReservationServicesTable> ReservationServicesTables { get; set; }
        public Coupons Coupons { get; set; }
    }
}
