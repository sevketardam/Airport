using Airport.MessageExtension.Interfaces;
using Airport.MessageExtension.VM;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;

namespace Airport.MessageExtension.Repos;

public class SMSRepo : ISMS
{
    readonly string username = "908502421901";
    readonly string password = "Mydn2929*";
    readonly string source_addr = "MYDN GROUP";

    public void SmsForReservation(Mesaj[] mesaj)
    {

        var smsistegi = new SmsIstegi()
        {
            username = username,
            password = password,
            source_addr = source_addr,
            messages = mesaj
        };

        string payload = JsonConvert.SerializeObject(smsistegi);

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
