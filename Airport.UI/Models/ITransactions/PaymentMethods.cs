using Airport.DBEntities.Entities;
using Airport.UI.Models.Interface;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Airport.UI.Models.ITransactions.PaymentMethods;

namespace Airport.UI.Models.ITransactions
{
    public class PaymentMethods : IPayment
    {
        private IConfiguration Configuration { get; set; }
        private string MERCHANT => Configuration["Payment:EsnekPos:Merchant"];
        private string MERCHANT_KEY => Configuration["Payment:EsnekPos:MerchantKey"];
        private string BACK_URL => Configuration["Payment:EsnekPos:BackURL"];
        private string POST_URL => Configuration["Payment:EsnekPos:PostURL"];
        public HttpContext Context { get; set; }


        public PaymentMethods(IConfiguration Configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.Configuration = Configuration;
            Context = httpContextAccessor.HttpContext;
        }

        public async Task<ReturnPayment> CreatePayment(PaymentCardDetailVM cardDetail,Reservations reservation)
        {
            var newPayDetail = new PayDetail();

            newPayDetail.Config = new PayConfig()
            {
                MERCHANT = this.MERCHANT,
                MERCHANT_KEY = this.MERCHANT_KEY,
                BACK_URL = this.BACK_URL,
                ORDER_AMOUNT = reservation.TotalPrice.ToString(),
                ORDER_REF_NUMBER = reservation.ReservationCode,
                PRICES_CURRENCY = "EUR"
            };

            newPayDetail.CreditCard = new PayCard()
            {
                CC_NUMBER = cardDetail.CardNumber.Replace("-",""),
                CC_CVV = cardDetail.CVC,
                EXP_MONTH = cardDetail.CardDate.Split("/")[0],
                EXP_YEAR = cardDetail.CardDate.Split("/")[1],
                CC_OWNER = cardDetail.CardHolderName,
                INSTALLMENT_NUMBER = cardDetail.InstallmentNumber
            };

            newPayDetail.Customer = new PayCustomer()
            {
                FIRST_NAME = reservation.Name,
                LAST_NAME = reservation.Surname,
                MAIL = reservation.Email,
                PHONE = reservation.RealPhone,
                CITY = "",
                STATE = "",
                ADDRESS = reservation.PickFullName,
                CLIENT_IP = Context.Connection.RemoteIpAddress?.ToString()
            };

            newPayDetail.Product.Add(new PayProduct
            {
                PRODUCT_AMOUNT = reservation.ReturnStatus ? Math.Round(reservation.TotalPrice / 2,2).ToString() : reservation.TotalPrice.ToString(),
                PRODUCT_CATEGORY = "TRANSFER",
                PRODUCT_DESCRIPTION = $"{reservation.PickFullName}'dan {reservation.DropFullName}'ye Transfer",
                PRODUCT_NAME = $"{reservation.ReservationCode} Kodlu Reservasyon'un Gidiş Ücreti",
                PRODUCT_ID = reservation.ReservationCode.ToString()
            });

            if (reservation.ReturnStatus)
            {
                newPayDetail.Product.Add(new PayProduct
                {
                    PRODUCT_AMOUNT = Math.Round(reservation.TotalPrice / 2, 2).ToString(),
                    PRODUCT_CATEGORY = "TRANSFER",
                    PRODUCT_DESCRIPTION = $"{reservation.DropFullName}'dan {reservation.PickFullName}'ye Transfer",
                    PRODUCT_NAME = $"{reservation.ReservationCode} Kodlu Reservasyon'un Dönüş Ücreti",
                    PRODUCT_ID = reservation.ReservationCode.ToString()
                });
            }

            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(newPayDetail), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(POST_URL, content);
                response.EnsureSuccessStatusCode();
                var JsonResult = JsonConvert.DeserializeObject<ReturnPayment>(await response.Content.ReadAsStringAsync());
                return JsonResult;
            }
        }
    }
}
