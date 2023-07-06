using Airport.UI.Models.IM;
using Airport.UI.Models.ITransactions;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airport.UI.Models.Interface
{
    public interface IPayment
    {
        double ReservationPrice(int StartForm, double KM, int PriceType, int UpTo, double fare);
        string HashGenerate(string hash);
        Task<Dictionary<string, string>> CreatePaymentLink(Dictionary<string, string> OrderData);
        Task<Dictionary<string, string>> SendPost(string postUrl, Dictionary<string, string> postData);
        string GetClientIp();
        void CreatePayment();
    }
}
