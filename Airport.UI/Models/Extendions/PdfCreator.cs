using Airport.DBEntities.Entities;
using Airport.UI.Models.VM;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Security.Policy;
using iText.Layout.Element;
using Microsoft.AspNetCore.Html;

namespace Airport.UI.Models.Extendions
{
    public class PdfCreator
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PdfCreator(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public void CreateReservationPDF(string fileName,Reservations reservation)
        {

            var httpClient = new HttpClient();
            var apiUrl = $"https://maps.googleapis.com/maps/api/directions/json?origin={reservation.PickLatLng.Replace("lat:", "").Replace("lng:", "")}&destination={reservation.DropLatLng.Replace("lat:","").Replace("lng:","")}&mode=driving&key=AIzaSyAnqSEVlrvgHJymL-F8GmxIwNbe8fYUjdg";
            var response = httpClient.GetAsync(apiUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var contentJsonResult = JsonConvert.DeserializeObject<JObject>(content);

            var point = contentJsonResult["routes"][0]["overview_polyline"]["points"].ToString();

            var convertLocationValue = DecodePolylinePoints(point);

            var str = "https://maps.googleapis.com/maps/api/staticmap?markers=color%3Ablue%7Clabel%3AB%7C41.1307027%2C28.9877942&markers=color%3Ablue%7Clabel%3AB%7C41.081402%2C28.9819771&size=303x156&key=AIzaSyAnqSEVlrvgHJymL-F8GmxIwNbe8fYUjdg&path=color%3A0xff0000ff%7Cweight%3A5%7C";

            convertLocationValue.ForEach(a =>
            {
                str += a.Latitude.ToString().Replace(",", ".") + "%2C" + a.Longitude.ToString().Replace(",", ".") + "%7C";
            });

            str = str.Substring(0, str.Length - 3);

            var imageResponse = httpClient.GetAsync(str).Result;
            
            byte[] imageBytes = imageResponse.Content.ReadAsByteArrayAsync().Result;
            string base64String = Convert.ToBase64String(imageBytes);


            var price = reservation.IsDiscount ? reservation.Discount : reservation.OfferPrice;
            var servicefee = reservation.IsDiscount ? 0 : reservation.ServiceFee;
            var totalprice = price + servicefee;

            var htmlContent = @$"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Document</title>

</head>

<body style=""margin: 0px; padding: 30px;"">
    <div style=""width: 100%;
                    gap: 22px;
                    background-color: #FF6900; justify-content: center;
                    text-align: center;
                    align-items: center; 
                    padding: 20px 0px;"">
        <h3 style=""font-size: 25px; font-weight: bold; margin: 0px;"">airportglobaltransfer.com</h3>
        <p style=""font-size: 13px; font-weight: 600;"">BOOKING CONFIRMATION</p>
    </div>


    <div style=""padding: 5px 5px;"">
        <div style=""margin-bottom:20px"">
            <h4 style=""font-weight: bold; margin: 5px 0px;"">CLIENT INFORMATION</h4>
            <p style=""margin: 5px 0px;"">{reservation.User.CompanyName}</p>
        </div>

        <div style=""margin-bottom:20px"">
            <h4 style=""font-weight: bold; margin: 5px 0px;"">CLIENT INFORMATION</h4>
            <p style=""margin: 0px;"">GLOBAL TRANSFER</p>
            <p style=""margin:0"">Phone number:</p>{reservation.User.CompanyPhoneNumber}

        </div>
        <div>
            <h4 style=""font-weight: bold; margin: 5px 0px;"">TRANSPORT INFORMATION</h4>
            <p style=""margin: 5px 0px;"">
                Small Bags x{reservation.LocationCars.Car.SmallBags}, Suit Case x{reservation.LocationCars.Car.SuitCase}, Passenger x{reservation.LocationCars.Car.MaxPassenger} : {reservation.LocationCars.Car.Brand.CarBrandName} {reservation.LocationCars.Car.Model.CarModelName}, License plate: {reservation.LocationCars.Car.Plate}

            </p>
            <p style=""margin: 5px 0px;"">
                Options:{(reservation.LocationCars.Car.Wifi ? "Free wifi" : "")}{(reservation.LocationCars.Car.Water ? ",Free water" : "")}{(reservation.LocationCars.Car.Partition ? ",Safety partition" : "")}{(reservation.LocationCars.Car.Charger ? ",Phone charger" : "")}
{(reservation.LocationCars.Car.Armored ? ",Armored voyage" : "")}{(reservation.LocationCars.Car.Disabled ? ",Handy for the disabled" : "")}
            </p>
        </div>
    </div>


    <div style=""padding: 5px 5px;"">

        <h3 style=""text-align: center; font-size: 23px;margin-bottom:20px;padding:0;"">BOOKING DETAILS</h3>
        <div style=""display: flex; justify-content: space-between;"">
            <div>

                <p style=""font-weight: bold; margin: 0;"">RIDE DATE</p>
                <div>
                    <p style=""margin: 0 0 15px 0px;"">{reservation.ReservationDate}</p>
                </div>

                <p style=""font-weight: bold; margin: 0px;"">FROM</p>
                <div style=""margin: 0 0 15px 0px;"">
                    <p style=""margin:0px"">{reservation.PickFullName}</p>
                </div>

                <p style=""font-weight: bold; margin: 0px;"">TO</p>
                <div style=""margin: 0 0 15px 0px;"">
                    <p style=""margin:0px"">{reservation.DropFullName}</p>
                </div>

                <div style=""margin: 0 0 15px 0px;"">
                    <span style=""font-weight: bold; margin: 0px;"">NAME SIGN:</span>
                    <span style=""margin:0px"">{reservation.Name} {reservation.Surname}</span>
                </div>

                <div style=""margin: 0 0 15px 0px;"">
                    <span style=""font-weight: bold; margin: 0px;"">PASSENGERS:</span>
                    <span style=""margin:0px"">{reservation.PeopleCount}</span>
                </div>

                <p style=""font-weight: bold; margin: 0px;"">COMMENT</p>
                <div style=""margin: 0 0 15px 0px;"">
                    <p style=""margin:0px"">{reservation.Comment}</p>
                </div>

            </div>

            <div style=""margin-left:20px;"">
                <img src=""data:image/png;base64,{base64String}"" alt=""Harita"">
            </div>
        </div>
    </div>

    <div style=""padding: 15px 15px; border-bottom: 4px solid #FF6900;"">
        <h3 style=""text-align: center; font-size: 23px;"">PAYMENT DETAILS</h3>
        <div>
            <div>
                <span>Offer price:</span>
                <span>{price}€</span>
            </div>
            <div>
                <span>Service fee:</span>
                <span>{servicefee}€</span>
            </div>
            <div style=""margin-top: 15px;"">
                <b>TOTAL {totalprice}</b>€
            </div>
        </div>
    </div>

    <div style=""padding: 15px 15px;"">
        <h6 style=""text-align: center; font-size: 15px; font-weight: bold; margin: 5px;"">SUPPORT</h6>
        <div>
            <p style=""float: right;"">info@airportglobaltransfer.com</p>
            <p style=""float: left;"">+90 850 242 1901</p>
        </div>
    </div>
</body>

</html>";       

            var outputPath = Path.Combine(_hostingEnvironment.WebRootPath, "pdf", fileName +".pdf");
            var pdfWriter = new PdfWriter(outputPath);
            var pdfDoc = new PdfDocument(pdfWriter);
            var converterProperties = new ConverterProperties();

            HtmlConverter.ConvertToPdf(htmlContent, pdfDoc, converterProperties);

            pdfDoc.Close();
        }


        private List<Location> DecodePolylinePoints(string encodedPoints)
        {
            if (encodedPoints == null || encodedPoints == "") return null;
            List<Location> poly = new List<Location>();
            char[] polylinechars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            try
            {
                while (index < polylinechars.Length)
                {
                    // calculate next latitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length)
                        break;

                    currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                    //calculate next longitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length && next5bits >= 32)
                        break;

                    currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                    Location p = new Location();
                    p.Latitude = Convert.ToDouble(currentLat) / 100000.0;
                    p.Longitude = Convert.ToDouble(currentLng) / 100000.0;
                    poly.Add(p);
                }
            }
            catch (Exception ex)
            {
                // logo it
            }
            return poly;
        }

        public class Location
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

    }
}
