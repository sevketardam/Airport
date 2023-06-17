using System;

namespace Airport.UI.Models.VM
{
    public class PageCouponVM
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public string CouponCode { get; set; }
        public DateTime EndTime { get; set; }
    }
}
