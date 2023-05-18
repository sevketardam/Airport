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
                carAttrHtml += "<img src='http://www.test.airportglobaltransfer.com/img/i5.png'>";

            }

            if (reservationDetail.LocationCars.Car.Wifi)
            {
                carAttrHtml += "<img src='http://www.test.airportglobaltransfer.com/img/i4.png'>";
            }

            if (reservationDetail.LocationCars.Car.Water)   
            {
                carAttrHtml += "<img src='http://www.test.airportglobaltransfer.com/img/i1.png'>";
            }

            if (reservationDetail.LocationCars.Car.Partition)
            {
                carAttrHtml += "<img src='http://www.test.airportglobaltransfer.com/img/i6.png'>";
            }

            if (reservationDetail.LocationCars.Car.Charger)
            {
                carAttrHtml += "<img src='http://www.test.airportglobaltransfer.com/img/i3.png'>";
            }

            if (reservationDetail.LocationCars.Car.Disabled)
            {
                carAttrHtml += "<img src='http://www.test.airportglobaltransfer.com/img/i2.png'>";
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
<link href=""https://fonts.googleapis.com/css2?family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap"" rel=""stylesheet"">
    <!-- Font Awesome -->
    <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"" integrity=""sha512-iecdLmaskl7CVkqkXNQ/ZH/XLlvWZOJyj7Yy7tcenmpD1ypASozpmT/E0iPtmFIB46ZmdtAc9eNBvH0H/ZpiBw=="" crossorigin=""anonymous"" referrerpolicy=""no-referrer"" />
</head>
<body>
    <!-- Header -->
    <div style=""
    width: 100%;
        height: auto;
        background-color: #ff6709;
        padding-top: 2rem;
        padding-bottom: 2rem;
        display: flex;
        justify-content: center;
        align-items: center;
        flex-direction: column;
        border-top-right-radius: 2rem;
        border-top-left-radius: 2rem;
        font-family: 'Poppins', sans-serif;"">
        <div style=""
        font-weight: 800;
        font-style: italic;
        font-size: 30px;
        padding-bottom: 1rem;"">airportglobaltransfer.com</div>
        <div  style=""
         font-size: 20px;"">Succesful Created Reservation</div>
    </div>
    <!-- Durum -->
    <div  style=""padding: 1rem;"">
        <div >
            <p style=""
                 font-size: 26px;
        margin: 0 0 .3rem 0;"">Your {reservationDetail.ReservationCode} transfer order is successful</h5>
            <p style=""
            font-size: 15px;
        color: rgb(62, 62, 62);
        margin: 0;"">Have a Question? You can contact support from your account.</h6>
            <p  style=""
                font-size: 15px;
        color: rgb(62, 62, 62);
        margin: 0;"">Do you have a problem? Contact us for solution.</h6>
        </div>
        <div >
            <div>
                <p style=""  font-size: 12px;
                font-weight: 300;"">{reservationDetail.ReservationDate}</p>
                <div style=""display: flex;
                align-items: center;"">
                    <div style=""background-color: #ff6709;
                    border-radius: 50%;
                    width: 25px;
                    height: 25px;
                    display: flex;
                    align-items: center;
                    justify-content: center;"">
                        A
                    </div>
                    <span>{reservationDetail.PickFullName}</span>
                </div>
                <i></i>
                <div style=""display: flex;
                align-items: center;"">
                    <div style=""background-color: #ff6709;
                    border-radius: 50%;
                    width: 25px;
                    height: 25px;
                    display: flex;
                    align-items: center;
                    justify-content: center;"">
                        B
                    </div>
                    <div style=""display: flex;
                    flex-direction: column;"">
                        <span>{reservationDetail.DropFullName}</span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div>
        <h4 style=""
    font-size: 2rem;
    margin: 0 0 0.7rem 0;
    text-align: center;
    font-weight: 600;
"">
            Details
        </h4>
        <div>
            <div style=""display: flex;
            align-items: flex-start;"">
                <span >Price : &nbsp; </span>
                <span> {reservationDetail.TotalPrice} € </span><!-- fiyat -->
            </div>
            <div style=""display: flex;
            align-items: flex-start;"">
                <span>Passengers : &nbsp; </span>
                <span>{reservationDetail.PeopleCount}</span> <!-- yolcu sayısı -->
            </div>   
            <div  style=""display: flex;
            align-items: flex-start;"">
                <span>Transport Types : &nbsp; </span>
                <span> Every </span>  <!-- every sabit kalacak  -->
            </div> 
        </div>
        <div style=""width: 80%; height: 1px; border-top: 2px dashed rgb(0, 0, 0, .3); margin: 1rem 0;""></div>
        <div >
            <div style=""display: flex;
            align-items: flex-start;"">
                <span>Transport Type : &nbsp; </span>
                <span> {reservationDetail.LocationCars.Car.Type.CarTypeName} {reservationDetail.LocationCars.Car.MaxPassenger} {reservationDetail.LocationCars.Car.SmallBags} {reservationDetail.LocationCars.Car.SuitCase} </span>
              
            </div>
            <div style=""display: flex;
            align-items: flex-start;"">
                <span>Car : &nbsp; </span>
                <span>{reservationDetail.LocationCars.Car.Brand.CarBrandName} {reservationDetail.LocationCars.Car.Model.CarModelName} {reservationDetail.LocationCars.Car.Series.CarSeriesName} </span>
            </div>   
            <div  style=""display: flex;
            align-items: flex-start;"">
                <span >Plate : &nbsp; </span>
                <span  {reservationDetail.LocationCars.Car.Plate} </span>
                <!--  -->
                
            </div> 
            <div style=""margin-top: .3rem;"">
                <span >Attributes : &nbsp; </span>
                <span>
{carAttrHtml}

</span> 
                
            </div> 
            
        </div>
        <br>
        <div style=""  display: flex;
        justify-content: center;
        padding-top: 2rem;
        "">

            <a style=""  
            margin-bottom: 20px;
             border-radius: 3rem;
            color: rgb(13, 188, 13);
            text-decoration: none;
      display: flex;
            justify-content: center;
            padding-top: 2rem;
            padding: 1rem 3rem;
            background-color: white ;
            padding: 1rem 3rem;
            background-color: rgb(13, 188, 13);
            border-radius: 3rem;
            color: white;
            text-decoration: none;
            transition: 200ms;"" href=""http://test.airportglobaltransfer.com/pdf/{reservationDetail.ReservationCode}-{reservationDetail.Id}.pdf"">Show Voucher</a>
        </div>


    </div>
    <div style=""      background-color: #ff6709;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    border-bottom-left-radius: 2rem;
    border-bottom-right-radius: 2rem;    
    padding: 2rem 0;
    font-family: 'Poppins', sans-serif;
    color: #320404;"">
        <div style=""   display: flex;
        align-items: center;
        justify-content: center;
        flex-direction: column;
        padding-bottom: .5rem;"">
            <p>Any Help?</p>
            <b><a href=""tel:+908502421901"">850 242 19014</a></b>
        </div>
        <div style=""      display: flex;
        align-items: center;
        justify-content: center;
        flex-direction: column;"">
<p>or</p>
            <b><p>info@airportglobaltransfer.com</p></b> 
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
