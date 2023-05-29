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
            var carAttrHtml = "";
            if (reservationDetail.LocationCars.Car.Armored)
            {
                carAttrHtml += "<img src='http://www.airportglobaltransfer.com/img/i5.png'>";

            }

            if (reservationDetail.LocationCars.Car.Wifi)
            {
                carAttrHtml += "<img src='http://www.airportglobaltransfer.com/img/i4.png'>";
            }

            if (reservationDetail.LocationCars.Car.Water)   
            {
                carAttrHtml += "<img src='http://www.airportglobaltransfer.com/img/i1.png'>";
            }

            if (reservationDetail.LocationCars.Car.Partition)
            {
                carAttrHtml += "<img src='http://www.airportglobaltransfer.com/img/i6.png'>";
            }

            if (reservationDetail.LocationCars.Car.Charger)
            {
                carAttrHtml += "<img src='http://www.airportglobaltransfer.com/img/i3.png'>";
            }

            if (reservationDetail.LocationCars.Car.Disabled)
            {
                carAttrHtml += "<img src='http://www.airportglobaltransfer.com/img/i2.png'>";
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
<style>
    .section {{

        /* border-left:1px solid ; */
        /* border-right: 1px solid; */
    }}

    body {{

        /* border: 1px solid black; */
    }}

    .btn a {{}}

    .btn a:hover {{
        /* padding: 1rem 3rem;
        background-color: white ;
       border: 2px solid rgb(13, 188, 13); 
        border-radius: 3rem;
        color: rgb(13, 188, 13);
        text-decoration: none; */
    }}

    .btn {{
        /* display: flex; */
        /* justify-content: center; */
    }}

    /* Header */
    .header {{

        /* display: flex; */
        /* justify-content: center; */
        /* align-items: center; */
        /* flex-direction: column; */
        /* background-color: #ff6709; */

    }}

    .logo {{

        /* padding-bottom: 1rem; */
    }}

    .header-text {{}}

    /* Durum */
    .durum {{}}

    .başarılı-text {{}}

    .başarılı-text-2 {{}}

    .date-text-icon {{}}

    .data-text-icon-group {{}}

    .durum-date .down {{}}

    .date-text-icon {{}}

    .km {{}}

    .span-group {{}}

    /* İstek */
    .istek-ayrıntı {{}}

    .istek-ayrinti-head {{

        /* padding: 1rem; */
    }}

    .istek-ayrinti-head h2 {{}}

    .istek-ayrinti-head i {{}}

    .istek-ayrinti-bilgi-item {{}}

    /* Teklif */
    .teklif {{}}

    .teklif-head {{}}

    .teklif-head h2 {{}}

    .teklif-head i {{}}

    .teklif-item {{}}

    /* Footer */
    .footer {{
        /* background-color: #ff6709; */
        /* display: flex; */
        /* flex-direction: column; */
        /* align-items: center; */
        /* justify-content: center; */
        /* border-bottom-left-radius: 2rem; */
        /* border-bottom-right-radius: 2rem;     */

    }}

    .footer a {{}}

    .footer-phone {{
        /* display: flex; */
        /* align-items: center; */
        /* justify-content: center; */
        /* flex-direction: column; */
       
    }}

    .footer-phone p {{
        
    }}

    .footer-mail {{
        /* display: flex; */
        /* align-items: center; */
        /* justify-content: center; */
        /* flex-direction: column; */
    }}

    .footer-mail p {{
       
    }}

    .footer-alt-bilgi {{
        

    }}

    .footer-alt-bilgi p {{
    }}

    .footer-alt-bilgi p:first-child {{
    }}

    .footer-alt-bilgi p:nth-child(2) {{
    }}

    .date-text-date {{
       

    }}

    .footer-alt-bilgi p:nth-child(1) {{
    }}

    mark {{
    }}

    .teklif-description,
    .istek-description {{
    }}


    .teklif-item>span:first-child {{
    }}

    .attrs-item {{
        
    }}

    .attrs-item .attrs {{
       

    }}

    .attrs img {{
        
    }}
</style>

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
        "">airportglobaltransfer.com</div>
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
                font-weight: 300;"">{reservationDetail.ReservationDate}</p>
                <div class=""data-text-icon-group"" style=""
                 display: flex;
                 "">
                    <div class=""date-text-icon"" style=""
                        background-color: #ff6709;
        border-radius: 50%;
        width: 25px;
        height: 25px;
        display: flex;
        align-items: center;
        justify-content: center;
        margin-right: 1rem;
        "">
                        A
                    </div>
                    <span>{reservationDetail.PickFullName}</span><!--   -->
                </div>
                <i class=""fa-solid fa-arrow-down down"" style=""
                margin-left: .4rem;
        color: #ff6709;
        ""></i>
                <div class=""data-text-icon-group"" style=""
                        align-items: center;
                        "">
                    <div class=""date-text-icon"" style=""
                        background-color: #ff6709;
        border-radius: 50%;
        width: 25px;
        height: 25px;
        display: flex;
        align-items: center;
        justify-content: center;
        margin-right: 1rem;
        "">
                        B
                    </div>
                    <div class=""span-group"" style=""
                     display: flex;
        flex-direction: column;"">
                        <span>{reservationDetail.DropFullName}</span><!--   -->
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
    font-weight: 600;
"">
            Details
        </h4>
        <div class=""teklif-bilgi"">
            <div class=""teklif-item"" style=""
                display: flex;
        align-items: flex-start;
        "">
                <span style=""width: 40%;
                font-weight: 300;"">Price : &nbsp; </span>
                <span class=""teklif-description"" style=""color: black;
        font-weight: 500;
        ""> {reservationDetail.TotalPrice} € </span><!--  -->
            </div>
            <div class=""teklif-item"" style=""
            display: flex;
        align-items: flex-start;
        "">
                <span style=""width: 40%;
                font-weight: 300;"">Passengers : &nbsp; </span>
                <span class=""teklif-description"" style=""color: black;
        font-weight: 500;
        "">{reservationDetail.PeopleCount}</span> <!--  -->
            </div>
            <div class=""teklif-item"" style=""
            display: flex;
        align-items: flex-start;
        "">
                <span style=""width: 40%;
                font-weight: 300;"">Transport Types : &nbsp; </span>
                <span class=""teklif-description"" style=""color: black;
        font-weight: 500;
        ""> Every </span> <!-- every sabit kalacak  -->
            </div>
        </div>
        <div style=""width: 80%; height: 1px; border-top: 2px dashed rgb(0, 0, 0, .3); margin: 1rem 0;""></div>
        <div class=""teklif-bilgi"">
            <div class=""teklif-item"" style=""
                display: flex;
        align-items: flex-start;
        "">
                <span style=""width: 40%;
                font-weight: 300;"">Transport Type : &nbsp; </span>
                <span class=""teklif-description"" style=""color: black;
        font-weight: 500;
        ""> {reservationDetail.LocationCars.Car.Type.CarTypeName} {reservationDetail.LocationCars.Car.MaxPassenger} {reservationDetail.LocationCars.Car.SmallBags} {reservationDetail.LocationCars.Car.SuitCase} </span>
            </div>
            <div class=""teklif-item"" style=""
            display: flex;
        align-items: flex-start;
        "">
                <span style=""width: 40%;
                font-weight: 300;"">Car : &nbsp; </span>
                <span class=""teklif-description"" style=""color: black;
        font-weight: 500;
        "">  {reservationDetail.LocationCars.Car.Brand.CarBrandName} {reservationDetail.LocationCars.Car.Model.CarModelName}{reservationDetail.LocationCars.Car.Series.CarSeriesName} </span>
               
            </div>
            <div class=""teklif-item"" style=""
            display: flex;
        align-items: flex-start;
        "">
                <span style=""width: 40%;
                font-weight: 300;"">Plate : &nbsp; </span>
                <span class=""teklif-description"" style=""color: black;
        font-weight: 500;
        "">  {reservationDetail.LocationCars.Car.Plate} </span>

            </div>
            <div class=""teklif-item attrs-item"" style=""
            display: flex;
        align-items: flex-start;
        margin-top: .3rem;
        "">
                <span style=""width: 40%;
                font-weight: 300;"">Attributes : &nbsp; </span>
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
                text-decoration: none;"">850 242 19014</a></b>
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
