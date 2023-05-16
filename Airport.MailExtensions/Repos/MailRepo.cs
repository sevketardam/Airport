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
<link href=""https://fonts.googleapis.com/css2?family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap"" rel=""stylesheet"">
    <!-- Font Awesome -->
    <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"" integrity=""sha512-iecdLmaskl7CVkqkXNQ/ZH/XLlvWZOJyj7Yy7tcenmpD1ypASozpmT/E0iPtmFIB46ZmdtAc9eNBvH0H/ZpiBw=="" crossorigin=""anonymous"" referrerpolicy=""no-referrer"" />
</head>
<style>
    section{{
        font-family: 'Poppins', sans-serif;
        background-color: antiquewhite;
    }}
    body{{
        margin: 0 auto;
        max-width: 35rem;
        padding-top: 2rem;
        padding-bottom: 2rem;
        /* border: 1px solid black; */
    }}
    .btn a {{
        padding: 1rem 3rem;
        background-color: rgb(13, 188, 13);
        border-radius: 3rem;
        color: white;
        text-decoration: none;
        transition: 200ms;
    }}
    .btn a:hover {{
        padding: 1rem 3rem;
        background-color: white ;
        /* border: 2px solid rgb(13, 188, 13); */
        border-radius: 3rem;
        color: rgb(13, 188, 13);
        text-decoration: none;
    }}
    .btn{{
        display: flex;
        justify-content: center;
        padding-top: 2rem;
    }}
    /* Header */
    header{{
        width: 100%;
        height: auto;
        background-color: orangered;
        padding-top: 2rem;
        padding-bottom: 2rem;
        display: flex;
        justify-content: center;
        align-items: center;
        flex-direction: column;
        border-top-right-radius: 2rem;
        border-top-left-radius: 2rem;
        font-family: 'Poppins', sans-serif;
    }}
    .logo{{
        font-weight: 800;
        font-style: italic;
        font-size: 30px;
        padding-bottom: 1rem;
    }}
    .header-text{{
        font-size: 20px;
    }}
    /* Durum */
    .durum {{
        padding: 1rem;
    }}
    .başarılı-text{{
        font-size: 20px;
    }}
    .başarılı-text-2{{
        font-size: 16px;
        color: rgb(62, 62, 62);
    }}
    .date-text-icon{{
        background-color: orangered;
        border-radius: 50%;
        width: 25px;
        height: 25px;
        display: flex;
        align-items: center;
        justify-content: center;
    }}
    .data-text-icon-group{{
        display: flex;
        align-items: center;

    }}
    .durum-date .down {{
        margin-left: .4rem;
        color: orangered;
    }}
    .date-text-icon{{
        margin-right: 1rem;

    }}
    .km {{
        display: inline;
    }}
    .span-group{{
        display: flex;
        flex-direction: column;
    }}
    /* İstek */
    .istek-ayrıntı {{
        padding: 1rem;
    }}
    .istek-ayrinti-head{{
        display: flex;
        justify-content: space-between;
        align-items: center;
        /* padding: 1rem; */
    }}
    .istek-ayrinti-head h2 {{
        margin: 0;
    }}
    .istek-ayrinti-head i {{
        font-size: 20px;
    }}
    .istek-ayrinti-bilgi-item{{
        display: flex;
        align-items: flex-start;
    }}
    /* Teklif */
    .teklif{{
        padding: 1rem;
    }}
    .teklif-head{{
        display: flex;
        justify-content: space-between;
        align-items: center;
    }}
    .teklif-head h2 {{
        margin: 0;
    }}
    .teklif-head i {{
        font-size: 20px;
        margin-right: 1rem;
    }}
    .teklif-item{{
        display: flex;
        align-items: flex-start;
    }}
    /* Footer */
    footer{{
        background-color: orangered;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        border-bottom-left-radius: 2rem;
        border-bottom-right-radius: 2rem;    
        padding-top: 2rem;
        font-family: 'Poppins', sans-serif;
        color: #320404;
    }}
    footer a{{
        color: #320404;
        text-decoration: none;
    }}
    .footer-phone{{
        display: flex;
        align-items: center;
        justify-content: center;
        flex-direction: column;
        padding-bottom: .5rem;
    }}
    .footer-phone p {{
        margin: 0;
    }}
    .footer-mail {{
        display: flex;
        align-items: center;
        justify-content: center;
        flex-direction: column;
    }}
    .footer-mail p {{
        margin: 0;
    }}
    .footer-alt-bilgi{{
        display: flex;
        align-items: center;
        justify-content: center;
        flex-direction: column;
        padding: 2rem;
        text-align: center;

    }}
    .footer-alt-bilgi p {{
        margin: 0;
    }}
    .footer-alt-bilgi p:first-child {{
        margin-bottom: .5rem ;
    }}
    .footer-alt-bilgi p:nth-child(2) {{
        font-size: 14px;
    }}
    .date-text-date{{
        font-size: 13px;
    }}
    .footer-alt-bilgi p:nth-child(1){{
        font-weight: 500;
    }}
    mark {{
        background-color: wheat;
        border-radius: 5px;
    }}
    .teklif-description, .istek-description{{
        color: black;
        font-weight: 600;
    }}
    @media screen and (max-width:468px){{
        body{{
            max-width: 30rem;
        }}
        .footer-phone:nth-child(1){{
            padding-left: .5rem;
            padding-right: .5rem;
            text-align: center;

        }}
    }}
    @media screen and (max-width:310px){{
        .btn a {{
            padding: .7rem 2rem;
        }}
    }}

