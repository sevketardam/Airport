using Airport.UI.Models.Interface;
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
        private string UserName => Configuration["PosInfo:UserName"];
        private string Password => Configuration["PosInfo:Password"];
        private string ShopCode => Configuration["PosInfo:ShopCode"];
        private string Hash => Configuration["PosInfo:HashCode"];
        public HttpContext Context { get; set; }


        public PaymentMethods(IConfiguration Configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.Configuration = Configuration;
            Context = httpContextAccessor.HttpContext;
        }

        public string HashGenerate(string hashCode)
        {
            string concatenatedString = UserName + Password + ShopCode + hashCode + Hash;
            byte[] sha1Bytes = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(concatenatedString));
            string hashed = BitConverter.ToString(sha1Bytes).Replace("-", "");
            byte[] result = new byte[hashed.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(hashed.Substring(i * 2, 2), 16);
            }
            return Convert.ToBase64String(result);
        }

        public double ReservationPrice(int StartForm, double KM, int PriceType, int UpTo, double fare)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Dictionary<string, string>> CreatePaymentLink(Dictionary<string, string> OrderData)
        {
            var postData = new Dictionary<string, string>
            {
                {"userName", this.UserName},
                {"password", this.Password},
                {"shopCode", this.ShopCode},
                {"productName", OrderData["productName"]},
                {"productData", OrderData["productData"]},
                {"productType", OrderData["productType"]},
                {"productsTotalPrice", OrderData["productsTotalPrice"]},
                {"orderPrice", OrderData["orderPrice"]},
                {"currency", OrderData["currency"]},
                {"orderId", OrderData["orderId"]},
                {"locale", OrderData["locale"]},
                {"conversationId", OrderData["conversationId"]},
                {"buyerName", OrderData["buyerName"]},
                {"buyerSurName", OrderData["buyerSurName"]},
                {"buyerGsmNo", OrderData["buyerGsmNo"]},
                {"buyerIp", OrderData["buyerIp"]},
                {"buyerMail", OrderData["buyerMail"]},
                {"buyerAdress", OrderData["buyerAdress"]},
                {"buyerCountry", OrderData["buyerCountry"]},
                {"buyerCity", OrderData["buyerCity"]},
                {"buyerDistrict", OrderData["buyerDistrict"]},
                {"callbackOkUrl", OrderData["callbackOkUrl"]},
                {"callbackFailUrl", OrderData["callbackFailUrl"]},
                {"module", "NIVUSOSYAL-V1.003"}
            };

            postData["hash"] = HashGenerate(postData["orderId"] + postData["currency"] + postData["orderPrice"] + postData["productsTotalPrice"] + postData["productType"] + postData["callbackOkUrl"] + postData["callbackFailUrl"]);

            var response = await SendPost("https://www.vallet.com.tr/api/v1/create-payment-link", postData);
            if (response["status"] == "success" && response.ContainsKey("payment_page_url"))
            {
                return response;
            }
            else
            {
                return response;
            }
        }

        public async Task<Dictionary<string, string>> SendPost(string postUrl, Dictionary<string, string> postData)
        {
            var httpClient = new HttpClient();
            var postContent = new FormUrlEncodedContent(postData);

            HttpResponseMessage response = null;
            Dictionary<string, string> result = null;

            try
            {
                response = await httpClient.PostAsync(postUrl, postContent);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                    if (result == null)
                    {
                        result = new Dictionary<string, string>
                {
                    { "status", "error" },
                    { "errorMessage", "Dönen cevap Array değildi" }
                };
                    }
                }
                else
                {
                    result = new Dictionary<string, string>
            {
                { "status", "error" },
                { "errorMessage", "Curl Geçersiz bir cevap aldı" }
            };
                }
            }
            catch (Exception ex)
            {
                result = new Dictionary<string, string>
        {
            { "status", "error" },
            { "errorMessage", ex.Message }
        };
            }

            return result;
        }

        public string GetClientIp()
        {
            if (Context.Request.Headers.ContainsKey("X-Real-IP"))
            {
                return Context.Request.Headers["X-Real-IP"];
            }
            else if (Context.Request.Headers.ContainsKey("HTTP_CLIENT_IP"))
            {
                return Context.Request.Headers["HTTP_CLIENT_IP"];
            }
            else if (Context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Context.Request.Headers["X-Forwarded-For"].First();
            }
            else
            {
                return Context.Connection.RemoteIpAddress.ToString();
            }
        }

        public void CreatePayment()
        {
            throw new NotImplementedException();
        }
    }
}
