using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.VM;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Linq;
using System.Security.Claims;

namespace Airport.UI.ViewComponents
{
    public class CouponMenu : ViewComponent
    {
        IUserDatasDAL _user;
        ILoginAuthDAL _loginAuth;
        ICouponsDAL _coupons;
        public CouponMenu(IUserDatasDAL user, ILoginAuthDAL loginAuth, ICouponsDAL coupons)
        {
            _user = user;
            _loginAuth = loginAuth;
            _coupons = coupons;
        }

        public IViewComponentResult Invoke()
        {
            var coupon = _coupons.SelectByFunc(a=>a.IsOffer && a.CouponStartDate <= DateTime.Now
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
}
