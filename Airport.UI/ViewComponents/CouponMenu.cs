using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Airport.UI.ViewComponents;

public class CouponMenu(ICouponsDAL couponsDal) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var coupon = couponsDal.SelectByFunc(a=>a.IsOffer && a.CouponStartDate <= DateTime.Now
                                                                                    && a.CouponFinishDate >= DateTime.Now && a.Active).FirstOrDefault();

        PageCouponVM couponVM = new PageCouponVM();

        if (coupon != null)
        {
            TimeSpan remaining = coupon.CouponFinishDate - DateTime.Now;

            couponVM.CouponCode = coupon.CouponCode;
            couponVM.Hour = remaining.Hours;
            couponVM.Minute = remaining.Minutes;
            couponVM.Second = remaining.Seconds;

            couponVM.EndTime = coupon.CouponFinishDate;
        }

        return View(couponVM);
    }

}
