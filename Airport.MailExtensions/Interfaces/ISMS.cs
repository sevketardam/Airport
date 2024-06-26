using Airport.MessageExtension.VM;

namespace Airport.MessageExtension.Interfaces;

public interface ISMS
{
    void SmsForReservation(Mesaj[] mesaj);
}
