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
using Airport.DBEntitiesDAL.Interfaces;

namespace Airport.UI.Models.Extendions
{
    public class PdfCreator
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public PdfCreator(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public void CreateReservationPDF(string fileName, Reservations reservation)
        {

            var httpClient = new HttpClient();
            var apiUrl = $"https://maps.googleapis.com/maps/api/directions/json?origin={reservation.PickLatLng.Replace("lat:", "").Replace("lng:", "")}&destination={reservation.DropLatLng.Replace("lat:", "").Replace("lng:", "")}&mode=driving&key=AIzaSyAnqSEVlrvgHJymL-F8GmxIwNbe8fYUjdg";
            var response = httpClient.GetAsync(apiUrl).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var contentJsonResult = JsonConvert.DeserializeObject<JObject>(content);

            var point = contentJsonResult["routes"][0]["overview_polyline"]["points"].ToString();

            var convertLocationValue = DecodePolylinePoints(point);

            var str = "https://maps.googleapis.com/maps/api/staticmap?markers=color%3Ared%7Clabel%3AA%7C41.1307027%2C28.9877942&markers=color%3Ared%7Clabel%3AB%7C41.081402%2C28.9819771&size=303x156&key=AIzaSyAnqSEVlrvgHJymL-F8GmxIwNbe8fYUjdg&path=color%3Ablue%7Cweight%3A5%7C";

            convertLocationValue.ForEach(a =>
            {
                str += a.Latitude.ToString().Replace(",", ".") + "%2C" + a.Longitude.ToString().Replace(",", ".") + "%7C";
            });

            str = str.Substring(0, str.Length - 3);

            var imageResponse = httpClient.GetAsync(str).Result;

            byte[] imageBytes = imageResponse.Content.ReadAsByteArrayAsync().Result;
            string base64String = Convert.ToBase64String(imageBytes);

            var price = reservation.IsDiscount ? reservation.Discount : reservation.OfferPrice;
            var servicefee = reservation.ServiceFee;

            var totalprice = price + servicefee;

            var priceHtml = "";

            if (reservation.HidePrice == false)
            {
                var discountHtml = "";
                var specialDiscountHtml = "";

                if (reservation.Coupons != null)
                {
                    totalprice = Math.Round(Convert.ToDouble(totalprice)- ((reservation.Coupons.Discount * Convert.ToDouble(totalprice)) / 100),2);
                    discountHtml = @$"        <div style=""display: flex; justify-content: space-between;width:100%;"">
            <div>
                      <p style=""margin: 0px;"">Discount:</p>
            </div>
            <div>
                      <p style=""margin: 0px;margin-left:.5rem;"">{reservation.Coupons.Discount}%</p>
            </div>
        </div>";
                }

                if (reservation.IsDiscount)
                {
                    specialDiscountHtml = @$"        <div style=""display: flex; justify-content: space-between;width:100%;"">
            <div>
                      <p style=""font-weight: bold;margin: 0px;"">Special Price:</p>
            </div>
            <div>
                      <p style=""margin: 0px;margin-left:.5rem;"">{reservation.Discount} €</p>
            </div>
        </div>";
                }
                else
                {
                    specialDiscountHtml = $@" <div style=""display: flex; justify-content: space-between;width:100%;"">
            <div>
                      <p style=""font-weight: bold;margin: 0px;"">TOTAL:</p>
            </div>
            <div>
                       <p style=""margin: 0px;margin-left:.5rem;"">€{totalprice}</p>
            </div>
        </div>";
                }

                priceHtml = @$"
        <h3 style=""font-size: 23px;text-align: center;"">PAYMENT DETAILS</h3>

        <div style=""display: flex; justify-content: space-between;width:100%;"">
            <div>
                      <p style=""margin: 0px;"">Offer price:</p>
            </div>
            <div>
                      <p style=""margin: 0px;margin-left:.5rem"">€{price}</p>
            </div>
        </div>
        <div style=""display: flex; justify-content: space-between;width:100%;"">
            <div>
                      <p style=""margin: 0px;"">Service fee:</p>
            </div>
            <div>
                      <p style=""margin: 0px;margin-left:.5rem;"">€{servicefee}</p>
            </div>
        </div>
        {discountHtml}
        {specialDiscountHtml}
";

            }

            var serviceHtml = "";
            if (reservation.ReservationServicesTables.Count > 0)
            {
                serviceHtml = @"
<div style=""margin: 10px 10px; border-bottom: 4px solid #FF6900;"">
<h3 style=""font-size: 23px;text-align: center;"">SERVICES DETAILS</h3>";
                foreach (var item in reservation.ReservationServicesTables)
                {
                    serviceHtml += @$" <div>
            <div>
                      <p style=""margin: 0px;font-weight: bold;"">{item.ServiceItem.ServiceProperty.ServicePropertyName}</p>
            </div>
            <div style=""margin: 0px;margin-left:.5rem;font-size: 10px;!important;"">
                     {item.ServiceItem.ServiceProperty.ServicePropertyDescription}
            </div>
        </div>"; 
                }
                serviceHtml += "</div></div>";
            }



            var htmlContent = @$"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Document</title>
<style>
*{{
    font-size:.9rem;
}}
</style>
</head>

<body style=""margin: 0px; padding: 30px;"">
    <div style=""width: 100%;
                    gap: 22px;
                    background-color: #FF6900; justify-content: center;
                    text-align: center;
                    align-items: center; 
                    padding: 20px 0px;"">
<img src=""http://www.test.airportglobaltransfer.com/images/Logo.png"" alt="""" style=""width:230px;height:auto;"">
        <h3 style=""font-size: 25px; font-weight: bold; margin: 0px;"">airportglobaltransfer.com</h3>
        <p style=""font-size: 13px; font-weight: 600;"">BOOKING CONFIRMATION</p>
    </div>


    <div style=""padding: 5px 5px;"">
        <div>
            <h4 style=""font-weight: bold; margin: 5px 0px;"">RERVATION CODE</h4>
            <p style=""margin: 5px 0px;"">{reservation.ReservationCode}</p>
        </div>

        <div>
            <h4 style=""font-weight: bold; margin: 5px 0px;"">CLIENT INFORMATION</h4>
            <p style=""margin: 5px 0px;"">{reservation.User.CompanyName}</p>
        </div>

        <div>
            <h4 style=""font-weight: bold; margin: 5px 0px;"">CARRIER INFORMATION</h4>
            <p style=""margin: 0px;"">GLOBAL TRANSFER</p>
            <p style=""margin: 0px;"">Phone number:{reservation.User.CompanyPhoneNumber}</p>
        </div>
        <div>
            <h4 style=""font-weight: bold; margin: 5px 0px;"">TRANSPORT INFORMATION</h4>
            <p style=""margin:0px;"">Transport type: 
                Small Bags x{reservation.LocationCars.Car.SmallBags}, Suit Case x{reservation.LocationCars.Car.SuitCase}, Passenger x{reservation.LocationCars.Car.MaxPassenger} 
            </p>
            <p style=""margin: 0px;"">Vehicle: {reservation.LocationCars.Car.Brand.CarBrandName} {reservation.LocationCars.Car.Model.CarModelName} </p>
            <p style=""margin: 0px;"">License plate: {reservation.LocationCars.Car.Plate}</p>
            <p style=""margin: 0px;"">
                Options:{(reservation.LocationCars.Car.Wifi ? "Free wifi" : "")}{(reservation.LocationCars.Car.Water ? ",Free water" : "")}{(reservation.LocationCars.Car.Partition ? ",Safety partition" : "")}{(reservation.LocationCars.Car.Charger ? ",Phone charger" : "")}
{(reservation.LocationCars.Car.Armored ? ",Armored voyage" : "")}{(reservation.LocationCars.Car.Disabled ? ",Handy for the disabled" : "")}
            </p>
        </div>
    </div>


    <div style=""padding: 5px 5px;"">

        <h3 style=""text-align: center; font-size: 23px;margin-bottom:20px;padding:0;"">BOOKING DETAILS</h3>
        <div style=""display: flex; justify-content: space-between;"">
            <div style=""float: right;"">
                <p style=""font-weight: bold; margin: 0;"">RIDE DATE</p>
                <div>
                    <p style=""margin: 0 0 10px 0px;"">{reservation.ReservationDate.ToString("d 'of' MMMM, yyyy")} {reservation.ReservationDate.ToShortTimeString()}</p>
                </div>

                <p style=""font-weight: bold; margin: 0px;"">FROM</p>
                <div style=""margin: 0 0 10px 0px;"">
                    <p style=""margin:0px"">{reservation.PickFullName}</p>
                </div>

                <p style=""font-weight: bold; margin: 0px;"">TO</p>
                <div style=""margin: 0 0 10px 0px;"">
                    <p style=""margin:0px"">{reservation.DropFullName}</p>
                </div>

                <div style=""margin: 0 0 10px 0px;"">
                    <span style=""font-weight: bold; margin: 0px;"">NAME SIGN:</span>
                    <span style=""margin:0px"">{reservation.Name} {reservation.Surname}</span>
                </div>

                <div style=""margin: 0 0 10px 0px;"">
                    <span style=""font-weight: bold; margin: 0px;"">PASSENGERS:</span>
                    <span style=""margin:0px"">{reservation.PeopleCount}</span>
                </div>

                <p style=""font-weight: bold; margin: 0px;"">COMMENT</p>
                <div style=""margin: 0 0 10px 0px;"">
                    <p style=""margin:0px"">{reservation.Comment}</p>
                </div>
                <p style=""font-weight: bold; margin: 0px;"">WAITING TIME:</p>
                <div style=""margin: 0;"">
                    <p style=""margin:0px"">Free waiting time included: at airports, sea or river passenger port terminals — 60 minutes,at railway stations — 30 minutes, all other locations — 15 minutes</p>
                </div>
            </div>

            <div style=""margin-left:20px;float: left;"">
                <img src=""data:image/png;base64,{base64String}"" alt=""Harita"">
            </div>
        </div>
    </div>

    {serviceHtml}

<div style=""margin: 10px 10px; border-bottom: 4px solid #FF6900;"">
    {priceHtml}
    </div>
 <div style=""margin: 10px 10px;"">
        <h6 style=""text-align: center; font-size: 15px; font-weight: bold; margin: 5px;"">CANCELATION POLICY</h6>
        <div>
            <p>Allowed cancellation period for reimbursement is over. Any payments will not be reimbursed.</p>
        </div>
    </div>

    <div style=""margin: 10px 10px;"">
        <h6 style=""text-align: center; font-size: 15px; font-weight: bold; margin: 5px;"">SUPPORT</h6>
        <div>
            <p style=""float: right;"">info@airportglobaltransfer.com</p>
            <p style=""float: left;"">+90 850 242 1901</p>
        </div>
    </div>
</body>

</html>";

            var outputPath = Path.Combine(_hostingEnvironment.WebRootPath, "pdf", fileName + ".pdf");
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
