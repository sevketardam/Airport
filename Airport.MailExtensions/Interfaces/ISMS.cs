using Airport.MessageExtension.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.MessageExtension.Interfaces
{
    public interface ISMS
    {
        void SmsForReservation(Mesaj[] mesaj);
    }
}
