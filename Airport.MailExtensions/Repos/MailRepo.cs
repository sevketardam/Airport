using Airport.MessageExtensions.Interfaces;
using System;
using MimeKit;
using MailKit.Net.Smtp;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Airport.MessageExtension.VM;
using Airport.DBEntities.Entities;

namespace Airport.MessageExtensions.Repos
{
    public class MailRepo : IMail
    {
        public void SendReservationMail(Reservations reservationDetail)
        {
            MimeMessage mimeMessage = new MimeMessage();

            MailboxAddress mailboxAddressFrom = new MailboxAddress
                ("Airport", "airportglobaltransfer@gmail.com");

            mimeMessage.From.Add(mailboxAddressFrom);

            MailboxAddress mailboxAddressTo = new MailboxAddress("User", reservationDetail.Email);

            var bodyBuilder = new BodyBuilder();
            mimeMessage.To.Add(mailboxAddressTo);

            var serviceText = "";

            if (reservationDetail.ReservationServicesTables != null && reservationDetail.ReservationServicesTables.Count > 0)
            {
                serviceText += @"<h4 style=""font-size: 2rem;margin: 0 0 0.7rem 0;font-weight: 600;"">Services</h4>";
                reservationDetail.ReservationServicesTables.ForEach(a =>
                {
                    serviceText += @$"        <div class=""teklif-bilgi"">
            <div class=""teklif-item"">
                <span style=""
                font-weight: 600;"">{a.ServiceItem.ServiceProperty.ServicePropertyName}</span>
                <p style=""font-weight: 400; font-size: .8rem;"">
                    {a.ServiceItem.ServiceProperty.ServicePropertyDescription}
                </p>
            </div>
        </div>";
                });
                
            }


            var carAttrHtml = "";
            if (reservationDetail.LocationCars.Car.Armored)
            {
                carAttrHtml += "<img style='width: 18px;' src='http://www.test.airportglobaltransfer.com/img/i5.png'>";
            }

            if (reservationDetail.LocationCars.Car.Wifi)
            {
                carAttrHtml += "<img style='width: 18px;' src='http://www.test.airportglobaltransfer.com/img/i4.png'>";
            }

            if (reservationDetail.LocationCars.Car.Water)
            {
                carAttrHtml += "<img style='width: 18px;' src='http://www.test.airportglobaltransfer.com/img/i1.png'>";
            }

            if (reservationDetail.LocationCars.Car.Partition)
            {
                carAttrHtml += "<img style='width: 18px;' src='http://www.test.airportglobaltransfer.com/img/i6.png'>";
            }

            if (reservationDetail.LocationCars.Car.Charger)
            {
                carAttrHtml += "<img style='width: 18px;' src='http://www.test.airportglobaltransfer.com/img/i3.png'>";
            }

            if (reservationDetail.LocationCars.Car.Disabled)
            {
                carAttrHtml += "<img style='width: 18px;' src='http://www.test.airportglobaltransfer.com/img/i2.png'>";
            }

            var returnIcon = "https://storage.acerapps.io/app-1348/asd/altok.png";
            var isReturn = false;
            if (reservationDetail.ReturnStatus)
            {
                returnIcon = "https://storage.acerapps.io/app-1348/asd/gelgit.png";
                isReturn = true;
            }

            mimeMessage.Subject = "Reservation Information";
            bodyBuilder.HtmlBody = @$"<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Document</title>
    <!-- Google Fonts -->
    <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
    <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
    <link
        href=""https://fonts.googleapis.com/css2?family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap""
        rel=""stylesheet"">
    <!-- Font Awesome -->
    <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css""
        integrity=""sha512-iecdLmaskl7CVkqkXNQ/ZH/XLlvWZOJyj7Yy7tcenmpD1ypASozpmT/E0iPtmFIB46ZmdtAc9eNBvH0H/ZpiBw==""
        crossorigin=""anonymous"" referrerpolicy=""no-referrer"" />
</head>

<body style=""
margin: 0 auto;
        max-width: 35rem;
        padding-top: 2rem;
        padding-bottom: 2rem;"">
    <!-- Header -->
    <div class=""header"" style=""
    width: 100%;
        height: auto;
        padding: 1rem 1rem 0 1rem;
        border-top-right-radius: 2rem;
        border-top-left-radius: 2rem;
        font-family: 'Poppins', sans-serif;"">
        <div class=""logo"" style=""
        font-weight: 800;
        font-style: italic;
        font-size: 30px;
        ""><img style=""width: 100%;max-width: 200px;"" src=""http://test.airportglobaltransfer.com/images/Logo.png""></div>
        <div class=""header-text"" style=""
        font-size: 20px;"">Succesful Created Reservation</div>
    </div>
    <div class=""durum section"" style=""
    font-family: 'Poppins', sans-serif;
        background-color: white;
        padding: 1rem 1rem 0 1rem; 
        "">
        <div class=""durum-text"">

            <p class=""başarılı-text"" style=""
                font-size: 26px;
        margin: 0 0 .3rem 0;
        "">Your {reservationDetail.ReservationCode} transfer order is successful</h5> 
            <p class=""başarılı-text-2"" style=""
            font-size: 15px;
        color: rgb(62, 62, 62);
        margin: 0;"">Have a Question? You can contact support from your account.</h6>
            <p class=""başarılı-text-2"" style=""
                font-size: 15px;
        color: rgb(62, 62, 62);
        margin: 0;"">Do you have a problem? Contact us for solution.</h6>
        </div>
        <div class=""durum-date"">
            <div class=""date-text"">
                <p class=""date-text-date"" style="" font-size: 12px;
                font-weight: 300;"">{reservationDetail.ReservationDate.ToString("dd.MM.yyyy HH:mm")}</p>

               <div style=""position: relative;"">
<div class=""data-text-icon-group"" style=""
                 display: flex;
                 "">
                    <div class=""date-text-icon"" style=""
        width: 25px;
        height: 25px;
        margin-right: .5rem;
        "">
                        <img src=""https://storage.acerapps.io/app-1348/asd/A.png"" width=""100%"" alt="""">
                    </div>
                    <span>{reservationDetail.PickFullName}</span>
                </div>
                <img src=""{returnIcon}"" style=""
                width: 25px;
    height: 18px;
    object-fit: contain;
    margin: 0.5rem 0;
    transform: translate(-50%, -75%);
    "" alt="""">
                <div class=""data-text-icon-group"" style=""
                display: flex;margin-top: 1.5rem;
                "">
                   <div class=""date-text-icon"" style=""
       width: 25px;
       height: 25px;
       margin-right: .5rem;
       "">
                       <img src=""https://storage.acerapps.io/app-1348/asd/B.png"" width=""100%"" alt="""">
                   </div>
                   <span>{reservationDetail.DropFullName}</span>
               </div>

</div>
                
            </div>
        </div>
    </div>
    <div class=""teklif section"" style=""
    font-family: 'Poppins', sans-serif;
        background-color: white;
        padding: 1rem 1rem 0 1rem;"">
        <h4 style=""
    font-size: 2rem;
    margin: 0 0 0.7rem 0;
    font-weight: 600;"">
            Details
        </h4>
        <div class=""teklif-bilgi"">
            <div class=""teklif-item"" style=""
                display: flex;
        align-items: flex-start;"">
                <span style=""width: 50%;
                font-weight: 300;"">Price : </span>
                <span class=""teklif-description"" style=""color: black;
        font-weight: 500;
        ""> {reservationDetail.TotalPrice} € </span>
            </div>
            <div class=""teklif-item"" style=""
            display: flex;
        align-items: flex-start;
        "">
                <span style=""width: 50%;font-weight: 300;"">Passengers : </span>
                <span class=""teklif-description"" style=""color: black;font-weight: 500; "">{reservationDetail.PeopleCount}</span>
            </div>
            <div class=""teklif-item"" style=""
            display: flex;
        align-items: flex-start;
        "">
                <span style=""width: 50%;
                font-weight: 300;"">Transport: </span>
                <span class=""teklif-description"" style=""color: black;
        font-weight: 500;
        ""> Every </span> 
            </div>
        </div>
        <div style=""width: 80%; height: 1px; border-top: 2px dashed rgb(0, 0, 0, .3); margin: 1rem 0;""></div>
        <div class=""teklif-bilgi"">
            <div class=""teklif-item"" style=""
                display: flex;
        align-items: flex-start;
        "">
                <span style=""width: 50%;
                font-weight: 300;"">Transport Type : </span>
                <span class=""teklif-description"" style=""color: black;
        font-weight: 500;
        ""> {reservationDetail.LocationCars.Car.Type.CarTypeName} {reservationDetail.LocationCars.Car.MaxPassenger} {reservationDetail.LocationCars.Car.SmallBags} {reservationDetail.LocationCars.Car.SuitCase} </span>
            </div>
            <div class=""teklif-item"" style=""
            display: flex;
        align-items: flex-start;
        "">
                <span style=""width: 50%;font-weight: 300;"">Car : </span>
                <span class=""teklif-description"" style=""color: black;font-weight: 500;"">  {reservationDetail.LocationCars.Car.Brand.CarBrandName} {reservationDetail.LocationCars.Car.Model.CarModelName}{reservationDetail.LocationCars.Car.Series.CarSeriesName} </span>
               
            </div>
            <div class=""teklif-item"" style=""
            display: flex;
        align-items: flex-start;
        "">
                <span style=""width: 50%;
                font-weight: 300;"">Plate : </span>
                <span class=""teklif-description"" style=""color: black;
        font-weight: 500;
        "">  {reservationDetail.LocationCars.Car.Plate} </span>

            </div>
            <div class=""teklif-item attrs-item"" style=""
            display: flex;
        align-items: flex-start;
        margin-top: .3rem;
        "">
                <span style=""width: 50%;
                font-weight: 300;"">Attributes :  </span>
                <span class=""teklif-description attrs"" style=""color: black;
        font-weight: 500;
        display: flex;
        align-items: center;
        gap: .5rem;
        "">
                    {carAttrHtml}
                </span>

            </div>

        </div>
    </div>
<div class=""teklif section"" style=""
    font-family: 'Poppins', sans-serif;
        background-color: white;
        padding: 1rem 1rem 0 1rem;"">

        {serviceText}
        
        <br>
        <div class=""btn"" style=""
        padding: 2rem 0;"">

            <a href=""http://test.airportglobaltransfer.com/pdf/{{reservationDetail.ReservationCode}}-{{reservationDetail.Id}}.pdf""
                style=""
                padding: 1rem 3rem;
        background-color: rgb(13, 188, 13);
        border-radius: 3rem;
        color: white;
        text-decoration: none;
        transition: 200ms;"">Show Voucher</a>
        </div>
    </div>
    <div class=""footer"" style="" padding: 1rem 1rem;
    font-family: 'Poppins', sans-serif;
    color: #320404;"">
        <div class=""footer-phone"" style="" padding-bottom: .5rem;"">
            <p style=""margin: 0;"">Any Help?</p>
            <b><a href=""tel:+908502421901"" style=""color: #320404;
                text-decoration: none;"">+90 850 242 19014</a></b>
        </div>
        <div class=""footer-mail"">
            <p style="" margin: 0;"">or</p>
            <b>
                <p style="" margin: 0;"">info@airportglobaltransfer.com</p>
            </b>
        </div>
    </div>
</body>

</html>";


            mimeMessage.Body = bodyBuilder.ToMessageBody();


            SmtpClient client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            //sepetispor@gmail.com //cmjvjyecqpnqwkis
            client.Authenticate("airportglobaltransfer@gmail.com", "jcgbdclwxjpcpcew");
            client.Send(mimeMessage);
            client.Disconnect(true);
        }
    }
}
