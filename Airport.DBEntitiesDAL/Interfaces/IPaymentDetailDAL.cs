using Airport.Data;
using Airport.DBEntities.Entities;
using System;
namespace Airport.DBEntitiesDAL.Interfaces;

public interface IPaymentDetailDAL : ISelectableRepo<PaymentDetail>, IEInsertableRepo<PaymentDetail>, IUpdatetableRepo<PaymentDetail>, IDeletableRepo<PaymentDetail>, IDisposable;