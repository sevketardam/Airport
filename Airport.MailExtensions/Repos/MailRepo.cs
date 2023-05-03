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

namespace Airport.MessageExtensions.Repos
{
    public class MailRepo : IMail
    {
        public void SendReservationMail(ReservationMailVM reservationDetail)
        {
            MimeMessage mimeMessage = new MimeMessage();

            MailboxAddress mailboxAddressFrom = new MailboxAddress
                ("Ben", "arda05697@gmail.com");

            mimeMessage.From.Add(mailboxAddressFrom);

            MailboxAddress mailboxAddressTo = new MailboxAddress("User", reservationDetail.Email);

            var bodyBuilder = new BodyBuilder();
            mimeMessage.To.Add(mailboxAddressTo);

            mimeMessage.Subject = "Reservation Information";

            bodyBuilder.HtmlBody = @$"<!DOCTYPE html>
<html>
<head>
	<meta charset=""UTF-8"">
	<title>Reservation Information</title>
	<style>
		body {{
			font-family: Arial, sans-serif;
			background-color: #f2f2f2;
			padding: 20px;
		}}
		.container {{
			background-color: white;
			border-radius: 10px;
			padding: 20px;
			max-width: 600px;
			margin: 0 auto;
			box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
		}}
		.logo {{
			display: block;
			margin: 0 auto;
			max-width: 300px;
			height: auto;
		{{
		.text {{
			margin-top: 20px;
			font-size: 18px;
		{{
		.bold {{
			font-weight: bold;
		{{
	</style>
</head>
<body>
	<div class=""container"">
		<img class=""logo"" src=""http://test.airportglobaltransfer.com/images/Logo.png"" alt=""Logo"">
		<div class=""text"">
			<p class=""bold"">Reservation Code:</p>
			<span>{reservationDetail.ReservationCode}</span>
			<p class=""bold"">Name:</p>
			<p>{reservationDetail.Name}</p>
			<p class=""bold"">Surname:</p>
			<p>{reservationDetail.Surname}</p>
			<p class=""bold"">Phone:</p>
			<p>{reservationDetail.Phone}</p>
			<p class=""bold"">Price:</p>
			<p>{reservationDetail.Price}</p>
		</div>
	</div>
</body>
</html>";


            mimeMessage.Body = bodyBuilder.ToMessageBody();


            SmtpClient client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            //sepetispor@gmail.com //cmjvjyecqpnqwkis
            client.Authenticate("arda05697@gmail.com", "ufgwwqoebmoodhzx");
            client.Send(mimeMessage);
            client.Disconnect(true);
        }
    }
}
