using Airport.DBEntities.Entities;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Hosting;
using System.IO;

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

            var price = reservation.IsDiscount ? reservation.Discount : reservation.Price;
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

    <div style=""padding:15px 15px;"">
        <div>
            <div>
                <h4 style=""font-weight: bold; margin: 5px 0px;"">RESERVATIN CODE</h4>
                <p style=""margin: 5px 0px;"">{reservation.ReservationCode}</p>
            </div>

            <div>
                <h4 style=""font-weight: bold; margin: 5px 0px;"">CLIENT INFORMATION</h4>
                <b style=""margin: 5px 0px;"">BOOKING - Global Transfer Service <br> <b>Phone number:</b> {reservation.Phone}
                </b>
            </div>
            <div>
                <h4 style=""font-weight: bold; margin: 5px 0px;"">CAR INFORMATION</h4>
                <p style=""margin: 5px 0px;"">{reservation.LocationCars.Car.Brand.CarBrandName},{reservation.LocationCars.Car.Model.CarModelName}</p>
            </div>
        </div>
    </div>

    <div style=""padding: 5px 5px;"">

        <h3 style=""text-align: center; font-size: 23px;margin:0;padding:0;"">BOOKING DETAILS</h3>
        <div style=""display: flex; justify-content: space-between;"">
            <div>

                <p style=""font-weight: bold; margin: 0 0 15px 0px;"">WHERE</p>
                <div>
                    <p style=""margin: 0 0 15px 0px;"">{reservation.PickFullName}</p>
                </div>

                <p style=""font-weight: bold; margin: 0 0 15px 0px;"">TO</p>
                <div style=""margin: 0 0 15px 0px;"">
                    <p style=""margin:0px"">{reservation.DropFullName}</p>
                </div>

                <div>
                    <div style=""margin: 0 0 15px 0px;display:flex;"">
                        <p style=""font-weight: bold; margin:0px;"">DATE:</p>
                        <p style=""margin: 0px;padding-left:5px;"">{reservation.ReservationDate}</p>
                    </div>
                    <div style=""margin: 0 0 15px 0px;display:flex;"">
                        <p style=""font-weight: bold; margin:0px;"">PASSENGERS:</p>
                        <p style=""margin: 0px;padding-left:5px;"">{reservation.PeopleCount}</p>
                    </div>
                </div>

                <div>
                    <ul>
                        <li>{reservation.LocationCars.Car.MaxPassenger} (Car seat)</li>
                    </ul>
                </div>

            </div>

            <div>
                <img src=""http://test.airportglobaltransfer.com{reservation.LocationCars.Car.Type.CarImageURL}"" alt="""" />
            </div>
        </div>
    </div>

    <div style=""padding: 15px 15px; border-bottom: 4px solid #FF6900;"">
        <h3 style=""text-align: center; font-size: 23px;"">BOOKING DETAILS</h3>
        <div>
            <div style=""display: flex; width: 100%; margin: 5px 0px; justify-content: space-between;"">
                <p style=""margin: 0px;  font-size: 14px; font-weight: bold;"">PRICE
                </p>

                <p style=""margin: 0px; font-size: 14px; font-weight: bold;"">US${price}
                </p>
            </div>
        </div>
    </div>

    <div style=""padding: 15px 15px;"">
        <h6 style=""text-align: center; font-size: 15px; font-weight: bold; margin: 5px;"">CANCELLATION TERMS
        </h6>

        <p style=""font-size: 14px;"">
            If you want to cancel a booked and paid trip, write an e-mail to the address of the service
            support
            info@airportglobaltransfer.com
            no later than March 22, 2023 14:50 local time (UTC+03:00).</p>

        <p style=""font-size: 14px;"">
            Refunds (full or prepaid) are not allowed if you do not show up on time at the agreed place
            start
            trips, as well as within 60 minutes at locations, sea or river passenger ports, 30 minutes on
            stations and
            15 minutes in
        </p>

        <h6 style=""text-align: center; font-size: 15px; font-weight: bold; margin: 5px;"">SUPPORT

        </h6>

        <div style=""display: flex; width: 100%; justify-content: space-between;"">
            <p style=""margin: 0px;  font-size: 14px;"">info@airportglobaltransfer.com

            </p>

            <p style=""margin: 0px; font-size: 14px;"">+90 850 242 1901
            </p>
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
    }
}
