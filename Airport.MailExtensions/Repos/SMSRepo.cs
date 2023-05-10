using Airport.MessageExtension.Interfaces;
using Airport.MessageExtension.VM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Airport.MessageExtension.Repos
{
    public class SMSRepo : ISMS
    {
        readonly string username = "908502421901";
        readonly string password = "Mydn2929*";

        public void SmsForReservation(SmsIstegi istek)
        {
            string payload = JsonConvert.SerializeObject(istek);

            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            wc.Headers["Content-Type"] = "application/json";

            try
            {
                string campaign_id = wc.UploadString("https://sms.verimor.com.tr/v2/send.json", payload);
            }
            catch (WebException ex) // 400 hatalarında response body'de hatanın ne olduğunu yakalıyoruz
            {
                if (ex.Status == WebExceptionStatus.ProtocolError) // 400 hataları
                {
                    var responseBody = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                }
                else // diğer hatalar
                {
                    // MessageBox.Show("Mesaj gönderilemedi, dönen hata: " + ex.Status);
                    throw;
                }
            }
        }
    }
}
