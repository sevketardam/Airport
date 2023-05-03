using Airport.MessageExtension.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.MessageExtensions.Interfaces
{
    public interface IMail
    {
        void SendReservationMail(ReservationMailVM mailVM);
    }
}
