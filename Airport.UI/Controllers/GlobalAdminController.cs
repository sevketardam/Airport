using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Airport.UI.Controllers
{
    public class GlobalAdminController : Controller
    {
        ICouponsDAL _coupons;
        public GlobalAdminController(ICouponsDAL coupons)
        {
            _coupons = coupons;
        }

        [HttpGet("panel/coupons")]
        public IActionResult CreateCoupon()
        {

            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var myCoupons = _coupons.SelectByFunc(a => a.UserId == userId);
            return View(myCoupons);
        }

        [HttpPost]
        public JsonResult CreateCouponJSON(Coupons coupon)
        {
            try
            {
                if (coupon != null)
                {
                    var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                    _coupons.Insert(new Coupons
                    {
                        Active = coupon.Active,
                        Comment = coupon.Comment,
                        CouponCode = coupon.CouponCode,
                        CouponFinishDate = coupon.CouponFinishDate,
                        CouponStartDate = coupon.CouponStartDate,
                        CouponLimit = coupon.CouponLimit,
                        CreatedDate = DateTime.Now,
                        IsPerma = coupon.IsPerma,
                        Discount = coupon.Discount,
                        UserId = userId,
                        UsingCount = 0
                    });
                    return new JsonResult(new { result = 1 });
                }
                return new JsonResult(new { result = 2 });
            }
            catch (Exception)
            {
                return new JsonResult(new { });
            }
        }

        [HttpPost]
        public JsonResult GetCouponJSON(int id)
        {
            try
            {
                var coupon = _coupons.SelectByID(id);
                if (coupon != null)
                {
                    return new JsonResult(new { result = 1, data = coupon });
                }
                return new JsonResult(new { result = 2 });
            }
            catch (Exception)
            {
                return new JsonResult(new { });
            }
        }

        [HttpPost]
        public JsonResult UpdateCouponJSON(Coupons updatedCoupon)
        {
            try
            {
                var coupon = _coupons.SelectByID(updatedCoupon.Id);
                if (coupon != null)
                {
                    coupon.Active = updatedCoupon.Active;
                    coupon.CouponLimit = updatedCoupon.CouponLimit;
                    coupon.Comment = updatedCoupon.Comment;
                    coupon.CouponCode = updatedCoupon.CouponCode;
                    coupon.CouponFinishDate = updatedCoupon.CouponFinishDate;
                    coupon.CouponStartDate = updatedCoupon.CouponStartDate;
                    coupon.Discount = updatedCoupon.Discount;
                    _coupons.Update(coupon);
                    return new JsonResult(new { result = 1 });
                }
                return new JsonResult(new { result = 2 });
            }
            catch (Exception)
            {
                return new JsonResult(new { });
            }
        }

    }
}
