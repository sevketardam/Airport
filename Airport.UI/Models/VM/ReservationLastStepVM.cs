using System.Collections.Generic;

namespace Airport.UI.Models.VM;

public class ReservationLastStepVM
{
    public string PassengerName { get; set; }
    public string PassengerSurname { get; set; }
    public string PassengerEmail { get; set; }
    public string PassengerPhone { get; set; }
    public string PassengerRealPhone { get; set; }
    public string CouponCode { get; set; } = "";
    public bool HidePrice { get; set; } = false;
    public string PassengerComment { get; set; }
    public string DiscountText { get; set; }
    public bool IsDiscount { get; set; } = false;
    public double Discount { get; set; } = 0;
    public List<ServiceList> ServiceList { get; set; }
    public List<PassengerList> PassengerList { get; set; }
}

public class ServiceList
{
    public int SelectedValue { get; set; }
    public int PeopleCountInput { get; set; }
}

public class PassengerList
{
    public string PassengerName { get; set; }
    public string PassengerSurname { get; set; }
}
