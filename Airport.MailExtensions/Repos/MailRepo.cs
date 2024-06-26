using Airport.MessageExtensions.Interfaces;
using System;
using MimeKit;
using MailKit.Net.Smtp;
using Airport.DBEntities.Entities;

namespace Airport.MessageExtensions.Repos;

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
            serviceText = @"
<div style=""margin: 10px 10px; border-bottom: 4px solid #FF6900;"">
<h3 style=""font-size: 23px;text-align: center;"">SERVICES DETAILS</h3>";
            reservationDetail.ReservationServicesTables.ForEach(a =>
            {
                serviceText += @$"            <div>
                <p style=""margin: 0px;font-weight: bold;"">{a.ServiceItem.ServiceProperty.ServicePropertyName}
                    {a.Price}x{a.PeopleCount}={a.Price*a.PeopleCount}€</p>
            </div>";
            });
            serviceText += "</div></div>";

        }
        var price = reservationDetail.IsDiscount ? reservationDetail.Discount : reservationDetail.OfferPrice;
        var servicefee = reservationDetail.ExtraServiceFee;

        var priceHtml = "";
        var totalprice = price + servicefee;

        if (reservationDetail.HidePrice == false)
        {
            var discountHtml = "";
            var specialDiscountHtml = "";

            if (reservationDetail.Coupons != null)
            {
                totalprice = Math.Round(Convert.ToDouble(totalprice) - ((reservationDetail.Coupons.Discount * Convert.ToDouble(totalprice)) / 100), 2);
                discountHtml = @$"       

<div style=""display: flex; justify-content: space-between;width:100%;"">
            <div>
                      <p style=""margin: 0px;"">Discount:</p>
            </div>
            <div>
                      <p style=""margin: 0px;margin-left:.5rem;"">{reservationDetail.Coupons.Discount}%</p>
            </div>
        </div>";
            }

            if (reservationDetail.IsDiscount)
            {
                specialDiscountHtml = @$"       
<div style=""display: flex; justify-content: space-between;width:100%;    text-decoration: line-through;"">
            <div>
                      <p style=""font-weight: bold;margin: 0px;"">Total:</p>
            </div>
            <div>
                      <p style=""margin: 0px;margin-left:.5rem;"">{reservationDetail.OfferPrice + reservationDetail.ExtraServiceFee} €</p>
            </div>
        </div>
<div style=""display: flex; justify-content: space-between;width:100%;"">
            <div>
                      <p style=""font-weight: bold;margin: 0px;"">Special Price:</p>
            </div>
            <div>
                      <p style=""margin: 0px;margin-left:.5rem;"">{totalprice} €</p>
            </div>
        </div>

<div style=""display: flex; justify-content: space-between;width:100%;"">
            <div>
                      <p style=""margin: 0px;"">{reservationDetail.DiscountText}</p>
            </div>
        </div>
       ";
            }
            else
            {
                if (reservationDetail.Coupons != null)
                {
                    specialDiscountHtml = $@"

<div style=""display: flex; justify-content: space-between;width:100%;text-decoration: line-through;"">
            <div>
                      <p style=""font-weight: bold;margin: 0px;"">TOTAL:</p>
            </div>
            <div>
                       <p style=""margin: 0px;margin-left:.5rem;"">{reservationDetail.ExtraServiceFee + reservationDetail.OfferPrice} €</p>
            </div>
        </div>
<div style=""display: flex; justify-content: space-between;width:100%;"">
            <div>
                      <p style=""font-weight: bold;margin: 0px;"">Discount Price:</p>
            </div>
            <div>
                       <p style=""margin: 0px;margin-left:.5rem;"">€{totalprice} €</p>
            </div>
        </div>

<div style=""display: flex; justify-content: space-between;width:100%;"">
            <div>
                       <p style=""margin: 0px;"">€{reservationDetail.Coupons.Comment} €</p>
            </div>
        </div>

";
                }
                else
                {
                    specialDiscountHtml = $@" <div style=""display: flex; justify-content: space-between;width:100%;"">
            <div>
                      <p style=""font-weight: bold;margin: 0px;"">TOTAL:</p>
            </div>
            <div>
                       <p style=""margin: 0px;margin-left:.5rem;"">{totalprice} €</p>
            </div>
        </div>";
                }

            }

            priceHtml = @$"
        <h3 style=""font-size: 23px;text-align: center;"">PAYMENT DETAILS</h3>

        <div style=""display: flex; justify-content: space-between;width:100%;"">
            <div>
                      <p style=""margin: 0px;"">Offer price:</p>
            </div>
            <div>
                      <p style=""margin: 0px;margin-left:.5rem"">{reservationDetail.OfferPrice} €</p>
            </div>
        </div>

        

        <div style=""display: flex; justify-content: space-between;width:100%;"">
            <div>
                      <p style=""margin: 0px;"">Service fee:</p>
            </div>
            <div>
                      <p style=""margin: 0px;margin-left:.5rem;"">{reservationDetail.ExtraServiceFee} €</p>
            </div>
        </div>
        {discountHtml}
        {specialDiscountHtml}
";

        }



        var carAttrHtml = "";
        if (reservationDetail.LocationCars.Car.Armored)
        {
            carAttrHtml += "<img style='width: 18px;' src='http://www.airportglobaltransfer.com/img/i5.png'>";
        }

        if (reservationDetail.LocationCars.Car.Wifi)
        {
            carAttrHtml += "<img style='width: 18px;' src='http://www.airportglobaltransfer.com/img/i4.png'>";
        }

        if (reservationDetail.LocationCars.Car.Water)
        {
            carAttrHtml += "<img style='width: 18px;' src='http://www.airportglobaltransfer.com/img/i1.png'>";
        }

        if (reservationDetail.LocationCars.Car.Partition)
        {
            carAttrHtml += "<img style='width: 18px;' src='http://www.airportglobaltransfer.com/img/i6.png'>";
        }

        if (reservationDetail.LocationCars.Car.Charger)
        {
            carAttrHtml += "<img style='width: 18px;' src='http://www.airportglobaltransfer.com/img/i3.png'>";
        }

        if (reservationDetail.LocationCars.Car.Disabled)
        {
            carAttrHtml += "<img style='width: 18px;' src='http://www.airportglobaltransfer.com/img/i2.png'>";
        }

        var returnIcon = "https://storage.acerapps.io/app-1348/asd/altok.png";
        var isReturn = false;
        if (reservationDetail.ReturnStatus)
        {
            returnIcon = "https://storage.acerapps.io/app-1348/asd/gelgit.png";
            isReturn = true;
        }


        var returnString = "";
        if (reservationDetail.ReturnStatus)
        {
            returnString = @$"                <p style=""font-weight: bold; margin: 0;"">RETURN DATE</p>
                <div>
                    <p style=""margin: 0 0 10px 0px;"">{reservationDetail.ReturnDate.ToString("dd.MM.yyyy HH:mm")} </p>
                </div>";
        }


        mimeMessage.Subject = "Reservation Information";
        bodyBuilder.HtmlBody = @$"<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Document</title>
    <style>
        * {{
            font-size: .9rem;
            font-family: 'Montserrat';
        }}
    </style>
    <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
    <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
    <link
        href=""https://fonts.googleapis.com/css2?family=Montserrat:wght@100;200;300;400;500;600;700;800;900&display=swap""
        rel=""stylesheet"">
