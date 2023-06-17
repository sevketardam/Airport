using Airport.UI.Models.IM;
using Airport.UI.Models.ITransactions;
using System.Threading.Tasks;

namespace Airport.UI.Models.Interface
{
    public interface IPayment
    {
        double ReservationPrice(int StartForm, double KM, int PriceType, int UpTo, double fare);
    }
}
