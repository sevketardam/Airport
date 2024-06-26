using Airport.DBEntities.Entities;

namespace Airport.MessageExtensions.Interfaces;

public interface IMail
{
    void SendReservationMail(Reservations mailVM);
}