</head>

<body style=""margin: 0px; padding: 30px; font-family: 'Montserrat';"">
    <div style=""width: 100%;
                    padding: 20px 0px;"">
        <img src=""data:image/png;base64,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""
            alt="" style="" width:230px;height:auto;"" />

    </div>


    <div style=""padding: 5px 5px;"">

        <h3 style=""text-align: center; font-size: 23px;margin-bottom:20px;padding:0;"">BOOKING DETAILS</h3>
        <div style=""display: flex; justify-content: space-between;"">
            <div style=""float: right;"">
                <p style=""font-weight: bold; margin: 0;"">RIDE DATE</p>
                <div>
                    <p style=""margin: 0 0 10px 0px;"">{reservationDetail.ReservationDate.ToString("g")} </p>
                </div>

                {returnString}

                <p style=""font-weight: bold; margin: 0px;"">FROM</p>
                <div style=""margin: 0 0 10px 0px;"">
                    <p style=""margin:0px"">{reservationDetail.PickFullName}</p>
                </div>

                <p style=""font-weight: bold; margin: 0px;"">TO</p>
                <div style=""margin: 0 0 10px 0px;"">
                    <p style=""margin:0px"">{reservationDetail.DropFullName}</p>
                </div>

                <div style=""margin: 0 0 10px 0px;"">
                    <span style=""font-weight: bold; margin: 0px;"">NAME SIGN:</span>
                    <span style=""margin:0px"">{reservationDetail.Name} {reservationDetail.Surname}</span>
                </div>

                <div style=""margin: 0 0 10px 0px;"">
                    <span style=""font-weight: bold; margin: 0px;"">PASSENGERS:</span>
                    <span style=""margin:0px"">{reservationDetail.PeopleCount}</span>
                </div>

                <p style=""font-weight: bold; margin: 0px;"">COMMENT</p>
                <div style=""margin: 0 0 10px 0px;"">
                    <p style=""margin:0px"">{reservationDetail.Comment}</p>
                </div>

            </div>

        </div>


    </div>

   {serviceText}
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


        mimeMessage.Body = bodyBuilder.ToMessageBody();


        SmtpClient client = new SmtpClient();
        client.Connect("smtp.gmail.com", 587, false);
        //sepetispor@gmail.com //cmjvjyecqpnqwkis
        client.Authenticate("airportglobaltransfer@gmail.com", "jcgbdclwxjpcpcew");
        client.Send(mimeMessage);
        client.Disconnect(true);
    }
}
