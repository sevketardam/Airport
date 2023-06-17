using Airport.DBEntities.Entities;
using Airport.UI.Models.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Airport.UI.Models.ITransactions
{
    public class GetPrice : IPayment
    {
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

    }

}
