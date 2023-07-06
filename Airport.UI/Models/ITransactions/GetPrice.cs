using Airport.DBEntities.Entities;
using Airport.UI.Models.Extendions;
using Airport.UI.Models.Interface;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Airport.UI.Models.ITransactions
{
    public class GetPrice : IPayment
    {
        public void CreatePayment()
        {
            throw new NotImplementedException();
        }

        public void CreatePaymentLink(Dictionary<string, string> OrderData)
        {
            throw new NotImplementedException();
        }

        public string GetClientIp()
        {
            throw new NotImplementedException();
        }

        public string HashGenerate(string hash)
        {
            throw new NotImplementedException();
        }

        public double ReservationPrice(int StartForm, double KM, int PriceType, int UpTo, double fare)
        {
            double price = 0;
            if (StartForm < KM)
            {
                if (PriceType == 2)
                {
                    if (UpTo < KM)
                    {
                        price += fare * (UpTo - StartForm);
                    }
                    else
                    {
                        price += fare * (KM - StartForm);
                    }
                }
                else
                {
                    price += fare;
                }
            }

            return price;
        }

        public Task<Dictionary<string, string>> SendPost(string postUrl, Dictionary<string, string> postData)
        {
            throw new NotImplementedException();
        }

        Task<Dictionary<string, string>> IPayment.CreatePaymentLink(Dictionary<string, string> OrderData)
        {
            throw new NotImplementedException();
        }
    }

}
