using Airport.DBEntities.Entities;
using Airport.UI.Models.IM;
using Airport.UI.Models.ITransactions;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airport.UI.Models.Interface;

public interface IPayment
{
    Task<ReturnPayment> CreatePayment(PaymentCardDetailVM payDetail,Reservations reservation);

}