</style>
<body>
    <!-- Header -->
    <header>
        <div class=""logo"">airportglobaltransfer.com</div>
        <div class=""header-text"">Succesful Created Reservation</div>
    </header>
    <!-- Durum -->
    <section class=""durum"">
        <div class=""durum-text"">
            <p class=""başarılı-text"">Your {reservationDetail.ReservationCode} transfer order is successful</h5>
            <p class=""başarılı-text-2"">Have a Question? You can contact support from your account.</h6>
            <p class=""başarılı-text-2"">Do you have a problem? Contact us for solution.</h6>
        </div>
        <div class=""durum-date"">
            <div class=""date-text"">
                <p class=""date-text-date"">{reservationDetail.ReservationDate}</p>
                <div class=""data-text-icon-group"">
                    <div class=""date-text-icon"">
                        <i class=""fa-solid fa-location-dot""></i>
                    </div>
                    <span>{reservationDetail.PickFullName}</span> 
                </div>
                <i class=""fa-solid fa-arrow-down down""></i>
                <div class=""data-text-icon-group"">
                    <div class=""date-text-icon"">
                        <i class=""fa-solid fa-location-dot""></i>
                    </div>
                    <div class=""span-group"">
                        <span>{reservationDetail.DropFullName}</span>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <!-- İstek Ayrıntı -->
    <section class=""istek-ayrıntı"">
        <div class=""istek-ayrinti-head"">
            <i class=""fa-solid fa-stop""></i>
            <h2>Detail</h2>
            <div><p>{reservationDetail.Comment}</p></div>
        </div>     
    </section>
    <!-- Teklif -->
    <section class=""teklif"">
        <div class=""teklif-bilgi"">
            <div class=""teklif-item"">
                <span >Car : &nbsp; </span>
                <span class=""teklif-description"">{reservationDetail.LocationCars.Car.Brand.CarBrandName} {reservationDetail.LocationCars.Car.Model.CarModelName} </span>
            </div>
            <div class=""teklif-item"">
                <span>Plate : &nbsp; </span>
                <span class=""teklif-description"">{reservationDetail.LocationCars.Car.Plate}</span>
            </div>          
        </div>
        <div class=""btn"">

            <a href=""http://test.airportglobaltransfer.com/pdf/{reservationDetail.ReservationCode}-{reservationDetail.Id}.pdf"">Show Voucher</a>
        </div>

    </section>
    <!-- Footer -->
    <footer>
        <div class=""footer-phone"">
            <p>Any Help?</p>
            <b><a href=""tel:+908502421901"">850 242 19014</a></b>
        </div>
        <div class=""footer-mail"">
<p>or</p>
            <b><p>info@airportglobaltransfer.com</p></b> 
        </div>      
    </footer> 
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
