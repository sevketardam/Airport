using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.MessageExtension.Interfaces;
using Airport.MessageExtensions.Interfaces;
using Airport.UI.Models.IM;
using Airport.UI.Models.Interface;
using Airport.UI.Models.VM;
using iText.Layout.Element;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airport.UI.Models.ITransactions
{
    public class TReservations : ITReservations
    {
        IReservationsDAL _reservations;
        IGetCarDetail _carDetail;
        ILocationCarsDAL _locationCars;
        ILocationCarsFareDAL _locationCarsFare;
        ITReservationHelpers _reservationTHelpers;
        ILocationsDAL _location;
        IApiResult _apiResult;
        IGlobalSettings _globalSettings;

        public TReservations(IReservationsDAL reservations, IGetCarDetail carDetail, ILocationCarsDAL locationCars, ILocationCarsFareDAL locationCarsFare, ITReservationHelpers reservationTHelpers, ILocationsDAL location, IApiResult apiResult, IGlobalSettings globalSettings)
        {
            _reservations = reservations;
            _carDetail = carDetail;
            _locationCars = locationCars;
            _locationCarsFare = locationCarsFare;
            _reservationTHelpers = reservationTHelpers;
            _location = location;
            _apiResult = apiResult;
            _globalSettings = globalSettings;
        }

        public async Task<List<LocationIsOutVM>> GetLocationIsOutList(List<AllDatas> allData)
        {
            var selectedLocations = new List<LocationIsOutVM>();
            allData = allData.Distinct().ToList();
            allData.ForEach(a =>
            {
                var convertLocation = _location.SelectByFunc(b => b.Lat == a.Lat && b.Lng == a.Lng && !b.IsDelete);
                convertLocation.ForEach(b =>
                {
                    var realRadiusValue = Convert.ToInt32(b.LocationRadius) * 1000;
                    if (realRadiusValue > Convert.ToInt32(a.DisanceValue))
                    {
                        if (realRadiusValue > Convert.ToInt32(a.DropDisanceValue))
                        {
                            selectedLocations.Add(new LocationIsOutVM
                            {
                                IsOutZone = false,
                                Location = b,
                                IsOutZoneOutside = false
                            });
                        }
                        else
                        {
                            selectedLocations.Add(new LocationIsOutVM
                            {
                                IsOutZone = false,
                                Location = b,
                                IsOutZoneOutside = true
                            });
                        }
                    }
                    else if (b.OutZonePricePerKM > 0 && realRadiusValue < Convert.ToInt32(a.DisanceValue))
                    {
                        if (realRadiusValue > Convert.ToInt32(a.DropDisanceValue))
                        {
                            selectedLocations.Add(new LocationIsOutVM
                            {
                                IsOutZone = true,
                                Location = b,
                                IsOutZoneOutside = false
                            });
                        }
                        else
                        {
                            selectedLocations.Add(new LocationIsOutVM
                            {
                                IsOutZone = true,
                                Location = b,
                                IsOutZoneOutside = true
                            });
                        }

                    }
                });
            });
            return selectedLocations.Distinct().ToList();
        }

        public async Task<GetReservationListVM> ReservationList(List<LocationIsOutVM> locationIsOutVM, GetResevationIM reservation, double minKm, GetGoogleAPIVM pickLocationValues, GetGoogleAPIVM dropLocationValues)
        {

            var getreservation = new List<GetReservationValues>();
            var selectedLocationsMini = new List<LocationIsOutMiniVM>();

            foreach (var a in locationIsOutVM)
            {
                a.Location.LocationCars = _locationCars.SelectByFunc(b => b.LocationId == a.Location.Id);

                foreach (var b in a.Location.LocationCars)
                {
                    selectedLocationsMini.Add(new LocationIsOutMiniVM
                    {
                        LocationCarId = b.Id,
                        IsOutZone = a.IsOutZone,
                        IsOutZoneOutside = a.IsOutZoneOutside,
                    });


                    b.Car = _carDetail.CarDetail(b.CarId);
                    if (b.Car.MaxPassenger >= reservation.PeopleCount)
                    {
                        var prices = _reservationTHelpers.ReservationPrice(b.Id, minKm, false, 0, reservation.ReturnStatus, a.IsOutZone);

                        var checkCar = getreservation.Where(c => c.LocationCars.CarId == b.CarId).FirstOrDefault();

                        var rate = _reservations.SelectByFunc(c => c.LocationCarId == b.Id && c.Rate > 0);

                        if (checkCar != null)
                        {
                            if (checkCar.LastPrice < prices.LastPrice)
                            {
                                var checkCar1 = _reservationTHelpers.CreateGetReservationValues(b, prices.LastPrice, reservation, pickLocationValues, dropLocationValues, rate); 
                                getreservation[getreservation.IndexOf(checkCar)] = checkCar1;
                            }
                        }
                        else
                        {
                            if (prices.LastPrice > 0)
                            {
                                bool isValid = !a.IsOutZoneOutside || a.Location.IsOkeyOut;
                                if (isValid)
                                {
                                    getreservation.Add(_reservationTHelpers.CreateGetReservationValues(b, prices.LastPrice, reservation, pickLocationValues, dropLocationValues, rate));
                                }
                            }
                        }
                    }
                }
            }

            return new GetReservationListVM { Locations = getreservation.Distinct().ToList(), MiniLocations = selectedLocationsMini.Distinct().ToList() };
        }


        public async Task<List<AllDatas>> GetLocationAllDataList(GetGoogleAPIVM pickLocationValues, GetGoogleAPIVM dropLocationValues)
        {
            var locations = _location.SelectByFunc(a => !a.IsDelete);
            var listlocation = new List<ReservationLocationCarsVM>();
            var locationCars = new List<List<ReservationLocationCarsVM>>();

            foreach (var a in locations)
            {
                a.LocationCars = _locationCars.SelectByFunc(b => b.LocationId == a.Id);

                listlocation.Add(new ReservationLocationCarsVM
                {
                    LocationCar = a.LocationCars,
                    PlaceId = a.LocationMapId,
                    ZoneValue = a.LocationRadius,
                    Lat = a.Lat,
                    Lng = a.Lng
                });
            }

            if (listlocation.Count != 0)
            {
                locationCars.Add(listlocation);
            }

            var allDatas = new List<AllDatas>();
            foreach (var a in locationCars)
            {
                string carLatLngString = "";
                a.ForEach(b =>
                {
                    carLatLngString += b.Lat + "," + b.Lng + "|";
                });

                //alış yeri
                var pickData = await _apiResult.DistanceMatrixValues(pickLocationValues.Result.Geometry.Location.lat, pickLocationValues.Result.Geometry.Location.lng, carLatLngString);

                //gidiş yeri
                var dropData = await _apiResult.DistanceMatrixValues(dropLocationValues.Result.Geometry.Location.lat, dropLocationValues.Result.Geometry.Location.lng, carLatLngString);

                if (pickData.status == "OK" && dropData.status == "OK")
                {
                    int i2 = 0;
                    var locationdatas = carLatLngString.Split("|");
                    pickData.destination_addresses.ForEach(a =>
                    {
                        if (pickData.rows[0].elements[i2].status == "OK" && dropData.rows[0].elements[i2].status == "OK")
                        {
                            allDatas.Add(new AllDatas
                            {
                                DisanceValue = pickData.rows[0].elements[i2].distance.value.ToString(),
                                DurationeValue = pickData.rows[0].elements[i2].distance.value.ToString(),
                                Lat = locationdatas[i2].Split(",")[0],
                                Lng = locationdatas[i2].Split(",")[1],
                                Destinationaddresses = pickData.destination_addresses[i2],
                                DropDisanceValue = dropData.rows[0].elements[i2].distance.value.ToString(),
                                DropDurationeValue = dropData.rows[0].elements[i2].distance.value.ToString(),
                                DropLat = locationdatas[i2].Split(",")[0],
                                DropLng = locationdatas[i2].Split(",")[1],
                                DropDestinationaddresses = dropData.destination_addresses[i2]
                            });
                        }
                        i2++;
                    });
                }
            }

            return allDatas.Distinct().ToList();
        }
    }
}
